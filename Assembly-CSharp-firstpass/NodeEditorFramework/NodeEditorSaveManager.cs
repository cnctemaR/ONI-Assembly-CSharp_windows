using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorSaveManager
	{
		private static void FetchSceneSaveHolder()
		{
			if (NodeEditorSaveManager.sceneSaveHolder == null)
			{
				NodeEditorSaveManager.sceneSaveHolder = GameObject.Find("NodeEditor_SceneSaveHolder");
				if (NodeEditorSaveManager.sceneSaveHolder == null)
				{
					NodeEditorSaveManager.sceneSaveHolder = new GameObject("NodeEditor_SceneSaveHolder");
				}
				NodeEditorSaveManager.sceneSaveHolder.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
			}
		}

		public static string[] GetSceneSaves()
		{
			NodeEditorSaveManager.FetchSceneSaveHolder();
			return (from save in NodeEditorSaveManager.sceneSaveHolder.GetComponents<NodeCanvasSceneSave>()
				select save.savedNodeCanvas.name).ToArray<string>();
		}

		private static NodeCanvasSceneSave FindSceneSave(string saveName)
		{
			NodeEditorSaveManager.FetchSceneSaveHolder();
			return NodeEditorSaveManager.sceneSaveHolder.GetComponents<NodeCanvasSceneSave>().ToList<NodeCanvasSceneSave>().Find((NodeCanvasSceneSave save) => save.savedNodeCanvas.name == saveName);
		}

		public static void SaveSceneNodeCanvas(string saveName, ref NodeCanvas nodeCanvas, bool createWorkingCopy)
		{
			if (string.IsNullOrEmpty(saveName))
			{
				Debug.LogError("Cannot save Canvas to scene: No save name specified!");
				return;
			}
			nodeCanvas.livesInScene = true;
			nodeCanvas.name = saveName;
			NodeCanvasSceneSave nodeCanvasSceneSave = NodeEditorSaveManager.FindSceneSave(saveName);
			if (nodeCanvasSceneSave == null)
			{
				nodeCanvasSceneSave = NodeEditorSaveManager.sceneSaveHolder.AddComponent<NodeCanvasSceneSave>();
			}
			nodeCanvasSceneSave.savedNodeCanvas = nodeCanvas;
			if (createWorkingCopy)
			{
				nodeCanvasSceneSave.savedNodeCanvas = NodeEditorSaveManager.CreateWorkingCopy(nodeCanvasSceneSave.savedNodeCanvas, true);
				NodeEditorSaveManager.Compress(ref nodeCanvasSceneSave.savedNodeCanvas);
			}
		}

		public static NodeCanvas LoadSceneNodeCanvas(string saveName, bool createWorkingCopy)
		{
			if (string.IsNullOrEmpty(saveName))
			{
				Debug.LogError("Cannot load Canvas from scene: No save name specified!");
				return null;
			}
			NodeCanvasSceneSave nodeCanvasSceneSave = NodeEditorSaveManager.FindSceneSave(saveName);
			if (nodeCanvasSceneSave == null)
			{
				return null;
			}
			NodeCanvas nodeCanvas = nodeCanvasSceneSave.savedNodeCanvas;
			nodeCanvas.livesInScene = true;
			if (createWorkingCopy)
			{
				nodeCanvas = NodeEditorSaveManager.CreateWorkingCopy(nodeCanvas, true);
			}
			NodeEditorSaveManager.Uncompress(ref nodeCanvas);
			return nodeCanvas;
		}

		public static void SaveNodeCanvas(string path, NodeCanvas nodeCanvas, bool createWorkingCopy)
		{
			throw new NotImplementedException();
		}

		public static NodeCanvas LoadNodeCanvas(string path, bool createWorkingCopy)
		{
			if (!File.Exists(path))
			{
				throw new UnityException("Cannot Load NodeCanvas: File '" + path + "' deos not exist!");
			}
			NodeCanvas nodeCanvas = ResourceManager.LoadResource<NodeCanvas>(path);
			if (nodeCanvas == null)
			{
				throw new UnityException("Cannot Load NodeCanvas: The file at the specified path '" + path + "' is no valid save file as it does not contain a NodeCanvas!");
			}
			if (createWorkingCopy)
			{
				nodeCanvas = NodeEditorSaveManager.CreateWorkingCopy(nodeCanvas, true);
			}
			else
			{
				nodeCanvas.Validate();
			}
			NodeEditorSaveManager.Uncompress(ref nodeCanvas);
			NodeEditorCallbacks.IssueOnLoadCanvas(nodeCanvas);
			return nodeCanvas;
		}

		public static void Compress(ref NodeCanvas nodeCanvas)
		{
		}

		public static void Uncompress(ref NodeCanvas nodeCanvas)
		{
			for (int i = 0; i < nodeCanvas.nodes.Count; i++)
			{
				Node node = nodeCanvas.nodes[i];
				if (node.Inputs == null || node.Inputs.Count == 0 || node.Outputs == null || node.Outputs.Count == 0)
				{
					node.Inputs = new List<NodeInput>();
					node.Outputs = new List<NodeOutput>();
					for (int j = 0; j < node.nodeKnobs.Count; j++)
					{
						NodeKnob nodeKnob = node.nodeKnobs[j];
						if (nodeKnob is NodeInput)
						{
							node.Inputs.Add(nodeKnob as NodeInput);
						}
						else if (nodeKnob is NodeOutput)
						{
							node.Outputs.Add(nodeKnob as NodeOutput);
						}
					}
				}
			}
		}

		public static NodeCanvas CreateWorkingCopy(NodeCanvas nodeCanvas, bool editorStates)
		{
			nodeCanvas.Validate();
			nodeCanvas = NodeEditorSaveManager.Clone<NodeCanvas>(nodeCanvas);
			List<ScriptableObject> allSOs = new List<ScriptableObject>();
			List<ScriptableObject> clonedSOs = new List<ScriptableObject>();
			for (int i = 0; i < nodeCanvas.nodes.Count; i++)
			{
				Node node = nodeCanvas.nodes[i];
				node.CheckNodeKnobMigration();
				Node node2 = NodeEditorSaveManager.AddClonedSO<Node>(allSOs, clonedSOs, node);
				NodeEditorSaveManager.AddClonedSOs(allSOs, clonedSOs, node2.GetScriptableObjects());
				foreach (NodeKnob nodeKnob in node2.nodeKnobs)
				{
					NodeEditorSaveManager.AddClonedSO<NodeKnob>(allSOs, clonedSOs, nodeKnob);
					NodeEditorSaveManager.AddClonedSOs(allSOs, clonedSOs, nodeKnob.GetScriptableObjects());
				}
			}
			for (int j = 0; j < nodeCanvas.nodes.Count; j++)
			{
				Node node3 = nodeCanvas.nodes[j];
				Node node4 = NodeEditorSaveManager.ReplaceSO<Node>(allSOs, clonedSOs, node3);
				nodeCanvas.nodes[j] = node4;
				Node node5 = node4;
				node5.CopyScriptableObjects((ScriptableObject so) => NodeEditorSaveManager.ReplaceSO<ScriptableObject>(allSOs, clonedSOs, so));
				for (int k = 0; k < node5.nodeKnobs.Count; k++)
				{
					NodeKnob nodeKnob2 = NodeEditorSaveManager.ReplaceSO<NodeKnob>(allSOs, clonedSOs, node5.nodeKnobs[k]);
					node5.nodeKnobs[k] = nodeKnob2;
					NodeKnob nodeKnob3 = nodeKnob2;
					nodeKnob3.body = node5;
					nodeKnob3.CopyScriptableObjects((ScriptableObject so) => NodeEditorSaveManager.ReplaceSO<ScriptableObject>(allSOs, clonedSOs, so));
				}
				for (int l = 0; l < node5.Inputs.Count; l++)
				{
					NodeInput nodeInput = NodeEditorSaveManager.ReplaceSO<NodeInput>(allSOs, clonedSOs, node5.Inputs[l]);
					node5.Inputs[l] = nodeInput;
					NodeInput nodeInput2 = nodeInput;
					nodeInput2.body = node5;
				}
				for (int m = 0; m < node5.Outputs.Count; m++)
				{
					NodeOutput nodeOutput = NodeEditorSaveManager.ReplaceSO<NodeOutput>(allSOs, clonedSOs, node5.Outputs[m]);
					node5.Outputs[m] = nodeOutput;
					NodeOutput nodeOutput2 = nodeOutput;
					nodeOutput2.body = node5;
				}
			}
			if (editorStates)
			{
				nodeCanvas.editorStates = NodeEditorSaveManager.CreateWorkingCopy(nodeCanvas.editorStates, nodeCanvas);
				foreach (NodeEditorState nodeEditorState in nodeCanvas.editorStates)
				{
					nodeEditorState.selectedNode = NodeEditorSaveManager.ReplaceSO<Node>(allSOs, clonedSOs, nodeEditorState.selectedNode);
				}
			}
			else
			{
				foreach (NodeEditorState nodeEditorState2 in nodeCanvas.editorStates)
				{
					nodeEditorState2.selectedNode = null;
				}
			}
			return nodeCanvas;
		}

		private static NodeEditorState[] CreateWorkingCopy(NodeEditorState[] editorStates, NodeCanvas associatedNodeCanvas)
		{
			if (editorStates == null)
			{
				return new NodeEditorState[0];
			}
			editorStates = (NodeEditorState[])editorStates.Clone();
			for (int i = 0; i < editorStates.Length; i++)
			{
				if (!(editorStates[i] == null))
				{
					NodeEditorState nodeEditorState = (editorStates[i] = NodeEditorSaveManager.Clone<NodeEditorState>(editorStates[i]));
					if (nodeEditorState == null)
					{
						Debug.LogError("Failed to create a working copy for an NodeEditorState during the loading process of " + associatedNodeCanvas.name + "!");
					}
					else
					{
						nodeEditorState.canvas = associatedNodeCanvas;
					}
				}
			}
			associatedNodeCanvas.editorStates = editorStates;
			return editorStates;
		}

		private static T Clone<T>(T SO) where T : ScriptableObject
		{
			string name = SO.name;
			SO = global::UnityEngine.Object.Instantiate<T>(SO);
			SO.name = name;
			return SO;
		}

		private static void AddClonedSOs(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, ScriptableObject[] initialSOs)
		{
			scriptableObjects.AddRange(initialSOs);
			clonedScriptableObjects.AddRange(initialSOs.Select<ScriptableObject, ScriptableObject>((ScriptableObject so) => NodeEditorSaveManager.Clone<ScriptableObject>(so)));
		}

		private static T AddClonedSO<T>(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, T initialSO) where T : ScriptableObject
		{
			if (initialSO == null)
			{
				return (T)((object)null);
			}
			scriptableObjects.Add(initialSO);
			T t = NodeEditorSaveManager.Clone<T>(initialSO);
			clonedScriptableObjects.Add(t);
			return t;
		}

		private static T ReplaceSO<T>(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, T initialSO) where T : ScriptableObject
		{
			if (initialSO == null)
			{
				return (T)((object)null);
			}
			int num = scriptableObjects.IndexOf(initialSO);
			if (num == -1)
			{
				Debug.LogError("GetWorkingCopy: ScriptableObject " + initialSO.name + " was not copied before! It will be null!");
			}
			return (num != -1) ? ((T)((object)clonedScriptableObjects[num])) : ((T)((object)null));
		}

		public static NodeEditorState ExtractEditorState(NodeCanvas canvas, string stateName)
		{
			NodeEditorState nodeEditorState = null;
			if (canvas.editorStates.Length > 0)
			{
				nodeEditorState = canvas.editorStates.First<NodeEditorState>((NodeEditorState s) => s.name == stateName);
				if (nodeEditorState == null)
				{
					nodeEditorState = canvas.editorStates[0];
				}
			}
			if (nodeEditorState == null)
			{
				nodeEditorState = ScriptableObject.CreateInstance<NodeEditorState>();
				nodeEditorState.canvas = canvas;
				canvas.editorStates = new NodeEditorState[] { nodeEditorState };
			}
			nodeEditorState.name = stateName;
			return nodeEditorState;
		}

		private static GameObject sceneSaveHolder;
	}
}
