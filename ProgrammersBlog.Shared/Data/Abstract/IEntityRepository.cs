using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Data.Abstract
{
    public interface IEntityRepository<T> where T : class, IEntity, new()  // veritabanı nesnelerimi newleyebilirim ama başka bir nesne gelirse newleyemem ve buraya gelirse sorun yaratır böylece sadece vt nesnelerim T yerine gelebilecek. 
    {
        // Ekleyeceğimiz tüm metotları asenkron şekilde yapacağız(bu yüzden Task kullanıyoruz)

        // Herhangi bir şeyi getirmek için oluşturduğumuz metot.
        // Bu metotta örneğin hangi makaleyi hangi kullanıcıyı getirmesi gerektiğini söylememiz gerekiyor.Bu yüzden burda Expression veriyoruz.
        // Hangi kullanıcıyı istiyorsak onu yazdığımız kısıma 'predicate' diyoruz.
        // Kullanıcıyı getirirken kullancıya ait makaleleri de almak isteyebiliriz.O yüzden bir tane daha Expression ekliyoruz.
        // Params: Bir parametre de verebiliriz birden fazla da verebiliriz(dizi tanımladık o yüzden).Verdiğimiz tüm parametreler tek tek includeProperties arrayine eklenecektir.


        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties); //var kullanıcı=repository.GetAsync(k=>k.Id==15);

        Task<T> GetAsyncV2(IList<Expression<Func<T, bool>>> predicates,IList<Expression<Func<T, object>>> includeProperties);


        // Birden fazla entity getirmek istersek(bir listeye ihtiyacımız olursa) bu metodu kullanıcaz.
        // predicate null olabilecek çünkü makalelerin hepsini de yüklemek isteyebilirz sadece bir kategorinin makalelerini de yüklemek isteyebiliriz.
        // null gelirse tüm entityler null gelmezse bize gelen filtreye göre getiricez.
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        Task<IList<T>> GetAllAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IList<T>> SearchAsync(IList<Expression<Func<T, bool>>> predicates, params Expression<Func<T, object>>[] includeProperties);

        // Bir entity eklerken bu daha önce var mıydı diye kontrol etmek isteriz.predicate kesin verilmeli çünkü neye göre sorcağımızı bilmemiz gerekiyo.
        // bool tipinde tanımladık geriye true false dönecek
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Admin panelinde kaç tane makale,kategori olduğunu listelemek isteyebiliriz.
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        IQueryable<T> GetAsQueryable();
    }
}
