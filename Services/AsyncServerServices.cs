using MauiPOS.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPOS.Services
{
    internal class AsyncServerServices
    {
        private SqlConnection _asyncCnn = new("Server=192.168.13.155;Database=pos;User Id=pos;Password=POSpassword; TrustServerCertificate=True; ");
        private SqlCommand _asyncCmd = new();

        public AsyncServerServices() { }

        private async Task<bool> IsConnected()
        {
            if (_asyncCnn.State.Equals(ConnectionState.Open))
            {
                return true;
            }
            else
            {
                try
                {
                    await _asyncCnn.OpenAsync().ConfigureAwait(false);
                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }

        public async Task<ActiveShift> RetActiveShift(int OpID)
        {
            IsConnected().Wait();
            SqlCommand cmd = new("SP_srv_OpsShifts")
            {
                Connection = _asyncCnn,
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@OpID", OpID);
            var rdr = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            var asheet = new ActiveShift();
            if (rdr.Read())
            {
                POSGlobals.OpShift = rdr.GetInt32(0);
                POSGlobals.ShiftStart = rdr.GetDateTime(1);
                asheet.ShiftID = rdr.GetInt32(0);
                asheet.ShiftStart = rdr.GetDateTime(1);
                return asheet;
            }
            else
            { return asheet; }
        }
    }
}
