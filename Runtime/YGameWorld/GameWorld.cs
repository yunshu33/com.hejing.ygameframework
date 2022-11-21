#region 
// **********************************************************************
// Create Time :		2022/11/15 12:16:18
// Description :
// **********************************************************************
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;
using HeJing.GameWrold.Module;

namespace HeJing.GameWrold
{
    public  class GameWorld : Singleton<GameWorld>
    {
        /// <summary>
        /// 模块字典
        /// </summary>
         Dictionary<Type, ModuleBase> Modules = new Dictionary<Type, ModuleBase>();

         GameObject yGameWorldFrame;

        private GameWorld()
        {

            yGameWorldFrame = new GameObject("YGameWorldFrame");

            UnityEngine.Object.DontDestroyOnLoad(yGameWorldFrame);
        }

        /// <summary>
        /// 获得模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public  T GetModule<T>() where T : ModuleBase, new()
        {
            foreach (var item in Modules)
            {
                if (item.Key == typeof(T))
                {
                    return (T)item.Value;
                }
            }
            return CreationModule<T>();
        }

        /// <summary>
        /// 创建模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T CreationModule<T>() where T : ModuleBase, new()
        {
            T module = null;

            module.m_gameObject = new GameObject(typeof(T).ToString());

            module = module.m_gameObject.AddComponent<T>();

            module.m_gameObject.transform.parent = yGameWorldFrame.transform;

            Modules.Add(typeof(T), module);

           // Modules = Modules.OrderBy(m => m.Value.Order).ToDictionary(k => k.Key, v => v.Value);

            return module;

        }

    }
}