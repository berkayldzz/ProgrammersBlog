using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;

namespace ProgrammersBlog.Services.Abstract
{
    public interface IMailService
    {
        IResult Send(EmailSendDto emailSendDto); 
        IResult SendContactEmail(EmailSendDto emailSendDto); 
    }
}
