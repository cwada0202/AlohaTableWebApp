using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;

namespace WebAppECart.Models
{
    public class Account
    {
        [Required(ErrorMessage = "Please enter your User ID")]
        //[Display(Name = "Username : ")]
        public string UserId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your Password.")]
        //[Display(Name = "Password : ")]
        public string Password { get; set; }
        public string UserName { get; set; }


        //This method validates the Login credentials
        public String LoginProcess(String strUserName, String strPassword)
        {
            String message = "";
            // my connection string
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand("Select * from login where userName=@userName", con);
            cmd.Parameters.AddWithValue("@userName", strUserName);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bool login = (strPassword.Equals(reader["Password"].ToString(), StringComparison.InvariantCulture)) ? true : false;
                    if (login)
                    {
                        message = "1";
                        UserName = reader["UserName"].ToString();
                    }
                    else
                        message = "Invalid Credentials";
                }
                else
                    message = "Invalid Credentials";

                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                con.Close();

            }
            catch (Exception ex)
            {
                message = ex.Message.ToString() + "Error.";
            }
            return message;
        }
    }
}