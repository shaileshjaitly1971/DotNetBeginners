using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace InventoryBeginners.Repositories
{
    public class SQLRepository : ISQLRepository
    {
        #region Constructor And IDisposable Methods

        private string connectionString;
        private readonly IAppConfig configuration;
        public SQLRepository(IAppConfig _configuration)
        {
            this.configuration = _configuration;
            connectionString = _configuration.GetConnectionString("DbConn");
        } /////

        private bool _disposedValue;

        ~SQLRepository() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {

                }

                _disposedValue = true;
            }
        }

        #endregion

        #region ADO .Net Methods

        public async Task<int> ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] Parms)
        {
            int iRetVal = 0;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                await sqlConnection.OpenAsync().ConfigureAwait(false);
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection))
                    {
                        sqlCommand.CommandType = commandType;
                        sqlCommand.Parameters.AddRange(Parms);
                        iRetVal = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    iRetVal = 0;
                    ex.Data.Clear();

                }
                finally
                {
                    await sqlConnection.CloseAsync().ConfigureAwait(false);
                }

            }
            return iRetVal;

        }

        public async Task<int> ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] Parms, SqlParameter OutputParameter)
        {
            int iRetVal = 0;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                await sqlConnection.OpenAsync().ConfigureAwait(false);
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection))
                    {
                        sqlCommand.CommandType = commandType;
                        sqlCommand.Parameters.AddRange(Parms);
                        sqlCommand.Parameters.Add(OutputParameter);
                        iRetVal = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    iRetVal = 0;
                    ex.Data.Clear();

                }
                finally
                {
                    await sqlConnection.CloseAsync().ConfigureAwait(false);
                }

            }
            return iRetVal;
        }

        public async Task<int> ExecuteNonQuery(string commandText, CommandType commandType)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlCommand.CommandType = commandType;
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                    return await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task<object> ExecuteScalar(string commandText)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                    return await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task<object> ExecuteScalar(string commandText, CommandType commandType)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlCommand.CommandType = commandType;
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                    return await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task<object> ExecuteScalar(string commandText, CommandType commandType, SqlParameter[] Parms)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlCommand.CommandType = commandType;
                    sqlCommand.Parameters.AddRange(Parms);
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                    return await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task<DataTable> GetTableAsync(string commandText, CommandType commandType)
        {

            return await GetTableAsync(commandText, commandType);
        }

        public async Task<DataTable> GetTableAsync(string commandText, CommandType commandType, SqlParameter[] Parms)
        {
            return await GetTableAsync(commandText, commandType, Parms);
        }


        //////private Task<DataTable> GetTable(string commandtext, CommandType commandtype, SqlParameter[] parameters)
        //////{
        //////    return Task.Run(() =>
        //////   {
        //////       using (SqlConnection newConnection = new SqlConnection(connectionString))
        //////       {
        //////           using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(commandtext, newConnection))
        //////           {
        //////               sqlDataAdapter.SelectCommand.CommandType = commandtype;
        //////               sqlDataAdapter.SelectCommand.Parameters.AddRange(parameters);

        //////               DataTable dataTable = new DataTable();
        //////               sqlDataAdapter.Fill(dataTable);
        //////               return dataTable;
        //////           }
        //////       }
        //////   });
        //////}

        //////private Task<DataTable> GetTable(string commandtext, CommandType commandtype)
        ////{
        ////    return Task.Run(() =>
        ////    {
        ////        using (SqlConnection newConnection = new SqlConnection(connectionString))
        ////        {
        ////            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(commandtext, newConnection))
        ////            {
        ////                sqlDataAdapter.SelectCommand.CommandType = commandtype;
        ////                DataTable dataTable = new DataTable();
        ////                sqlDataAdapter.Fill(dataTable);
        ////                return dataTable;
        ////            }
        ////        }
        ////    });
        ////}

        #endregion

        #region Common Application Functions

        public async Task<List<T>> GetList<T>(string commandText, CommandType commandType)
        {
            List<T> list = new List<T>();
            Type typ = typeof(T);

            SqlConnection newConnection = new SqlConnection(connectionString);
            if (newConnection.State == ConnectionState.Closed)
                newConnection.Open();

            try
            {

                SqlCommand sqlcmd = new SqlCommand(commandText, newConnection);
                sqlcmd.CommandTimeout = 0;
                sqlcmd.CommandType = commandType;
                using (SqlDataReader reader = await sqlcmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (reader.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnType = reader.GetDataTypeName(i);
                            var columnName = reader.GetName(i);
                            var columnValue = reader.GetValue(i);
                            PropertyInfo prop = typ.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (null != prop && prop.CanWrite)
                            {
                                if (prop.Name.ToLower() == columnName.ToLower())
                                {
                                    if (columnValue != DBNull.Value)
                                    {
                                        if (columnType.ToLower() == "image" && columnValue != null)
                                        {
                                            prop.SetValue(obj, columnValue as byte[], null);
                                        }
                                        else
                                            prop.SetValue(obj, columnValue, null);
                                    }
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            finally
            {
                if (newConnection.State == ConnectionState.Closed)
                    newConnection.Open();
            }
            return list;
        }

        public async Task<List<T>> GetList<T>(string commandText, CommandType commandType, SqlParameter[] Parms)
        {
            List<T> list = new List<T>();
            Type typ = typeof(T);
            for (int i = 0; i < Parms.Length; i++)
            {
                if (Parms[i].Value == null)
                    Parms[i].Value = DBNull.Value;
            }

            SqlConnection newConnection = new SqlConnection(connectionString);
            if (newConnection.State == ConnectionState.Closed)
                newConnection.Open();

            try
            {

                SqlCommand sqlcmd = new SqlCommand(commandText, newConnection);
                sqlcmd.CommandTimeout = 0;
                sqlcmd.CommandType = commandType;
                sqlcmd.Parameters.AddRange(Parms);
                using (SqlDataReader reader = await sqlcmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (reader.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnType = reader.GetDataTypeName(i);
                            var columnName = reader.GetName(i);
                            var columnValue = reader.GetValue(i);
                            PropertyInfo prop = typ.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (null != prop && prop.CanWrite)
                            {
                                if (prop.Name.ToLower() == columnName.ToLower())
                                {
                                    if (columnValue != DBNull.Value)
                                    {
                                        if (columnType.ToLower() == "image" && columnValue != null)
                                        {
                                            prop.SetValue(obj, columnValue as byte[], null);
                                        }
                                        else
                                            prop.SetValue(obj, columnValue, null);
                                    }
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            finally
            {
                if (newConnection.State == ConnectionState.Closed)
                    newConnection.Open();
            }
            return list;
        }

        public async Task<T> GetObject<T>(string commandText, CommandType commandType)
        {

            T obj = Activator.CreateInstance<T>();
            Type typ = typeof(T);

            SqlConnection newConnection = new SqlConnection(connectionString);
            if (newConnection.State == ConnectionState.Closed)
                newConnection.Open();

            try
            {
                using (SqlCommand sqlcmd = new SqlCommand(commandText, newConnection))
                {
                    sqlcmd.CommandTimeout = 0;
                    sqlcmd.CommandType = commandType;
                    using (SqlDataReader reader = await sqlcmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                string columnType = reader.GetDataTypeName(i);
                                string columnName = reader.GetName(i);
                                object columnValue = reader.GetValue(i);
                                PropertyInfo prop = typ.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                if (null != prop && prop.CanWrite)
                                {
                                    if (prop.Name.ToLower() == columnName.ToLower())
                                    {
                                        if (columnValue != DBNull.Value)
                                        {
                                            if (columnType.ToLower() == "image" && columnValue != null)
                                            {
                                                prop.SetValue(obj, columnValue as byte[], null);
                                            }
                                            else
                                            {
                                                prop.SetValue(obj, columnValue, null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            finally
            {
                if (newConnection.State == ConnectionState.Closed)
                    newConnection.Open();
            }
            return obj;
        }

        public async Task<T> GetObject<T>(string commandText, CommandType commandType, SqlParameter[] Parms)
        {
            for (int i = 0; i < Parms.Length; i++)
            {
                if (Parms[i].Value == null)
                    Parms[i].Value = DBNull.Value;
            }
            T obj = Activator.CreateInstance<T>();
            Type typ = typeof(T);

            SqlConnection newConnection = new SqlConnection(connectionString);
            if (newConnection.State == ConnectionState.Closed)
                newConnection.Open();
            try
            {

                using (SqlCommand sqlcmd = new SqlCommand(commandText, newConnection))
                {
                    sqlcmd.CommandTimeout = 0;
                    sqlcmd.CommandType = commandType;
                    sqlcmd.Parameters.AddRange(Parms);
                    using (SqlDataReader reader = await sqlcmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                string columnType = reader.GetDataTypeName(i);
                                string columnName = reader.GetName(i);
                                object columnValue = reader.GetValue(i);
                                PropertyInfo prop = typ.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                if (null != prop && prop.CanWrite)
                                {
                                    if (prop.Name.ToLower() == columnName.ToLower())
                                    {
                                        if (columnValue != DBNull.Value)
                                        {
                                            if (columnType.ToLower() == "image" && columnValue != null)
                                            {
                                                prop.SetValue(obj, columnValue as byte[], null);
                                            }
                                            else
                                            {
                                                prop.SetValue(obj, columnValue, null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            finally
            {
                if (newConnection.State == ConnectionState.Closed)
                    newConnection.Open();
            }
            return obj;
        }



        #endregion
    }
}
