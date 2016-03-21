using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using ProcCore.DatabaseCore.DatabaseName;

namespace ProcCore.DatabaseCore.DataBaseConnection
{
    /// <summary>
    /// BaseConnection主要在處理不同類型Database的連線字串，並回傳CommConnection物件
    /// </summary>
    public class BaseConnection
    {
        public String Server { get; set; }
        public String Account { get; set; }
        public String Password { get; set; }
        public String Path { get; set; }

        public CommConnection conn;
        public CommConnection GetConnection()
        {
            return GetConnection(DataBases.DB00S_C1305_SWIU);
        }
        public CommConnection GetConnection(DataBases DBName)
        {
            ConnectionType type = ConnectionType.SqlClient;

            if (DBName.ToString().Substring(4, 1) == "S")
                type = ConnectionType.SqlClient;

            if (DBName.ToString().Substring(4, 1) == "M")
                type = ConnectionType.MySqlClient;

            if (DBName.ToString().Substring(4, 1) == "O")
                type = ConnectionType.OleDb;

            Int32 _DotPos = DBName.ToString().IndexOf('_');
            String GetDBName = DBName.ToString().Substring(_DotPos + 1);

            String connectionstring = String.Empty;

            if (type == ConnectionType.SqlClient)
                connectionstring = String.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};MultipleActiveResultSets=True", Server, GetDBName, Account, Password);

            if (type == ConnectionType.OleDb)
            {
                //Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\myFolder\myAccess2007file.accdb;Persist Security Info=False;
                //Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\myFolder\myAccess2007file.accdb;Jet OLEDB:Database Password=MyDbPassword;
                //Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};

                String FilePath = Path + "_Code\\OleDB\\" + Server;
                //FilePath = "D:\\WebDB.accdb";
                connectionstring = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", FilePath);
            }

            if (type == ConnectionType.MySqlClient)
                connectionstring = String.Format("Server={0};Database={1};Uid={2};Pwd={3};", Server, GetDBName, Account, Password);

            if (conn == null)
                conn = new CommConnection(connectionstring, type);

            return conn;
        }
        public void CloseDB()
        {
            if (conn != null)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
    public class CommConnection : IDisposable
    {
        private OleDbConnection OleConn;
        private MySqlConnection MySqlConn;
        private SqlConnection SqlConn;

        private SqlTransaction _SqlTrans;
        private OleDbTransaction _OleTrans;
        private MySqlTransaction _MySqlTrans;

        private Dictionary<String, Object> _Params;

        public Object Connection
        {
            get
            {
                if (ConnectionType == ConnectionType.SqlClient)
                    return SqlConn;
                else if (ConnectionType == ConnectionType.OleDb)
                    return OleConn;
                else //if (connType == ConnectionDBType.MySql)
                    return MySqlConn;

            }
        }
        public Object Transaction
        {
            get
            {
                if (ConnectionType == ConnectionType.SqlClient)
                {
                    return _SqlTrans;
                }
                else if (ConnectionType == ConnectionType.OleDb)
                {
                    return _OleTrans;
                }
                else //if (connType == ConnectionDBType.MySql)
                {
                    return _MySqlTrans;
                }
            }
        }
        public String ConnectionString { get; set; }
        public void AddParm(String paramName, Object paramValue)
        {

            if (_Params == null)
                _Params = new Dictionary<String, Object>();

            if (_Params.ContainsKey(paramName))
            {
                _Params[paramName] = paramValue;
            }
            else
            {
                _Params.Add(paramName, paramValue);
            }
        }

        public ConnectionType ConnectionType { get; set; }
        public System.Data.ConnectionState State
        {
            get
            {
                if (ConnectionType == ConnectionType.SqlClient)
                {
                    return SqlConn.State;
                }
                else if (ConnectionType == ConnectionType.OleDb)
                {
                    return OleConn.State;
                }
                else if (ConnectionType == ConnectionType.MySqlClient)
                {
                    return MySqlConn.State;
                }
                else
                {
                    return SqlConn.State;
                }
            }
        }

        public CommConnection(String connectionString)
        {
            SqlConn = new SqlConnection(connectionString);
            ConnectionString = connectionString;
            ConnectionType = ConnectionType.SqlClient;
            MustRunTrans = false;
        }
        public CommConnection(String connectionString, ConnectionType type)
        {
            ConnectionType = type;
            MustRunTrans = false;
            ConnectionString = connectionString;

            if (type == ConnectionType.SqlClient)
                SqlConn = new SqlConnection(connectionString);
            else if (type == ConnectionType.OleDb)
                OleConn = new OleDbConnection(connectionString);
            else if (type == ConnectionType.MySqlClient)
                MySqlConn = new MySqlConnection(connectionString);
        }

        public void Tran()
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                if (SqlConn.State == ConnectionState.Closed) SqlConn.Open();
                _SqlTrans = SqlConn.BeginTransaction();
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                if (OleConn.State == ConnectionState.Closed) OleConn.Open();
                _OleTrans = OleConn.BeginTransaction();
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                if (MySqlConn.State == ConnectionState.Closed) MySqlConn.Open();
                _MySqlTrans = MySqlConn.BeginTransaction();
            }
        }
        public void Commit()
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                _SqlTrans.Commit();
                if (SqlConn.State == ConnectionState.Open) SqlConn.Close();
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                _OleTrans.Commit();
                if (OleConn.State == ConnectionState.Open) OleConn.Close();
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                _MySqlTrans.Commit();
                if (MySqlConn.State == ConnectionState.Open) MySqlConn.Close();
            }
        }
        public void Roll()
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                if (_SqlTrans != null)
                    _SqlTrans.Rollback();

                if (SqlConn.State == ConnectionState.Open)
                    SqlConn.Close();
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                _OleTrans.Rollback();
                if (OleConn.State == ConnectionState.Open) OleConn.Close();
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                _MySqlTrans.Rollback();
                if (MySqlConn.State == ConnectionState.Open) MySqlConn.Close();
            }
        }

        /// <summary>
        /// 如果您要做BeginTransaction、EndCommit及Rollback，這個屬性要先設成true才會實際運做。預設是false。
        /// </summary>
        public Boolean MustRunTrans { get; set; }
        public void BeginTransaction()
        {
            if (MustRunTrans)
            {
                if (ConnectionType == ConnectionType.SqlClient)
                {
                    if (SqlConn.State == ConnectionState.Closed) SqlConn.Open();
                    _SqlTrans = SqlConn.BeginTransaction();
                }
                else if (ConnectionType == ConnectionType.OleDb)
                {
                    if (OleConn.State == ConnectionState.Closed) OleConn.Open();
                    _OleTrans = OleConn.BeginTransaction();
                }
                else if (ConnectionType == ConnectionType.MySqlClient)
                {
                    if (MySqlConn.State == ConnectionState.Closed) MySqlConn.Open();
                    _MySqlTrans = MySqlConn.BeginTransaction();
                }
            }
        }
        public void EndCommit()
        {
            if (MustRunTrans)
            {
                if (ConnectionType == ConnectionType.SqlClient)
                {
                    _SqlTrans.Commit();
                    if (SqlConn.State == ConnectionState.Open) SqlConn.Close();
                }
                else if (ConnectionType == ConnectionType.OleDb)
                {
                    _OleTrans.Commit();
                    if (OleConn.State == ConnectionState.Open) OleConn.Close();
                }
                else if (ConnectionType == ConnectionType.MySqlClient)
                {
                    _MySqlTrans.Commit();
                    if (MySqlConn.State == ConnectionState.Open) MySqlConn.Close();
                }
            }
        }
        public void Rollback()
        {
            if (MustRunTrans)
            {
                if (ConnectionType == ConnectionType.SqlClient)
                {
                    if (_SqlTrans != null)
                    {
                        _SqlTrans.Rollback();
                    }
                }
                else if (ConnectionType == ConnectionType.OleDb)
                {
                    _OleTrans.Rollback();
                }
                else if (ConnectionType == ConnectionType.MySqlClient)
                {
                    _MySqlTrans.Rollback();
                }
            }
        }

        public void Open()
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                SqlConn.Open();
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                OleConn.Open();
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                MySqlConn.Open();
            }
        }
        public void Close()
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                SqlConn.Close();

            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                OleConn.Close();

            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                MySqlConn.Close();

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ConnectionType == ConnectionType.SqlClient)
                {
                    SqlConn.Dispose();
                }
                else if (ConnectionType == ConnectionType.OleDb)
                {
                    OleConn.Dispose();
                }
                else if (ConnectionType == ConnectionType.MySqlClient)
                {
                    MySqlConn.Dispose();
                }
            }
        }

        public void ExecuteNonQuery(String SQL)
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                SqlCommand Cmd = new SqlCommand(SQL, SqlConn);
                if (_SqlTrans != null)
                {
                    Cmd.Transaction = _SqlTrans;
                }
                Cmd.ExecuteNonQuery();
                Cmd.Dispose();
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                OleDbCommand Cmd = new OleDbCommand(SQL, OleConn);
                if (_OleTrans != null)
                {
                    Cmd.Transaction = _OleTrans;
                }
                Cmd.ExecuteNonQuery();
                Cmd.Dispose();
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                MySqlCommand Cmd = new MySqlCommand(SQL, MySqlConn);
                if (_MySqlTrans != null)
                {
                    Cmd.Transaction = _MySqlTrans;
                }
                Cmd.ExecuteNonQuery();
                Cmd.Dispose();
            }
        }
        public DataTable ExecuteData(String SQL)
        {
            DataTable dt = new DataTable();

            if (ConnectionType == ConnectionType.SqlClient)
            {
                SqlDataAdapter _SQLAdp = new SqlDataAdapter(SQL, SqlConn);
                if (this._Params != null)
                {
                    foreach (var Parm in this._Params)
                    {
                        _SQLAdp.SelectCommand.Parameters.Add(new SqlParameter { ParameterName = Parm.Key, Value = Parm.Value });
                    }
                }

                if (_SqlTrans != null) _SQLAdp.SelectCommand.Transaction = _SqlTrans;
                _SQLAdp.Fill(dt);
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                OleDbDataAdapter _OLEAdp = new OleDbDataAdapter(SQL, OleConn);

                if (this._Params != null)
                {
                    foreach (var Parm in this._Params)
                    {
                        _OLEAdp.SelectCommand.Parameters.Add(new OleDbParameter { ParameterName = Parm.Key, Value = Parm.Value });
                    }
                }

                if (_OleTrans != null) _OLEAdp.SelectCommand.Transaction = _OleTrans;
                _OLEAdp.Fill(dt);
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                MySqlDataAdapter _MyAdp = new MySqlDataAdapter(SQL, MySqlConn);

                if (this._Params != null)
                {
                    foreach (var Parm in this._Params)
                    {
                        _MyAdp.SelectCommand.Parameters.Add(new MySqlParameter { ParameterName = Parm.Key, Value = Parm.Value });
                    }
                }

                if (_MySqlTrans != null) _MyAdp.SelectCommand.Transaction = _MySqlTrans;
                _MyAdp.Fill(dt);
            }
            return dt;
        }
        public Object ExecuteScalar(String SQL)
        {
            if (ConnectionType == ConnectionType.SqlClient)
            {
                SqlCommand Cmd = new SqlCommand(SQL, SqlConn);
                if (_SqlTrans != null)
                    Cmd.Transaction = _SqlTrans;

                return Cmd.ExecuteScalar();
            }
            else if (ConnectionType == ConnectionType.OleDb)
            {
                OleDbCommand Cmd = new OleDbCommand(SQL, OleConn);
                if (_SqlTrans != null)
                    Cmd.Transaction = _OleTrans;

                return Cmd.ExecuteScalar();
            }
            else if (ConnectionType == ConnectionType.MySqlClient)
            {
                MySqlCommand Cmd = new MySqlCommand(SQL, MySqlConn);
                if (_MySqlTrans != null)
                    Cmd.Transaction = _MySqlTrans;

                return Cmd.ExecuteScalar();
            }
            else
            {
                return null;
            }
        }
    }
}
