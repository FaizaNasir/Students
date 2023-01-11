using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.GenericInterfaces
{
    public interface IGenericInterface<T> where T : class
    {
        IEnumerable<T> GetAll();
        T FindById(int Id);
        void Add(T entity);
        void Remove(T entity);
        void RemoveChild(T entity);
        void Update(T entity);
        void UpdateParentChild(T entity);
        public void UpdateChildWithParent(T entity, string childPropertyName);
        void UpdateRange(IEnumerable<T> entityList);
    }
}
