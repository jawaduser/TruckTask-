using System;
using System.Collections.Generic;
using System.Text;

namespace tracktask.Service.Interface
{
    public interface IBaseService<T> where T : new()
    {
        int Insert(T item);
        int Update(T item);
        int Delete(T item);
        List<T> GetAll();
        T GetById(int id);
        
    }
}
