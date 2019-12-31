using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public interface IDatabaseItemService<T>
    {
        Task Create(T item);
        Task CreateManyItems(List<T> items);
        Task<T> Get(string id);
        Task<IEnumerable<T>> GetAllItems(string id); //user id
        Task<IEnumerable<T>> GetAllItemsFromFolder(string id); //folder id
        Task<IEnumerable<T>> GetAllItemsFromDatabase();
        Task Update(string id, T item);
        Task Delete(string id);
    }
}
