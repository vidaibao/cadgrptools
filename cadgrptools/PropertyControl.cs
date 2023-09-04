using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;


namespace cadgrptools
{





    public class PropertyControl
    {
        string fileMulPath = "cadgrpmulproperties.xml";

        public PropertyControl() 
        {
            // Tạo một đối tượng XmlData
            //XmlData dataToWrite = new XmlData(1, "Đây là một comment");

            // Ghi dữ liệu vào tệp XML
            //dataToWrite.SerializeToXml("data.xml");



            // Đọc dữ liệu từ tệp XML
            XmlData dataToRead = XmlData.DeserializeFromXml(fileMulPath);

            if (dataToRead != null)
            {
                Console.WriteLine($"Option: {dataToRead.Option}");
                Console.WriteLine($"Comment: {dataToRead.Comment}");
            }
        }


         

    }
}

