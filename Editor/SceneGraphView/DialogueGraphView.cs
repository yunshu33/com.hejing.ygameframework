
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Edge = UnityEditor.Experimental.GraphView.Edge;

using SceneNode = HeJing.YGameWorldFrame.RunTime.SceneNode;
using HeJing.YGameWorldFrame.RunTime;
namespace HeJing.YGameWorldFrame.Editor
{

    public class DialogueGraphView : GraphView
    {
        bool isChange = false;

        private Rect DefaultRect = new Rect(x: 100, y: 200, width: 100, height: 150);

        public SceneConfigAsset sceneConfigAsset;

        private SerializedObject serializedObject;



        private List<string> SceneList = new();

        public bool IsChange
        {
            get => isChange;
            set
            {

                isChange = value;
            }
        }


        /// <summary>
        /// 删除选中 节点
        /// </summary>
        /// <returns></returns>
        public override EventPropagation DeleteSelection()
        {
            foreach (var item in selection)
            {
                var node = item as ViewSceneNode;

                SceneList.Add(node?.node.m_node.m_Name);
            }
            return base.DeleteSelection();
        }

        // 在构造函数里，对GraphView进行一些初始的设置
        public DialogueGraphView()
        {
            ///读取 数据文件
            sceneConfigAsset = Selection.activeObject as SceneConfigAsset;
            viewTransform.position = sceneConfigAsset.viewTransformPos;
            SceneList = GetSceneList();
            Debug.Log(SceneList.Count);


            ///功能配置
            {
                // 允许对Graph进行Zoom in/out
                SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

                // 允许拖拽Content
                this.AddManipulator(new ContentDragger());

                // 允许Selection里的内容
                this.AddManipulator(new SelectionDragger());

                // GraphView允许进行框选
                this.AddManipulator(new RectangleSelector());
            }



            ///初始化已有节点
            InitNode();
            ///初始化连线
            InitEdge();


            ///视图变化时触发
            graphViewChanged += OnGraphViewChanged;

        }

        /// <summary>
        /// 初始化连线
        /// </summary>
        private void InitEdge()
        {
            var viewSceneNodes = new List<ViewSceneNode>();

            foreach (var item in nodes)
            {
                viewSceneNodes.Add(item as ViewSceneNode);
            }

            for (int i = 0; i < sceneConfigAsset.sceneNodes.Count; i++)
            {
                Port startPort = null;   

                if (viewSceneNodes[i].GUID != sceneConfigAsset.sceneNodes[i].m_node.m_Guid)
                {
                     startPort = viewSceneNodes.Where((n) =>
                    {
                        return n.node.f_Node.m_Guid == sceneConfigAsset.sceneNodes[i].m_node.m_Guid;
                    }).FirstOrDefault().outputContainer.Children().FirstOrDefault() as Port;
                }
                else
                {
                    startPort = viewSceneNodes[i].outputContainer.Children().FirstOrDefault() as Port;
                }
                if (startPort is null)
                {
                    break;
                }

                foreach (var children in sceneConfigAsset.sceneNodes[i].c_Node)
                {

                    var targetNode = viewSceneNodes.Where((n) =>
                    {
                        return n.GUID == children.m_Guid;
                    }).FirstOrDefault();


                    AddElement((targetNode.inputContainer.Children().FirstOrDefault() as Port)
                        .ConnectTo(startPort));
                }
            }


        }

        /// <summary>
        /// 初始化 已经存在的节点
        /// </summary>
        void InitNode()
        {
            foreach (var item in sceneConfigAsset.sceneNodes)
            {
                InitNode(item);
            }
        }

        /// <summary>
        /// 视图变化时触发
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            IsChange = true;
            return graphViewChange;
        }


        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="evt"></param>
        public override void HandleEvent(EventBase evt)
        {

            switch (evt.imguiEvent?.keyCode)
            {
                ///保存
                case KeyCode.S:
                    if (evt.imguiEvent.control && IsChange)
                    {
                        SaveAssets();
                        IsChange = false;
                    }
                    break;
                default:
                    break;
            }

            base.HandleEvent(evt);
        }

        // 为节点n创建input port或者output port
        // Direction: 是一个简单的枚举，分为Input和Output两种
        private Port GenPortForNode(Node n, Direction portDir, Port.Capacity capacity = Port.Capacity.Single)
        {
            // Orientation也是个简单的枚举，分为Horizontal和Vertical两种，port的数据类型是float
            return n.InstantiatePort(Orientation.Horizontal, portDir, capacity, typeof(float));
        }


        private void InitNode(SceneNode sceneNode)
        {
            if (SceneList.Count <= 0)
            {
                return;
            }

            IsChange = true;
            // 1. 创建Node
            ViewSceneNode viewSceneNode = new ViewSceneNode
            {
                GUID = sceneNode.m_node.m_Guid,
                Entry = false
            };

            viewSceneNode.SetPosition(sceneNode.pos);

            ///添加一个下拉选项框
            var popupField = new PopupField<string>(SceneList, sceneNode.m_node.m_Name);
            popupField.style.width = 100;

            //设置初始值 并在下拉列表中移除
            viewSceneNode.node.m_node.m_Name = sceneNode.m_node.m_Name;

            SceneList.Remove(sceneNode.m_node.m_Name);

            viewSceneNode.node.m_node.m_Guid = sceneNode.m_node.m_Guid;

            /// 值变化回调
            popupField.RegisterValueChangedCallback(
               (@event) =>
               {
                   IsChange = true;
                   viewSceneNode.node.m_node.m_Name = @event.newValue;
                   SceneList.Add(@event.previousValue);
                   SceneList.Remove(@event.newValue);
               });


            viewSceneNode.Q("title").Insert(0, popupField);
            // 2. 为其创建InputPort
            var iport = GenPortForNode(viewSceneNode, Direction.Input, Port.Capacity.Single);

            iport.portName = "input";
            viewSceneNode.inputContainer.Add(iport);


            var Output = GenPortForNode(viewSceneNode, Direction.Output, Port.Capacity.Multi);
            Output.portName = "Output";
            viewSceneNode.outputContainer.Add(Output);


            ///刷新
            viewSceneNode.RefreshExpandedState();
            viewSceneNode.RefreshPorts();

            AddElement(viewSceneNode);
        }


        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="rect"></param>
        public void AddNode(Rect rect)
        {

            if (SceneList.Count <= 0)
            {
                return;
            }
            IsChange = true;
            // 1. 创建Node
            ViewSceneNode node = new ViewSceneNode
            {
                GUID = Guid.NewGuid().ToString(),
                Entry = false
            };

            node.SetPosition(rect);


            ///添加一个下拉选项框
            var popupField = new PopupField<string>(SceneList, SceneList[0]);


            //设置初始值 并在下拉列表中移除
            node.node.m_node.m_Name = SceneList[0];

            SceneList.RemoveAt(0);

            /// 值变化回调
            popupField.RegisterValueChangedCallback(
               (@event) =>
                {
                    IsChange = true;
                    node.node.m_node.m_Name = @event.newValue;
                    SceneList.Add(@event.previousValue);
                    SceneList.Remove(@event.newValue);
                });


            node.Q("title").Insert(0, popupField);

            // 2. 为其创建InputPort
            var iport = GenPortForNode(node, Direction.Input, Port.Capacity.Single);
            iport.portName = "Input";
            node.inputContainer.Add(iport);

            var Output = GenPortForNode(node, Direction.Output, Port.Capacity.Multi);
            Output.portName = "Output";
            node.outputContainer.Add(Output);


            node.RefreshExpandedState();
            node.RefreshPorts();

            AddElement(node);
        }

        /// <summary>
        /// 获得所有场景名称
        /// </summary>
        /// <returns></returns>
        List<string> GetSceneList()
        {
            List<string> sceneList = new List<string>();

            foreach (var item in GetScenePathList())
            {
                string fileName = Path.GetFileNameWithoutExtension(item);
                sceneList.Add(fileName);
            }

            return sceneList;
        }


        List<string> GetScenePathList()
        {
            List<string> PathList = new List<string>();

            string[] resFiles = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" });

            foreach (var item in resFiles)
            {
                PathList.Add(AssetDatabase.GUIDToAssetPath(item));
            }

            return PathList;
        }

        /// <summary>
        /// 连线规则
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()!.Where(endPort =>
                                     endPort.direction != startPort.direction &&
                                     endPort.node != startPort.node &&
                                     endPort.portType == startPort.portType).ToList();


            //// 继承的GraphView里有个Property：ports, 代表graph里所有的port
            //    // 对每一个在graph里的port，进行判断，这里有两个规则：
            //    // 1. port不可以与自身相连
            //    // 2. 同一个节点的port之间不可以相连

            //// 在我理解，这个函数就是把所有除了startNode里的port都收集起来，放到了List里
            //// 所以这个函数能让StartNode的Output port与任何其他的Node的Input port相连（output port应该默认不能与output port相连吧）

        }

        /// <summary>
        /// 构建右击菜单 
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var graphMousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);
            evt.menu.AppendAction("创建节点",
                (a) =>
                {
                    AddNode(new Rect(graphMousePosition, new Vector2(100, 150)));
                });

            base.BuildContextualMenu(evt);
        }

        /// <summary>
        /// 保存资源
        /// </summary>
        private void SaveAssets()
        {
            EditorUtility.SetDirty(sceneConfigAsset);

            ///保存 当前查看的 中心坐标
            sceneConfigAsset.viewTransformPos = this.viewTransform.position;


            sceneConfigAsset.sceneNodes.Clear();

            foreach (var item in nodes)
            {
                var sceneNode = new SceneNode();


                ///获得所有子节点sceneName
                var cs = item.outputContainer.contentContainer.Children();

                var childers = new List<SceneNode.Node>();
                foreach (var s in cs)
                {
                    var ports = s as Port;
                    foreach (var connection in ports.connections)
                    {
                        var n = connection.input.node as ViewSceneNode;

                        childers.Add(new SceneNode.Node()
                        {
                            m_Guid = n.GUID,
                            m_Name = n.node.m_node.m_Name
                        });

                    }
                }
                /// 获得父节点sceneName
                var fnode = item.inputContainer.contentContainer.Children();

                SceneNode.Node f_Node = null;

                foreach (var f in fnode)
                {
                    var ports = f as Port;
                    foreach (var connection in ports.connections)
                    {
                        var n = connection.output.node as ViewSceneNode;

                        f_Node = new SceneNode.Node()
                        {
                            m_Name = n.node.m_node.m_Name,
                            m_Guid = n.GUID,
                        };
                    }
                }

                var node = item as ViewSceneNode;

                sceneNode.pos = node.GetPosition();
                sceneNode.m_node.m_Name = node.node.m_node.m_Name;
                sceneNode.m_node.m_Guid = node.GUID;
                sceneNode.c_Node = childers;

                if (f_Node is not null)
                {
                    sceneNode.f_Node = f_Node;
                }
                sceneConfigAsset.sceneNodes.Add(sceneNode);
            }

            AssetDatabase.SaveAssets();

            EditorUtility.ClearDirty(sceneConfigAsset);

            Debug.Log("保存");

        }


    }
}