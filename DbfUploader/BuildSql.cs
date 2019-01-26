using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbfUploader
{
    class BuildSql
    {

        public string CreateDataBase(string databaseName) {
            string sql = "";
            sql = "CREATE DATABASE IF NOT EXISTS " + databaseName+" CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            return sql;
        }

        public string CreateTableSql(string tableName,string[] isLong,string[] columnSize,string[] columnName, string[] columnType) {
            string sql = "";
            string completeQuery = "";
            string headerQuery = " CREATE TABLE IF NOT EXISTS " + tableName + "  (id_" + tableName + " int(11) NOT NULL AUTO_INCREMENT, ";
            string footerQuery = " PRIMARY KEY(id_" + tableName + ") ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci;";

            for (int x = 0; x < columnName.Length; x++) {
                sql = sql + this.ColumnType(isLong[x], columnSize[x], columnName[x], columnType[x]); ;
            }

            completeQuery = headerQuery + sql + footerQuery;
            return completeQuery;
        }

        public string CreateInsertSql(string tableName,List<string> columnData,List<string> data) {
            string sql = "";
            sql = String.Format("INSERT INTO "+ tableName + " ({0}) VALUES({1}) ;",
               ProcessList(columnData),
               ProcessList(data));
            Console.WriteLine(sql);
            return sql;
        }
        
        string ProcessList<T>(IEnumerable<T> enumerable)
        {
            List<T> list = new List<T>(enumerable);
            return string.Join(",", list.ToArray());
        }
        
        

        public string ColumnType(string isLong, string columnSize, string columnName, string columnType)
        {
            if (columnName.Equals("desc")) {
                columnName = "descripcion";
            }
            if (columnType.Equals("System.String"))
            {
                if (isLong.Equals("False"))
                {
                    return columnName + " varchar(" + columnSize + ") COLLATE utf8mb4_unicode_ci DEFAULT NULL, \n";
                }
                else
                {
                    return columnName + " LONGTEXT COLLATE utf8mb4_unicode_ci DEFAULT NULL, \n";
                }
            }
            if (columnType.Equals("System.DateTime"))
            {
                return columnName + " DATE DEFAULT NULL, \n";
            }
            if (columnType.Equals("System.Decimal"))
            {
                return columnName + " float DEFAULT NULL, \n";
            }

            if (columnType.Equals("System.Boolean"))
            {
                return columnName + " tinyint(1) DEFAULT NULL, \n";
            }
            return "";
        }

    }
}
