using System.Net.Mail;

namespace BaseFuncoes.classes
{
    public static class AppUtil
    {

        public static void EnviarEmail(string email, string nome, string subject, string body)
        {
            /*
            try
            {
                var fromAddress = new MailAddress("hallanpagani@gmail.com", "Hallan Pagani");
                var toAddress = new MailAddress(email, nome);
                const string fromPassword = "pagani2210123456";

                var smtp = new SmtpClient
                {
                    Host = "smtp.umbler.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch
            {

            } */
        }

    }
}
