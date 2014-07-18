using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security;

namespace NwnxAssembly
{
    public static class EMailServices
    {

        public static string SendMail(string SMTP, string port, string sAddress, string sSubject, string sContent, string sFromAddress, string sPass, string sFromName)
        {

            try
            {
                MailMessage message = new System.Net.Mail.MailMessage();
                message.BodyEncoding = Encoding.ASCII;

                message.To.Add(sAddress);


                message.Subject = sSubject;
                message.From = new System.Net.Mail.MailAddress(sFromAddress, sFromName);

                message.Body = sContent;

                int iport = 587;

                Int32.TryParse(port, out iport);

                SmtpClient smtp = new SmtpClient(SMTP, iport);
                smtp.Credentials = new NetworkCredential(sFromAddress, sPass, "");
                smtp.UseDefaultCredentials = true;
                 
                using (AlternateView altView = AlternateView.CreateAlternateViewFromString(sContent, new ContentType(MediaTypeNames.Text.Html)))
                {

                    message.AlternateViews.Add(altView);
                    smtp.Send(message);
                }

                //Thread.Sleep(2000);

                message = null;
                smtp = null;

                return "Email Sent";
            }
            catch (Exception e)
            {

                return "Email Failed:" + e.ToString();
            }

        }

    }
}