using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.GenericInterfaces
{
    public class GenericImplementation<T> : IGenericInterface<T> where T : class
    {
        private readonly StudentDataContext _Context;
        List<int> DeleteChilds = new();
        public GenericImplementation(StudentDataContext _context)
        {
            _Context = _context;
        }
        public void Add(T entity)
        {
            _Context.Set<T>().Add(entity);
        }

        public T FindById(int Id)
        {
            return _Context.Set<T>().Find(Id);
        }

        public IEnumerable<T> GetAll()
        {
            return _Context.Set<T>().ToList();
        }
        public void Remove(T entity)
        {
            _Context.Set<T>().Remove(entity);
        }

        public void RemoveChild(T entity)
        {
            _Context.Entry(entity).State = EntityState.Deleted;
        }

        public void Update(T entity)
        {
            // _Context.Set<T>().Update(entity);
            _Context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<T> entityList)
        {
            _Context.UpdateRange(entityList);
        }

        public void UpdateParentChild(T entity)
        {
            //_Context.Entry(entity).State = EntityState.Detached;
            _Context.Set<T>().Update(entity);
            //_Context.Update(entity);
        }
        public void UpdateChildWithParent(T entity, string childPropertyName)
        {
            IEnumerable<dynamic> childPropertyFromModel = (IEnumerable<dynamic>)typeof(T).GetProperty(childPropertyName).GetValue(entity);
            var PrimarykeyName = _Context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name).Single();
            var primaryKeyValue = (int)entity.GetType().GetProperty(PrimarykeyName).GetValue(entity, null);
            var ParentElement = _Context.Set<T>().AsNoTracking().Include(childPropertyName).Where(GetPropertyEqualsValueExpression(PrimarykeyName, primaryKeyValue)).FirstOrDefault();
            IEnumerable<dynamic> childPropertyFromDB = (IEnumerable<dynamic>)typeof(T).GetProperty(childPropertyName).GetValue(ParentElement);
            //Update Parent
            _Context.Update(entity);
            _Context.SaveChanges();
            _Context.Entry(entity).State = EntityState.Detached;
            UpdateChilds(childPropertyFromDB, childPropertyFromModel);
            if (DeleteChilds.Count() > 0)
            {
                var dbChild = childPropertyFromDB.ToList();
                foreach (var item in dbChild)
                {
                    int pk = (int)item.GetType().GetProperty("id").GetValue(item, null);
                    if (DeleteChilds.Contains(pk))
                    {
                        _Context.Remove(item);
                    }
                }
            }
        }
        public void UpdateChilds(IEnumerable<dynamic> childPropertyFromDB, IEnumerable<dynamic> childPropertyFromModel)
        {
            var DB_cp = childPropertyFromDB.ToList();
            var M_cp = childPropertyFromModel.ToList();
            foreach (var dbChild in DB_cp)
            {
                var DeleteFlag = true;
                var DBchildKeyValue = (int)dbChild.GetType().GetProperty("id").GetValue(dbChild, null);
                foreach (var modelChild in M_cp)
                {
                    var modelChildKeyValue = (int)modelChild.GetType().GetProperty("id").GetValue(modelChild, null);
                    if (DBchildKeyValue == modelChildKeyValue)
                    {
                        DeleteFlag = false;
                        _Context.Update(modelChild);
                        _Context.SaveChanges();
                        _Context.Entry(modelChild).State = EntityState.Detached;
                        break;
                    }
                }
                if (DeleteFlag)
                {
                    DeleteChilds.Add(DBchildKeyValue);
                }
            }
        }
        public static Expression<Func<T, bool>> GetPropertyEqualsValueExpression(string propertyName, int value)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(param, propertyName);
            var valueExp = Expression.Constant(value, typeof(int));
            var equals = Expression.Call(prop, "Equals", null, valueExp);
            return Expression.Lambda<Func<T, bool>>(equals, param);
        }
    }
}
