using ProgrammersBlog.Shared.Entites.Concrete;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Utilities.Results.Concrete
{
    public class Result : IResult
    {
        // IResult Interfaceimizi implemente edecek sınıfımız.

        // get propertylerine değer vermemiz gerek bu yüzden bir constructor içersinde bu sınıf oluşturulurken bu değerleri atamamız gerek.
        public Result(ResultStatus resultStatus)
        {
            // Resultın durumunu öğrenmemiz gerekiyor.

            ResultStatus = resultStatus;
        }
        public Result(ResultStatus resultStatus,IEnumerable<ValidationError> validationErrors)
        {
            ResultStatus = resultStatus;
            ValidationErrors = validationErrors;
        }
        public Result(ResultStatus resultStatus, string message)
        {
            ResultStatus = resultStatus;
            Message = message;
        }
        public Result(ResultStatus resultStatus, string message, IEnumerable<ValidationError> validationErrors)
        {
            ResultStatus = resultStatus;
            Message = message;
            ValidationErrors = validationErrors;
        }
        public Result(ResultStatus resultStatus, string message, Exception exception)
        {
            ResultStatus = resultStatus;
            Message = message;
            Exception = exception;
        }
        public Result(ResultStatus resultStatus, string message, Exception exception, IEnumerable<ValidationError> validationErrors)
        {
            ResultStatus = resultStatus;
            Message = message;
            Exception = exception;
            ValidationErrors = validationErrors;
        }
        public ResultStatus ResultStatus { get; }
        public string Message { get; }
        public Exception Exception { get; }

        // new Result(ResultStatus.Error,exception.message,exception) // 1.kısım mutlaka olmalı diğer kısımlar olmasa da olur yani
        public IEnumerable<ValidationError> ValidationErrors { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    }
}
