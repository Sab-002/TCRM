using System;
using System.Data.SqlClient;

namespace TCRM_1
{
    public static class AppConfig
    {
        //  Database connection
        public static string ConnectionString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\harol_xag05qs\\source\\repos\\TCRM_1\\TCRM_1\\Database.mdf;Integrated Security=True";

        //  Logged-in user info
        public static int CurrentAccId { get; set; }
        public static string CurrentUsername { get; set; }

        //  Global log writer for the whole system
        public static void LogEvent(int? accId, string type, string message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO Logs (AccId, type, message) VALUES (@AccId, @Type, @Message)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", accId.HasValue ? (object)accId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", type);
                        cmd.Parameters.AddWithValue("@Message", message);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Silently fail — avoid interrupting main actions
            }
        }
    }
}
