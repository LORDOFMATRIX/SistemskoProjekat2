using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Gif_Server
{
    public class LRUCache
    {
        private static int capacity=3;
        private static readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();
        private static Dictionary<string, LinkedListNode<CacheObject>> cacheDict = new Dictionary<string, LinkedListNode<CacheObject>>(3);
        private static LinkedList<CacheObject> LRUList = new LinkedList<CacheObject>();

        public static byte[] GetFromCache(string key)
        {
            LinkedListNode<CacheObject> node;
            CacheLock.EnterWriteLock();
            if (cacheDict.TryGetValue(key, out node))
            {
                byte[] value = node.Value.value;
                LRUList.Remove(node);
                LRUList.AddLast(node);
                CacheLock.ExitWriteLock();
                return value;
            }
            CacheLock.ExitWriteLock();
            return default(byte[]);
        }

        public static void AddToCache(string key, byte[] value)
        {
            CacheLock.EnterWriteLock();
            if (cacheDict.TryGetValue(key, out var existingNode))
            {
                LRUList.Remove(existingNode);
            }
            else if (cacheDict.Count >= capacity)
            {
                RemoveFirst();
            }

            CacheObject cacheItem = new CacheObject(key, value);
            LinkedListNode<CacheObject> node = new LinkedListNode<CacheObject>(cacheItem);
            LRUList.AddLast(node);
            cacheDict[key] = node;
            CacheLock.ExitWriteLock();
        }

        private static void RemoveFirst()
        {

            LinkedListNode<CacheObject> node = LRUList.First;
            LRUList.RemoveFirst();

            cacheDict.Remove(node.Value.key);
            Console.WriteLine();
            Console.WriteLine($"Iz keša je izbačena slika: {node.Value.key}");
            Console.WriteLine();
        }
    }
}
