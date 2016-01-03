using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace TKUCourseEngHttpWebPost
{
    class Program
    {
        static void Main(string[] args)
        {
            TKUCourse tkucourse = new TKUCourse();
            String txt_StuID, txt_StuPWD, txt_CourseCode;
            int i;

            if (tkucourse.get_login_key() == false) Console.WriteLine("Cannot get the login key!!");
            Console.WriteLine("This program only provides ease of enrollment, developer for not assume any responsibility for the following conditions:\n1. Any error when using this program (including the program crashes, the school system revision, etc.).\n2. The use of any improper use.\n3. The breakdown of the above statements since many developers retain the final interpretation.\n＊ If you use it means that you have agreed to the above specification or else click \"X\" to exit the application.\n");
            Console.WriteLine("This application version : v1.0 for English Website\nMake sure to check the latest version from the following  URL http://tkucourseapplication.souceforge.net");
            Console.WriteLine("=====================================================");
            Console.Write("Please input your student ID : ");
            txt_StuID = Console.ReadLine();
            Console.Write("Please input your student password : ");
            txt_StuPWD = InputPassword();
            Console.Write("Please input course code (separate by \" , \" symbol ) : ");
            txt_CourseCode = Console.ReadLine();

            while (true)
            {
                i = 0;
                while (tkucourse.login(txt_StuID, txt_StuPWD) == false) System.Console.Write("\rAttempt login " + i++ + " times...");
                for(i=0;i<5;i++)
                {
                    string[] CourseArray = txt_CourseCode.Split(',');
                     foreach (string s in CourseArray)
                     {
                         if (tkucourse.add_course(s) == false) Console.Write("\rNow Add No." + s + " ...");
                         else Console.WriteLine("\nNo." + s + " add successfully!!");
                     }
                } 
            }
        }

        static string InputPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password;
        }
    }
    public class TKUCourse
    {
        private static CookieContainer cc = new CookieContainer();
        private static string key = "";
        private static string login_key = "";

        public bool get_login_key()
        {
            string content, state, valid, viewstate;
            Stream stream;
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            StreamReader streamReader = null;
            Encoding myEncoding = Encoding.GetEncoding("gb2312");
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create("http://www.ais.tku.edu.tw/EleCos_English/");
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                webResponse = (HttpWebResponse)webRequest.GetResponse();
                stream = webResponse.GetResponseStream();
                streamReader = new StreamReader(stream, Encoding.UTF8);
                content = streamReader.ReadToEnd();
                stream.Close();
                webResponse.Close();
                streamReader.Close();

                viewstate = content.Substring(content.IndexOf("__VIEWSTATE\" value=") + 20, content.IndexOf("\" />", content.IndexOf("__VIEWSTATE\" value=")) - content.IndexOf("__VIEWSTATE\" value=") - 20);
                viewstate = System.Web.HttpUtility.UrlEncode(viewstate);
                state = content.Substring(content.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=\"") + 33, content.IndexOf("\" />", content.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=\"")) - content.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=\"") - 33);
                state = System.Web.HttpUtility.UrlEncode(state);
                valid = content.Substring(content.IndexOf("id=\"__EVENTVALIDATION\" value=\"") + 30, content.IndexOf("\" />", content.IndexOf("id=\"__EVENTVALIDATION\" value=\"")) - content.IndexOf("id=\"__EVENTVALIDATION\" value=\"") - 30);
                valid = System.Web.HttpUtility.UrlEncode(valid);
                login_key = "__EVENTTARGET=btnLogin&__EVENTARGUMENT=&__VIEWSTATE=" + viewstate + "&__VIEWSTATEGENERATOR=" + state + "&__EVENTVALIDATION=" + valid;
            }
            catch (Exception ee) { login_key = ""; return false; }
            return true;
        }
        public bool login(string stu_id, string stu_pwd)
        {
            string content, state, valid, viewstate;
            ASCIIEncoding encode = new ASCIIEncoding();
            Stream stream;
            byte[] paramData = encode.GetBytes(login_key + "&txtStuNo=" + stu_id + "&txtPSWD=" + stu_pwd);
            HttpWebRequest webReq;

            try
            {
                webReq = (HttpWebRequest)HttpWebRequest.Create("http://www.ais.tku.edu.tw/EleCos_English/");
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = paramData.Length;
                webReq.CookieContainer = cc;
                webReq.KeepAlive = true;

                stream = webReq.GetRequestStream();
                stream.Write(paramData, 0, paramData.Length);
                stream.Close();

                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                cookieHeader = webReq.CookieContainer.GetCookieHeader(webReq.RequestUri);


                StreamReader sr = new StreamReader(webResp.GetResponseStream());
                content = sr.ReadToEnd();
                sr.Close();
                if (content.IndexOf("Your Class List:") == -1) return false;

                viewstate = content.Substring(content.IndexOf("__VIEWSTATE\" value=") + 20, content.IndexOf("\" />", content.IndexOf("__VIEWSTATE\" value=")) - content.IndexOf("__VIEWSTATE\" value=") - 20);
                viewstate = System.Web.HttpUtility.UrlEncode(viewstate);
                state = content.Substring(content.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=\"") + 33, content.IndexOf("\" />", content.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=\"")) - content.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=\"") - 33);
                state = System.Web.HttpUtility.UrlEncode(state);
                valid = content.Substring(content.IndexOf("id=\"__EVENTVALIDATION\" value=\"") + 30, content.IndexOf("\" />", content.IndexOf("id=\"__EVENTVALIDATION\" value=\"")) - content.IndexOf("id=\"__EVENTVALIDATION\" value=\"") - 30);
                valid = System.Web.HttpUtility.UrlEncode(valid);
                key = "&__EVENTARGUMENT=&__VIEWSTATE=" + viewstate + "&__VIEWSTATEGENERATOR=" + state + "&__EVENTVALIDATION=" + valid;
                cc.Add(webResp.Cookies);
                webResp.Close();
            }
            catch (Exception ee) { return false; }
            return true;
        }
        public bool add_course(string course_id)
        {
            HttpWebRequest webReq;
            HttpWebResponse webResp;
            StreamReader sr;
            string content;
            ASCIIEncoding encode = new ASCIIEncoding();
            byte[] dd = encode.GetBytes("__EVENTTARGET=btnAdd" + key + "&txtCosEleSeq=" + course_id);
            Stream stream;
            try
            {
                webReq = (HttpWebRequest)HttpWebRequest.Create("http://www.ais.tku.edu.tw/EleCos_English/actionE.aspx");
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = dd.Length;
                webReq.CookieContainer = cc;
                webReq.KeepAlive = true;
                stream = webReq.GetRequestStream();
                stream.Write(dd, 0, dd.Length);
                stream.Close();

                webResp = (HttpWebResponse)webReq.GetResponse();

                sr = new StreamReader(webResp.GetResponseStream());
                content = sr.ReadToEnd();
                if (content.IndexOf("Add successfully!!!") != -1) return true;
                else return false;
            }
            catch (Exception ee) { return false; }
        }
        public bool drop_course(string course_id)
        {
            HttpWebRequest webReq;
            HttpWebResponse webResp;
            StreamReader sr;
            string content;
            ASCIIEncoding encode = new ASCIIEncoding();
            byte[] dd = encode.GetBytes("__EVENTTARGET=btnDel" + key + "&txtCosEleSeq=" + course_id);
            Stream stream;

            try
            {
                webReq = (HttpWebRequest)HttpWebRequest.Create("http://www.ais.tku.edu.tw/EleCos_English/actionE.aspx");
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = dd.Length;
                webReq.CookieContainer = cc;
                webReq.KeepAlive = true;
                stream = webReq.GetRequestStream();
                stream.Write(dd, 0, dd.Length);
                stream.Close();

                webResp = (HttpWebResponse)webReq.GetResponse();

                sr = new StreamReader(webResp.GetResponseStream());
                content = sr.ReadToEnd();
                if (content.IndexOf("Drop successfully!!!") != -1) return true;
                else return false;
            }
            catch (Exception ee) { return false; }
        }
        private static string cookieHeader { get; set; }
    }
}
