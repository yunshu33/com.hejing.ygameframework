using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeJing.GameWrold.Module
{
    /// <summary>
    /// 单例
    /// </summary>
    public class Singleton<T> where T : class
    {
        private static T instance;

        /// <summary>
        ///多线程双重锁
        /// </summary>
        private static readonly object lockRoot = new();
        public static T Instance
        {
            get
            {
                lock (lockRoot)
                {
                    if (instance is null)
                    {
                        instance = (T)Activator.CreateInstance(typeof(T), true);
                    }
                }
                return instance;
            }
        }

    }
}
