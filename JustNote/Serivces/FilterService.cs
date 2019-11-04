using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public static class FilterService<T>
    {
        private static FilterDefinitionBuilder<T> builderByOneParam = new FilterDefinitionBuilder<T>();
        private static FilterDefinitionBuilder<T> builderByManyParam = new FilterDefinitionBuilder<T>();
        private static FilterDefinition<T> filter;
        
        public static FilterDefinition<T> GetFilterByOneParam(string param, object value)
        {
            return builderByOneParam.Eq(param, value);
        }
        public static FilterDefinition<T> GetFilterByTwoParam(List<string> paramList, List<object> valueList)
        {
            filter = builderByManyParam.And(
                new List<FilterDefinition<T>>
                {
                    Builders<T>.Filter.Eq(paramList.ElementAt(0), valueList.ElementAt(0)),
                    Builders<T>.Filter.Eq(paramList.ElementAt(1), valueList.ElementAt(1)),
                });

            return filter;
        }
    }
}
