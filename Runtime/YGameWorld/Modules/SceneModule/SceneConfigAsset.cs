#region 
// **********************************************************************
// Create Time :		2022/11/17 13:48:24
// Description :
// **********************************************************************
#endregion


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeJing.YGameWorldFrame.RunTime
{

    [CreateAssetMenu(fileName = "SceneConfig", menuName = "YGameWorld/SceneConfig")]
    public class SceneConfigAsset : ScriptableObject
    {

        [SerializeField]
        public List<SceneNode> sceneNodes = new List<SceneNode>();
        [SerializeField]
        public Vector3 viewTransformPos;

    }

}

namespace HeJing.YGameWorldFrame.RunTime
{

    /// <summary>
    /// 场景节点 包含父与子
    /// </summary>
    [System.Serializable]
    public class SceneNode
    {
        [SerializeField]
        public Node m_node = new Node();

        [SerializeField]
        /// <summary>
        /// 记录坐标和大小
        /// </summary>
        public Rect pos;
        [SerializeField]
        /// <summary>
        /// 上一级， 父级
        /// </summary>
        public Node f_Node = new Node();

        [SerializeField]
        /// <summary>
        /// 子集
        /// </summary>
        public List<Node> c_Node = new List<Node>();


        /// <summary>
        /// 场景节点单个
        /// </summary>
        [System.Serializable]
        public class Node {
            public string m_Guid;
            /// <summary>
            /// 场景名称
            /// </summary>
            public string m_Name;
        }

    }
}