using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.Shared.Utilities.Results.Concrete;

namespace ProgrammersBlog.Services.Concrete
{
    public class MailManager:IMailService
    {
        private readonly SmtpSettings _smtpSettings;

        public MailManager(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        public IResult Send(EmailSendDto emailSendDto)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail), //alpertunga004@outlook.com
                To = { new MailAddress(emailSendDto.Email) }, //alper@altu.dev
                Subject = emailSendDto.Subject, //Şifremi Unuttum // Siparişiniz Başarıyla Kargolandı.
                IsBodyHtml = true,
                Body = emailSendDto.Message // "12345" No'lu siparişiniz kargolanmıştır.
            };
            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpSettings.Server,
                Port = _smtpSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.Send(message);

            return new Result(ResultStatus.Success, $"E-Postanız başarıyla gönderilmiştir.");
        }

        public IResult SendContactEmail(EmailSendDto emailSendDto)
        {
            MailMessage message = new MailMessage                       // E-posta mesajı oluşturuyoruz. 
            {
                From = new MailAddress(_smtpSettings.SenderEmail),      // E-postayı kim gönderiyor.appsettingsden aldık.
                To = { new MailAddress("yildizberkay359@gmail.com")},   // E-postayı nereye göndericez.Birden fazla email adresi verebilirz.(o yüzden süslü parantez içinde)
                Subject = emailSendDto.Subject, 
                IsBodyHtml = true,                                      // E-posta içeriğinin HTML formatında olup olmadığını belirtir. 
                // Body: E-posta içeriğini belirler. 
                Body = $"Gönderen Kişi: {emailSendDto.Name}, Gönderen E-Posta Adresi:{emailSendDto.Email}<br/>{emailSendDto.Message}"
            };

            SmtpClient smtpClient = new SmtpClient                      // Oluşturduğumuz e-postayı gönderiyoruz.Bunun için SmtpClient sınıfının bir örneği oluşturuyoruz.
            {
                Host = _smtpSettings.Server,                            // SMTP sunucu adresi
                Port = _smtpSettings.Port,                              // SMTP sunucu bağlantı noktası
                EnableSsl = true,                                       // SSL kullanarak güvenli bir bağlantı kurup kurmayacağını belirtir.
                UseDefaultCredentials = false,                          // Varsayılan kimlik bilgilerini kullanıp kullanmayacağını belirtir.(hayır)
                Credentials = new NetworkCredential(_smtpSettings.Username,_smtpSettings.Password),   // SMTP kimlik bilgilerini belirler. 
                DeliveryMethod = SmtpDeliveryMethod.Network // DeliveryMethod: E-posta gönderme yöntemini belirtir. SmtpDeliveryMethod.Network olarak ayarlanmıştır, yani ağ üzerinden SMTP sunucusuna doğrudan gönderme yapılacaktır.
            };
            smtpClient.Send(message);                       // Oluşturulan e-posta message örneğini gönderir.

            return new Result(ResultStatus.Success, $"E-Postanız başarıyla gönderilmiştir.");
        }
    }
}
