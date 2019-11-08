using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace DbfUploader
{
    class DbConnect
    {
        //private const string Server = "localhost";
        //private const string Database = "prod_sait";
        //private const string UserID = "root";
        //private const string Pass = "root";
        //private const string Port = "3306";

        public MySqlConnection mySqlConnection;//declare variable for connection
        public MySqlCommand mySqlCommand;//varialbe for command
        public List<string> columnsName;
        public List<string> columnType;
        public int size;
        public int count=0;

        public DbConnect()
        {
            //This will call the other constructor with the const values
        }

        //public DbConnect(string server,string port, string userID, string pass,string database)
        //{
        //    mySqlConnection = CreateConnection(server,port ,userID,pass, database);
        //}

        public void CreateDb(string dataBaseName)
        {
            Console.WriteLine("Enterint to the method were is Creating the fucking database");
            BuildSql builder = new BuildSql();
            /*mySqlConnection = CreateConnectionToLocalhost(ip, port, user, pass);*/
            System.IO.File.WriteAllText(@"C:\opt\createdb.sql", builder.CreateDataBase(dataBaseName));
           /* if (InitSqlCommand(builder.CreateDataBase(dataBaseName)) == 1)
            {
                MessageBox.Show("successfully created a database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show("There was an error creating the database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/

        }

        public void CreateTables(string dataBaseName, string path)
        {
            BuildSql builder = new BuildSql();
            /*mySqlConnection = CreateConnection(ip, port, user, pass, dataBaseName);*/
            
            System.IO.File.WriteAllText(@"C:\opt\"+ Path.GetFileName(path).Replace(".dbf", "").Replace(".DBF", "") +".sql", DbfConnection(path));
            
            /*if (InitSqlCommand(DbfConnection(path)) == 1)
            {
                Console.WriteLine("successfully created a database");
            }
            else {
                MessageBox.Show("Error creating table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
            
        }

        public void InsertData(string dataBaseName, string path)
        {
            BuildSql builder = new BuildSql();
            //List<string> columnsName = new List<string>();
            List<string> dataQuery = new List<string>();
            DbfColumnGetter(path);
            dataQuery = DbfDataGetter(path, columnsName, columnType);
            /*mySqlConnection = CreateConnection(ip, port, user, pass, dataBaseName);*/
            foreach (string query in dataQuery) {
                try {

                    System.IO.File.AppendAllText(@"C:\opt\" + Path.GetFileName(path).Replace(".dbf", "").Replace(".DBF", "") + "Inserts.sql", query+"\n");
                    /*if (InitSqlCommand(query) == 1)
                    {

                    }
                    else {
                        MessageBox.Show("Error inserting data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }*/




                }
                catch (Exception ex) {
                    Console.WriteLine("---------------");
                    Console.WriteLine(ex);
                    Console.WriteLine("---------------");
                }
                

                
            }
            
            //builder.CreateInsertSql("",columnsName,data);
        }

        

        public void DbfColumnGetter(string path)
        {
            columnsName = new List<string>();
            columnType = new List<string>();
            OleDbConnection conDBF;
            OleDbDataReader myReader;
            DataTable columnsTable = new DataTable();
            DataTable dataTable = new DataTable();
            conDBF = new OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=" + path + ";deleted=false;");
            conDBF.Open();

            string sqlEventos = "select *from " + Path.GetFileName(path);
            string tableName = Path.GetFileName(path).Replace(".dbf", "").Replace(".DBF", "");
            OleDbCommand query = new OleDbCommand();
            query.Connection = conDBF;
            query.CommandText = sqlEventos;
            myReader = query.ExecuteReader(CommandBehavior.KeyInfo);
            columnsTable = myReader.GetSchemaTable();
            foreach (DataRow myField in columnsTable.Rows)
            {
                if (myField["ColumnName"].ToString().Equals("desc"))
                {
                    string descrip = "descripcion";
                    columnsName.Add(descrip);
                    columnType.Add(myField["DataType"].ToString());
                }
                else
                {
                    columnsName.Add(myField["ColumnName"].ToString());
                    columnType.Add(myField["DataType"].ToString());
                }

            }
            //End column reader

            //Closing connection reader and dbf connection
            myReader.Close();
            conDBF.Close();
            //return columnsName;
        }



        public List<string> DbfDataGetter(string path, List<string> columns, List<string> columnType)
        {
            List<string> dataList = new List<string>();
            List<string> queryList = new List<string>();
            OleDbConnection conDBF;

            BuildSql builder = new BuildSql();
            DataTable columnsTable = new DataTable();
            DataTable dataTable = new DataTable();
            OleDbDataReader myReader;
            conDBF = new OleDbConnection(@"Provider = VFPOLEDB.1; Data Source = " + path + "; Extended Properties = dBase IV;deleted = false;");
            //conDBF = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";deleted=false;");
            conDBF.Open();

            string sqlEventos = "select * from " + Path.GetFileName(path);
            string tableName = Path.GetFileName(path).Replace(".dbf", "").Replace(".DBF", "");
            OleDbCommand query = new OleDbCommand();
            query.Connection = conDBF;
            query.CommandText = sqlEventos;

            //End column reader

            //Start data reader
            OleDbDataAdapter data = new OleDbDataAdapter(query);

            myReader = query.ExecuteReader(CommandBehavior.KeyInfo);
            //Retrieve column schema into a DataTable.
            dataTable = myReader.GetSchemaTable();

            data = new OleDbDataAdapter(query);
            dataTable = FPReaderToDataTable(myReader, tableName);
            //data.Fill(dataTable);

            size = dataTable.Rows.Count;

            foreach (DataRow row in dataTable.Rows)
            {
                dataList = new List<string>();
                for (int i = 0; i < columns.Count; i++)
                {

                    if (columnType[i].Equals("System.String"))
                    {
                        if (columns[i].ToString().Equals("descripcion"))
                        {
                            dataList.Add("'" + row["desc"].ToString().Replace("'", "''") + "'");
                        }
                        else
                        {
                            dataList.Add("'" + row[columns[i]].ToString().Replace("'", "''") + "'");
                        }

                    }
                    if (columnType[i].Equals("System.DateTime"))
                    {
                        DateTime dt;
                        DateTime.TryParse(row[columns[i]].ToString().Substring(0, 10), out dt);
                        string s = dt.ToString("yyyy-MM-dd");
                        //dataList.Add("'" + row[columns[i]].ToString().Substring(0, 10) + "'");
                        dataList.Add("'" + s + "'");
                    }
                    if (columnType[i].Equals("System.Decimal"))
                    {
                        if (row[columns[i]].ToString().Equals(""))
                        {
                            dataList.Add("0.0");
                        }
                        else {
                            dataList.Add(row[columns[i]].ToString());
                        }
                        
                    }

                    if (columnType[i].Equals("System.Boolean"))
                    {
                        dataList.Add(row[columns[i]].ToString());
                    }


                }

                queryList.Add(builder.CreateInsertSql(tableName, columns, dataList));

            }
            //Closing dbf connection

            conDBF.Close();
            return queryList;
        }

        private DataTable FPReaderToDataTable(OleDbDataReader dr, string TableName)
        {
            DataTable dt = new DataTable();

            //get datareader schema
            DataTable SchemaTable = dr.GetSchemaTable();
            List<DataColumn> cols = new List<DataColumn>();
            if (SchemaTable != null)
            {
                foreach (DataRow drow in SchemaTable.Rows)
                {
                    string columnName = drow["ColumnName"].ToString();
                    DataColumn col = new DataColumn(columnName, (Type)(drow["DataType"]));
                    col.Unique = (bool)drow["IsUnique"];
                    col.AllowDBNull = (bool)drow["AllowDBNull"];
                    col.AutoIncrement = (bool)drow["IsAutoIncrement"];
                    cols.Add(col);
                    dt.Columns.Add(col);
                }
            }

            //populate data
            int RowCount = 1;
            while (dr.Read())
            {
                DataRow row = dt.NewRow();

                for (int i = 0; i < cols.Count; i++)
                {
                    try
                    {
                        row[((DataColumn)cols[i])] = dr[i];
                    }
                    catch (Exception ex)
                    {
                        if (i > 0)
                        {
                            Console.WriteLine(TableName);
                            Console.WriteLine(cols[i].ColumnName);
                            Console.WriteLine(RowCount);
                            Console.WriteLine(ex.ToString());
                            Console.WriteLine(dr[0].ToString());

                            // LogImportError(TableName, cols[i].ColumnName, RowCount, ex.ToString(), dr[0].ToString());
                        }
                        else
                        {
                            Console.WriteLine(TableName);
                            Console.WriteLine(cols[i].ColumnName);
                            Console.WriteLine(RowCount);
                            Console.WriteLine(ex.ToString());

                            // LogImportError(TableName, cols[i].ColumnName, RowCount, ex.ToString(), "");
                        }
                    }
                }
                RowCount++;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public string DbfConnection(string path)
        {
            List<string> columnsName = new List<string>();
            List<string> columnType = new List<string>();
            List<string> isLong = new List<string>();
            List<string> columnSize = new List<string>();
            OleDbConnection conDBF;
            OleDbDataReader myReader;
            BuildSql builder = new BuildSql();

            DataTable dataTable = new DataTable();

            conDBF = new OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=" + path + ";deleted=false;");
            conDBF.Open();

            string sqlEventos = "select *from " + Path.GetFileName(path);
            string tableName = Path.GetFileName(path).Replace(".dbf", "").Replace(".DBF", "");
            OleDbCommand query = new OleDbCommand();
            query.Connection = conDBF;
            query.CommandText = sqlEventos;
            //OleDbDataAdapter da = new OleDbDataAdapter(query);
            //da = new OleDbDataAdapter(query);
            //da.Fill(eventosTable);
            myReader = query.ExecuteReader(CommandBehavior.KeyInfo);
            //Retrieve column schema into a DataTable.
            dataTable = myReader.GetSchemaTable();

            //For each field in the table...
            foreach (DataRow myField in dataTable.Rows)
            {

                columnsName.Add(myField["ColumnName"].ToString());
                columnType.Add(myField["DataType"].ToString());
                isLong.Add(myField["IsLong"].ToString());
                columnSize.Add(myField["ColumnSize"].ToString());


            }
            foreach (DataColumn myProperty in dataTable.Columns)
            {
                //Display the field name and value.
                foreach (DataRow myField in dataTable.Rows)
                {
                    Console.WriteLine(myProperty.ColumnName + " = " + myField[myProperty].ToString());
                }

            }


            string firstCreate = builder.CreateTableSql(tableName.ToLower(), isLong.ToArray(), columnSize.ToArray(), columnsName.ToArray(), columnType.ToArray());

            ///Console.WriteLine(firstCreate);
            //CreateTables(firstCreate);
            //Always close the DataReader and connection.
            myReader.Close();
            conDBF.Close();
            return firstCreate;
        }

        public void insertToMysql(DataTable data)
        {
            foreach (DataRow row in data.Rows)
            {

                //Console.WriteLine(row["tipo"]);
                //Console.WriteLine(row["enviado"]);
                //Console.WriteLine(row["hora"]);
                //Console.WriteLine(row["suc"]);
                //Console.WriteLine(row["id"]);

            }
        }

        private MySqlConnection CreateConnection(string server, string port, string userID, string pass, string database)
        {
            Console.Write(server, port, userID, pass, database);
            var connectionString = String.Format("server={0};port={1};user={2};password={3};database={4}", server, port, userID, pass, database);
            return new MySqlConnection(connectionString);
        }

        private MySqlConnection CreateConnectionToLocalhost(string server, string port, string userID, string pass)
        {
            var connectionString = String.Format("server={0};port={1};user={2};password={3}", server, port, userID, pass);

            return new MySqlConnection(connectionString);
        }

        public int InitSqlCommand(string query)
        {
            try {
                mySqlConnection.Open();
                var mySqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception ex) {
               MessageBox.Show("Cannot connect to mysql ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            
            return 1;
        }

      

        private void FinalizeConnection()
        {
            if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Closed)
                mySqlConnection.Dispose();
        }
    }
}

//if (myField["IsLong"].ToString().Equals("True")) {
//    Console.WriteLine(myField["ColumnName"].ToString());
//    Console.WriteLine(myField["DataType"].ToString());
//    Console.WriteLine(myField["IsLong"].ToString());
//}
//Console.WriteLine(myField["ColumnName"].ToString());
//Console.WriteLine(myField["DataType"].ToString());
//Console.WriteLine(myField["IsLong"].ToString());

//////For each property of the field...
//foreach (DataColumn myProperty in eventosTable.Columns)
//{
//    //Display the field name and value.
//    Console.WriteLine(myProperty.ColumnName + " = " + myField[myProperty].ToString());
//}
//Console.WriteLine();

////Pause.
//Console.ReadLine();