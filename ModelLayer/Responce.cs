using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class Responce<T>
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = string.Empty;

        public T Data { get; set; } = default(T);   
    }
}
