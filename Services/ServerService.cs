using MauiPOS.Data;
using MauiPOS.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MauiPOS.Services
{
    public class ServerService
    {
        private SqlConnection _connection = new("Server=192.168.13.155;Database=pos;User Id=pos;Password=POSpassword; TrustServerCertificate=True; ");
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
        public POSOps RetOpID(string passHash)
        {
            POSOps po = new();

            if (!IsConnected()) return po;
            _command.Connection = _connection;
            _command.CommandType = CommandType.Text;
            _command.Parameters.Clear();
            _command.CommandText = @"SELECT *, isnull((select MAX(OrderID) from srvOrders where OpID=so.OpID),0) as MaxOrderID from srvOps so where OpPass=N'" + passHash + @"'";
            SqlDataReader _reader = _command.ExecuteReader();
            if (_reader.HasRows)
            {
                _reader.Read();

                po.OpID = _reader.GetInt32(0);
                po.OpName = _reader.GetString(1);
                po.OpPass = _reader.GetString(2);
                po.OpRole = _reader.GetInt16(3);
                po.OpFName = _reader.GetString(4);
                po.OpCode = _reader.GetString(5);
                po.OpApp = _reader.GetString(6);
                po.RoleEnd = _reader.IsDBNull(8) ? DateTime.Now.AddMonths(1) : _reader.GetDateTime(8);
                POSGlobals.MaxOrderID = _reader.GetInt32(9);

                _reader.Close();
                return po;
            }
            else {
                _reader.Close();
                return po; }
        }


        public IEnumerable<ViewActiveOrders> RetActiveOrders(int OpID)
        {
            IsConnected();
            _command.Connection = _connection;
            _command.CommandType = CommandType.Text;
            _command.Parameters.Clear();
            _command.CommandText = @"SELECT OrderID, ObjName, CurrentSum from View_OpActiveOrders where OpID=" + OpID.ToString() + " ORDER BY ObjName, DateOpen";
            Models.ViewActiveOrders result;
            List<Models.ViewActiveOrders> results = new List<Models.ViewActiveOrders>();
            using (var _reader = _command.ExecuteReader())
            {
                while (_reader.Read())
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
        public bool SyncGoods()
        {
            POSGoods _srvGood;
            if (!IsConnected()) return false;
            _command.Connection = _connection;
            _command.CommandType = CommandType.Text;
            _command.Parameters.Clear();
            _command.CommandText = @"SELECT GoodsID, isGroup, GParent, CashName, CashCode, DiscountAllowed from srvGoods";
            SqlDataReader _reader = _command.ExecuteReader();
            if (_reader.HasRows)
            {
                try
                {
                    while (_reader.Read())
                    {
                        _srvGood = new()
                        {
                            GoodsID = _reader.GetInt32(0),
                            isGroup = _reader.GetBoolean(1),
                            GParent = _reader.GetInt32(2),
                            CashName = _reader.IsDBNull(3) ? null : _reader.GetString(3),
                            CashCode = _reader.IsDBNull(4) ? null: _reader.GetString(4),
                            DiscountAllowed = _reader.IsDBNull(5) ? null : _reader.GetBoolean(5)
                        };
                        POSdata.SaveGood(_srvGood);
                    }
                    _reader.Close();
                    return true;
                }
                catch { 
                    _reader.Close();
                    return false; }
            }
            _reader.Close();
            return true;
        }

        public bool SyncObjects()
        {
            POSObjects _srvObject;
            if (!IsConnected()) return false;
            _command.Connection = _connection;
            _command.CommandType = CommandType.Text;
            _command.Parameters.Clear();
            _command.CommandText = @"SELECT ObjectID, ObjName, Parent, ObjCode, isActive from srvObjects";
            SqlDataReader _reader = _command.ExecuteReader();
            if (_reader.HasRows)
            {
                try
                {
                    while (_reader.Read())
                    {
                        _srvObject = new()
                        {
                            ObjectID = _reader.GetInt32(0),
                            ObjName = _reader.GetString(1),
                            Parent = _reader.GetInt32(2),
                            ObjCode = _reader.IsDBNull(3) ? null : _reader.GetString(3),
                            IsActive = _reader.IsDBNull(4) ? null : _reader.GetBoolean(4)
                        };
                        POSdata.SaveObjects(_srvObject);
                    }
                    _reader.Close() ;
                    return true;
                }
                catch {
                    _reader.Close();
                    return false; }
            }
            _reader.Close();
            return true;
        }
        public bool SyncCashPrice()
        {
            POSCashPrice _srvCashPrice;
            if (!IsConnected()) return false;
            _command.Connection = _connection;
            _command.CommandType = CommandType.Text;
            _command.Parameters.Clear();
            _command.CommandText = @"SELECT ID, GoodsID, ObjectID, CashEnabled, PosPrinter, CashPrice, DFPrice from srvCashPrice";
            SqlDataReader _reader = _command.ExecuteReader();
            if (_reader.HasRows)
            {
                try
                {
                    while (_reader.Read())
                    {
                        _srvCashPrice = new()
                        {
                            ID = _reader.GetInt32(0),
                            GoodsID = _reader.GetInt32(1),
                            ObjectID = _reader.GetInt32(2),
                            CashEnabled = _reader.GetBoolean(3),
                            PosPrinter = _reader.IsDBNull(4) ? null : _reader.GetInt16(4),
                            CashPrice = (decimal)_reader.GetFloat(5),
                            DFPrice = _reader.IsDBNull(6) ? null : (decimal)_reader.GetFloat(6)
                        };
                        POSdata.SaveCashPrice(_srvCashPrice);
                    }
                    _reader.Close();
                    return true;
                }
                catch { 
                    _reader.Close();
                    return false; }
            }
            _reader.Close ();
            return true;
        }

        public bool SrvSendOrders()
        {
            try
            {
                SqlConnection _cnn = new("Server=192.168.13.155;Database=pos;User Id=pos;Password=POSpassword; TrustServerCertificate=True; ");
                SqlCommand _cmd = new();
                if (!_cnn.State.Equals(ConnectionState.Open))
                {
                    _cnn.Open();
                }

                _cmd.Connection = _cnn;
                _cmd.CommandType = CommandType.Text;
                _cmd.Parameters.Clear();
                _cmd.CommandText = "BEGIN TRANSACTION;";
                _cmd.ExecuteNonQuery();
                POSRetData retData = new();
                foreach (POSOrders order in retData.RetOrders(POSGlobals.MaxOrderID))
                {
                    _cmd.CommandText = $"INSERT INTO srvOrdes(OrderID, OpID, DateOpen, DateModified, DateClosed, ObjectID, CurrentSum, FinalSum, RE, REOrderID) VALUES({order.OrderID}, {order.OpID}, {order.DateOpen}, {order.DateModified} , {order.DateClosed} , {order.ObjectID}, {order.CurrentSum}, {order.FinalSum}, {order.RE}, {order.REOrderID})";
                    if (_cmd.ExecuteNonQuery() <= 0)
                    {
                        _cmd.CommandText = "ROLLBACK;";
                        _cmd.ExecuteNonQuery();
                        return false;
                    }
                    foreach (POSOrderDetails details in retData.RetOrderDetails(order.OrderID))
                    {
                        _cmd.CommandText = $"INSERT INTO srvOrderDetails(OrderID, OpID, GoodsID, Cnt, Annul, Modiff, CashPrice) VALUES({details.OrderID}, {details.OpID}, {details.GoodsID}, {details.Cnt}, {details.Annul}, {details.Modiff}, {details.CashPrice})";
                        _cmd.ExecuteNonQuery();
                    }
                    foreach (POSOrderChrono chrono in retData.RetOrdersChrono(order.OrderID))
                    {
                        _cmd.CommandText = String.Format("INSERT INTO srvOrdersChrono(OrderID, OpID, GoodsID, Cnt, Modiff, Created, OrderNum, Confirmed, Accepted, Printed, OpName, OpCode, OpRole) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})", chrono.OrderID, chrono.OpID, chrono.GoodsID, chrono.Cnt, chrono.Modiff, chrono.Created, chrono.OrderNum, chrono.Confirmed, chrono.Accepted, chrono.Printed, chrono.OpName, chrono.OpCode, chrono.OpRole);
                        _cmd.ExecuteNonQuery();
                    }
                }
                _cmd.CommandText = "COMMIT";
                _cmd.ExecuteNonQuery();
                _cnn.Close();
                return true;
            }
            catch { return false; }
        }

    }
}
