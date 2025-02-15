﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace DeviceManagement_WebApp.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(Guid? id);
        void Update(T entity);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }

}
