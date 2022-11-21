#region 
// **********************************************************************
// Create Time :		2022/11/15 15:20:50
// Description :
// **********************************************************************
#endregion


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace HeJing.GameWrold.Module
{
    /// <summary>
    /// ab包管理模块
    /// </summary>
    public class AssetBundleManagerModule : ModuleBase
    {
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, AssetBundleNode> bundles = new Dictionary<string, AssetBundleNode>();

        /// <summary>
        /// 根据包名加载ab 包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundleByName(string name)
        {
            AssetBundle bundle;

            AssetBundleNode node;

            bundles.TryGetValue(name, out node);

            if (node is null)
            {
                bundle = AssetBundle.LoadFromFileAsync($"{Application.streamingAssetsPath}/{name}").assetBundle;
                bundles.Add(name, new AssetBundleNode(bundle));
                return bundle;
            }
            else
            {
                return node.GetAssetBundle();
            }
           
        }




        public AssetBundleManagerModule()
        {
            /// 内存不足时回调
            Application.lowMemory += Application_lowMemory;
        }

        /// <summary>
        /// 内存不足时回调事件 卸载一些ab 包
        /// </summary>
        protected virtual void Application_lowMemory()
        {

        }



        ~AssetBundleManagerModule() {

            Application.lowMemory -= Application_lowMemory;
            bundles.Clear();

        } 
    }
}
