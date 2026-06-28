using System;
using UnityEngine;

namespace NodeEditorFramework
{
	public class NodeEditorInputInfo
	{
		public NodeEditorInputInfo(NodeEditorState EditorState)
		{
			this.message = null;
			this.editorState = EditorState;
			this.inputEvent = Event.current;
			this.inputPos = this.inputEvent.mousePosition;
		}

		public NodeEditorInputInfo(string Message, NodeEditorState EditorState)
		{
			this.message = Message;
			this.editorState = EditorState;
			this.inputEvent = Event.current;
			this.inputPos = this.inputEvent.mousePosition;
		}

		public void SetAsCurrentEnvironment()
		{
			NodeEditor.curEditorState = this.editorState;
			NodeEditor.curNodeCanvas = this.editorState.canvas;
		}

		public string message;

		public NodeEditorState editorState;

		public Event inputEvent;

		public Vector2 inputPos;
	}
}
