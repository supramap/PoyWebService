using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace PoyService
{
    public class Email
    {
        SmtpClient smtp;

        public Email()
        {
            smtp = new SmtpClient
           {
               Host = "smtp.gmail.com",
               Port = 587,
               EnableSsl = true,
			  
               DeliveryMethod = SmtpDeliveryMethod.Network,
               UseDefaultCredentials = false,
               Credentials = new NetworkCredential("supramap@gmail.com", "chUs6e2p")
           };
			ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            
			smtp.ClientCertificates.Clear();

        }

        public void sendMailToUs(string user, string desc)
        {
            var message = new MailMessage("supramap@gmail.com", "supramap@gmail.com")
                     {
                         Subject =user+" wants a poy web service develper account ",
                         Body = "There Descritopn they left:\n\n" + desc + "\n\n You can give them access at http://glenn-service.bmi.ohio-state.edu/Admin.aspx",
                     };
        
			ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            smtp.Send(message);

        }

        public void sendMailToUser(PoyServiceUser user)
        {
            var message = new MailMessage("supramap@gmail.com", user.Email)
            {
Subject = "Welcome to the Poy Web Service" ,
Body = string.Format(@"Hello {0}, 
Your request to use the Poy Web Service on {1} has been granted 
Your Access Code is as follows: {2} 
Documentation to use the web service as well as the web service its self can be found at : http://glenn-service.bmi.ohio-state.edu/PoyService.asmx
Please feel free to contact us if you have any questions 
                ~The Supra Map Team", user.UserName,user.Date,user.PassPhrase),
            };

            smtp.Send(message);
        }
    }
}