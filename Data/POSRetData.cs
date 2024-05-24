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

        public List<POSOrders> RetOrders(int orderID)
        {
            return [.._cnn.Table<POSOrders>().Where(i => i.OrderID > orderID)] ;
        }

        public List<POSOrderDetails> RetOrderDetails(int orderID)
        {
            return [.._cnn.Table<POSOrderDetails>().Where(i => i.OrderID == orderID)];
        }
        public List<POSOrderChrono> RetOrdersChrono(int orderID)
        {
            return [.._cnn.Table<POSOrderChrono>().Where(i => i.OrderID == orderID)];
        }

    }
}
