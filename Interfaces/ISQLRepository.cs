using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InventoryBeginners.Interfaces
{
    public interface ISQLRepository : IDisposable
    {
        public Task<object> ExecuteScalar(string commandText);
        public Task<object> ExecuteScalar(string commandText, CommandType commandType);
        public Task<object> ExecuteScalar(string commandText, CommandType commandType, SqlParameter[] Parms);
        public Task<int> ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] Parms);
        public Task<int> ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] Parms, SqlParameter OutputParameter);
        public Task<int> ExecuteNonQuery(string commandText, CommandType commandType);
        public Task<DataTable> GetTableAsync(string commandText, CommandType commandType);
        public Task<DataTable> GetTableAsync(string commandText, CommandType commandType, SqlParameter[] Parms);
        public Task<List<T>> GetList<T>(string commandText, CommandType commandType);
        public Task<List<T>> GetList<T>(string commandText, CommandType commandType, SqlParameter[] Parms);
        public Task<T> GetObject<T>(string commandText, CommandType commandType);
        public Task<T> GetObject<T>(string commandText, CommandType commandType, SqlParameter[] Parms);
    }
}
