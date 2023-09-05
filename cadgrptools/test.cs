﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.Configuration;
using CADDB;

namespace cadgrptools
{
    public class test
    {
        /*  If we know the name of the Layout (the text on the tab), we can
            use GetBtrIDByLayoutName to get the corresponding BlockTableRecord, open it, and then draw to it*/
        [CommandMethod("DrawOnLayout2")]
        public void DrawOnLayout2()
        {
            ObjectId btrID = Utilities.GetBtrIDByLayoutName(HostApplicationServices.WorkingDatabase, "Layout2");
            if (btrID != null)
            {
                using (Transaction tr = btrID.Database.TransactionManager.StartTransaction()) 
                {
                    BlockTableRecord myBTR = btrID.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                    Circle myCircle = new Circle(Point3d.Origin, Vector3d.ZAxis, 4);
                    myBTR.AppendEntity(myCircle);
                    tr.AddNewlyCreatedDBObject(myCircle, true);

                    tr.Commit(); 
                }
            }
        }




        [CommandMethod("mconfig")]  // not work cause .net 3.5 
        public static void ReadConfigurationFile()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            var option = ConfigurationManager.AppSettings["option"];
            //var comment = ConfigurationManager.AppSettings["comment"];

            ed.WriteMessage($"\nOption : {option}");
            ed.WriteMessage($"\nComment : {ConfigurationManager.AppSettings["comment"]}");
        }





        [CommandMethod("readXML")]  // OK
        public static void ReadXml()
        {
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load("cadgrpproperties.xml");
            //xmlDoc.Save(Console.Out);

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            int opt = 0;
            string cmt = "";

            XmlTextReader xtr = new XmlTextReader("cadgrpproperties.xml");
            while (xtr.Read()) // read next node from the stream
            {
                
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "Option")
                {
                    opt = int.Parse(xtr.ReadElementString()); // read a text only element
                    //ed.WriteMessage("\nOption = " + opt);
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "Comment")
                {
                    cmt = xtr.ReadElementString();
                    //ed.WriteMessage("\nComment = " + cmt);
                }

                

                /*
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "name")
                {
                    string s1 = xtr.ReadElementString();
                    ed.WriteMessage("\nName = " + s1);
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "class")
                {
                    string s1 = xtr.ReadElementString();
                    ed.WriteMessage("\nClass = " + s1);
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "result")
                {
                    string s1 = xtr.ReadElementString();
                    ed.WriteMessage("\nResult = " + s1);
                    //ed.WriteMessage("  "); // not work
                    //ed.WriteMessage("\n");

                }
                */
            }

            MulConfig mulOpt = new MulConfig(opt, cmt,"","");

            ed.WriteMessage("\nComment = " + mulOpt.comment);


        }

        [CommandMethod("wXML")]
        public static void WriteXml()
        {
            string fileName = "employee.xml";
            XmlTextWriter xtw = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);

            xtw.Formatting = Formatting.Indented;

            xtw.WriteStartDocument();

            xtw.WriteComment("Creating xml file using c#");

            xtw.WriteStartElement("Employees"); // root node

            for (int i = 1; i <= 3; i++) // 3 sub nodes
            {
                xtw.WriteStartElement("Employee");
                xtw.WriteElementString("name", "employee no_" + i);
                xtw.WriteElementString("age", (i + 20).ToString());
            }

            xtw.WriteEndElement();
            xtw.WriteEndDocument();
            xtw.Flush();
            xtw.Close();

        }



        [CommandMethod("ExtendObject")]
        public static void ExtendObject()
        {

            // test
            PropertyControl mul = new PropertyControl();
            



            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create a line that starts at (4,4,0) and ends at (7,7,0)
                using (Line acLine = new Line(new Point3d(4, 4, 0),
                                        new Point3d(7, 7, 0)))
                {

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acLine);
                    acTrans.AddNewlyCreatedDBObject(acLine, true);

                    // Update the display and diaplay a message box
                    acDoc.Editor.Regen();
                    Application.ShowAlertDialog("Before extend");

                    // Ext 100 the length of the line from end point
                    acLine.EndPoint = acLine.EndPoint + (acLine.EndPoint - acLine.StartPoint).GetNormal() * 10;
                    // Ext 100 the length of the line from start point
                    acLine.StartPoint = acLine.StartPoint - (acLine.EndPoint - acLine.StartPoint).GetNormal() * 10;
                }

                // Save the new object to the database
                acTrans.Commit();
            }
        }













        [CommandMethod("fpadl")]
        public static void FindPointAndDrawLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;

            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = transaction.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Vẽ đoạn đường gốc
                Point3d startPoint = new Point3d(0, 0, 0);
                Point3d endPoint = new Point3d(50, 0, 0);
                Line originalLine = new Line(startPoint, endPoint);
                modelSpace.AppendEntity(originalLine);
                transaction.AddNewlyCreatedDBObject(originalLine, true);

                // Tìm điểm cách startPoint 20 đơn vị
                double distance = 20.0;
                Vector3d direction = (endPoint - startPoint).GetNormal();
                Point3d newPoint = startPoint + direction * distance;

                // Tạo đoạn đường xéo từ điểm mới
                double angleInRadians = Math.PI / 4; // 45 độ (45 * Math.PI / 180)
                double lineLength = 50.0; // Độ dài đoạn đường xéo
                Vector3d lineDirection = new Vector3d(Math.Cos(angleInRadians), Math.Sin(angleInRadians), 0); // Vector định hướng đoạn đường xéo
                Line diagonalLine = new Line(newPoint, newPoint + lineDirection * lineLength);
                modelSpace.AppendEntity(diagonalLine);
                transaction.AddNewlyCreatedDBObject(diagonalLine, true);

                transaction.Commit();
            }
        }



        [CommandMethod("fpol")]
        public static void FindPointOnLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityOptions opt = new PromptEntityOptions("\nChọn đường thẳng: ");
            opt.SetRejectMessage("\nVui lòng chỉ chọn đường thẳng.");
            opt.AddAllowedClass(typeof(Line), true);

            PromptEntityResult res = ed.GetEntity(opt);

            if (res.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Line line = tr.GetObject(res.ObjectId, OpenMode.ForRead) as Line;
                    if (line != null)
                    {
                        Point3d startPoint = line.StartPoint;
                        double angleInRadians = Math.PI / 4; // 45 degrees in radians
                        double distance = 20.0;

                        double xOffset = distance * Math.Cos(angleInRadians);
                        double yOffset = distance * Math.Sin(angleInRadians);

                        Point3d newPoint = new Point3d(startPoint.X + xOffset, startPoint.Y + yOffset, startPoint.Z);

                        ed.WriteMessage("\nĐiểm cách StartPoint 20 đơn vị và nằm trên hướng 45 độ: " + newPoint.ToString());

                        // Vẽ một đường thẳng mới từ điểm mới theo hướng 45 độ
                        Line newLine = new Line(newPoint, newPoint + (new Vector3d(Math.Cos(angleInRadians), Math.Sin(angleInRadians), 0) * 100));
                        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        btr.AppendEntity(newLine);
                        tr.AddNewlyCreatedDBObject(newLine, true);
                    }
                    tr.Commit();
                }
            }
        }





    }
}


