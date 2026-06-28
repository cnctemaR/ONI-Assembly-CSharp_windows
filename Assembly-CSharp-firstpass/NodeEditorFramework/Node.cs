using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework
{
	public abstract class Node : ScriptableObject
	{
		protected internal void InitBase()
		{
			NodeEditor.RecalculateFrom(this);
			if (NodeEditor.curNodeCanvas == null || NodeEditor.curNodeCanvas.nodes == null)
			{
				return;
			}
			if (!NodeEditor.curNodeCanvas.nodes.Contains(this))
			{
				NodeEditor.curNodeCanvas.nodes.Add(this);
			}
			NodeEditor.RepaintClients();
		}

		public void Delete()
		{
			if (!NodeEditor.curNodeCanvas.nodes.Contains(this))
			{
				throw new UnityException(string.Concat(new string[]
				{
					"The Node ",
					base.name,
					" does not exist on the Canvas ",
					NodeEditor.curNodeCanvas.name,
					"!"
				}));
			}
			NodeEditorCallbacks.IssueOnDeleteNode(this);
			NodeEditor.curNodeCanvas.nodes.Remove(this);
			for (int i = 0; i < this.Outputs.Count; i++)
			{
				NodeOutput nodeOutput = this.Outputs[i];
				while (nodeOutput.connections.Count != 0)
				{
					nodeOutput.connections[0].RemoveConnection();
				}
				global::UnityEngine.Object.DestroyImmediate(nodeOutput, true);
			}
			for (int j = 0; j < this.Inputs.Count; j++)
			{
				NodeInput nodeInput = this.Inputs[j];
				if (nodeInput.connection != null)
				{
					nodeInput.connection.connections.Remove(nodeInput);
				}
				global::UnityEngine.Object.DestroyImmediate(nodeInput, true);
			}
			for (int k = 0; k < this.nodeKnobs.Count; k++)
			{
				if (this.nodeKnobs[k] != null)
				{
					global::UnityEngine.Object.DestroyImmediate(this.nodeKnobs[k], true);
				}
			}
			global::UnityEngine.Object.DestroyImmediate(this, true);
		}

		public static Node Create(string nodeID, Vector2 position)
		{
			return Node.Create(nodeID, position, null);
		}

		public static Node Create(string nodeID, Vector2 position, NodeOutput connectingOutput)
		{
			Node node = NodeTypes.getDefaultNode(nodeID);
			if (node == null)
			{
				throw new UnityException("Cannot create Node with id " + nodeID + " as no such Node type is registered!");
			}
			node = node.Create(position);
			node.InitBase();
			if (connectingOutput != null)
			{
				foreach (NodeInput nodeInput in node.Inputs)
				{
					if (nodeInput.TryApplyConnection(connectingOutput))
					{
						break;
					}
				}
			}
			NodeEditorCallbacks.IssueOnAddNode(node);
			return node;
		}

		internal void CheckNodeKnobMigration()
		{
			if (this.nodeKnobs.Count == 0 && (this.Inputs.Count != 0 || this.Outputs.Count != 0))
			{
				this.nodeKnobs.AddRange(this.Inputs.Cast<NodeKnob>());
				this.nodeKnobs.AddRange(this.Outputs.Cast<NodeKnob>());
			}
		}

		public abstract string GetID { get; }

		public abstract Node Create(Vector2 pos);

		protected internal abstract void NodeGUI();

		public virtual void DrawNodePropertyEditor()
		{
		}

		public virtual bool Calculate()
		{
			return true;
		}

		public virtual bool AllowRecursion
		{
			get
			{
				return false;
			}
		}

		public virtual bool ContinueCalculation
		{
			get
			{
				return true;
			}
		}

		protected internal virtual void OnDelete()
		{
		}

		protected internal virtual void OnAddInputConnection(NodeInput input)
		{
		}

		protected internal virtual void OnAddOutputConnection(NodeOutput output)
		{
		}

		public virtual ScriptableObject[] GetScriptableObjects()
		{
			return new ScriptableObject[0];
		}

		protected internal virtual void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
		}

		public void SerializeInputsAndOutputs(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
		}

		protected internal virtual void DrawNode()
		{
			Rect rect = this.rect;
			rect.position += NodeEditor.curEditorState.zoomPanAdjust + NodeEditor.curEditorState.panOffset;
			this.contentOffset = new Vector2(0f, 20f);
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.contentOffset.y);
			GUI.Label(rect2, base.name, (!(NodeEditor.curEditorState.selectedNode == this)) ? NodeEditorGUI.nodeBox : NodeEditorGUI.nodeBoxBold);
			Rect rect3 = new Rect(rect.x, rect.y + this.contentOffset.y, rect.width, rect.height - this.contentOffset.y);
			GUI.BeginGroup(rect3, GUI.skin.box);
			rect3.position = Vector2.zero;
			GUILayout.BeginArea(rect3, GUI.skin.box);
			GUI.changed = false;
			this.NodeGUI();
			GUILayout.EndArea();
			GUI.EndGroup();
		}

		protected internal virtual void DrawKnobs()
		{
			this.CheckNodeKnobMigration();
			for (int i = 0; i < this.nodeKnobs.Count; i++)
			{
				this.nodeKnobs[i].DrawKnob();
			}
		}

		protected internal virtual void DrawConnections()
		{
			this.CheckNodeKnobMigration();
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			for (int i = 0; i < this.Outputs.Count; i++)
			{
				NodeOutput nodeOutput = this.Outputs[i];
				Vector2 center = nodeOutput.GetGUIKnob().center;
				Vector2 direction = nodeOutput.GetDirection();
				for (int j = 0; j < nodeOutput.connections.Count; j++)
				{
					NodeInput nodeInput = nodeOutput.connections[j];
					NodeEditorGUI.DrawConnection(center, direction, nodeInput.GetGUIKnob().center, nodeInput.GetDirection(), nodeOutput.typeData.Color);
				}
			}
		}

		protected internal bool allInputsReady()
		{
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				if (this.Inputs[i].connection == null || this.Inputs[i].connection.IsValueNull)
				{
					return false;
				}
			}
			return true;
		}

		protected internal bool hasUnassignedInputs()
		{
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				if (this.Inputs[i].connection == null)
				{
					return true;
				}
			}
			return false;
		}

		protected internal bool descendantsCalculated()
		{
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				if (this.Inputs[i].connection != null && !this.Inputs[i].connection.body.calculated)
				{
					return false;
				}
			}
			return true;
		}

		protected internal bool isInput()
		{
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				if (this.Inputs[i].connection != null)
				{
					return false;
				}
			}
			return true;
		}

		public NodeOutput CreateOutput(string outputName, string outputType)
		{
			return NodeOutput.Create(this, outputName, outputType);
		}

		public NodeOutput CreateOutput(string outputName, string outputType, NodeSide nodeSide)
		{
			return NodeOutput.Create(this, outputName, outputType, nodeSide);
		}

		public NodeOutput CreateOutput(string outputName, string outputType, NodeSide nodeSide, float sidePosition)
		{
			return NodeOutput.Create(this, outputName, outputType, nodeSide, sidePosition);
		}

		protected void OutputKnob(int outputIdx)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.Outputs[outputIdx].SetPosition();
			}
		}

		public NodeInput CreateInput(string inputName, string inputType)
		{
			return NodeInput.Create(this, inputName, inputType);
		}

		public NodeInput CreateInput(string inputName, string inputType, NodeSide nodeSide)
		{
			return NodeInput.Create(this, inputName, inputType, nodeSide);
		}

		public NodeInput CreateInput(string inputName, string inputType, NodeSide nodeSide, float sidePosition)
		{
			return NodeInput.Create(this, inputName, inputType, nodeSide, sidePosition);
		}

		protected void InputKnob(int inputIdx)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.Inputs[inputIdx].SetPosition();
			}
		}

		protected static void ReassignOutputType(ref NodeOutput output, Type newOutputType)
		{
			Node body = output.body;
			string name = output.name;
			IEnumerable<NodeInput> enumerable = output.connections.Where<NodeInput>((NodeInput connection) => connection.typeData.Type.IsAssignableFrom(newOutputType));
			output.Delete();
			NodeEditorCallbacks.IssueOnAddNodeKnob(NodeOutput.Create(body, name, newOutputType.AssemblyQualifiedName));
			output = body.Outputs[body.Outputs.Count - 1];
			foreach (NodeInput nodeInput in enumerable)
			{
				nodeInput.ApplyConnection(output);
			}
		}

		protected static void ReassignInputType(ref NodeInput input, Type newInputType)
		{
			Node body = input.body;
			string name = input.name;
			NodeOutput nodeOutput = null;
			if (input.connection != null && newInputType.IsAssignableFrom(input.connection.typeData.Type))
			{
				nodeOutput = input.connection;
			}
			input.Delete();
			NodeEditorCallbacks.IssueOnAddNodeKnob(NodeInput.Create(body, name, newInputType.AssemblyQualifiedName));
			input = body.Inputs[body.Inputs.Count - 1];
			if (nodeOutput != null)
			{
				input.ApplyConnection(nodeOutput);
			}
		}

		public bool isChildOf(Node otherNode)
		{
			if (otherNode == null || otherNode == this)
			{
				return false;
			}
			if (this.BeginRecursiveSearchLoop())
			{
				return false;
			}
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				NodeOutput connection = this.Inputs[i].connection;
				if (connection != null && connection.body != this.startRecursiveSearchNode && (connection.body == otherNode || connection.body.isChildOf(otherNode)))
				{
					this.StopRecursiveSearchLoop();
					return true;
				}
			}
			this.EndRecursiveSearchLoop();
			return false;
		}

		internal bool isInLoop()
		{
			if (this.BeginRecursiveSearchLoop())
			{
				return this == this.startRecursiveSearchNode;
			}
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				NodeOutput connection = this.Inputs[i].connection;
				if (connection != null && connection.body.isInLoop())
				{
					this.StopRecursiveSearchLoop();
					return true;
				}
			}
			this.EndRecursiveSearchLoop();
			return false;
		}

		internal bool allowsLoopRecursion(Node otherNode)
		{
			if (this.AllowRecursion)
			{
				return true;
			}
			if (otherNode == null)
			{
				return false;
			}
			if (this.BeginRecursiveSearchLoop())
			{
				return false;
			}
			for (int i = 0; i < this.Inputs.Count; i++)
			{
				NodeOutput connection = this.Inputs[i].connection;
				if (connection != null && connection.body.allowsLoopRecursion(otherNode))
				{
					this.StopRecursiveSearchLoop();
					return true;
				}
			}
			this.EndRecursiveSearchLoop();
			return false;
		}

		public void ClearCalculation()
		{
			if (this.BeginRecursiveSearchLoop())
			{
				return;
			}
			this.calculated = false;
			for (int i = 0; i < this.Outputs.Count; i++)
			{
				NodeOutput nodeOutput = this.Outputs[i];
				for (int j = 0; j < nodeOutput.connections.Count; j++)
				{
					nodeOutput.connections[j].body.ClearCalculation();
				}
			}
			this.EndRecursiveSearchLoop();
		}

		internal bool BeginRecursiveSearchLoop()
		{
			if (this.startRecursiveSearchNode == null || this.recursiveSearchSurpassed == null)
			{
				this.recursiveSearchSurpassed = new List<Node>();
				this.startRecursiveSearchNode = this;
			}
			if (this.recursiveSearchSurpassed.Contains(this))
			{
				return true;
			}
			this.recursiveSearchSurpassed.Add(this);
			return false;
		}

		internal void EndRecursiveSearchLoop()
		{
			if (this.startRecursiveSearchNode == this)
			{
				this.recursiveSearchSurpassed = null;
				this.startRecursiveSearchNode = null;
			}
		}

		internal void StopRecursiveSearchLoop()
		{
			this.recursiveSearchSurpassed = null;
			this.startRecursiveSearchNode = null;
		}

		public Rect rect = default(Rect);

		internal Vector2 contentOffset = Vector2.zero;

		[SerializeField]
		public List<NodeKnob> nodeKnobs = new List<NodeKnob>();

		[SerializeField]
		public List<NodeInput> Inputs = new List<NodeInput>();

		[SerializeField]
		public List<NodeOutput> Outputs = new List<NodeOutput>();

		[HideInInspector]
		[NonSerialized]
		internal bool calculated = true;

		[NonSerialized]
		private List<Node> recursiveSearchSurpassed;

		[NonSerialized]
		private Node startRecursiveSearchNode;
	}
}
