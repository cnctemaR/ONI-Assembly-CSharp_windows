using System;
using System.Collections.Generic;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditor
	{
		public static void Update()
		{
			if (NodeEditor.NEUpdate != null)
			{
				NodeEditor.NEUpdate();
			}
		}

		public static void RepaintClients()
		{
			if (NodeEditor.ClientRepaints != null)
			{
				NodeEditor.ClientRepaints();
			}
		}

		public static void checkInit(bool GUIFunction)
		{
			if (!NodeEditor.initiated && !NodeEditor.InitiationError)
			{
				NodeEditor.ReInit(GUIFunction);
			}
		}

		public static void ReInit(bool GUIFunction)
		{
			NodeEditor.CheckEditorPath();
			ResourceManager.SetDefaultResourcePath(NodeEditor.editorPath + "Resources/");
			if (!NodeEditorGUI.Init(GUIFunction))
			{
				NodeEditor.InitiationError = true;
				return;
			}
			ConnectionTypes.FetchTypes();
			NodeTypes.FetchNodes();
			NodeCanvasManager.GetAllCanvasTypes();
			NodeEditorCallbacks.SetupReceivers();
			NodeEditorCallbacks.IssueOnEditorStartUp();
			GUIScaleUtility.CheckInit();
			NodeEditorInputSystem.SetupInput();
			NodeEditor.initiated = GUIFunction;
		}

		public static void CheckEditorPath()
		{
		}

		public static void DrawCanvas(NodeCanvas nodeCanvas, NodeEditorState editorState)
		{
			if (!editorState.drawing)
			{
				return;
			}
			NodeEditor.checkInit(true);
			NodeEditor.DrawSubCanvas(nodeCanvas, editorState);
		}

		private static void DrawSubCanvas(NodeCanvas nodeCanvas, NodeEditorState editorState)
		{
			if (!editorState.drawing)
			{
				return;
			}
			NodeCanvas nodeCanvas2 = NodeEditor.curNodeCanvas;
			NodeEditorState nodeEditorState = NodeEditor.curEditorState;
			NodeEditor.curNodeCanvas = nodeCanvas;
			NodeEditor.curEditorState = editorState;
			if (Event.current.type == EventType.Repaint)
			{
				float num = NodeEditor.curEditorState.zoom / (float)NodeEditorGUI.Background.width;
				float num2 = NodeEditor.curEditorState.zoom / (float)NodeEditorGUI.Background.height;
				Vector2 vector = NodeEditor.curEditorState.zoomPos + NodeEditor.curEditorState.panOffset / NodeEditor.curEditorState.zoom;
				Rect rect = new Rect(-vector.x * num, (vector.y - NodeEditor.curEditorState.canvasRect.height) * num2, NodeEditor.curEditorState.canvasRect.width * num, NodeEditor.curEditorState.canvasRect.height * num2);
				GUI.DrawTextureWithTexCoords(NodeEditor.curEditorState.canvasRect, NodeEditorGUI.Background, rect);
			}
			NodeEditorInputSystem.HandleInputEvents(NodeEditor.curEditorState);
			if (Event.current.type != EventType.Layout)
			{
				NodeEditor.curEditorState.ignoreInput = new List<Rect>();
			}
			Rect canvasRect = NodeEditor.curEditorState.canvasRect;
			NodeEditor.curEditorState.zoomPanAdjust = GUIScaleUtility.BeginScale(ref canvasRect, NodeEditor.curEditorState.zoomPos, NodeEditor.curEditorState.zoom, false);
			if (NodeEditor.curEditorState.navigate)
			{
				Vector2 vector2 = ((!(NodeEditor.curEditorState.selectedNode != null)) ? NodeEditor.curEditorState.panOffset : NodeEditor.curEditorState.selectedNode.rect.center) + NodeEditor.curEditorState.zoomPanAdjust;
				Vector2 mousePosition = Event.current.mousePosition;
				RTEditorGUI.DrawLine(vector2, mousePosition, Color.green, null, 3f);
				NodeEditor.RepaintClients();
			}
			if (NodeEditor.curEditorState.connectOutput != null)
			{
				NodeOutput connectOutput = NodeEditor.curEditorState.connectOutput;
				Vector2 center = connectOutput.GetGUIKnob().center;
				Vector2 direction = connectOutput.GetDirection();
				Vector2 mousePosition2 = Event.current.mousePosition;
				Vector2 secondConnectionVector = NodeEditorGUI.GetSecondConnectionVector(center, mousePosition2, direction);
				NodeEditorGUI.DrawConnection(center, direction, mousePosition2, secondConnectionVector, connectOutput.typeData.Color);
				NodeEditor.RepaintClients();
			}
			if (Event.current.type == EventType.Layout && NodeEditor.curEditorState.selectedNode != null)
			{
				NodeEditor.curNodeCanvas.nodes.Remove(NodeEditor.curEditorState.selectedNode);
				NodeEditor.curNodeCanvas.nodes.Add(NodeEditor.curEditorState.selectedNode);
			}
			for (int i = 0; i < NodeEditor.curNodeCanvas.nodes.Count; i++)
			{
				NodeEditor.curNodeCanvas.nodes[i].DrawConnections();
			}
			for (int j = 0; j < NodeEditor.curNodeCanvas.nodes.Count; j++)
			{
				Node node = NodeEditor.curNodeCanvas.nodes[j];
				node.DrawNode();
				if (Event.current.type == EventType.Repaint)
				{
					node.DrawKnobs();
				}
			}
			GUIScaleUtility.EndScale();
			NodeEditorInputSystem.HandleLateInputEvents(NodeEditor.curEditorState);
			NodeEditor.curNodeCanvas = nodeCanvas2;
			NodeEditor.curEditorState = nodeEditorState;
		}

		public static Node NodeAtPosition(Vector2 canvasPos)
		{
			NodeKnob nodeKnob;
			return NodeEditor.NodeAtPosition(NodeEditor.curEditorState, canvasPos, out nodeKnob);
		}

		public static Node NodeAtPosition(Vector2 canvasPos, out NodeKnob focusedKnob)
		{
			return NodeEditor.NodeAtPosition(NodeEditor.curEditorState, canvasPos, out focusedKnob);
		}

		public static Node NodeAtPosition(NodeEditorState editorState, Vector2 canvasPos, out NodeKnob focusedKnob)
		{
			focusedKnob = null;
			if (NodeEditorInputSystem.shouldIgnoreInput(editorState))
			{
				return null;
			}
			NodeCanvas canvas = editorState.canvas;
			for (int i = canvas.nodes.Count - 1; i >= 0; i--)
			{
				Node node = canvas.nodes[i];
				if (node.rect.Contains(canvasPos))
				{
					return node;
				}
				for (int j = 0; j < node.nodeKnobs.Count; j++)
				{
					if (node.nodeKnobs[j].GetCanvasSpaceKnob().Contains(canvasPos))
					{
						focusedKnob = node.nodeKnobs[j];
						return node;
					}
				}
			}
			return null;
		}

		public static Vector2 ScreenToCanvasSpace(Vector2 screenPos)
		{
			return NodeEditor.ScreenToCanvasSpace(NodeEditor.curEditorState, screenPos);
		}

		public static Vector2 ScreenToCanvasSpace(NodeEditorState editorState, Vector2 screenPos)
		{
			return (screenPos - editorState.canvasRect.position - editorState.zoomPos) * editorState.zoom - editorState.panOffset;
		}

		public static void RecalculateAll(NodeCanvas nodeCanvas)
		{
			NodeEditor.workList = new List<Node>();
			foreach (Node node in nodeCanvas.nodes)
			{
				if (node.isInput())
				{
					node.ClearCalculation();
					NodeEditor.workList.Add(node);
				}
			}
			NodeEditor.StartCalculation();
		}

		public static void RecalculateFrom(Node node)
		{
			node.ClearCalculation();
			NodeEditor.workList = new List<Node> { node };
			NodeEditor.StartCalculation();
		}

		public static void StartCalculation()
		{
			NodeEditor.checkInit(false);
			if (NodeEditor.InitiationError)
			{
				return;
			}
			if (NodeEditor.workList == null || NodeEditor.workList.Count == 0)
			{
				return;
			}
			NodeEditor.calculationCount = 0;
			bool flag = false;
			int num = 0;
			while (!flag)
			{
				flag = true;
				for (int i = 0; i < NodeEditor.workList.Count; i++)
				{
					if (NodeEditor.ContinueCalculation(NodeEditor.workList[i]))
					{
						flag = false;
					}
				}
				num++;
			}
		}

		private static bool ContinueCalculation(Node node)
		{
			if (node.calculated)
			{
				return false;
			}
			if ((node.descendantsCalculated() || node.isInLoop()) && node.Calculate())
			{
				node.calculated = true;
				NodeEditor.calculationCount++;
				NodeEditor.workList.Remove(node);
				if (node.ContinueCalculation && NodeEditor.calculationCount < 1000)
				{
					for (int i = 0; i < node.Outputs.Count; i++)
					{
						NodeOutput nodeOutput = node.Outputs[i];
						if (!nodeOutput.calculationBlockade)
						{
							for (int j = 0; j < nodeOutput.connections.Count; j++)
							{
								NodeEditor.ContinueCalculation(nodeOutput.connections[j].body);
							}
						}
					}
				}
				else if (NodeEditor.calculationCount >= 1000)
				{
					Debug.LogError("Stopped calculation because of suspected Recursion. Maximum calculation iteration is currently at 1000!");
				}
				return true;
			}
			if (!NodeEditor.workList.Contains(node))
			{
				NodeEditor.workList.Add(node);
			}
			return false;
		}

		public static string editorPath = "Assets/Plugins/Node_Editor/";

		public static NodeCanvas curNodeCanvas;

		public static NodeEditorState curEditorState;

		internal static global::System.Action NEUpdate;

		public static global::System.Action ClientRepaints;

		public static bool initiated;

		public static bool InitiationError;

		public static List<Node> workList;

		private static int calculationCount;
	}
}
