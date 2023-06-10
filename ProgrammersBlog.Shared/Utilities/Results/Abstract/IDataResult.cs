using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Utilities.Results.Abstract
{
    public interface IDataResult<out T> : IResult    // Data Result sayesinde Resultlarımız içersinde verilerimizi de taşıyabileceğiz.Bir categoryi de taşıyabilir userı da o yüzden generic yapıyoruz.
    {
        // out tanımlamamızın sebebi tek bir prop içersinde istersek bir liste istersek tek bir entity taşıyabiliyor olacağımız.

        public T Data { get; } // new DataResult<Category>(ResultStatus.Success,category);
                               // new DataResult<IList<Category>>(ResultStatus.Success, categoryList);
    }
}
