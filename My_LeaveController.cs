using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using System.Data.SqlClient;
using System.Data;
using LMS.Repository;
using System.Text.RegularExpressions;
using LMS.IRepository;
using LMS.Filter;


namespace LMS.Controllers;


[ExceptionLogFilter]
public class My_Leave : Controller
{
    private readonly ILogger<My_Leave> _logger;
    List<RequestLeaveDetails> ec = new List<RequestLeaveDetails>();
    public string s1="Apply";
    public string s2="Accept";
    int count;
    public object Session { get; private set; }
     private readonly IDatabasedetails _databasedetails;
       
     
     private readonly string  ? ConnectionString;
    public My_Leave(ILogger<My_Leave> logger,IDatabasedetails  databasedetails,IConfiguration configuration)
    {
        _logger = logger;
        _databasedetails = databasedetails;
        ConnectionString=configuration["ConnectionStrings:DefaultConnection"];
    }
    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }
    [HttpPost]
     public IActionResult Signup(Signup Value)
    {
        string result =  _databasedetails.SearchDetails(Value.UserName,Value.PassWord,Value.UserID);

        if(result == "Ok")
        {
         HttpContext.Session.SetString("Session",Value.UserID);
         HttpContext.Session.SetString("Username",Value.UserName);
         var cookieOptions = new CookieOptions();
         cookieOptions.Expires = DateTime.Now.AddDays(1);
         Response.Cookies.Append("LastLoginTime",DateTime.Now.ToString(),cookieOptions);
         Console.WriteLine(Request.Cookies["LastLoginTime"]);
         TempData["Time"]="Last Login:"+Request.Cookies["LastLoginTime"];
         TempData["alertmessage"] = "Logged in succussfully, Welcome";
        
        return RedirectToAction("Login","Home");
        }
        else{
            ViewBag.Message="Invalid Data";
            return View();
        }
    }
    
    [HttpGet]
        public IActionResult ReEnterPassWord()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ReEnterPassWord(ForgetPassWord ResetPassWord)
        {
           
            string result = _databasedetails.Forgetpassword(ResetPassWord.Name,ResetPassWord.PassWord2);
            Console.WriteLine(result);
            if(result=="ok")
            {
            return View("Signup");
            }
            else
            {
                ViewBag.Message1="Wrong UserName";
                return View("ReEnterPassWord");
            }
            }
           
         [HttpGet]
         public IActionResult RequestLeave()
         {
             Regex regex = new Regex("^(?=.*?[INT])(?=.*?[00])(?=.*?[0-9]).{8,8}$");
            if(regex.IsMatch(HttpContext.Session.GetString("Session"))==true)
            { 
            TempData["Userid"]= HttpContext.Session.GetString("Session");
            TempData["Username"]= HttpContext.Session.GetString("Username");
            return View();
            }
         TempData["Warning"] ="This is Employess Page don't access.";
         return RedirectToAction("Index","Home");
         }
        
        [HttpPost]
        public IActionResult RequestLeave(RequestLeaveDetails leb,string Submit)
        {
            
            
            
            
            string result= _databasedetails.LeaveRequest(leb.UserID,leb.UserName,leb.Email,leb.Description,leb.StartDate,leb.EndDate,leb.LeaveType,Submit);
            
           
           
            if(result=="ok")
            { 
            
            return View();
            }
            else
            {
                ViewBag.Message="Wrong Data";
                return View();
             }
            
        
        }
  
        public IActionResult ShowLeaveDetails(string empsearch)
        {
            Regex regex2 = new Regex("^(?=.*?[AD])(?=.*?[00])(?=.*?[0-9]).{7,7}$");
           
             
           
            
            if(regex2.IsMatch(HttpContext.Session.GetString("Session"))==true)
            {
                
            
            
            using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("CancelvalueProcedure",sqlConnection))
                {
                   if(!string.IsNullOrEmpty(empsearch)){
                   sqlCommand.CommandType = CommandType.StoredProcedure;
                   sqlCommand.Parameters.AddWithValue("@Action","SELECT");
                   sqlCommand.Parameters.AddWithValue("@SpecificCondition","SEARCH");
                   sqlCommand.Parameters.AddWithValue("@UserName",empsearch);
                   sqlCommand.Parameters.AddWithValue("@Userid",empsearch);
                   sqlCommand.Parameters.AddWithValue("@Mail",empsearch);
                   sqlCommand.Parameters.AddWithValue("@Des",empsearch);
                   sqlCommand.Parameters.AddWithValue("@Start_Date",empsearch);
                   sqlCommand.Parameters.AddWithValue("@End_Date",empsearch);
                   sqlCommand.Parameters.AddWithValue("@Leave_Type",empsearch);
                   sqlCommand.Parameters.AddWithValue("@EmpRequest",empsearch);
                   sqlConnection.Open();
                   SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
                   System.Data.DataSet ds = new System.Data.DataSet();
                   sda .Fill(ds);
                  
                   foreach(System.Data.DataRow dr in ds.Tables[0].Rows)
                   {
                    ec.Add(new RequestLeaveDetails {
                    UserName =Convert.ToString(dr["UserName"]),
                    Email = Convert.ToString(dr["Mail"]),
                    UserID = Convert.ToString(dr["Userid"]),
                    StartDate=Convert.ToString(dr["Start_Date"]),
                    EndDate=Convert.ToString(dr["End_Date"]),
                    LeaveType=Convert.ToString(dr["Leave_Type"]),
                    Description=Convert.ToString(dr["Des"]),
                    EmRequest=Convert.ToString(dr["EmRequest"]),
                    ManRequest = Convert.ToString(dr["ManRequest"])
                   });
                  
                   
                   }

                  ModelState.Clear();
                  return View(ec);
                   }
                   else{
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                   sqlCommand.Parameters.AddWithValue("@Action","SELECT");
                   sqlCommand.Parameters.AddWithValue("@SpecificCondition","Print");
                   sqlConnection.Open();
                   SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
                   System.Data.DataSet ds = new System.Data.DataSet();
                   sda .Fill(ds);
                  
                   foreach(System.Data.DataRow dr in ds.Tables[0].Rows)
                   {
                    ec.Add(new RequestLeaveDetails {
                    UserName =Convert.ToString(dr["UserName"]),
                    Email = Convert.ToString(dr["Mail"]),
                    UserID = Convert.ToString(dr["Userid"]),
                    StartDate=Convert.ToString(dr["Start_Date"]),
                    EndDate=Convert.ToString(dr["End_Date"]),
                    LeaveType=Convert.ToString(dr["Leave_Type"]),
                    Description=Convert.ToString(dr["Des"]),
                    EmRequest=Convert.ToString(dr["EmRequest"]),
                    ManRequest = Convert.ToString(dr["ManRequest"])
                    
                   });
              
                    }
                  ModelState.Clear();
                  return View(ec);
                   }
                }
           
              
            
            }
           
             
            }
             
            
           TempData["Warning"] ="This is Manager Page don't access.";
          
         
           return RedirectToAction("Index","Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString("Session","");
            return RedirectToAction("Index","Home");
        }
      
        [HttpGet]
        public IActionResult CancelLeave()
        {
             Regex regex = new Regex("^(?=.*?[INT])(?=.*?[00])(?=.*?[0-9]).{8,8}$"); 
            if(regex.IsMatch(HttpContext.Session.GetString("Session"))==true)                                                                      
            {
            return View();
            }
          TempData["Warning"] ="This is Employess Page don't access.";
          
          return RedirectToAction("Index","Home");
        }
        [HttpPost]
         public IActionResult CancelLeave(CancelRequest RLD)
        {
          
                
            string result= Repositorydetails.RemoveDetails(RLD);
            if(result == "ok")
            {
            return View("CancelLeave");
            }
            return View();
         
        }
       
       
       

     [HttpGet]

       public IActionResult LeaveSummary(RequestLeaveDetails LER)
        {
            
           
            string Name = HttpContext.Session.GetString("Session").ToString();
            Console.WriteLine(Name);
             
            
            using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("FinaltableProcedure",sqlConnection))
            {  
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","SELECT");
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","SEARCH");
            sqlCommand.Parameters.AddWithValue("@Userid",Name);
            sqlConnection.Open();
            SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
             System.Data.DataSet ds = new System.Data.DataSet();
             sda .Fill(ds);
             List<RequestLeaveDetails> ec =new List<RequestLeaveDetails>();
             foreach(System.Data.DataRow dr in ds.Tables[0].Rows)
             {
                ec.Add(new RequestLeaveDetails {
            Privileged_Leave=((int)dr["Privileged_Leave"]),
            Compansatory_Leave=((int)dr["Compansatory_Leave"]),
            Maternity_Leave=((int)dr["Maternity_Leave"]),
            Paternity_Leave=((int)dr["Paternity_Leave"]),
            LOP_Leave=((int)dr["LOP_Leave"]),
            Carry_Forward=((int)dr["Carry_Leave"]),
            Encashed_Days=((int)dr["Encashed_Leave"]),
            Marriage_Leave=((int)dr["Marriage_Leave"]),
            MTP_Leave=((int)dr["MTP_Leave"]),
            UserName = (dr["UserName"].ToString()),
            UserID = (dr["UserID"].ToString())
            
            
            
                });
             }
            
             ModelState.Clear();
             return View(ec);
            }
            }
         
            
        } 

        [HttpPost] // [HttpGet]
        public IActionResult Compansatory_Eligibility( RequestLeaveDetails LER ,string username, string userid, string sDate, string EDate,string Submit,string Type)
        {
        
       
           if(!string.IsNullOrEmpty(HttpContext.Session.GetString("Session")))
           {
            // Console.WriteLine(Type);
            int privileged=0, Compansatory=0,Maternity=0,Paternity=0,MTP=0,Marriage=0,Carry=0,LOP=0,Encashed=0;
           
        //    SqlConnection sqlconn = new SqlConnection("Data source = DESKTOP-1MRHJV7;Initial Catalog = LMS;integrated security=SSPI");
            
            
        //    sqlconn.Open();
        //    SqlCommand command = new SqlCommand("select count(*) from Table_4 WHERE Userid='"+userid+"'AND UserName ='"+username+"'AND Start_Date='"+sDate+"'AND End_Date='"+EDate+"';",sqlconn);
             using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("CancelvalueProcedure",sqlConnection))
            {  
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","SELECT");
            
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","COUNT");
            sqlCommand.Parameters.AddWithValue("@Userid",userid);
            sqlCommand.Parameters.AddWithValue("@UserName",username);
            sqlCommand.Parameters.AddWithValue("@Start_Date",sDate);
            sqlCommand.Parameters.AddWithValue("@End_Date",EDate);
               
            // sqlCommand.Parameters.AddWithValue("@Userid",Name);
            sqlConnection.Open();
            int count = Convert.ToInt32(sqlCommand.ExecuteScalar());
        //    Console.WriteLine(count);
           
        
      
    
           if(count>0)
           {
           
           
            // SqlCommand command1= new SqlCommand("Update Table_4 set ManRequest='"+Submit+"' where Userid='"+userid+"'AND UserName ='"+username+"'AND Start_Date='"+sDate+"'AND End_Date='"+EDate+"';",sqlconn);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","UPDATE");
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","VALUE1");
            sqlCommand.Parameters.AddWithValue("@Userid",userid);
            sqlCommand.Parameters.AddWithValue("@UserName",username);
            sqlCommand.Parameters.AddWithValue("@Start_Date",sDate);
            sqlCommand.Parameters.AddWithValue("@End_Date",EDate);
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            
            int Value =0;
           
            // SqlCommand sqlcommand = new SqlCommand("select * from Table_4 where Userid='"+userid+"'AND UserName ='"+username+"'AND Start_Date='"+sDate+"'AND End_Date='"+EDate+"' AND ManRequest='"+s2+"' AND EmRequest='"+s1+"' AND Leave_Type ='"+Type+"';",sqlconn );
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","SELECT");
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","ADDVALUE");
            sqlCommand.Parameters.AddWithValue("@Userid",userid);
            sqlCommand.Parameters.AddWithValue("@UserName",username);
            sqlCommand.Parameters.AddWithValue("@Start_Date",sDate);
            sqlCommand.Parameters.AddWithValue("@End_Date",EDate);
            sqlCommand.Parameters.AddWithValue("@ManRequest",s2);
            sqlCommand.Parameters.AddWithValue("@EmRequest",s1);
            sqlCommand.Parameters.AddWithValue("@Leave_Type",Type);
            sqlConnection.Open();
            
            
        
           
             SqlDataReader srd = sqlcommand.ExecuteReader();
           
             while(srd.Read())
             {
                if((srd["Userid"].Equals(userid))&&(srd["UserName"].Equals(username))&&(srd["Start_Date"].Equals(sDate))&&(srd["End_Date"].Equals(EDate))&&(srd["ManRequest"].Equals(s2))&&(srd["EmRequest"].Equals(s1))&&(srd["Leave_Type"].Equals(Type))==true)
                {
                    Value = Value+1;
                }
               
             }
            
             Console.WriteLine(Value);
            
           

            if(Value == 1)
            {
                using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommandline = new SqlCommand("FinaltableProcedure",sqlConnection))
            {  
            // SqlCommand command5 =new SqlCommand("select* from Table_6 where UserName ='"+username+"' and UserID ='"+userid+"';",sqlconn);
            sqlCommandline.CommandType = CommandType.StoredProcedure;
            sqlCommandline.Parameters.AddWithValue("@Action","SELECT");
            
            sqlCommandline.Parameters.AddWithValue("@SpecificCondition","VALUE2");
            sqlCommandline.Parameters.AddWithValue("@UserName",username);
            
            
            Console.WriteLine(username);
            SqlConnection.Open();
            SqlDataReader sdr = sqlCommandline.ExecuteReader(); 
           
             
            while(sdr.Read())
            {
           
                
               
                if(Type.Equals("Privileged Leave")==true)
                {
                    
                    privileged = ((int)sdr["Privileged_Leave"]-1);
                   
                    
            
                 
                }
                else if(Type.Equals("Compensatory Leave")==true)
                {
                
                     
                  Compansatory = ((int)sdr["Compansatory_Leave"]-1);
                }
                else if(Type.Equals("Maternity Leave")==true)
                {
                   
                  Maternity = ((int)sdr["Maternity_Leave"]-1);
                }
                else if(Type.Equals("Paternity Leave")==true)
                {
                  Paternity = ((int)sdr["Paternity_Leave"]-1);
                }  
                else if(Type.Equals("LOP Leave")==true)
                {
                    LOP = ((int)sdr["LOP_Leave"]-1);
                }
                else if(Type.Equals("Carry Forward")==true)
                {
                    Carry = ((int)sdr["Carry_Leave"]-1);
                }
                else if(Type.Equals("Encashed Days")==true)
                {
                    Encashed= ((int)sdr["Encashed_Leave"]-1);
                }
                else if(Type.Equals("Marriage Leave")==true)
                {
                    Marriage = ((int)sdr["Marriage_Leave"]-1);
                }
                else if(Type.Equals("MTP Leave")==true)
                {
                    MTP = ((int)sdr["MTP_Leave"]-1);
                }
                
            }
            // Console.WriteLine(privileged);
            sqlconn.Close();
            if(Type.Equals("Privileged Leave")==true)
            {
                // sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set  Privileged_Leave='"+ privileged +"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","PRIVILEGED_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Privileged_Leave",privileged);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                 
            }
            else if(Type.Equals("Compensatory Leave")==true)
                {
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set  Compansatory_Leave='"+ Compansatory+"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);

                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","COMPANSATORY_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Compansatory_Leave",Compansatory);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                }
            else if(Type.Equals("Maternity Leave")==true)
            {
                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set Maternity_Leave ='"+Maternity+"'where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","MATERNITY_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Maternity_Leave",Maternity);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
            }

            else if(Type.Equals("Paternity Leave")==true)
                {
                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set Paternity_Leave='"+Paternity+"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","PATERNITY_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Paternity_Leave",Paternity);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                }
                else if(Type.Equals("LOP Leave")==true)
                {
                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set LOP_Leave='"+LOP+"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","LOP_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@LOP_Leave",LOP);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                }
                else if(Type.Equals("Carry Forward")==true)
                {
                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set Carry_Leave='"+Carry+"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","CARRY_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Carry_Leave",Carry);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                }
               else  if(Type.Equals("Encashed Days")==true)
                {

                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set Encashed_Leave='"+Encashed+"'where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","ENCASHED_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Encashed_Leave",Encashed);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                   
                }
               else if(Type.Equals("Marriage Leave")==true)
                {
                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set Marriage_Leave='"+Marriage+"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","MARRIAGE_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@Marriage_Leave",Marriage);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                }
               else if(Type.Equals("MTP Leave")==true)
                {
                    
                //  sqlconn.Open();
                //  SqlCommand command2= new SqlCommand("Update Table_6 set MTP_Leave='"+MTP+"' where Userid='"+userid+"'AND UserName ='"+username+"';",sqlconn);
                //  command2.ExecuteNonQuery();
                //  sqlconn.Close();
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","UPDATE");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","MTP_LEAVE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlCommandline.Parameters.AddWithValue("@MTP_Leave",MTP);
                 sqlConnection.Open();   
                 sqlCommandline.ExecuteNonQuery();
                }
           
        //    SqlDataAdapter dataAdapter = new SqlDataAdapter( "select*from Table_6 where UserName ='"+username+"' and UserID ='"+userid+"';", sqlconn);
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","SELECT");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","VALUE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlConnection.Open();
                 SqlDataAdapter dataAdapter = new SqlDataReader(sqlCommandline);   
           System.Data.DataSet ds = new System.Data.DataSet();
           dataAdapter .Fill(ds);
           List<RequestLeaveDetails> ec =new List<RequestLeaveDetails>();
           
                   
            foreach(System.Data.DataRow dr in ds.Tables[0].Rows)
            {
            ec.Add(new RequestLeaveDetails 
            {
            
            Privileged_Leave=((int)dr["Privileged_Leave"]),
            Compansatory_Leave=((int)dr["Compansatory_Leave"]),
            Maternity_Leave=((int)dr["Maternity_Leave"]),
            Paternity_Leave=((int)dr["Paternity_Leave"]),
            LOP_Leave=((int)dr["LOP_Leave"]),
            Carry_Forward=((int)dr["Carry_Leave"]),
            Encashed_Days=((int)dr["Encashed_Leave"]),
            Marriage_Leave=((int)dr["Marriage_Leave"]),
            MTP_Leave=((int)dr["MTP_Leave"]),
            UserName = (dr["UserName"].ToString()),
            UserID = (dr["UserID"].ToString()),
            DateTime = (dr["DateTime"].ToString())
            
            
            });
            }
             ModelState.Clear();
             return View(ec);  
           }}
                 sqlCommandline.CommandType = CommandType.StoredProcedure;
                 sqlCommandline.Parameters.AddWithValue("@Action","SELECT");
            
                 sqlCommandline.Parameters.AddWithValue("@SpecificCondition","VALUE");
                 sqlCommandline.Parameters.AddWithValue("@UserName",username); 
                 sqlCommandline.Parameters.AddWithValue("@UserID",userid); 
                 sqlConnection.Open();
                 SqlDataAdapter dataAdapter = new SqlDataReader(sqlCommandline);   
           System.Data.DataSet da = new System.Data.DataSet();
           data .Fill(da);
           List<RequestLeaveDetails> ecb =new List<RequestLeaveDetails>();
           foreach(System.Data.DataRow dr in da.Tables[0].Rows)
            {
            ecb.Add(new RequestLeaveDetails 
            {
            
            Privileged_Leave=((int)dr["Privileged_Leave"]),
            Compansatory_Leave=((int)dr["Compansatory_Leave"]),
            Maternity_Leave=((int)dr["Maternity_Leave"]),
            Paternity_Leave=((int)dr["Paternity_Leave"]),
            LOP_Leave=((int)dr["LOP_Leave"]),
            Carry_Forward=((int)dr["Carry_Leave"]),
            Encashed_Days=((int)dr["Encashed_Leave"]),
            Marriage_Leave=((int)dr["Marriage_Leave"]),
            MTP_Leave=((int)dr["MTP_Leave"]),
            UserName = (dr["UserName"].ToString()),
            UserID = (dr["UserID"].ToString()),
             DateTime = (dr["DateTime"].ToString())
            
            
            });
            }
             ModelState.Clear();

             return View(ecb);
            }
            }
        }
            }
           }
        return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult ManagerCheckingLeave()
       {
             return View();
       }
       [HttpPost]
       public IActionResult ManagerCheckingLeave(RequestLeaveDetails RLD)
       {
            using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("CancelvalueProcedure",sqlConnection))
            {  
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","SELECT");
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","STORE");
            sqlCommand.Parameters.AddWithValue("@Userid",RLD.UserID);
            sqlConnection.Open();
           int count = Convert.ToInt32(command.ExecuteScalar());
          
           
           if(count>0)
           {

            TempData["mydata"]=RLD.UserID;
            // Console.WriteLine( TempData["mydata"]);
            
            return RedirectToAction("EmplyoeeLeaveDetail","My_Leave");
           }
           else{
            ViewBag.Message="Invalid Data";
            return View();
           }
        }
         }    
        }
        [HttpGet]
        public IActionResult EmpChecking()
        {
            Regex regex = new Regex("^(?=.*?[INT])(?=.*?[00])(?=.*?[0-9]).{8,8}$"); 
            if(regex.IsMatch(HttpContext.Session.GetString("Session"))==true)                                                                      
            {
            return View();
            
             }
            TempData["Warning"] ="This is Employess Page don't access. ";
          
             return RedirectToAction("Index","Home");
        }
        [HttpPost]
        public IActionResult EmpChecking(RequestLeaveDetails EC)
        {
             
        //    SqlConnection sqlconn = new SqlConnection("Data source = DESKTOP-1MRHJV7;Initial Catalog = LMS;integrated security=SSPI");
        //    sqlconn.Open();
        //    SqlCommand command = new SqlCommand("select count(*) from Table_4 WHERE Userid='"+EC.UserID+"';",sqlconn);
          
          using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("CancelvalueProcedure",sqlConnection))
            {  
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","SELECT");
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","Value");
            sqlCommand.Parameters.AddWithValue("@Userid",EC.UserID);
            sqlConnection.Open();
           int count = Convert.ToInt32(command.ExecuteScalar());
         
           if(count>0)
           {
            TempData["EmpData"] = EC.UserID;
            return RedirectToAction("Show_Result","My_Leave");
           }
           else 
           {
            ViewBag.Message="Invalid Data";
            return View();
           }
            }
        }
        [HttpGet]
        public IActionResult EmplyoeeLeaveDetail()
       {
        // Console.WriteLine("Hello"); 
        string? EmpID = TempData["mydata"].ToString();
        // Console.WriteLine(EmpID);
            using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("FinaltableProcedure",sqlConnection))
            {  
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@Action","SELECT");
            sqlCommand.Parameters.AddWithValue("@SpecificCondition","SEARCH");
            sqlCommand.Parameters.AddWithValue("@UserID",EmpID);
            sqlConnection.Open();
            SqlDataAdapter data = new SqlDataAdapter(sqlCommand);
           System.Data.DataSet da = new System.Data.DataSet();
           data .Fill(da);
           List<RequestLeaveDetails> ecb =new List<RequestLeaveDetails>();
           foreach(System.Data.DataRow dr in da.Tables[0].Rows)
            {
            ecb.Add(new RequestLeaveDetails 
            {
            
            Privileged_Leave=((int)dr["Privileged_Leave"]),
            Compansatory_Leave=((int)dr["Compansatory_Leave"]),
            Maternity_Leave=((int)dr["Maternity_Leave"]),
            Paternity_Leave=((int)dr["Paternity_Leave"]),
            LOP_Leave=((int)dr["LOP_Leave"]),
            Carry_Forward=((int)dr["Carry_Leave"]),
            Encashed_Days=((int)dr["Encashed_Leave"]),
            Marriage_Leave=((int)dr["Marriage_Leave"]),
            MTP_Leave=((int)dr["MTP_Leave"]),
            UserName = (dr["UserName"].ToString()),
            UserID = (dr["UserID"].ToString()),
            DateTime=(dr["DateTime"].ToString())
            
            
            });
            }
            
             ModelState.Clear();
             return View(ecb);
            }
            }
       }
       

        [HttpPost]
        public IActionResult EmplyoeeLeaveDetail(RequestLeaveDetails RLD)
        {
         
        //   string? EmpID = TempData["mydata"].ToString();
          Console.WriteLine("Hello"); 
        

          return View();
            // return View(Repository.AllList);
        }
         [HttpGet]
        
        public IActionResult Show_Result()
        {
           string? EmpID =  TempData["EmpData"].ToString();
           Regex regex = new Regex("^(?=.*?[INT])(?=.*?[00])(?=.*?[0-9]).{8,8}$"); 
             if(regex.IsMatch(HttpContext.Session.GetString("Session"))==true)                                                                      
            {
                
              
                 using(SqlConnection sqlConnection=new SqlConnection(ConnectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand("CancelvalueProcedure",sqlConnection))
                {     
                   sqlCommand.CommandType = CommandType.StoredProcedure;
                   sqlCommand.Parameters.AddWithValue("@Action","SELECT");
                   sqlCommand.Parameters.AddWithValue("@SpecificCondition","Multivalue");
                   sqlCommand.Parameters.AddWithValue("@Userid",EmpID);
                   sqlConnection.Open();
                   SqlDataAdapter data = new SqlDataAdapter(sqlCommand);
                   System.Data.DataSet da = new System.Data.DataSet();
                  data .Fill(da);
                  List<RequestLeaveDetails> ecb =new List<RequestLeaveDetails>();
                  if(da.Tables.Count>0){
                 foreach(System.Data.DataRow dr in da.Tables[0].Rows)
                 {
                ecb.Add(new RequestLeaveDetails 
                {
            
            Privileged_Leave=((int)dr["Privileged_Leave"]),
            Compansatory_Leave=((int)dr["Compansatory_Leave"]),
            Maternity_Leave=((int)dr["Maternity_Leave"]),
            Paternity_Leave=((int)dr["Paternity_Leave"]),
            LOP_Leave=((int)dr["LOP_Leave"]),
            Carry_Forward=((int)dr["Carry_Leave"]),
            Encashed_Days=((int)dr["Encashed_Leave"]),
            Marriage_Leave=((int)dr["Marriage_Leave"]),
            MTP_Leave=((int)dr["MTP_Leave"]),
            UserName = (dr["UserName"].ToString()),
            UserID = (dr["UserID"].ToString()),
            DateTime=(dr["DateTime"].ToString())
            
            
            
                  });
                  }
                 ModelState.Clear();
                 return View(ecb);
                  }
                }  
              }
            }
             ViewBag.Message="Wrong Data";
             return View();
          }
      
    //   HttpClient client  = new HttpClient();
     public IActionResult Send_Message(){
        Regex regex = new Regex("^(?=.*?[AD])(?=.*?[00])(?=.*?[0-9]).{7,7}$"); 
            if(regex.IsMatch(HttpContext.Session.GetString("Session"))==true)                                                                      
            {
   

     return View();
            }
           TempData["Warning"] ="This is Manager Page don't access.";
           return RedirectToAction("Index","Home");
       
     
      }

        
                 
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 