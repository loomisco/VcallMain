using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Vcall.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vcall.Helpers;

namespace Vcall.Controllers
{
    public class Vcall_ClientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public class MyDbContext : DbContext
        {
            public MyDbContext() : base("WebApps") { }
        }

        // GET: Vcall_Client



        public ActionResult Completed()
        {

            return View();
        }

        public ActionResult Errors()
        {

            return View();
        }

        public void  BuildErrors()
        {
            string _connectionString = WebConfigurationManager.ConnectionStrings["WebApps"].ToString();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //
                // Open the SqlConnection.
                //
                con.Open();
                using (SqlCommand command = new SqlCommand("Select isnull(question_text, -1) question_text from [dbo].[Vcall_Questions] where Question_Type='RU' and Question_Cust_Id_Fk = " + Request.Cookies["Cust_Id_Fk"].Value + "", con))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //  ViewBag.Cust_Id_Fk = reader["Id"].ToString();

                        
                            ViewBag.Error = reader["question_text"].ToString();

                       
                    }
                }
            }

        }

        // GET: Vcall_Client/Create
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Question_Num"] ="0";
            getGreeting();

            return View();
        }

        // POST: Vcall_Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Customer_Id_FK,Client_PID,Client_Fname,Client_MI,Client_Lname,Client_DOB,Client_Email,Client_Phone, Client_Other_Pid")] Vcall_Client vcall_Client)
        {
            try
                
            {
                var status = true;
                //Captcha!_____________________________________________
                string Robot = Request.Cookies["customer_robot"].Value;
                if (Robot == "1")
                {
                    var recaptcha = Request["g-recaptcha-response"];
                    string secretKey = "6Ldu6jIUAAAAAMef2oKOODA7lZYGdWT5KqJF1A5-";

                    var Client = new WebClient();
                    var result = Client.DownloadString(string.Format("Https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, recaptcha));
                    var obj = JObject.Parse(result);
                    status = (bool)obj.SelectToken("success");
                }

                if (status == false)
                {
                    ViewBag.Captcha = "Please verify you are not a robot!";
                    return View();

                }

                //End captcha_____________________________________________


                else if (ModelState.IsValid)
                {
                    
                    db.Vcall_Client.Add(vcall_Client);
                    vcall_Client.Customer_Id_FK = Int32.Parse(Request.Cookies["Cust_id_Fk"].Value);
                    db.SaveChanges();
                    System.Web.HttpContext.Current.Session["Personid"] = vcall_Client.Id.ToString();
                    SaveIp();
                    return RedirectToAction("Questions");

                }
                else
                {
                    return RedirectToAction("Errors");

                }
            }
            catch (Exception ex)
            {

                writelog(ex.ToString());
                return RedirectToAction("Errors");

            }
        }

        public void getGreeting()
        {

            string _connectionString = WebConfigurationManager.ConnectionStrings["WebApps"].ToString();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //
                // Open the SqlConnection.
                //
                con.Open();
                //
                // The following code uses an SqlCommand based on the SqlConnection.
                //
                ViewBag.Question = "-1";
                using (SqlCommand command = new SqlCommand("Select isnull(question_text, -1) question_text, Id from [dbo].[Vcall_Questions] where   Question_Cust_Id_Fk = " + Request.Cookies["Cust_Id_Fk"].Value + " and Question_Type = 'Greeting'", con))
                using (SqlDataReader reader = command.ExecuteReader())
                    if (reader.HasRows)
                    {
                        {
                            while (reader.Read())
                            {
                                ViewBag.Greeting = reader["question_text"].ToString();
                                System.Web.HttpContext.Current.Session["QuestionId"] = reader["Id"].ToString();
                            }
                        }
                    }
            }
        }
       

        // POST: Vcall_Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Customer_Id_FK,Client_PID,Client_Fname,Client_MI,Client_Lname,Client_DOB,Client_Email,Client_Phone,Client_Q1,Client_Q2,Client_Q3,Client_Q4,Client_Q5,Client_Q6,Client_Q7,Client_Q8,Client_Q9,Client_Q10,Dt_Complete")] Vcall_Client vcall_Client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vcall_Client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vcall_Client);
        }

        

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Questions()
        {
           // if (System.Web.HttpContext.Current.Session["Question_Num"].ToString() == null)
          //  {
          //      System.Web.HttpContext.Current.Session["Question_Num"] = "0";
          //  }

            if (System.Web.HttpContext.Current.Session["Question_Num"].ToString() == "0" )
                System.Web.HttpContext.Current.Session["Question_Num"] = "1";
            else 
                 System.Web.HttpContext.Current.Session["Question_Num"] = Convert.ToInt32(System.Web.HttpContext.Current.Session["Question_Num"]) + 1;


            ViewBag.Question = Init(System.Web.HttpContext.Current.Session["Question_Num"].ToString());

            return View();
        }

        public String Init(string Q)
        {
            try
            {


                string _connectionString = WebConfigurationManager.ConnectionStrings["WebApps"].ToString();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    //
                    // Open the SqlConnection.
                    //
                    con.Open();
                    //
                    // The following code uses an SqlCommand based on the SqlConnection.
                    //
                    ViewBag.Question = "-1";
                    using (SqlCommand command = new SqlCommand("Select isnull(question_text, -1) question_text, Id from [dbo].[Vcall_Questions] where  Question_Sort=" + Q + " and Question_Cust_Id_Fk = " + Request.Cookies["Cust_Id_Fk"].Value + " ", con))
                    using (SqlDataReader reader = command.ExecuteReader())
                        if (reader.HasRows)
                        {
                            {
                                while (reader.Read())
                                {
                                    ViewBag.Question = reader["question_text"].ToString();
                                    System.Web.HttpContext.Current.Session["QuestionId"] = reader["Id"].ToString();
                                }
                            }
                        }
                        else
                        {
                            reader.Close();

                            using (SqlCommand command1 = new SqlCommand("Select isnull(question_text, -1) question_text from [dbo].[Vcall_Questions] where Question_Type='RS' and Question_Cust_Id_Fk = " + Request.Cookies["Cust_Id_Fk"].Value + "", con))
                            using (SqlDataReader reader1 = command1.ExecuteReader())
                                if (reader1.HasRows)
                                {
                                    {
                                        while (reader1.Read())
                                        {
                                            ViewBag.Question = reader1["question_text"].ToString();
                                            Response.Redirect("Completed");
                                        }
                                    }
                                }

                        }


                }
            }
            catch (Exception ex)
            {
                writelog(ex.ToString());

            }




            return ViewBag.Question;

            }
        
        [HttpPost]
        public ActionResult SaveQuestion(FormCollection collection)
        {
            try
            {
                /* ViewData["Q_Response"] = collection[0].ToString();
                 if (ViewData["Q_Response"].ToString() == "Yes")
                     return RedirectToAction("Questions");
                 else
                     return RedirectToAction("Errors");
                 //Do something*/
                string Res = collection["Q_Response"];
                string PersonId = System.Web.HttpContext.Current.Session["Personid"].ToString();
                string QuestionId = System.Web.HttpContext.Current.Session["QuestionId"].ToString();
                SaveData(PersonId, QuestionId, Res);
                if (Res == "No")
                {
                    BuildErrors();
                    return View("Errors");
                }





                
            }
            catch (Exception ex)
            {
                writelog(ex.ToString());

            }
            return RedirectToAction("Questions");
        }

        public void SaveData(string Client, string Question, string Answer )
        {


            try
            {

           
            string _connectionString = WebConfigurationManager.ConnectionStrings["WebApps"].ToString();
            System.Data.SqlClient.SqlConnection sqlConnection1 =
               new System.Data.SqlClient.SqlConnection(_connectionString);

            //
            // Open the SqlConnection.
            //

            //
            // The following code uses an SqlCommand based on the SqlConnection.
            //
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT INTO [dbo].[Vcall_Client_Question_Response] ";
            cmd.CommandText = cmd.CommandText + "([Client_id_Fk],[Question_id_Fk],[Answer]) ";
            cmd.CommandText = cmd.CommandText + "VALUES (" + Client + "," + Question + ",'" + Answer.Substring(0, 1) + "')";
            cmd.Connection = sqlConnection1;


                sqlConnection1.Open();
                cmd.ExecuteNonQuery();
                sqlConnection1.Close();

            }
             catch (Exception ex)
            {
                writelog(ex.ToString());


            }

        }

        public void SaveIp()
        {
            try
            {
                string _connectionString = WebConfigurationManager.ConnectionStrings["WebApps"].ToString();
                System.Data.SqlClient.SqlConnection sqlConnection1 =
                   new System.Data.SqlClient.SqlConnection(_connectionString);

                //
                // Open the SqlConnection.
                //

                //
                // The following code uses an SqlCommand based on the SqlConnection.
                //
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "Update Vcall_Client set IpAddress = '" + Request.UserHostAddress + "' where id = " + System.Web.HttpContext.Current.Session["Personid"] + "";
                cmd.Connection = sqlConnection1;


                sqlConnection1.Open();
                cmd.ExecuteNonQuery();
                sqlConnection1.Close();
            }
            catch (Exception ex)
            {
                writelog(ex.ToString());

            }


        }

    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Error">The error message</param>
    private void writelog(string Error)
    {
        Log lg = new Log();
        lg.Header = DateTime.Now.ToString() + "--------------------------------------------------------------=>Error in Vcall_ClientController";

        lg.Message = Error;
        lg.writeToLog();


    }
}
}





