using System;
using System.Collections.Generic;
using System.Net;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace CADDB
{
    public static class CommonUtil
    {





        // 20230903 
        public static void ExtendLine(Database db, Line line, double extensionDistance, 
                                        bool directionStartPoint = true, bool directionEndPoint = true)
        {
            using(Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Mở rộng đoạn line thêm extensionDistance đơn vị ở cả hai đầu
                Point3d newStartPoint, newEndPoint;
                if (directionStartPoint)
                {
                    newStartPoint = line.StartPoint - (line.EndPoint - line.StartPoint).GetNormal() * extensionDistance;
                    line.StartPoint = newStartPoint;
                }
                    
                if (directionEndPoint)
                {
                    newEndPoint = line.EndPoint + (line.EndPoint - line.StartPoint).GetNormal() * extensionDistance;
                    line.EndPoint = newEndPoint;
                }

                tr.Commit();
            }
        }



        public static void AddOffsetLine(Line baseLine, double offsetDistance, Color color, string lineType = "CONTINUOUS")
        {
            foreach (Entity acEnt in baseLine.GetOffsetCurves(offsetDistance))
            {
                Line offsetLine1 = (Line)acEnt;
                offsetLine1.Linetype = lineType;
                offsetLine1.Color = color;  // green

                AddToModelSpace(HostApplicationServices.WorkingDatabase, offsetLine1);
            }
        }

        public static ObjectId AddToModelSpace(Database db, Entity ent)
        {
            
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                btr.AppendEntity(ent);
                tr.AddNewlyCreatedDBObject(ent, true);

                tr.Commit();
                return ent.ObjectId;
            }
        }


        public static ObjectIdCollection AddToModelSpace(Database db, List<Entity> ents)
        {
            ObjectIdCollection objIdCollect = new ObjectIdCollection();
            
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                foreach (var myEnt in ents)
                {
                    btr.AppendEntity(myEnt);
                    tr.AddNewlyCreatedDBObject(myEnt, true);
                    objIdCollect.Add(myEnt.Id); // Id or ObjectId ???
                }
                
                tr.Commit();
                return objIdCollect;
            }
        }


        public static void SetDefaultColor(Database db, Color color)
        {
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = transaction.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Đặt màu mặc định là green (xanh)
                db.Clayer = db.Clayer;
                Color defaultColor = color; // Green color

                //modelSpace.color = defaultColor;

                transaction.Commit();
            }
        }




        // Get all the vertices of the polyline and return it to the calling method in comma seperated value
        public static string GetPolylineCoordinates(Polyline pl)
        {
            var vCount = pl.NumberOfVertices;
            Point2d coord;
            string coords = "";

            for (int i = 0; i <= vCount - 1; i++)
            {
                coord = pl.GetPoint2dAt(i);
                coords += coord[0].ToString() + "," + coord[1].ToString();
                if (i < vCount - 1)  coords += ",";
            }
            return coords;
        }

        internal static void AddXDataToEntity(string appName, Entity ent, int xdValue)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Get the registered application names table
                RegAppTable regTable = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                if (!regTable.Has(appName))
                {
                    regTable.UpgradeOpen();
                    // Add the application name to Xdata
                    RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                    regAppTableRecord.Name = appName;
                    regTable.Add(regAppTableRecord);
                    tr.AddNewlyCreatedDBObject(regAppTableRecord, true);
                }
                // Append the Xdata to entity
                ResultBuffer rb = new ResultBuffer(
                    new TypedValue(1001, appName), 
                    new TypedValue((int)DxfCode.ExtendedDataInteger32, xdValue));
                ent.XData = rb;
                rb.Dispose();
                tr.Commit();
            }
        }

        internal static int GetColorIndex(string colorName)
        {
            int color = 7;  // white
            switch (colorName.ToUpper())
            {
                case "RED":
                    color = 1;
                    break;
                case "YELLOW":
                    color = 2;
                    break;
                case "GREEN":
                    color = 3;
                    break;
                case "CYAN":
                    color = 4;
                    break;
                case "BLUE":
                    color = 5;
                    break;
                case "MAGENTA":
                    color = 6;
                    break;
                case "WHITE":
                    color = 7;
                    break;
                case "BYBLOCK":
                    color = 0;
                    break;
                case "BYLAYER":
                    color = 256;
                    break;
                default:
                    color = 256;
                    break;
            }
            return color;
        }

        public static int ReadXDataFromEntity(string appName, Entity ent)
        {
            Int32 id = 0;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                ResultBuffer rb = ent.GetXDataForApplication(appName);
                if (rb != null)
                {
                    TypedValue[] arr = rb.AsArray();
                    foreach (TypedValue tv in arr)
                    {
                        switch ((DxfCode)tv.TypeCode)
                        {
                            case DxfCode.ExtendedDataInteger32:
                                id = Convert.ToInt32(tv.Value);
                                break;
                        }
                    }
                    rb.Dispose();
                }

            }
            return id;
        }
    }
}
