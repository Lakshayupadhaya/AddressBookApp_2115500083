using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IRedisCacheService
    {
        void SetData<T>(string key, T value, TimeSpan expiration);
        T GetData<T>(string key);
        void RemoveData(string key);
    }
}
