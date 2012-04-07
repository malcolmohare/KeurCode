using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeurCode
{
    // courtesy of Kyle McClellan
    // fromhttp://blogs.msdn.com/b/kylemc/archive/2011/08/18/unit-testing-a-wcf-ria-domainservice-part-2-the-repository-pattern.aspx
    public interface IRepository<T> : IRepository
    {
        IQueryable<T> GetEntities();
        void Insert(T entity);
        void Update(T entity, T original);
        void Delete(T entity);
    }

    public interface IRepository
    {
        IQueryable GetEntities();
        void Insert(object o);
        void Update(object o, object o);
        void Delete(object o);
    }
}
