using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADDB;

using Microsoft.VisualBasic;

namespace cadgrptools
{
    public class LamUtils
    {
        [CommandMethod("mul")]
        public static void Draw9lines()
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // Read properties
            //PropertyControl.MulWriteProperties();
            MulConfig mulConfig = XmlData.ReadXml();

            if (mulConfig.option < 1 || mulConfig.option > 3)
            {
                ed.WriteMessage("\nCould not get option value from cadgrpproperties.xml");
                return;
            }
            else
            {
                ed.WriteMessage($"\nOption default value is: {mulConfig.option}");
                ed.WriteMessage($"\nOption description: {mulConfig.comment}");
            }
            

            // get 2 points from acad model screen
            PromptPointOptions basePO = new PromptPointOptions("\nPick the start point:");
            PromptPointResult basePR = ed.GetPoint(basePO);

            if (basePR.Status != PromptStatus.OK) 
                return;
            
            Point3d point1 = basePR.Value;

            // Chọn điểm thứ 2
            PromptPointOptions point2Options = new PromptPointOptions("\nPick the 2nd point: ");
            point2Options.UseBasePoint = true;
            point2Options.BasePoint = point1;
            PromptPointResult point2Result = ed.GetPoint(point2Options);
            if (point2Result.Status != PromptStatus.OK)
                return;

            Point3d point2 = point2Result.Value;


            // get profile of beam, gear from editor command prompt
            string profile = Interaction.InputBox("Enter profile (format SH- 800X300 X16 X32 or 800X300x16x32)", "Profile");
            
            
            //PromptStringOptions pso = new PromptStringOptions("\nEnter profile (format SH- 800X300 X16 X32 or 800X300x16x32):");
            //PromptResult pr = ed.GetString(pso);
            //if(pr.Status != PromptStatus.OK)
            //{
            //    ed.WriteMessage("\nCould not get profile!");
            //    return; 
            //}
            //profile = pr.StringResult;
            // ed.WriteMessage(profile);
            // get HBtf    H-800X300 X16X32
            var HBtf = GetProfile(profile);
            foreach (var i in HBtf)
            {
                if (i <= 0)
                {
                    ed.WriteMessage($"\nHBtf = {i} >> Error");
                    return;
                }
                ed.WriteMessage($"\n{i}");
            }

            // draw BASE lines 
            Line baseLine = new Line(point1, point2);
            baseLine.Color = Color.FromRgb(0, 255, 0);  // green
            CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, baseLine);

            
            // Mặt bằng dầm:
            // 2 đường dày bụng t type HIDDEN2 ;
            // 2 đường rộng cánh B type CONTINUOUS 
            
            // Tạo đoạn đường offset 
            // Offset the object t in the first direction
            double offsetDistance = HBtf[2] / 2;
            
            // base line start to end: offsetDistance + on left, - right side
            CommonUtil.AddOffsetLine(baseLine, offsetDistance, Color.FromRgb(0, 255, 0), "HIDDEN2");
            
            // Now offset the object t in the second direction
            CommonUtil.AddOffsetLine(baseLine, -offsetDistance, Color.FromRgb(0, 255, 0), "HIDDEN2");
            


            // Offset the object B in the first direction
            offsetDistance = HBtf[1] / 2;
            Point3d pt1, pt2, pt3, pt4;
            foreach (Entity acEnt in baseLine.GetOffsetCurves(offsetDistance))
            {
                Line offsetLine1 = (Line)acEnt;
                pt1 = offsetLine1.StartPoint;
                pt2 = offsetLine1.EndPoint;
                offsetLine1.Color = Color.FromRgb(0, 255, 0);  // green

                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, offsetLine1);
            }
            // Now offset the object B in the second direction
            foreach (Entity acEnt in baseLine.GetOffsetCurves(-offsetDistance))
            {
                Line offsetLine2 = (Line)acEnt;
                pt3 = offsetLine2.StartPoint;
                pt4 = offsetLine2.EndPoint;
                offsetLine2.Color = Color.FromRgb(0, 255, 0);  // green

                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, offsetLine2);
            }

            // 2 dau dam/mat bang
            Line line1 = new Line(pt1, pt3);
            line1.Color = Color.FromRgb(0, 255, 0);
            CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, line1);
            Line line2 = new Line(pt2, pt4);
            line2.Color = Color.FromRgb(0, 255, 0);
            CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, line2);
            
            
            // Mặt đứng dầm:
            // 2 đường cánh trên f  type CONTINUOUS 
            // 2 đường cánh dưới f  type CONTINUOUS 

            
            CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[1] / 2), Color.FromRgb(0, 255, 0));

            CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[3] + HBtf[1] / 2), Color.FromRgb(0, 255, 0));

            CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[0] + HBtf[1] / 2), Color.FromRgb(0, 255, 0));

            CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[0] - HBtf[3] + HBtf[1] / 2), Color.FromRgb(0, 255, 0));

            

            // find a point with base point and vector



        }

        // s  SH-800X300X16X32
        static int[] GetProfile(string s)
        {
            int[] HBtf = new int[4];
            for (int i = 0; i < HBtf.Length; i++)
                HBtf[i] = 0;
            
            s = s.Trim().Replace('x', 'X');
            s = s.Replace(" ", ""); // 20230903
            var temp1 = s.Split('-');
            if (temp1.Length == 1)  // 800X300X16X32
            {
                var temp2 = temp1[0].Split('X');
                if (temp2.Length < 4)
                {
                    return HBtf;
                }
                HBtf[0] = int.Parse(temp2[0]);
                HBtf[1] = int.Parse(temp2[1]);
                HBtf[2] = int.Parse(temp2[2]);
                HBtf[3] = int.Parse(temp2[3]);
            }
            else if (temp1.Length == 2) // SH-800X300X16X32
            {
                var temp2 = temp1[1].Split('X');
                if (temp2.Length < 4)
                {
                    return HBtf;
                }
                HBtf[0] = int.Parse(temp2[0]);
                HBtf[1] = int.Parse(temp2[1]);
                HBtf[2] = int.Parse(temp2[2]);
                HBtf[3] = int.Parse(temp2[3]);
            }

            return HBtf;
        }


       




    }

    /*
    CAD 2010 và CAD 2012 (đều là R18) xài cùng bộ thư viện nên xài chung dc khỏi làm 2 projoect mắc công Big Grin
    CAD 2013 (R19) xài .NET 4.0 nên và một số namespace đã được chuyển sang accoremgd.dll nên 2013 phải build riêng.
     */
}
