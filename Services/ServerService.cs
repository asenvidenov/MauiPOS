using MauiPOS.Data;
using MauiPOS.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MauiPOS.Services
{
    public class ServerService
    {
        private static SqlConnection _connection = new("Server=192.168.13.155;Database=pos;User Id=pos;Password=POSpassword; TrustServerCertificate=True; ");
        private static SqlCommand _command = new();

        private static bool IsConnected()
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


        public static bool SyncGoods()
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

        public static bool SyncObjects()
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
        public static bool SyncCashPrice()
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

        public static bool SrvSendOrders(int OpID, int OrderID)
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
                foreach (POSOrders order in retData.RetOrders(OrderID, OpID))
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

        public static bool InitialSync()
        {
            var result = false;
            var localres = new POSRetData().RetLocalMaxOrderIDByOps();
            if(localres != null )
            {
                foreach (MaxOrderIDByOps local in localres)
                {
                    var remote = RetServerServices.RetMaxOrderID(local.opID);
                    if (local.MaxOrderID > remote.MaxOrderID)
                    {
                        result = result || SrvSendOrders(local.opID, remote.MaxOrderID);
                    }
                    else
                    {
                        result = result || RetServerServices.RetSrvOrders(local.opID, false, local.MaxOrderID)>0;
                    }
                }
                return result;
            }
            return true;
        }
    }
}
