using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeEditorFramework
{
	public class NodeInput : NodeKnob
	{
		protected override NodeSide defaultSide
		{
			get
			{
				return NodeSide.Left;
			}
		}

		internal TypeData typeData
		{
			get
			{
				this.CheckType();
				return this._typeData;
			}
		}

		public static NodeInput Create(Node nodeBody, string inputName, string inputType)
		{
			return NodeInput.Create(nodeBody, inputName, inputType, NodeSide.Left, 20f);
		}

		public static NodeInput Create(Node nodeBody, string inputName, string inputType, NodeSide nodeSide)
		{
			return NodeInput.Create(nodeBody, inputName, inputType, nodeSide, 20f);
		}

		public static NodeInput Create(Node nodeBody, string inputName, string inputType, NodeSide nodeSide, float sidePosition)
		{
			NodeInput nodeInput = ScriptableObject.CreateInstance<NodeInput>();
			nodeInput.typeID = inputType;
			nodeInput.InitBase(nodeBody, nodeSide, sidePosition, inputName);
			nodeBody.Inputs.Add(nodeInput);
			return nodeInput;
		}

		public override void Delete()
		{
			this.RemoveConnection();
			this.body.Inputs.Remove(this);
			base.Delete();
		}

		protected internal override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
			this.connection = replaceSerializableObject(this.connection) as NodeOutput;
		}

		protected override void ReloadTexture()
		{
			this.CheckType();
			this.knobTexture = this.typeData.InKnobTex;
		}

		private void CheckType()
		{
			if (this._typeData == null || !this._typeData.isValid())
			{
				this._typeData = ConnectionTypes.GetTypeData(this.typeID);
			}
			if (this._typeData == null || !this._typeData.isValid())
			{
				ConnectionTypes.FetchTypes();
				this._typeData = ConnectionTypes.GetTypeData(this.typeID);
				if (this._typeData == null || !this._typeData.isValid())
				{
					throw new UnityException("Could not find type " + this.typeID + "!");
				}
			}
		}

		public bool IsValueNull
		{
			get
			{
				return !(this.connection != null) || this.connection.IsValueNull;
			}
		}

		public object GetValue()
		{
			return (!(this.connection != null)) ? null : this.connection.GetValue();
		}

		public object GetValue(Type type)
		{
			return (!(this.connection != null)) ? null : this.connection.GetValue(type);
		}

		public void SetValue(object value)
		{
			if (this.connection != null)
			{
				this.connection.SetValue(value);
			}
		}

		public T GetValue<T>()
		{
			return (!(this.connection != null)) ? NodeOutput.GetDefault<T>() : this.connection.GetValue<T>();
		}

		public void SetValue<T>(T value)
		{
			if (this.connection != null)
			{
				this.connection.SetValue<T>(value);
			}
		}

		public bool TryApplyConnection(NodeOutput output)
		{
			if (this.CanApplyConnection(output))
			{
				this.ApplyConnection(output);
				return true;
			}
			return false;
		}

		public bool CanApplyConnection(NodeOutput output)
		{
			if (output == null || this.body == output.body || this.connection == output || !this.typeData.Type.IsAssignableFrom(output.typeData.Type))
			{
				return false;
			}
			if (output.body.isChildOf(this.body) && !output.body.allowsLoopRecursion(this.body))
			{
				global::Debug.LogWarning("Cannot apply connection: Recursion detected!");
				return false;
			}
			return true;
		}

		public void ApplyConnection(NodeOutput output)
		{
			if (output == null)
			{
				return;
			}
			if (this.connection != null)
			{
				NodeEditorCallbacks.IssueOnRemoveConnection(this);
				this.connection.connections.Remove(this);
			}
			this.connection = output;
			output.connections.Add(this);
			if (!output.body.calculated)
			{
				NodeEditor.RecalculateFrom(output.body);
			}
			else
			{
				NodeEditor.RecalculateFrom(this.body);
			}
			output.body.OnAddOutputConnection(output);
			this.body.OnAddInputConnection(this);
			NodeEditorCallbacks.IssueOnAddConnection(this);
		}

		public void RemoveConnection()
		{
			if (this.connection == null)
			{
				return;
			}
			NodeEditorCallbacks.IssueOnRemoveConnection(this);
			this.connection.connections.Remove(this);
			this.connection = null;
			NodeEditor.RecalculateFrom(this.body);
		}

		public override Node GetNodeAcrossConnection()
		{
			return (!(this.connection != null)) ? null : this.connection.body;
		}

		public NodeOutput connection;

		[FormerlySerializedAs("type")]
		public string typeID;

		private TypeData _typeData;
	}
}
