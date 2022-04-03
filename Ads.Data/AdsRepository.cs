using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Ads.Data
{
    public class AdsRepository  
    {
        private string _connectionString;
        public AdsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
      
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT  * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
        public void NewAd(Ad ad)
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads VALUES (@userId, @date, @phoneNumber, @text)";
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@text",ad.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeleteAd(int adId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "Delete from Ads Where Id=@id";
            cmd.Parameters.AddWithValue("@id", adId);
            
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetAllAds()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "select a.*, u.Name from Ads a join Users u on a.UserId=u.Id";
            conn.Open();
            var ads = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    DateSubmitted = (DateTime)reader["DateSubmitted"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Text = (string)reader["Text"],                  
                    UserName = (string)reader["Name"]
                });
            }
            return ads;
        }
        public List<Ad> GetAdsForId(int? userId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "select a.*, u.Name from Ads a join Users u on a.UserId=u.Id WHERE a.UserId=@id";
            cmd.Parameters.AddWithValue("@id", userId);
            conn.Open();
            var ads = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    DateSubmitted = (DateTime)reader["DateSubmitted"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Text = (string)reader["Text"],
                    UserName = (string)reader["Name"]
                });
            }
            return ads;
        }

    }
}
