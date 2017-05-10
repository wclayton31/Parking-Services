﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public partial class AdminSearch : System.Web.UI.Page
{
    string SearchString = "";
    public string connection;

    protected void Page_Load(object sender, EventArgs e)
    {
        // If the user is not logged in, redirect them to the login page
        if (Session["New"] != null)
        {
            // If the user is not the administrator, redirect them to the home page
            if ((Session["New"].ToString().StartsWith("2")) || (Session["New"].ToString().StartsWith("3")))
            {
                Response.Redirect("/Default.aspx");
            }
        }
        else
        {
            Response.Redirect("/Login.aspx");
        }
        
        
        connection = System.Configuration.ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ToString();
        txtSearch.Attributes.Add("onkeyup", "setTimeout('__doPostBack(\'" + txtSearch.ClientID.Replace("_", "$") + "\',\'\')', 0);");
        
        if (!IsPostBack)
        {
            Gridview2.DataBind();
        }
    }

    protected void LogoutButton_Click(object sender, EventArgs e)
    {
        // End the current session, and redirect the user to the login page
        Session.Clear();
        Session["New"] = null;
        Response.Redirect("/Login.aspx");
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        SearchString = txtSearch.Text;
    }
    public string HighlightText(string InputTxt)
    {
        string Search_Str = txtSearch.Text.ToString();       
        Regex RegExp = new Regex(Search_Str.Replace(" ", "|").Trim(), RegexOptions.IgnoreCase);             
        return RegExp.Replace(InputTxt, new MatchEvaluator(ReplaceKeyWords));
    }
    public string ReplaceKeyWords(Match m)
    {
        return "<span class=highlight>" + m.Value + "</span>";
    }
    protected void dsGridview_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {

    }
}