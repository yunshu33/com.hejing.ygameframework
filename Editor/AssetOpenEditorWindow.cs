
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEngine;
using HeJing.YGameWorldFrame.Editor;

namespace HeJing.YGameWorldFrame.Editor
{
    public class AssetOpenEditorWindow
    {
        [OnOpenAsset]
        public static bool step1(int instanceID, int line, int column)
        {
            // string name = EditorUtility.InstanceIDToObject(instanceID).GetType().ToString();

            // Debug.Log(AssetDatabase.GetAssetPath(instanceID));

            var obj = EditorUtility.InstanceIDToObject(instanceID);

            switch (obj.GetType().ToString())
            {
                case "HeJing.YGameWorldFrame.RunTime.SceneConfigAsset":
                    SceneGraphViewEditorWindow.Init();
                    return true;

                default:
                    break;
            }

            return false; // we did not handle the open
        }
    }
}
