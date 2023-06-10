using ProgrammersBlog.Shared.Entites.Concrete;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Utilities.Results.Abstract
{
    public interface IResult
    {
        // Kullanıcılara bir sonuç dönücez ve bu sonucun durumunu kullanıcalarla paylaşmamız gerekiyor.
        public ResultStatus ResultStatus { get; } // ResultStatus.Success : Sonuç başarılıysa bu işlemi yap // ResultStatus.Error : sonuç başarısızsa bu işlemi yap 
        public string Message { get; }    
        public Exception Exception { get; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }  // Birden fazla hata olabilir o yüzden liste halinde dönücez.IEnumerable sayesinde dışardan müdahale edilemez.

    }
}
