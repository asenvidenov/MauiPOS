using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace MauiPOS.Services
{
    public class ServerService
    {
        private SqlConnection _connection=new ("Server=192.168.13.155;Database=pos;User Id=pos;Password=POSpassword; TrustServerCertificate=True; ");
        private SqlCommand _command = new();

        private bool IsConnected()
        {
            if (_connection.State.Equals(ConnectionState.Open))
            {
                return true;
            }
            else
            {
                try
                {
                    _connection.Open();
                    return true;
                }
                catch
                {
                    return false; 
                }
                
            }
        }
        public Models.POSOps RetOpID(string passHash)
        {
            Models.POSOps po = new();

            if (!IsConnected()) return po;
            _command.Connection = _connection;
            _command.CommandType = CommandType.Text;
            _command.Parameters.Clear();
            _command.CommandText = @"SELECT *, isnull((select MAX(OrderID) from srvOrders where OpID=so.OpID),0) as MaxOrderID from srvOps so where OpPass=N'" + passHash + @"'";
            SqlDataReader _reader = _command.ExecuteReader();
            if(_reader.HasRows)
            {
                _reader.Read();
                
                po.OpID = _reader.GetInt32(0);
                po.OpName = _reader.GetString(1);
                po.OpPass = _reader.GetString(2);
                po.OpRole = _reader.GetInt16(3);
                po.OpFName = _reader.GetString(4);
                po.OpCode = _reader.GetString(5);
                po.OpApp = _reader.GetString(6);
                po.RoleEnd = _reader.IsDBNull(8) ? DateTime.Now.AddMonths(1) :_reader.GetDateTime(8);
                POSGlobals.MaxOrderID = _reader.GetInt32(9);
                
                _reader.Close();
                return po;
            }
            else { return po; }
        }

        public IEnumerable<Models.ViewActiveOrders> RetActiveOrders(int OpID)
        {
                IsConnected();
                _command.Connection = _connection;
                _command.CommandType = CommandType.Text;
                _command.Parameters.Clear();
                _command.CommandText = @"SELECT OrderID, ObjName, CurrentSum from View_OpActiveOrders where OpID=" + OpID.ToString() + " ORDER BY ObjName, DateOpen";
            Models.ViewActiveOrders result;
            List<Models.ViewActiveOrders> results = new List<Models.ViewActiveOrders>();
                using (var _reader =_command.ExecuteReader())
                {
                    while(_reader.Read())
                    {
                        result = new();
                        result.OrderID = _reader.GetInt64(0);
                        result.ObjName = _reader.GetString(1);
                        result.CurrentSum = _reader.IsDBNull(2) ? 0 : (decimal)_reader.GetFloat(2);
                        results.Add(result);
                    }
                }
                return results;
        }
    }
}
