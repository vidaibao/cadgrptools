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

        /* Adding to BlockTableRecords:
         * We provide a Database, and Entity to add to the Block and a Block Name. 
         * If the block exists, we open it. 
         * If it does not exist, we create a new BlockTableRecord using the provided name, 
         * and add it to the BlockTable and the Transaction. 
        The Function “AddEntity” returns the ObjectID of the entity added to the Block.*/
        public static ObjectId AddEntity(Database db, Entity entityToAdd, string blockName)
        {
            using(Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForWrite) as BlockTable;
                BlockTableRecord btr = null;
                if (bt.Has(blockName))
                {
                    btr = bt[blockName].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                }
                else
                {
                    btr = new BlockTableRecord();
                    btr.Name = blockName;
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                }
                btr.AppendEntity(entityToAdd);
                tr.AddNewlyCreatedDBObject(entityToAdd,true);
                tr.Commit();
                return entityToAdd.Id;
            }
        }


        public static ObjectIdCollection AddEntities(Database db, DBObjectCollection entitiesToAdd, string blockName)
        {
            ObjectIdCollection retIDColl = new ObjectIdCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForWrite) as BlockTable;
                BlockTableRecord btr = null;
                if (bt.Has(blockName))
                {
                    btr = bt[blockName].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                }
                else
                {
                    btr = new BlockTableRecord();
                    btr.Name = blockName;
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                }

                foreach (DBObject myDBObject in entitiesToAdd)
                {
                    btr.AppendEntity((Entity)myDBObject);
                    tr.AddNewlyCreatedDBObject(myDBObject, true);
                    retIDColl.Add(myDBObject.Id);
                }
                
                tr.Commit();
                return retIDColl;
            }
        }


    }
}
