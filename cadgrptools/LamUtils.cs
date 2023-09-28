using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADDB;

namespace cadgrptools
{
    public class LamUtils
    {
        Dictionary<string, string> data = new Dictionary<string, string>();


        [CommandMethod("mul")]
        public static void Draw9lines()
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // set the color Green current with the CECOLOR system variable
            Application.SetSystemVariable("CECOLOR", "3"); //https://help.autodesk.com/view/OARX/2023/ENU/?guid=GUID-E50CC1AF-9448-4F30-9A2A-9142AB57163D


            // Application.SetSystemVariable("CELTYPE", "Center");

            // Read properties
            string filePath = ConfigFileRW.GetDLLFilePath("cadgrptoolconfig.txt");
            Dictionary<string, string> configData = ConfigFileRW.ReadKeyValueFromFile(filePath);

            // In ra màn hình để kiểm tra
            foreach (var kvp in configData)
            {
                ed.WriteMessage($"\n{kvp.Key}: {kvp.Value}");
            }

            //20230906 
            CommonUtil.CheckLinetype(HostApplicationServices.WorkingDatabase, configData["hidden"]);
            int configOption = int.Parse(configData["option"]);
            if (configOption != 1 && configOption != 3 &&
                configOption != 21 && configOption != 22)
            {
                ed.WriteMessage("\nCould not get option value from cadgrpproperties.xml");
                return;
            }
            else 
            {
                ed.WriteMessage($"\nOption default value is: {configData["option"]}");
                ed.WriteMessage($"\nOption description: {configData["desc"]}"); // sai dau ':' - Mat bang dam; 21-22 for E truc doc - Mat dung dam (21: canh 1 net; 22: 2 net); 3 for Detail - Mat dung va Mat bang

            }



            Point3d point1 = new Point3d();
            Point3d point2 = new Point3d();
            Get2Points(ed, ref point1, ref point2);

            // TODO: Choose input from copy-paste or click on ModelSpace?
            int res = CommonUtil.GetIntegerFromUser(doc, "\nChoose 1 or 2 to input profile format: SH- 800X300 X16 X32 or 800X300x16*32 \n(1 - Click on text profile; <2 - Enter or Paste profile>)  \n1 or 2 <2>: ");
            string profile = "";
            if (res == 1)
            {
                profile = CommonUtil.UserSelectText(doc);// chon 2 text thi sao?
            }
            else
            {
                profile = CommonUtil.GetStringFromUser(doc, "\nEnter profile (format: SH- 800X300 X16 X32 or 800X300x16*32)");
            }


            // get profile of beam, gear from dialog box MS VBasic
            //string profile = Interaction.InputBox("Enter profile (format: SH- 800X300 X16 X32 or 800X300x16*32)", "Profile");

            // get HBtf    H-800X300 X16X32
            var HBtf = GetProfile(profile.Trim());

            if (!CheckProfile(HBtf))
            {
                return;
            }
            
            ed.WriteMessage("\nProfile: " + string.Join("x", HBtf.Select(x => x.ToString()).ToArray()));

            // draw BASE lines 
            Line baseLine = new Line(point1, point2);
            if (configOption == 1 || configOption == 3)
            {
                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, baseLine);
            }

            if (configOption == 1)
            {
                Draw4MasterPlan(baseLine, HBtf);
            }
            else if (configOption == 21)
            {
                Draw4EColumn(baseLine, HBtf);
            }
            else if (configOption == 22)
            {
                Draw4EColumn(baseLine, HBtf);
                Draw4EColumnInnerF(baseLine, HBtf);
            }
            else if (configOption == 3)
            {
                Draw4Detail(baseLine, HBtf, configData["hidden"]);
            }

                       

        }





        private static bool CheckProfile(int[] hBtf)
        {
            foreach (var i in hBtf)
            {
                if (i <= 0)
                {
                    //ed.WriteMessage($"\nHBtf = {i} >> Error");
                    return false;
                }
            }
            return true;
        }

        private static void Draw4Detail(Line baseLine, int[] HBtf, string hiddenType)
        {

            // Mặt bằng dầm:
            // 2 đường dày bụng t type HIDDEN2 ;
            // 2 đường rộng cánh B type CONTINUOUS 

            // Tạo đoạn đường offset 
            // Offset the object t in the first direction
            double offsetDistance = HBtf[2] / 2;

            // base line start to end: offsetDistance + on left, - right side
            CommonUtil.AddOffsetLine(baseLine, offsetDistance, hiddenType);

            // Now offset the object t in the second direction
            CommonUtil.AddOffsetLine(baseLine, -offsetDistance, hiddenType);


            Draw4MasterPlan(baseLine, HBtf);

            // Mặt đứng dầm:
            Draw4EColumn(baseLine, HBtf);

            // 2 đường trong cánh trên - dưới f  type CONTINUOUS 
            Draw4EColumnInnerF(baseLine, HBtf);

            

        }

        private static void Draw4EColumn(Line baseLine, int[] HBtf)
        {
            // Mặt đứng dầm:
            // 2 đường ngoai cánh trên - dưới  f  type CONTINUOUS 
            Point3d pt1 = new Point3d(), pt2 = new Point3d(), pt3 = new Point3d(), pt4 = new Point3d();
            //CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[1] / 2));
            foreach (Entity acEnt in baseLine.GetOffsetCurves(-(2500.0 + HBtf[1] / 2)))
            {
                Line upperFLine = (Line)acEnt;
                pt1 = upperFLine.StartPoint;
                pt2 = upperFLine.EndPoint;

                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, upperFLine);
            }

            foreach (Entity acEnt in baseLine.GetOffsetCurves(-(2500.0 + HBtf[0] + HBtf[1] / 2)))
            {
                Line lowerFLine = (Line)acEnt;
                pt3 = lowerFLine.StartPoint;
                pt4 = lowerFLine.EndPoint;

                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, lowerFLine);
            }

            // 2 bounder line - mat dung
            Line line3 = new Line(pt1, pt3);
            //CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, line3);
            Line line4 = new Line(pt2, pt4);
            CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, line3, line4);//20230906

        }

        private static void Draw4EColumnInnerF(Line baseLine, int[] HBtf)
        {
            CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[3] + HBtf[1] / 2));

            CommonUtil.AddOffsetLine(baseLine, -(2500.0 + HBtf[0] - HBtf[3] + HBtf[1] / 2));
        }

        private static void Draw4MasterPlan(Line baseLine, int[] HBtf)
        {

            // Offset the object B in the first direction
            double offsetDistance = HBtf[1] / 2;
            Point3d pt1 = new Point3d(), pt2 = new Point3d(), pt3 = new Point3d(), pt4 = new Point3d();
            foreach (Entity acEnt in baseLine.GetOffsetCurves(offsetDistance))
            {
                Line offsetLine1 = (Line)acEnt;
                pt1 = offsetLine1.StartPoint;
                pt2 = offsetLine1.EndPoint;

                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, offsetLine1);
            }
            // Now offset the object B in the second direction
            foreach (Entity acEnt in baseLine.GetOffsetCurves(-offsetDistance))
            {
                Line offsetLine2 = (Line)acEnt;
                pt3 = offsetLine2.StartPoint;
                pt4 = offsetLine2.EndPoint;

                CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, offsetLine2);
            }

            // 2 dau dam/mat bang
            Line line1 = new Line(pt1, pt3);
            //CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, line1);
            Line line2 = new Line(pt2, pt4);
            CommonUtil.AddToModelSpace(HostApplicationServices.WorkingDatabase, line1, line2);//20230906


        }

        static void Get2Points(Editor ed, ref Point3d point1, ref Point3d point2)
        {
            // get 2 points from acad model screen
            PromptPointOptions basePO = new PromptPointOptions("\nPick the start point:");
            PromptPointResult basePR = ed.GetPoint(basePO);

            if (basePR.Status != PromptStatus.OK)
                return;

            point1 = basePR.Value;

            // Chọn điểm thứ 2
            PromptPointOptions point2Options = new PromptPointOptions("\nPick the 2nd point: ");
            point2Options.UseBasePoint = true;
            point2Options.BasePoint = point1;
            PromptPointResult point2Result = ed.GetPoint(point2Options);
            if (point2Result.Status != PromptStatus.OK)
                return;

            point2 = point2Result.Value;

        }


        // s  SH-800X300X16X32
        static int[] GetProfile(string s)
        {
            int[] HBtf = new int[4];
            for (int i = 0; i < HBtf.Length; i++)
                HBtf[i] = 0;
            
            s = s.Replace('*', 'X');
            s = s.Replace('x', 'X');
            s = s.Replace(" ", string.Empty); // 20230903
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
