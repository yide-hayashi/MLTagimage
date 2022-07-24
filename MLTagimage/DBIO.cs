using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
//Microsoft.Data.SqlClient 是支援後面.NET Core的版本 System.Data.SqlClient是舊有.Net Framework 現在都用套件形式 要去nuget載
namespace auto_click
{
    class DBIO
    {
        /// <summary>
        /// pleace cread loadData(list object) before useing the load sqlData function
        ///  
        /// </summary>
        /// <param name="ls">引用 loadData 的資料格式近來</param>
        /// <returns></returns>
        public loadData sqlload(loadList ls)
        {
            int count = 0;
            loadData ld = new loadData();
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ls.connectionString;
            sqlConnection.Open();
            SqlCommand sc = new SqlCommand();
            sc=sqlConnection.CreateCommand();
            for(int i=0;i<ls.loadstring.Count;i++)
            {
                sc.CommandText = ls.loadstring[i].String;
                SqlDataReader dataReader = sc.ExecuteReader();

                    
                //丟資料到這邊的list
                while (dataReader.Read())
                {
                    count = 0;
                    foreach(var x in ls.loadstring[i].wantcolumns)
                    {
                        if (ld.datalist.Count ==0)
                        {
                            ld.datalist.Add(new Data());
                        }
                        ld.datalist[count].TableName ="";
                        ld.datalist[count].columeName = x;
                        ld.datalist[count].data.Add(dataReader[x]);
                    }
                    count++;
                }
                dataReader.Close();
            }
            sqlConnection.Close();
            return ld;
        }
        public void sqlsave(loadList ls)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ls.connectionString;
            sqlConnection.Open();
            SqlCommand sc = new SqlCommand();
            sc = sqlConnection.CreateCommand();
            foreach(var i in ls.loadstring)
            {
                sc.CommandText = i.String;
                sc.Parameters.Clear();
                for (int item=0;item <i.obj.Count;item++)
                {
                    sc.Parameters.AddWithValue(i.obj[item].obj_column, i.obj[item].objs);
                }
                sc.ExecuteNonQuery();
            }
            sqlConnection.Close();
        }
        public void sqlupdate(loadList ls)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ls.connectionString;
            sqlConnection.Open();
            SqlCommand sc = new SqlCommand();
            sc = sqlConnection.CreateCommand();
            sc.CommandText = ls.loadstring[0].String;
            sc.Parameters.Clear();
            for (int i=0; i < ls.loadstring[0].obj.Count; i++)
            {
               sc.Parameters.AddWithValue(ls.loadstring[0].obj[i].obj_column, ls.loadstring[0].obj[i].objs);
            }
            sc.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    #region dataMoudle
    /// <summary>
    /// sql連接字串以及相關資料
    /// the List is a string of connection and some date
    /// </summary>
    class loadList
    {
        public string connectionString;
        public List<loadstring> loadstring = new List<loadstring>(); 
    }
    class loadstring
    {
        /// <summary>
        /// sql commend string
        /// </summary>
        public string String;
        /// <summary>
        /// want to select table's colunm name
        /// </summary>
        public List<string> wantcolumns;
        /// <summary>
        /// the obj is  to the value of column  
        /// </summary>
        public List<obj> obj;
    }
    class obj
    {
        /// <summary>
        /// 物件的值
        /// </summary>
        public object objs;
        /// <summary>
        /// 值得所屬column
        /// </summary>
        public string obj_column;
    }
    /// <summary>
    /// 資料丟出去的格式
    /// </summary>
    class loadData
    {
       public List<Data> datalist=new List<Data>();
    }
    class Data
    {
        public string TableName;
        public string columeName;
       public List<object> data = new List<object>();
    }
    #endregion
}
