using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeJing.GameWrold.Module
{
    /// <summary>
    /// ab包节点  引用计数
    /// </summary>
    public class AssetBundleNode 
    {
        AssetBundle m_assetBundle;

        /// <summary>
        /// 依赖的资源
        /// </summary>
        public  List<AssetBundleNode> DependenciesAssets = new List<AssetBundleNode> ();

        /// <summary>
        /// 被依赖次数
        /// </summary>
        public int DependenciesCount { get => dependenciesCount; set => dependenciesCount = value; }
        int dependenciesCount = 0;

        /// <summary>
        /// 被引用次数
        /// </summary>
        public int RefCount { get => refCount; set => refCount = value; }
        int refCount = 0;

        /// <summary>
        /// 无引用次数 每次轮询时 被引用次数 和 被依赖次数 为 0 时 +1，不为零 是清零
        /// 内存不足时 优先清理 NotReferenceCount 最大的
        /// </summary>
        public int NotReferenceCount { get => notReferenceCount; set => notReferenceCount = value; }
        int notReferenceCount = 0;

        public AssetBundleNode(AssetBundle assetBundle )
        {
            
            this.m_assetBundle = assetBundle;
        }

        /// <summary>
        /// 卸载ab包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnloadAsync(bool unloadAllLoadedObjects = false)
        {
#if UNITY_2021_1_OR_NEWER
            m_assetBundle.UnloadAsync(unloadAllLoadedObjects);
#else
            m_assetBundle.Unload(unloadAllLoadedObjects);
#endif
        }

        /// <summary>
        /// 获得资源包
        /// </summary>
        /// <returns></returns>
        public AssetBundle GetAssetBundle() { 
            return m_assetBundle;
        }

    }
}
