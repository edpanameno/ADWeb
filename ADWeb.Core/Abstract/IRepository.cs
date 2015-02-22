using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Abstract
{
    interface IRepository<T>
    {
        List<T> GetAll();
        T Find(string id);
        void Add(T obj);
        T Update(T obj);
        void Remove(string id);
    }
}
