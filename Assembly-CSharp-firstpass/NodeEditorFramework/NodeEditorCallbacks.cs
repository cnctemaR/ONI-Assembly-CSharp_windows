using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorCallbacks
	{
		public static void SetupReceivers()
		{
			NodeEditorCallbacks.callbackReceiver = new List<NodeEditorCallbackReceiver>(global::UnityEngine.Object.FindObjectsByType<NodeEditorCallbackReceiver>(FindObjectsSortMode.InstanceID));
			NodeEditorCallbacks.receiverCount = NodeEditorCallbacks.callbackReceiver.Count;
		}

		public static void IssueOnEditorStartUp()
		{
			if (NodeEditorCallbacks.OnEditorStartUp != null)
			{
				NodeEditorCallbacks.OnEditorStartUp();
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnEditorStartUp();
				}
			}
		}

		public static void IssueOnLoadCanvas(NodeCanvas canvas)
		{
			if (NodeEditorCallbacks.OnLoadCanvas != null)
			{
				NodeEditorCallbacks.OnLoadCanvas(canvas);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnLoadCanvas(canvas);
				}
			}
		}

		public static void IssueOnLoadEditorState(NodeEditorState editorState)
		{
			if (NodeEditorCallbacks.OnLoadEditorState != null)
			{
				NodeEditorCallbacks.OnLoadEditorState(editorState);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnLoadEditorState(editorState);
				}
			}
		}

		public static void IssueOnSaveCanvas(NodeCanvas canvas)
		{
			if (NodeEditorCallbacks.OnSaveCanvas != null)
			{
				NodeEditorCallbacks.OnSaveCanvas(canvas);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnSaveCanvas(canvas);
				}
			}
		}

		public static void IssueOnSaveEditorState(NodeEditorState editorState)
		{
			if (NodeEditorCallbacks.OnSaveEditorState != null)
			{
				NodeEditorCallbacks.OnSaveEditorState(editorState);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnSaveEditorState(editorState);
				}
			}
		}

		public static void IssueOnAddNode(Node node)
		{
			if (NodeEditorCallbacks.OnAddNode != null)
			{
				NodeEditorCallbacks.OnAddNode(node);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnAddNode(node);
				}
			}
		}

		public static void IssueOnDeleteNode(Node node)
		{
			if (NodeEditorCallbacks.OnDeleteNode != null)
			{
				NodeEditorCallbacks.OnDeleteNode(node);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnDeleteNode(node);
					node.OnDelete();
				}
			}
		}

		public static void IssueOnMoveNode(Node node)
		{
			if (NodeEditorCallbacks.OnMoveNode != null)
			{
				NodeEditorCallbacks.OnMoveNode(node);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnMoveNode(node);
				}
			}
		}

		public static void IssueOnAddNodeKnob(NodeKnob nodeKnob)
		{
			if (NodeEditorCallbacks.OnAddNodeKnob != null)
			{
				NodeEditorCallbacks.OnAddNodeKnob(nodeKnob);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnAddNodeKnob(nodeKnob);
				}
			}
		}

		public static void IssueOnAddConnection(NodeInput input)
		{
			if (NodeEditorCallbacks.OnAddConnection != null)
			{
				NodeEditorCallbacks.OnAddConnection(input);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnAddConnection(input);
				}
			}
		}

		public static void IssueOnRemoveConnection(NodeInput input)
		{
			if (NodeEditorCallbacks.OnRemoveConnection != null)
			{
				NodeEditorCallbacks.OnRemoveConnection(input);
			}
			for (int i = 0; i < NodeEditorCallbacks.receiverCount; i++)
			{
				if (NodeEditorCallbacks.callbackReceiver[i] == null)
				{
					NodeEditorCallbacks.callbackReceiver.RemoveAt(i--);
				}
				else
				{
					NodeEditorCallbacks.callbackReceiver[i].OnRemoveConnection(input);
				}
			}
		}

		private static int receiverCount;

		private static List<NodeEditorCallbackReceiver> callbackReceiver;

		public static global::System.Action OnEditorStartUp;

		public static Action<NodeCanvas> OnLoadCanvas;

		public static Action<NodeEditorState> OnLoadEditorState;

		public static Action<NodeCanvas> OnSaveCanvas;

		public static Action<NodeEditorState> OnSaveEditorState;

		public static Action<Node> OnAddNode;

		public static Action<Node> OnDeleteNode;

		public static Action<Node> OnMoveNode;

		public static Action<NodeKnob> OnAddNodeKnob;

		public static Action<NodeInput> OnAddConnection;

		public static Action<NodeInput> OnRemoveConnection;
	}
}
