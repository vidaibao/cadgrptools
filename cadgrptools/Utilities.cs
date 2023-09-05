using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cadgrptools
{
    internal class Utilities
    {
        /* Give it a Database and the name of a Layout (the name that appears in
        the layout tab), and the ObjectID of the BlockTableRecord is returned. Of course, if the Layout
        name does not exist, a Null ObjectID is returned 
            DrawOnLayout2()
        */
        public static ObjectId GetBtrIDByLayoutName(Database DBIn, string layoutName)
        {
            using(Transaction tr = DBIn.TransactionManager.StartTransaction())
            {
                DBDictionary dBDictionaryEntries = DBIn.LayoutDictionaryId.GetObject(OpenMode.ForRead) as DBDictionary;
                if (dBDictionaryEntries.Contains(layoutName))
                {
                    Layout layout = dBDictionaryEntries.GetAt(layoutName).GetObject(OpenMode.ForRead) as Layout;
                    return layout.BlockTableRecordId;
                }
                else
                {
                    return ObjectId.Null;
                }
            }
        }







    }
}
