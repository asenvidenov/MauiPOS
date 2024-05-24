using MauiPOS.Models;
using Microsoft.Data.SqlClient;
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

        public static void RetActiveShift(int OpID, ref ActiveShift asheet)
        {
            if(!IsConnected()) return;
            SqlCommand cmd = new("SP_srv_OpsShifts")
            {
                Connection = _Cnn,
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@OpID", OpID);
            var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                POSGlobals.OpShift = rdr.GetInt32(0);
                POSGlobals.ShiftStart = rdr.GetDateTime(1);
                asheet.ShiftID = rdr.GetInt32(0);
                asheet.ShiftStart = rdr.GetDateTime(1);
                return;
            }
            else
            { return; }
        }
    }
}
