using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Odbc;
using System.Data;
using System.Windows.Forms;

namespace Maze.Classes
{
    public class WorkDBF
    {
        private OdbcConnection Conn = null;

        public WorkDBF(string BaseDirectoryPath)
        {
            this.Conn = new System.Data.Odbc.OdbcConnection();
            /*Conn.ConnectionString = @"Driver={Microsoft dBase  Driver (*.dbf)};" +
                   "SourceType=DBF;Exclusive=No;" +
                   "Collate=Machine;NULL=NO;DELETED=NO;" +
                   "BACKGROUNDFETCH=NO;";*/
            //Conn.ConnectionString = @"DSN=EffectDataSource;DBQ=D:\;DefaultDir=D:\;DriverId=533;FIL=dBase 5.0;MaxBufferSize=2048;PageTimeout=5;";
            Conn.ConnectionString = @"DSN=EffectDataSource;DBQ=" + BaseDirectoryPath + ";DefaultDir=" + BaseDirectoryPath + ";DriverId=533;FIL=dBase 5.0;MaxBufferSize=2048;PageTimeout=5;";
        }

        public void CreateTable(string TableName)
        {
            using (var dBaseConnection = new OdbcConnection(Conn.ConnectionString)) 
            { 
                dBaseConnection.Open(); 
                string str0 = "Create Table " + TableName + "(EffectId char(4), EffectName char(64), EffectType char(3), EffectValue char(10), Duration char(5), Description char(254))";
                var cmd = new OdbcCommand(str0, dBaseConnection); 
                cmd.ExecuteNonQuery(); 
            }
        }

        public void FillTable(string TableName, string EffectId, string EffectName, string EffectType, 
            string EffectValue, string Duration, string Description)
        {
            using (var dBaseConnection = new OdbcConnection(Conn.ConnectionString))
            {
                dBaseConnection.Open();
                string str0 = "Insert INTO " + TableName + "(EffectId, EffectName, EffectType, EffectValue," +
                    "Duration, Description)" +
                    "VALUES ('" + EffectId + "','" + EffectName + "','" + EffectType + "','" + EffectValue + "','" +
                    Duration + "','" + Description + "')";
                var cmd = new OdbcCommand(str0, dBaseConnection);
                cmd.ExecuteNonQuery();
               
            }
        }

        public void FillTable(string TableName, int EffectId, string EffectName, int Attributes, int Targets, 
            int Range, int EffectType, int EffectValue, int Duration, string Description)
        {
            if(IdSearcher(GetAll(TableName),EffectId))
                using (var dBaseConnection = new OdbcConnection(Conn.ConnectionString))
                {
                    dBaseConnection.Open();
                    string str0 = "Insert INTO " + TableName + "(EffectId, EffectName, Attributes, Targets," +
                    "Range, EffectType, EffectValue, Duration, Description)" +
                    "VALUES (" + EffectId + ",'" + EffectName + "'," + Attributes + "," + Targets +
                    "," + Range + "," + EffectType + "," + EffectValue + "," + Duration + ",'" + Description + "')";
                
                    var cmd = new OdbcCommand(str0, dBaseConnection);
                    cmd.ExecuteNonQuery();

                }
        }

        public DataTable Execute(string Command)
        {
            DataTable dt = null;
            if (Conn != null)
            {
                try
                {
                    Conn.Open();
                    dt = new DataTable();
                    System.Data.Odbc.OdbcCommand oCmd = Conn.CreateCommand();
                    oCmd.CommandText = Command;
                    dt.Load(oCmd.ExecuteReader());
                    Conn.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return dt;
        }

        public DataTable GetAll(string DB_path)
        {
            return Execute("SELECT * FROM " + DB_path);
        }

        public bool IdSearcher(DataTable table, int strId)
        {
            using (DataTableReader tr = table.CreateDataReader())
            {
                bool flag = true;
                while (flag)
                {
                    if (tr.Read() /*&& tr.HasRows*/)
                    {
                        int EId = System.Convert.ToInt32(tr["EffectId"]);
                        if (strId.Equals(EId))
                        {
                            flag = false;
                            return false;
                        }
                    }
                    else
                        flag = false;
                }
            }
            return true;
        }
    }
}
