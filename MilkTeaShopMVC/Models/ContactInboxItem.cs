using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MilkTeaShopMVC.Models
{
    public class ContactInboxItem
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsRead { get; set; }
        public string Summary
        {
            get
            {
                if (string.IsNullOrEmpty(Message)) return "";
                var text = Message.Replace("\r", " ").Replace("\n", " ").Trim();
                return text.Length > 95 ? text.Substring(0, 95) + "..." : text;
            }
        }
        public DateTime CreatedAtLocal
        {
            get { return DateTime.SpecifyKind(CreatedAtUtc, DateTimeKind.Utc).ToLocalTime(); }
        }
    }

    public static class ContactInboxStore
    {
        private static SqlConnection OpenConnection()
        {
            var setting = ConfigurationManager.ConnectionStrings["MilkTeaDb"];
            if (setting == null) throw new ConfigurationErrorsException("Missing MilkTeaDb connection string.");
            var connection = new SqlConnection(setting.ConnectionString);
            connection.Open();
            return connection;
        }

        private static ContactInboxItem ReadItem(SqlDataReader reader)
        {
            return new ContactInboxItem
            {
                Id = Convert.ToInt32(reader["Id"]),
                FullName = Convert.ToString(reader["FullName"]),
                Email = Convert.ToString(reader["Email"]),
                Phone = reader["Phone"] == DBNull.Value ? "" : Convert.ToString(reader["Phone"]),
                Address = reader["Address"] == DBNull.Value ? "" : Convert.ToString(reader["Address"]),
                Message = Convert.ToString(reader["Message"]),
                CreatedAtUtc = Convert.ToDateTime(reader["CreatedAtUtc"]),
                IsRead = Convert.ToBoolean(reader["IsRead"])
            };
        }

        public static List<ContactInboxItem> Latest(int limit)
        {
            var messages = new List<ContactInboxItem>();
            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand(@"SELECT TOP (@Limit) Id, FullName, Email, Phone, Address, Message, CreatedAtUtc, IsRead
FROM dbo.ContactMessages ORDER BY CreatedAtUtc DESC, Id DESC", conn))
            {
                cmd.Parameters.Add("@Limit", SqlDbType.Int).Value = Math.Max(1, Math.Min(limit, 500));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) messages.Add(ReadItem(reader));
                }
            }
            return messages;
        }

        public static ContactInboxItem ById(int id)
        {
            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand(@"SELECT Id, FullName, Email, Phone, Address, Message, CreatedAtUtc, IsRead
FROM dbo.ContactMessages WHERE Id = @Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? ReadItem(reader) : null;
                }
            }
        }

        public static int UnreadCount()
        {
            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.ContactMessages WHERE IsRead = 0", conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void MarkRead(int id)
        {
            using (var conn = OpenConnection())
            using (var cmd = new SqlCommand("UPDATE dbo.ContactMessages SET IsRead = 1 WHERE Id = @Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
