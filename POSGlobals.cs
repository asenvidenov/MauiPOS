
using System.Data.SqlTypes;

namespace MauiPOS
{
    public static class POSGlobals
    {
        public const string DatabaseFilename = "poslocal.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        }

        public static Int32 MaxOrderID { get; set; }
        public static Int32 LocalOrderID { get; set; }
        public static int OpShift {  get; set; }
        public static DateTime ShiftStart { get; set; }

        public static Boolean LocalOnly { get; set; }
    }
}
