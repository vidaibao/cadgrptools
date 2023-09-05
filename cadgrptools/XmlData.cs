using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using CADDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace cadgrptools
{


    public class XmlData
    {
        public int Option { get; set; }
        public string Comment { get; set; }


        public XmlData()
        {
            // Hàm tạo mặc định
        }

        public XmlData(int option, string comment)
        {
            Option = option;
            Comment = comment;
        }


        public static MulConfig ReadXml()
        {
            
            int opt = 0;
            string cmt = "", center = "", hidden = "";

            XmlTextReader xtr = new XmlTextReader("cadgrpproperties.xml");
            while (xtr.Read()) // read next node from the stream
            {

                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "Option")
                {
                    opt = int.Parse(xtr.ReadElementString().Trim()); // read a text only element
                    //ed.WriteMessage("\nOption = " + opt);
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "Comment")
                {
                    cmt = xtr.ReadElementString().Trim();
                    //ed.WriteMessage("\nComment = " + cmt);
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "center")
                {
                    center = xtr.ReadElementString().Trim();
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "hidden")
                {
                    hidden = xtr.ReadElementString().Trim();
                }
            }


            xtr.Close();
            return new MulConfig(opt, cmt, center, hidden);

            
        }



        public void SerializeToXml(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(XmlData));
                    serializer.Serialize(fs, this);
                }
                Console.WriteLine("Ghi dữ liệu vào tệp XML thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        public static XmlData DeserializeFromXml(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(XmlData));
                    return (XmlData)serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return null;
            }
        }
    }
}




/*
class Program
{
    static void Main(string[] args)
    {
        // Tạo một đối tượng XmlData
        XmlData dataToWrite = new XmlData(1, "Đây là một comment");

        // Ghi dữ liệu vào tệp XML
        dataToWrite.SerializeToXml("cadgrpmulproperties.xml");

        // Đọc dữ liệu từ tệp XML
        XmlData dataToRead = XmlData.DeserializeFromXml("data.xml");

        if (dataToRead != null)
        {
            Console.WriteLine($"Option: {dataToRead.Option}");
            Console.WriteLine($"Comment: {dataToRead.Comment}");
        }

        Console.ReadLine();
    }
}
*/