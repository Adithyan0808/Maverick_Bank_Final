﻿namespace MaverickBank.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(K id);
        Task<T> Add(T entity);
        Task<T> Update(K id, T entity);
        Task<T> Delete(K id);
    }
}
