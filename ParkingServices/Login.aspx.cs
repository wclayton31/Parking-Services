using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


    }
    protected void LoginButton_Click(object sender, EventArgs e)
    {
        // Gather User ID
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString);
        conn.Open();
        String checkUser = "Get_USERID";
        SqlCommand com = new SqlCommand(checkUser, conn);
        com.CommandType = CommandType.StoredProcedure;
        com.Parameters.Add("@login_id", SqlDbType.Int).Value = UserIDLoginTextBox.Text;
        com.Parameters.Add("@valid", SqlDbType.Bit).Direction = ParameterDirection.Output;
        bool check = false;
        string value="";
        try
        {
           if (com.Connection.State == ConnectionState.Closed)
                com.Connection.Open();
           com.ExecuteNonQuery();
           value = com.Parameters["@valid"].Value.ToString();
        }
        catch (Exception) {}
        if (value == "True")
            check = true;

        conn.Close();
       
        // Check if the user exists
        if (check)
        {
            conn.Open();
            string checkPW = "Get_Password";
            SqlCommand pass = new SqlCommand(checkPW, conn);
            pass.CommandType = CommandType.StoredProcedure;
            pass.Parameters.Add("@login_id", SqlDbType.Int).Value = UserIDLoginTextBox.Text;
            pass.Parameters.Add("@password", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output;
            string password = "nop"; 
            try
            {
                if (pass.Connection.State == ConnectionState.Closed)
                    pass.Connection.Open();
                pass.ExecuteNonQuery();
                password = pass.Parameters["@password"].Value.ToString().Replace(" ", "");

            }
            catch (Exception) { }
            // Check if the password matches, create a new session
            if(password == PasswordLoginTextBox.Text)
            {
                // Redirect the user to their corresponding page
                string checkUserType = "Get_User_Type";
                SqlCommand utype = new SqlCommand(checkUserType, conn);
                utype.CommandType = CommandType.StoredProcedure;
                utype.Parameters.Add("@user_id", SqlDbType.Int).Value = UserIDLoginTextBox.Text;
                utype.Parameters.Add("@type", SqlDbType.Int).Direction = ParameterDirection.Output;
                int usertype = 0;
                try
                {
                      if (utype.Connection.State == ConnectionState.Closed)
                        utype.Connection.Open();
                      utype.ExecuteNonQuery();
                      usertype = Convert.ToInt32(utype.Parameters["@type"].Value.ToString());
                }
                catch (Exception) { }

                Session["New"] = UserIDLoginTextBox.Text;
  
                // 1 = Admin, 2 = Student, 3 = Faculty
                if (usertype==1)
                {
                    Response.Redirect("Admin.aspx");
                }
                else if (usertype==2)
                {
                    Response.Redirect("Students.aspx");
                }
                else if (usertype==3)
                {
                    Response.Redirect("Faculty.aspx");
                }
            }
            else
            {
                NoUserLabel.Text =  "Invalid Password.";
            }
        }
        else
        {
          NoUserLabel.Text = "The user does not exist.";
        }

        conn.Close();
    }
    protected void UserIDLoginTextBox_TextChanged(object sender, EventArgs e)
    {
        NoUserLabel.Text = "";
    }
}