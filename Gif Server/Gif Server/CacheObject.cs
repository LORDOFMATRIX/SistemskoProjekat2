using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gif_Server
{
    public class CacheObject
    {
        public string key;
        public byte[] value;
        public CacheObject(string key, byte[] value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
