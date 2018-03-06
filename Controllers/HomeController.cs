using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Vcall.Helpers;

namespace Vcall.Controllers
{

    //get /home/index
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

           try
            {

                String Guid = Request.QueryString["Id"];

           


                if (String.IsNullOrEmpty(Guid)== true)
                {


                    ViewBag.Error = "Bad Company Id";

                    //If there is an error, go to the home view, which will show a error message
                    return View();



                }


               int GuidResponse = Cust_Init(Guid + "");
               string Country = checkIp(Request.UserHostAddress);

                


                if (Country == "US" && GuidResponse == 1)
                {
                    //All is well, go to client controller, the client entry form
                   Response.Redirect("~/Vcall_Client/Create");
                }
                else
                {
                    ViewBag.Error = "Bad Country Code";
                    return View();
                }
                     
            }

            catch (Exception ex)
            {
                writelog(ex.ToString());

            }


            //Ifit made it this far, there is an error.  It was not redirected to ~/Vcall_Client/Create
            return View();




        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Error">The error message</param>
        private void writelog(string Error)
        {
            Log lg = new Log();
            lg.Header = DateTime.Now.ToString() + "=>Error in HomeController" ;

            lg.Message = Error;
            lg.writeToLog();


        }



        public int Cust_Init(string id)
        {
            int ok=-1;

            string _connectionString = WebConfigurationManager.ConnectionStrings["WebApps"].ToString();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                
                con.Open();
                
                String sql;
                sql = "SELECT [Id], Customer_Name,Customer_logo, ";
                sql = sql + " customer_robot   FROM [Vcall_Customer] ";
                sql = sql + "where Customer_Guid='" + id.ToString().Replace("'", "''") + "'";

                using (SqlCommand command = new SqlCommand(sql, con))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                        if (reader.HasRows == false)
                        {
                        ok= - 1;
                        }
                        else
                        {
                         while (reader.Read())
                          {


                           Response.Cookies["Cust_Id_Fk"].Value = reader["Id"].ToString();
                           Response.Cookies["Customer_Name"].Value = reader["Customer_Name"].ToString();
                           Response.Cookies["Customer_logo"].Value = reader["Customer_logo"].ToString();
                           Response.Cookies["customer_robot"].Value = reader["customer_robot"].ToString();
                            ok= 1;

                            }
                       }

                    }
                }

            return ok;

        }
        

        
        /// Check that the IP Address is coming from the US
        public string checkIp(string Ip)
        {
            string Country;
            try
            {

                if (Ip == "::1" || Ip.ToString().Substring(0, 7) == "192.168")
                {
                    return "US";
                }
                else
                {
                  
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ipapi.co/" + Ip + "/country/");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII);
                    Country = reader.ReadToEnd();
                }
                return Country;
            }
            catch
            {
               

                return "-1";

            }
        }


    }
   

}