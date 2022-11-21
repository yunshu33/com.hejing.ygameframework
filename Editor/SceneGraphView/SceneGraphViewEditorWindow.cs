#region 
// **********************************************************************
// Create Time :		2022/11/16 16:24:25
// Description :
// **********************************************************************
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace HeJing.YGameWorldFrame.Editor
{
    public class SceneGraphViewEditorWindow : EditorWindow
    {

        DialogueGraphView dialogueGraphView;


        static SceneGraphViewEditorWindow sceneGraphView;

        
        public static void Init()
        {
            sceneGraphView = GetWindow<SceneGraphViewEditorWindow>();

            sceneGraphView.Show();
        }
        private void Awake()
        {
        }


        private void OnEnable()
        {

            dialogueGraphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };

            // 让graphView铺满整个Editor窗口
            dialogueGraphView.StretchToParentSize();

            // 把它添加到EditorWindow的可视化Root元素下面
            rootVisualElement.Add(dialogueGraphView);


            //Toolbar toolbar = new Toolbar();
            ////创建lambda函数，代表点击按钮后发生的函数调用
            //Button btn = new Button(clickEvent: () => { dialogueGraphView.AddNode("Dialogue"); });
            //btn.text = "Add Dialogue Node";
            //toolbar.Add(btn);
            //rootVisualElement.Add(toolbar);

        }



        // 关闭窗口时销毁graphView
        private void OnDisable()
        {
            rootVisualElement.Remove(dialogueGraphView);

        }


    }
}


