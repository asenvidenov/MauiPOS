using MauiPOS.Data;
using MauiPOS.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace MauiPOS.Services
{
    internal static class RetServerServices
    {
        private static SqlConnection _Cnn = new("Server=192.168.13.155;Database=pos;User Id=pos;Password=POSpassword; TrustServerCertificate=True; ");


        private static bool IsConnected()
        {
            if (_Cnn.State.Equals(ConnectionState.Open))
            {
                return true;
            }
            else
            {
                try
                {
                    _Cnn.Open();
                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }

        public static POSOps RetOpID(string passHash)
        {
            POSOps po = new();

            if (!IsConnected()) return po;
            SqlCommand _command = new()
            {
                Connection = _Cnn,
                CommandType = CommandType.Text,
                CommandText = @"SELECT *, isnull((select MAX(OrderID) from srvOrders where OpID=so.OpID),0) as MaxOrderID from srvOps so where OpPass=N'" + passHash + @"'"
            };
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
            else
            {
                _reader.Close();
                return po;
            }
        }

        public static IEnumerable<ViewActiveOrders> RetActiveOrders(int OpID)
        {
            ViewActiveOrders result;
            List<ViewActiveOrders> results = [];
            if (!IsConnected()) return results;
            SqlCommand _command = new()
            {
                Connection = _Cnn,
                CommandType = CommandType.Text,
                CommandText = @"SELECT OrderID, ObjName, CurrentSum from View_OpActiveOrders where OpID=" + OpID.ToString() + " ORDER BY ObjName, DateOpen"
            };
            using (var _reader = _command.ExecuteReader())
            {
                while (_reader.Read())
                {
                    result = new();
                    result.OrderID = _reader.GetInt32(0);
                    result.ObjName = _reader.GetString(1);
                    result.CurrentSum = _reader.IsDBNull(2) ? 0 : (decimal)_reader.GetFloat(2);
                    results.Add(result);
                }
                _reader.Close();
            }
            return results;
        }
        public static void RetActiveShift(int OpID, ref ActiveShift asheet)
        {
            if(!IsConnected()) return;
            SqlCommand cmd = new("SP_srv_OpsShifts")
            {
                Connection = _Cnn,
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@OpID", OpID);
            var _reader = cmd.ExecuteReader();
            if (_reader.Read())
            {
                POSGlobals.OpShift = _reader.GetInt32(0);
                POSGlobals.ShiftStart = _reader.GetDateTime(1);
                asheet.ShiftID = _reader.GetInt32(0);
                asheet.ShiftStart = _reader.GetDateTime(1);
                _reader.Close();
                return;
            }
            else
            {
                _reader.Close();
                return; 
            }
        }

        public static int RetSrvOrders(int opID, bool activeOnly, int orderID)
        {
            if(!IsConnected()) return 0;
            SqlCommand cmd = new()
            {
                Connection = _Cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "SP_srv_RetShiftOrders"
            };
            cmd.Parameters.AddWithValue("@OpID", opID);
            cmd.Parameters.AddWithValue("@ActiveOnly", activeOnly ? 1 : 0);
            cmd.Parameters.AddWithValue("@OrderID", orderID);
            var _reader = cmd.ExecuteReader();
            POSOrders pOSOrder;
            List<POSOrders> orders = [];
            while (_reader.Read())
            {
                pOSOrder = new()
                {
                    OrderID = _reader.GetInt32(0),
                    OpID = opID,
                    DateOpen = _reader.GetDateTime(1),
                    DateModified = _reader.IsDBNull(2) ? null : _reader.GetDateTime(2),
                    DateClosed = _reader.IsDBNull(3) ? null : _reader.GetDateTime(3),
                    ObjectID = _reader.GetInt32(4),
                    CurrentSum = _reader.IsDBNull(5) ? null : (decimal)_reader.GetFloat(5),
                    FinalSum = _reader.IsDBNull(6) ? null : (decimal)_reader.GetFloat(6),
                    RE = _reader.IsDBNull(7) ? null : bool.Parse(_reader.GetString(7)),
                    REOrderID = _reader.IsDBNull(8) ? null : _reader.GetInt32(8)
                };
                orders.Add(pOSOrder);
            }
            _reader.Close();
            return POSdata.SaveOrders(orders);

        }

        public static MaxOrderIDByOps RetMaxOrderID(int OpID)
        {
            if(!IsConnected()) return new MaxOrderIDByOps();
            SqlCommand cmd = new()
            {
                Connection = _Cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "SP_srv_RetMaxOrderID"
            };
            cmd.Parameters.AddWithValue("@OpID", OpID);
            return new MaxOrderIDByOps()
            {
                opID = OpID,
                MaxOrderID = (int)cmd.ExecuteScalar()
            };
        }

        public static List<POSOrderDetailsView> RetSrvOrderDetails(int OrderID)
        {
            POSOrderDetailsView pod = new();
            List<POSOrderDetailsView> result = [];
            if(!IsConnected() ) return [];
            SqlCommand cmd = new()
            {
                Connection = _Cnn,
                CommandType = CommandType.Text,
                CommandText = $"SELECT ID, OrderID, OpID, GoodsID, Cnt, Annul, Modif, CashPrice FROM srvOrderDetails WHERE OrderID={OrderID} and OpID={POSGlobals.localOpID}"
            };
            var _reader = cmd.ExecuteReader();
            try
            {
                while (_reader.Read())
                {
                    pod.ID = _reader.GetInt32(0);
                    pod.OpID = POSGlobals.localOpID;
                    pod.OrderID = OrderID;
                    pod.GoodsID = _reader.GetInt32(3);
                    pod.CashName = new POSRetData().RetCashName(pod.GoodsID);
                    pod.Cnt = _reader.GetInt32(4) - _reader.GetInt32(5);
                    pod.Annul = _reader.GetInt32(5);
                    pod.Modiff = _reader.IsDBNull(6) ? "" : _reader.GetString(6);
                    pod.CashPrice = (decimal)_reader.GetFloat(7);

                    result.Add(pod);
                }
                var _res = result.Cast<POSOrderDetails>().ToList();
                POSdata.SaveOrderDetails(ref _res);
                return result;
            }
            catch { return []; }
        }
    }
}
