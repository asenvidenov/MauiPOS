using SQLite;
using MauiPOS.Models;

namespace MauiPOS.Data
{
    internal class POSRetData
    {
        private SQLiteConnection _cnn;
        public POSRetData()
        { 
            _cnn= new SQLiteConnection(POSGlobals.DatabasePath(), POSGlobals.Flags, true);
        }

        public List<POSOrders> RetOrders(int orderID, int opID)
        {
            return [.._cnn.Table<POSOrders>().Where(i=>i.OpID==opID).Where(i => i.OrderID > orderID)] ;
        }

        public List<POSOrderDetails> RetOrderDetails(int orderID)
        {
            return [.._cnn.Table<POSOrderDetails>().Where(i => i.OrderID == orderID)];
        }
        public List<POSOrderChrono> RetOrdersChrono(int orderID)
        {
            return [.._cnn.Table<POSOrderChrono>().Where(i => i.OrderID == orderID)];
        }

        public List<MaxOrderIDByOps> RetLocalMaxOrderIDByOps()
        {
            var cmd = new SQLiteCommand(_cnn);
            cmd.CommandText = "SELECT OpID, MAX(OrderID) from posOrders GROUP BY OpID";
            return cmd.ExecuteQuery<MaxOrderIDByOps>();
        }

        public List<ViewActiveOrders> RetActiveOrders(int OpID)
        {
            var cmd = new SQLiteCommand(_cnn);
            cmd.CommandText = $"select OrderID, (select ObjName from POSObjects where ObjectID=po.ObjectID) ObjName, CurrentSUM from POSOrders po where opID={OpID} and DateClosed is NULL ORDER BY ObjName, OrderID";
            return cmd.ExecuteQuery<ViewActiveOrders>();
        }

        public string RetCashName(int GoodsID)
        {
            var cmd = new SQLiteCommand(_cnn);
            cmd.CommandText = $"SELECT CashName from POSGoods where GoodsID={GoodsID}";
            return cmd.ExecuteScalar<string>();
        }

        public List<GoodsByGroup> RetGoodsByGroup(int GoodsID)
        {
            var cmd = new SQLiteCommand(_cnn);
            cmd.CommandText=$"SELECT GoodsID, isGroup, GParent, CashName FROM POSGoods WHERE GParent = {GoodsID}";
            return cmd.ExecuteQuery<GoodsByGroup>();
        }

        public int RetGoodParent(int GoodsID)
        {
            var cmd = new SQLiteCommand(_cnn);
            cmd.CommandText = $"SELECT GParent from POSGoods where GoodsID={GoodsID}";
            return cmd.ExecuteScalar<int>();
        }

        public bool RetGoodIsGroup(int GoodsID)
        {
            var cmd = new SQLiteCommand(_cnn);
            cmd.CommandText = $"SELECT isGroup from POSGoods where GoodsID={GoodsID}";
            return cmd.ExecuteScalar<bool>();
        }
        public (decimal, decimal) RetGoodPrice(int GoodsID, int ObjectID =0)
        {
            (decimal, decimal) gPrice = (0, 0);
            var cmd = new SQLiteCommand(_cnn);
            if(ObjectID == 0)
            {
                cmd.CommandText = $"SELECT CashPrice from POSCashPrice where GoodsID={GoodsID} and CashEnabled=1";
            }
            else
            {
                cmd.CommandText = $"SELECT CashPrice from POSCashPrice where GoodsID={GoodsID} and ObjectID={ObjectID} and CashEnabled=1";
            }
            
            gPrice.Item1= cmd.ExecuteScalar<decimal>();
            if (ObjectID == 0)
            {
                cmd.CommandText = $"SELECT DFPrice from POSCashPrice where GoodsID={GoodsID} and CashEnabled=1";
            }
            else
            {
                cmd.CommandText = $"SELECT DFPrice from POSCashPrice where GoodsID={GoodsID} and ObjectID={ObjectID} and CashEnabled=1";
            }
            gPrice.Item2 = cmd.ExecuteScalar<decimal>();

            return gPrice;
        }
    }
}
