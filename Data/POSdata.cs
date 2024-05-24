using MauiPOS.Models;
using SQLite;

namespace MauiPOS.Data
{
    public static class POSdata
    {
        static SQLiteConnection? LocalDB;

        static void Init()
        {
            if (LocalDB is not null) return;

            LocalDB = new SQLiteConnection(POSGlobals.DatabasePath(), POSGlobals.Flags, true);
            LocalDB.CreateTable<POSCashPrice>();
            LocalDB.CreateTable<POSDiscount>();
            LocalDB.CreateTable<POSFP>();
            LocalDB.CreateTable<POSGoods>();
            LocalDB.CreateTable<POSObjects>();
            LocalDB.CreateTable<POSOps>();
            LocalDB.CreateTable<POSOrderChrono>();
            LocalDB.CreateTable<POSOrderDetails>();
            LocalDB.CreateTable<POSOrders>();
            LocalDB.CreateTable<POSSales>();
            LocalDB.CreateTable<POSSalesPay>();
        }
        
        public static int SavePOSops(POSOps posOp)
        {
            Init();
            return LocalDB.InsertOrReplace(posOp);
        }
        public static void LogOut()
        {
            Init();
            SQLiteCommand cmd = new(LocalDB);
            cmd.CommandText = "DELETE from POSOps";
            cmd.ExecuteNonQuery();
        }

        public static int SaveMaxOrderID()
        {
             Init();
            sqlite_sequence _sequence = new sqlite_sequence() { name="POSOrders", seq=POSGlobals.MaxOrderID};
            SQLiteCommand cmd = new(LocalDB);
            cmd.CommandText = "SELECT MAX(OrderID) from POSOrders";
            Int32? maxID = cmd.ExecuteScalar<Int32?>();
            maxID = maxID == null ? 0 : maxID.Value;
            if (maxID > _sequence.seq)
            {
                POSGlobals.LocalOrderID = (int)maxID;
                return -1;
            }
            
            LocalDB.Execute("DELETE FROM sqlite_sequence where name = ?", _sequence.name);
            return LocalDB.InsertOrReplace(_sequence);
        }

        public static int SaveGood(POSGoods posGood)
        { 
            Init();
            return LocalDB.InsertOrReplace(posGood);
        }

        public static int SaveCashPrice(POSCashPrice posCashPrice)
        {
            Init();
            return LocalDB.InsertOrReplace(posCashPrice);
        }

        public static int SaveObjects(POSObjects posObject)
        {
            Init();
            return LocalDB.InsertOrReplace(posObject);
        }
    }
}
