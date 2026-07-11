using System;
using System.IO;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework.Standard
{
	public class RuntimeNodeEditor : MonoBehaviour
	{
		public void Start()
		{
			NodeEditor.checkInit(false);
			NodeEditor.initiated = false;
			this.LoadNodeCanvas(this.canvasPath);
			FPSCounter.Create();
		}

		public void Update()
		{
			NodeEditor.Update();
			FPSCounter.Update();
		}

		public void OnGUI()
		{
			if (this.canvas != null)
			{
				if (this.state == null)
				{
					this.NewEditorState();
				}
				NodeEditor.checkInit(true);
				if (NodeEditor.InitiationError)
				{
					GUILayout.Label("Initiation failed! Check console for more information!", Array.Empty<GUILayoutOption>());
					return;
				}
				try
				{
					if (!this.screenSize && this.specifiedRootRect.max != this.specifiedRootRect.min)
					{
						GUI.BeginGroup(this.specifiedRootRect, NodeEditorGUI.nodeSkin.box);
					}
					NodeEditorGUI.StartNodeGUI();
					this.canvasRect = ((!this.screenSize) ? this.specifiedCanvasRect : new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
					this.canvasRect.width = this.canvasRect.width - 200f;
					this.state.canvasRect = this.canvasRect;
					NodeEditor.DrawCanvas(this.canvas, this.state);
					GUILayout.BeginArea(new Rect(this.canvasRect.x + this.state.canvasRect.width, this.state.canvasRect.y, 200f, this.state.canvasRect.height), NodeEditorGUI.nodeSkin.box);
					this.SideGUI();
					GUILayout.EndArea();
					NodeEditorGUI.EndNodeGUI();
					if (!this.screenSize && this.specifiedRootRect.max != this.specifiedRootRect.min)
					{
						GUI.EndGroup();
					}
				}
				catch (UnityException ex)
				{
					this.NewNodeCanvas();
					NodeEditor.ReInit(true);
					global::Debug.LogError("Unloaded Canvas due to exception in Draw!", null);
					global::Debug.LogException(ex);
				}
			}
		}

		public void SideGUI()
		{
			GUILayout.Label(new GUIContent("Node Editor (" + this.canvas.name + ")", "The currently opened canvas in the Node Editor"), Array.Empty<GUILayoutOption>());
			this.screenSize = GUILayout.Toggle(this.screenSize, "Adapt to Screen", Array.Empty<GUILayoutOption>());
			GUILayout.Label("FPS: " + FPSCounter.currentFPS, Array.Empty<GUILayoutOption>());
			GUILayout.Label(new GUIContent("Node Editor (" + this.canvas.name + ")"), NodeEditorGUI.nodeLabelBold, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button(new GUIContent("New Canvas", "Loads an empty Canvas"), Array.Empty<GUILayoutOption>()))
			{
				this.NewNodeCanvas();
			}
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.sceneCanvasName = GUILayout.TextField(this.sceneCanvasName, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			if (GUILayout.Button(new GUIContent("Save to Scene", "Saves the Canvas to the Scene"), new GUILayoutOption[] { GUILayout.ExpandWidth(false) }))
			{
				this.SaveSceneNodeCanvas(this.sceneCanvasName);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button(new GUIContent("Load from Scene", "Loads the Canvas from the Scene"), Array.Empty<GUILayoutOption>()))
			{
				GenericMenu genericMenu = new GenericMenu();
				foreach (string text in NodeEditorSaveManager.GetSceneSaves())
				{
					genericMenu.AddItem(new GUIContent(text), false, new PopupMenu.MenuFunctionData(this.LoadSceneCanvasCallback), text);
				}
				genericMenu.Show(this.loadScenePos, 40f);
			}
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				this.loadScenePos = new Vector2(lastRect.x + 2f, lastRect.yMax + 2f);
			}
			GUILayout.Space(6f);
			if (GUILayout.Button(new GUIContent("Recalculate All", "Initiates complete recalculate. Usually does not need to be triggered manually."), Array.Empty<GUILayoutOption>()))
			{
				NodeEditor.RecalculateAll(this.canvas);
			}
			if (GUILayout.Button("Force Re-Init", Array.Empty<GUILayoutOption>()))
			{
				NodeEditor.ReInit(true);
			}
			NodeEditorGUI.knobSize = RTEditorGUI.IntSlider(new GUIContent("Handle Size", "The size of the Node Input/Output handles"), NodeEditorGUI.knobSize, 12, 20, Array.Empty<GUILayoutOption>());
			this.state.zoom = RTEditorGUI.Slider(new GUIContent("Zoom", "Use the Mousewheel. Seriously."), this.state.zoom, 0.6f, 2f, Array.Empty<GUILayoutOption>());
		}

		private void LoadSceneCanvasCallback(object save)
		{
			this.LoadSceneNodeCanvas((string)save);
		}

		public void SaveSceneNodeCanvas(string path)
		{
			this.canvas.editorStates = new NodeEditorState[] { this.state };
			NodeEditorSaveManager.SaveSceneNodeCanvas(path, ref this.canvas, true);
		}

		public void LoadSceneNodeCanvas(string path)
		{
			if ((this.canvas = NodeEditorSaveManager.LoadSceneNodeCanvas(path, true)) == null)
			{
				this.NewNodeCanvas();
				return;
			}
			this.state = NodeEditorSaveManager.ExtractEditorState(this.canvas, "MainEditorState");
			NodeEditor.RecalculateAll(this.canvas);
		}

		public void LoadNodeCanvas(string path)
		{
			if (!File.Exists(path) || (this.canvas = NodeEditorSaveManager.LoadNodeCanvas(path, true)) == null)
			{
				this.NewNodeCanvas();
				return;
			}
			this.state = NodeEditorSaveManager.ExtractEditorState(this.canvas, "MainEditorState");
			NodeEditor.RecalculateAll(this.canvas);
		}

		public void NewNodeCanvas()
		{
			this.canvas = ScriptableObject.CreateInstance<NodeCanvas>();
			this.canvas.name = "New Canvas";
			this.NewEditorState();
		}

		private void NewEditorState()
		{
			this.state = ScriptableObject.CreateInstance<NodeEditorState>();
			this.state.canvas = this.canvas;
			this.state.name = "MainEditorState";
			this.canvas.editorStates = new NodeEditorState[] { this.state };
		}

		public string canvasPath;

		public NodeCanvas canvas;

		private NodeEditorState state;

		public bool screenSize;

		private Rect canvasRect;

		public Rect specifiedRootRect;

		public Rect specifiedCanvasRect;

		private string sceneCanvasName = string.Empty;

		private Vector2 loadScenePos;
	}
}
