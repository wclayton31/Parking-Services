using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the focus on the User ID Textbox
        UserIDTextBox.Focus();
        
        if (IsPostBack)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString);
            conn.Open();

            try
            {
                String checkUser = "Get_USERID";
                SqlCommand com = new SqlCommand(checkUser, conn);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@login_id", SqlDbType.Int).Value = UserIDTextBox.Text;
                com.Parameters.Add("@valid", SqlDbType.Bit).Direction = ParameterDirection.Output;
                bool check = false;
                string value = "";
                try
                {
                    if (com.Connection.State == ConnectionState.Closed)
                        com.Connection.Open();
                    com.ExecuteNonQuery();
                    value = com.Parameters["@valid"].Value.ToString();
                }
                catch (Exception) { }
                if (value == "True")
                    check = true;

                conn.Close();

                // Check if the user already exists
                if (!check)
                {
                    ErrorLabel.Text = "The user already exists.";
                }
            }
            catch(Exception )
            {
               
            }
            conn.Close();
        }
    }

    protected void RegisterButton_Click(object sender, EventArgs e)
    {
        try
        {
            
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString);
            conn.Open();

            String checkValidID = "Get_AllowedID";  
            SqlCommand com2 = new SqlCommand(checkValidID, conn);
            com2.CommandType = CommandType.StoredProcedure;
            com2.Parameters.AddWithValue("@id", UserIDTextBox.Text);
            int validID = Convert.ToInt32(com2.ExecuteScalar().ToString());

            if (!(validID == 1))
            {
                ErrorLabel.Text = "The ID is not registered in our system. Please contact Parking Services to validate your ID";
            }
            else
            {
                // Check if the PIN number matches the User ID
                string checkPIN = "Get_Confirmation_Pin";
                SqlCommand pin = new SqlCommand(checkPIN, conn);
                pin.CommandType = CommandType.StoredProcedure;
                pin.Parameters.AddWithValue("@id", UserIDTextBox.Text);
                string dbPIN = pin.ExecuteScalar().ToString().Replace(" ", "");
                
                if (dbPIN != ValidationPINTextBox.Text)
                {
                    ErrorLabel.Text = "The PIN you entered is incorrect.";
                }
                else
                {
                    string insert_personal = "Insert_Personal";
                    SqlCommand com3 = new SqlCommand(insert_personal, conn);
                    com3.CommandType = CommandType.StoredProcedure;
                    com3.Parameters.AddWithValue("@PID", int.Parse(UserIDTextBox.Text));
                    com3.Parameters.AddWithValue("@FName", FirstNameTextBox.Text);
                    com3.Parameters.AddWithValue("@LName", LastNameTextBox.Text);
                    com3.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                    com3.Parameters.AddWithValue("@ExpirationDate", DateTime.Now);

                    int usertype;
                    if (UserIDTextBox.Text.StartsWith("1"))
                    {
                        com3.Parameters.AddWithValue("@ParkingTypeID", 1);
                        usertype = 1;
                        }
                    else if (UserIDTextBox.Text.StartsWith("2"))
                    {
                        com3.Parameters.AddWithValue("@ParkingTypeID", 2);
                        usertype = 2;
                    }
                    else
                    {
                        com3.Parameters.AddWithValue("@ParkingTypeID", 3);
                        usertype = 3;
                    }

                    com3.ExecuteNonQuery();


                    string insert_login = "Insert_Login";
                    SqlCommand com = new SqlCommand(insert_login, conn);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@UID", int.Parse(UserIDTextBox.Text));
                    com.Parameters.AddWithValue("@Password", PasswordTextBox.Text);
                    com.Parameters.AddWithValue("@PID", int.Parse(UserIDTextBox.Text));
                    com.Parameters.AddWithValue("@UTYPE", usertype);
                    com.ExecuteNonQuery();

                    
             
                    // Take us to the login page if the user was sucessfully registered
                    Response.Write("Account created sucessfully");
                    Response.Redirect("/Login.aspx");
                }
            }

            // Close the connection
            conn.Close();
        }
        catch(Exception )
        {
           
        }
    }

    protected void ResetButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Register.aspx");
    }
}