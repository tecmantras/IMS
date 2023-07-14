using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using UserManagememet.Data.ViewModel;
using UserManagement.Services.IRepositories;

namespace UserManagement.Services.Repositories
{
    public class EmailHelper : IEmailHelper
    {
        public readonly IConfiguration _configuration;
        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> VerifyEmailAsync(ConfimEmailRequestViewModel model)
        {
            string mailbody = $@"
        <h1>Confirmation Email</h1>
        <p>Dear {{userName}},</p>
        <p>Welcome to the Team. Your account details are:</p>
        <ul>
            <li><strong>Username:</strong> {{userEmail}}</li>
        </ul>
        <p>Please click the following link to confirm your email:</p>
        <a href=""{{confirmationLink}}"">{{confirmationLink}}</a>
        <p>Please click the following for reset your password:</p>
        <a href=""{{confirmPasswordLink}}"">{{confirmPasswordLink}}</a>
        <p>Regards,</p>
        <p>Tec Mantras</p>
    ";
            mailbody = mailbody.Replace("{userName}", model.UserName).Replace("{userEmail}", model.UserEmail)
                .Replace("{Password}", model.UserPassword).Replace("{confirmationLink}", model.ConfirmEmailLink).Replace("{confirmPasswordLink}", model.ConfirmPasswordLink);

            SendEmailViewModel sendEmailViewModel = new SendEmailViewModel();
            sendEmailViewModel.Subject = "Email Confirmation";
            sendEmailViewModel.Body = mailbody;
            sendEmailViewModel.Email = model.UserEmail;
            return await SendEmail(sendEmailViewModel);

        }
        
        public async Task<bool> SendEmail(SendEmailViewModel model)
        {
            bool sendEmail = false;
            try
            {

                // Configure the SMTP client
                var emailConfiguration = new EmailConfigurationViewModel();
                emailConfiguration.SmtpType = "smtp.gmail.com";//_configuration.GetSection("EmailConfiguration").Value;
                emailConfiguration.SmtpPort = 587;// Convert.ToInt32(_configuration.GetSection("EmailConfiguration:smtpPort").Value);
                emailConfiguration.EmailSender = "gourav.tecmantras@gmail.com";// _configuration.GetSection("EmailConfiguration:EmailSender").Value;
                emailConfiguration.Password = "dkzdjhjpvsqzgtdr";// _configuration.GetSection("EmailConfiguration:Password").Value;
                emailConfiguration.IsSsl = true;// Convert.ToBoolean(_configuration.GetSection("EmailConfiguration:IsSsl").Value);

                MailMessage mailmsg = new MailMessage();
                mailmsg.IsBodyHtml = true;
                //Set From Email ID  
                mailmsg.From = new MailAddress(emailConfiguration.EmailSender);
                //Set To Email ID  
                mailmsg.To.Add(model.Email);
                //Set Subject  
                mailmsg.Subject = model.Subject;
                //Set Body Text of Email   
                mailmsg.Body = model.Body;
                //Now set your SMTP   
                SmtpClient smtp = new SmtpClient();
                //Set HOST server SMTP detail  
                smtp.Host = emailConfiguration.SmtpType;
                //Set PORT number of SMTP  
                smtp.Port = emailConfiguration.SmtpPort;
                //Set SSL --> True / False  
                smtp.EnableSsl = emailConfiguration.IsSsl;
                NetworkCredential network = new NetworkCredential(emailConfiguration.EmailSender, emailConfiguration.Password);
                smtp.Credentials = network;
                smtp.UseDefaultCredentials = false;
                //Send Method will send your MailMessage create above.  
                await smtp.SendMailAsync(mailmsg);
                sendEmail = true;
                return sendEmail;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
