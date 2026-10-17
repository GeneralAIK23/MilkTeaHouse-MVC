using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MilkTeaShopMVC.Models
{
    public class FranchiseRequestItem
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string Status { get; set; }
        public bool IsRead { get; set; }
        public DateTime LocalTime { get { return DateTime.SpecifyKind(CreatedAtUtc, DateTimeKind.Utc).ToLocalTime(); } }
    }

    public static class FranchiseRequestsStore
    {
        private static SqlConnection Open()
        {
            var cs = ConfigurationManager.ConnectionStrings["MilkTeaDb"];
            if (cs == null) throw new ConfigurationErrorsException("MilkTeaDb connection string missing.");
            var c = new SqlConnection(cs.ConnectionString);
            c.Open();
            return c;
        }
        public static void Add(FranchiseRequestForm f)
        {
            using (var conn = Open())
            using (var cmd = new SqlCommand(@"INSERT INTO dbo.FranchiseRequests
(FullName,Gender,Phone,Email,Region,Message,AcceptedPrivacy)
VALUES (@FullName,@Gender,@Phone,@Email,@Region,@Message,@AcceptedPrivacy)", conn))
            {
                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 120).Value = f.FullName.Trim();
                cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 12).Value = DbText(f.Gender);
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 25).Value = f.Phone.Trim();
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = f.Email.Trim();
                cmd.Parameters.Add("@Region", SqlDbType.NVarChar, 160).Value = f.Region.Trim();
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 1500).Value = DbText(f.Message);
                cmd.Parameters.Add("@AcceptedPrivacy", SqlDbType.Bit).Value = f.AcceptPrivacy;
                cmd.ExecuteNonQuery();
            }
        }
        private static object DbText(string s) { return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim(); }
        private static FranchiseRequestItem ToItem(IDataRecord r)
        {
            return new FranchiseRequestItem
            {
                Id = Convert.ToInt32(r["Id"]),
                FullName = Convert.ToString(r["FullName"]),
                Gender = r["Gender"] == DBNull.Value ? "" : Convert.ToString(r["Gender"]),
                Phone = Convert.ToString(r["Phone"]),
                Email = Convert.ToString(r["Email"]),
                Region = Convert.ToString(r["Region"]),
                Message = r["Message"] == DBNull.Value ? "" : Convert.ToString(r["Message"]),
                Status = Convert.ToString(r["Status"]),
                IsRead = Convert.ToBoolean(r["IsRead"]),
                CreatedAtUtc = Convert.ToDateTime(r["CreatedAtUtc"])
            };
        }
        public static List<FranchiseRequestItem> Latest(int limit = 200)
        {
            var items = new List<FranchiseRequestItem>();
            using (var conn = Open())
            using (var cmd = new SqlCommand(@"SELECT TOP (@Take) Id,FullName,Gender,Phone,Email,Region,Message,Status,IsRead,CreatedAtUtc
FROM dbo.FranchiseRequests ORDER BY CreatedAtUtc DESC,Id DESC", conn))
            {
                cmd.Parameters.Add("@Take", SqlDbType.Int).Value = Math.Max(1, Math.Min(500, limit));
                using (var rd = cmd.ExecuteReader()) while (rd.Read()) items.Add(ToItem(rd));
            }
            return items;
        }
        public static FranchiseRequestItem Get(int id)
        {
            using (var conn = Open())
            using (var cmd = new SqlCommand(@"SELECT Id,FullName,Gender,Phone,Email,Region,Message,Status,IsRead,CreatedAtUtc
FROM dbo.FranchiseRequests WHERE Id=@Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                using (var rd = cmd.ExecuteReader()) return rd.Read() ? ToItem(rd) : null;
            }
        }
        public static void Update(int id, string status)
        {
            if (status != "New" && status != "Contacted" && status != "Closed")
                throw new ArgumentException("Unknown status.");
            using (var conn = Open())
            using (var cmd = new SqlCommand("UPDATE dbo.FranchiseRequests SET Status=@Status, IsRead=1 WHERE Id=@Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 25).Value = status;
                cmd.ExecuteNonQuery();
            }
        }
        public static int NewCount()
        {
            using (var conn = Open())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.FranchiseRequests WHERE Status=N'New'", conn))
                return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
