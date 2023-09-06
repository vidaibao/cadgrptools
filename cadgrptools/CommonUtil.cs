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

    public class MulConfig
    {
        public int option { get;  }
        public string comment { get;  }
        public string CenterType { get;  }
        public string HiddenType { get;  }
       

        public MulConfig(int option, string comment, string centerType, string hiddenType)
        {
            this.option = option;
            this.comment = comment;
            this.CenterType = centerType;
            this.HiddenType = hiddenType;
        }
    }

    public static class CommonUtil
    {





        public static string UserSelectText(Document doc)
        {
            string res = "";
            Editor editor = doc.Editor;
            // Định dạng văn bản cần tìm kiếm
            //string targetTextFormat = "800x300x16x32";

            // Tạo một SelectionFilter để chọn Text
            TypedValue[] filterList = new TypedValue[] {
                new TypedValue((int)DxfCode.Start, "TEXT"),
                //new TypedValue((int)DxfCode.Text, formatStr)
            };
            SelectionFilter filter = new SelectionFilter(filterList);
            // Yêu cầu người dùng chọn Text có định dạng cụ thể
            PromptSelectionResult selectionResult = editor.GetSelection(filter);
            if (selectionResult.Status == PromptStatus.OK)
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    SelectionSet selectionSet = selectionResult.Value;
                    foreach (ObjectId objectId in selectionSet.GetObjectIds())
                    {
                        DBText textEntity = tr.GetObject(objectId, OpenMode.ForRead) as DBText;
                        if (textEntity != null)
                        {
                            // Xử lý TextEntity ở đây, ví dụ: hiển thị thông tin, thay đổi màu sắc, v.v.
                            editor.WriteMessage($"\nText found: {textEntity.TextString}");
                            res = textEntity.TextString;
                        }
                    }
                    tr.Commit();
                }
            }
            else
            {
                editor.WriteMessage("\nProfile not found!");
            }

            return res;
        }





        public static int GetIntegerFromUser(Document acDoc, string msg, int defaultValue = 2)
        {
            //acDoc = Application.DocumentManager.MdiActiveDocument;

            PromptIntegerOptions pIntOpts = new PromptIntegerOptions("");
            pIntOpts.Message = msg;

            // Restrict input to positive and non-negative values
            pIntOpts.AllowZero = false;
            pIntOpts.AllowNegative = false;
            pIntOpts.AllowNone = true;

            // Get the value entered by the user
            PromptIntegerResult pIntRes = acDoc.Editor.GetInteger(pIntOpts);

            if (pIntRes.Status == PromptStatus.OK)
            {
                return pIntRes.Value;
            }
            else
            {
                return defaultValue;
            }
        }


        public static string GetStringFromUser(Document acDoc, string promptUser)
        {
            //acDoc = Application.DocumentManager.MdiActiveDocument;

            PromptStringOptions pStrOpts = new PromptStringOptions(promptUser);
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);
            if (pStrRes.Status == PromptStatus.OK)
            {
                return pStrRes.StringResult;
            }
            //Application.ShowAlertDialog("The name entered was: " + pStrRes.StringResult);
            return "";
        }




        // 20230906
        public static void CheckLinetype(Database db, string linetypeName)
        {
            using(Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Open the Linetype table for read
                LinetypeTable acLineTypTbl;
                acLineTypTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                
                if (acLineTypTbl.Has(linetypeName) == false)
                {
                    // Load the *** Linetype
                    db.LoadLineTypeFile(linetypeName, "acad.lin");
                    // Set the *** linetype current
                    //db.Celtype = acLineTypTbl[linetypeName];
                }
                tr.Commit();
            }
        }






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



        public static void AddOffsetLine(Line baseLine, double offsetDistance, string lineType = "CONTINUOUS")
        {
            foreach (Entity acEnt in baseLine.GetOffsetCurves(offsetDistance))
            {
                Line offsetLine1 = (Line)acEnt;
                offsetLine1.Linetype = lineType;
                //offsetLine1.ColorIndex = 3;//green
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

        public static ObjectIdCollection AddToModelSpace(Database db, params Entity[] ents)
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
