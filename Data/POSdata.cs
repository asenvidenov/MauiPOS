using MauiPOS.Models;
using Microsoft.Data.SqlClient;
using SQLite;

namespace MauiPOS.Data
{
    public class POSdata
    {
        SQLiteConnection? LocalDB;
        public POSdata() { }

        async Task Init()
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
        
        public async Task<int> SavePOSops(POSOps posOp)
        {
            await Init();
            return LocalDB.InsertOrReplace(posOp);
        }
        public async void LogOut()
        {
            await Init();
            SQLiteCommand cmd = new(LocalDB);
            cmd.CommandText = "DELETE from POSOps";
            cmd.ExecuteNonQuery();
        }

        public async Task<int> SaveMaxOrderID()
        {
            await Init();
            sqlite_sequence _sequence = new sqlite_sequence() { name="POSOrders", seq=POSGlobals.MaxOrderID};
            SQLiteCommand cmd = new(LocalDB);
            cmd.CommandText = "SELECT MAX(OrderID) from POSOrders";
            Int32? maxID = cmd.ExecuteScalar<Int32?>();
            maxID = maxID == null ? 0 : maxID.Value;
            if (maxID > _sequence.seq)
            {
                POSGlobals.MaxOrderID = -1;
                return -1;
            }
            
            LocalDB.Execute("DELETE FROM sqlite_sequence where name = ?", _sequence.name);
            return LocalDB.InsertOrReplace(_sequence);
        }

    }
}
