using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using Maze.Classes;

namespace Maze
{
    class ReadDataTable
    {
        EffectEntry[] sEffectStore;
        string tableName;
        private OdbcConnection conn;
        private const string BaseDirectoryPath = "Data\\Base\\";
        
        public ReadDataTable(string tableName)
        {
            this.tableName = tableName;

            this.conn = new System.Data.Odbc.OdbcConnection();

            // work
            conn.ConnectionString = @"DSN=EffectDataSource;DBQ=D:\МОЁ Ё-Ё\ИРА\REPO\DATATABLE\BIN\BASE;DefaultDir=D:\МОЁ Ё-Ё\ИРА\REPO\DATATABLE\BIN\BASE;DriverId=533;FIL=dBase 5.0;MaxBufferSize=2048;PageTimeout=5;UID=admin;";
            //doesn't work
            //conn.ConnectionString = @"DSN=EffectDataSource;DBQ=" + BaseDirectoryPath + ";DefaultDir=" + BaseDirectoryPath + ";DriverId=533;FIL=dBase 5.0;MaxBufferSize=2048;PageTimeout=5;UID=admin;";
            
            sEffectStore = new EffectEntry[GetRowCount(tableName)];
            FillArray(tableName);
        }

        public int GetRowCount(string tableName)
        {
            System.Data.DataTable dt = GetAll(tableName);
            return dt.Rows.Count;
        }

        public void FillArray(string tableName)
        {
            LookupEntry(GetAll(tableName));
        }

        public void LookupEntry(System.Data.DataTable table)
        {
            using (System.Data.DataTableReader tr = table.CreateDataReader())
            {
                bool flag = true;
                int i = 0;
                while (flag)
                {
                    if (tr.Read())
                    {
                        this.sEffectStore[i].ID = System.Convert.ToUInt16(tr["EffectId"]);
                        this.sEffectStore[i].EffectName = System.Convert.ToString(tr["EffectName"]);
                        this.sEffectStore[i].Attributes = System.Convert.ToUInt16(tr["Attributes"]);
                        this.sEffectStore[i].Targets = (EffectTargets)tr["Targets"];
                        this.sEffectStore[i].Range = System.Convert.ToInt16(tr["Range"]);
                        this.sEffectStore[i].EffectType = (EffectTypes)tr["EffectType"];
                        this.sEffectStore[i].Value = System.Convert.ToInt32(tr["EffectValue"]);
                        this.sEffectStore[i].Duration = System.Convert.ToInt16(tr["Duration"]);
                        this.sEffectStore[i].ND1 = System.Convert.ToInt16(tr["ND1"]);
                        this.sEffectStore[i].ND2 = System.Convert.ToInt16(tr["ND2"]);
                        this.sEffectStore[i].ND3 = System.Convert.ToInt16(tr["ND3"]);
                        this.sEffectStore[i].ND4 = System.Convert.ToInt16(tr["ND4"]);
                        this.sEffectStore[i].Description = System.Convert.ToString(tr["Description"]);
                        ++i;
                    }
                    else
                        flag = false;
                }
            }
         }


        public System.Data.DataTable GetAll(string dB_path)
        {
            return Execute("SELECT * FROM " + dB_path);
        }

        public System.Data.DataTable Execute(string command)
        {
            System.Data.DataTable dt = null;
            if (conn != null)
            {
                try
                {
                    conn.Open();
                    dt = new System.Data.DataTable();
                    System.Data.Odbc.OdbcCommand oCmd = conn.CreateCommand();
                    oCmd.CommandText = command;
                    dt.Load(oCmd.ExecuteReader());
                    conn.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return dt;
        }
    }
}
