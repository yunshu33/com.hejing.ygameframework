
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using HeJing.YGameWorldFrame.RunTime;

namespace HeJing.YGameWorldFrame.Editor
{
    [System.Serializable]
    public class ViewSceneNode : Node
    {

        public string GUID;

        public string Text;

        [SerializeField]
        public SceneNode node = new SceneNode();

        public bool Entry = false;

    }
}