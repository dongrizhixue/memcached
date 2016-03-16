using Memcached.ClientLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace Common
{
    public class MemcachedHelper
    {
        private static MemcachedClient mc;
        static MemcachedHelper()
        {
            //分布Memcachedf服务IP 端口
            string[] servers = { ConfigurationManager.AppSettings["MemcachedServers"] };
            //初始化池
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(servers);
            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 5;
            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;
            pool.MaintenanceSleep = 30;
            pool.Failover = true;
            pool.Nagle = false;
            pool.Initialize();
            mc = new MemcachedClient();
            mc.EnableCompression = false;
        }
        public static bool Set(string key, object value)
        {
            return mc.Set("$" + key, value);
        }
        public static bool IsExists(string key)
        {
            return mc.KeyExists("$" + key);
        }
        public static T Get<T>(string key)
        {
            return (T)mc.Get("$" + key);
        }

        public static void Replace(string key, object value)
        {
            mc.Replace("$" + key, value);
        }
        public static void Replace(string key, object value, DateTime expiry)
        {
            mc.Replace("$" + key, value, expiry);
        }

        public static bool Delete(string key)
        {
            return mc.Delete("$" + key);
        }
        public static string[] Allkeys()
        {
            Hashtable ht = mc.Stats();
            int i = 0;
            string[] allkey = new string[] { };
            foreach (DictionaryEntry de in ht)
            {
                Hashtable info = (Hashtable)de.Value;
                foreach (DictionaryEntry de2 in info)
                {
                    allkey[i] = de2.Key.ToString();
                }
                i++;
            }
            return allkey;
        }
        public static List<T> GetMultipleList<T>(string[] keys)
        {

            object[] obj = mc.GetMultipleArray(keys);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
    }
}
