using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeEditorFramework
{
	public class NodeOutput : NodeKnob
	{
		protected override NodeSide defaultSide
		{
			get
			{
				return NodeSide.Right;
			}
		}

		protected override GUIStyle defaultLabelStyle
		{
			get
			{
				if (NodeOutput._defaultStyle == null)
				{
					NodeOutput._defaultStyle = new GUIStyle(GUI.skin.label);
					NodeOutput._defaultStyle.alignment = TextAnchor.MiddleRight;
				}
				return NodeOutput._defaultStyle;
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

		public static NodeOutput Create(Node nodeBody, string outputName, string outputType)
		{
			return NodeOutput.Create(nodeBody, outputName, outputType, NodeSide.Right, 20f);
		}

		public static NodeOutput Create(Node nodeBody, string outputName, string outputType, NodeSide nodeSide)
		{
			return NodeOutput.Create(nodeBody, outputName, outputType, nodeSide, 20f);
		}

		public static NodeOutput Create(Node nodeBody, string outputName, string outputType, NodeSide nodeSide, float sidePosition)
		{
			NodeOutput nodeOutput = ScriptableObject.CreateInstance<NodeOutput>();
			nodeOutput.typeID = outputType;
			nodeOutput.InitBase(nodeBody, nodeSide, sidePosition, outputName);
			nodeBody.Outputs.Add(nodeOutput);
			return nodeOutput;
		}

		public override void Delete()
		{
			while (this.connections.Count > 0)
			{
				this.connections[0].RemoveConnection();
			}
			this.body.Outputs.Remove(this);
			base.Delete();
		}

		protected internal override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
			for (int i = 0; i < this.connections.Count; i++)
			{
				this.connections[i] = replaceSerializableObject(this.connections[i]) as NodeInput;
			}
		}

		protected override void ReloadTexture()
		{
			this.CheckType();
			this.knobTexture = this.typeData.OutKnobTex;
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
				return this.value == null;
			}
		}

		public object GetValue()
		{
			return this.value;
		}

		public object GetValue(Type type)
		{
			if (type == null)
			{
				throw new UnityException("Trying to get value of " + base.name + " with null type!");
			}
			this.CheckType();
			if (type.IsAssignableFrom(this.typeData.Type))
			{
				return this.value;
			}
			global::Debug.LogError("Trying to GetValue<" + type.FullName + "> for Output Type: " + this.typeData.Type.FullName);
			return null;
		}

		public void SetValue(object Value)
		{
			this.CheckType();
			if (Value == null || this.typeData.Type.IsAssignableFrom(Value.GetType()))
			{
				this.value = Value;
			}
			else
			{
				global::Debug.LogError("Trying to SetValue of type " + Value.GetType().FullName + " for Output Type: " + this.typeData.Type.FullName);
			}
		}

		public T GetValue<T>()
		{
			this.CheckType();
			if (typeof(T).IsAssignableFrom(this.typeData.Type))
			{
				object obj;
				if ((obj = this.value) == null)
				{
					obj = (this.value = NodeOutput.GetDefault<T>());
				}
				return (T)((object)obj);
			}
			global::Debug.LogError("Trying to GetValue<" + typeof(T).FullName + "> for Output Type: " + this.typeData.Type.FullName);
			return NodeOutput.GetDefault<T>();
		}

		public void SetValue<T>(T Value)
		{
			this.CheckType();
			if (this.typeData.Type.IsAssignableFrom(typeof(T)))
			{
				this.value = Value;
			}
			else
			{
				global::Debug.LogError("Trying to SetValue<" + typeof(T).FullName + "> for Output Type: " + this.typeData.Type.FullName);
			}
		}

		public void ResetValue()
		{
			this.value = null;
		}

		public static T GetDefault<T>()
		{
			if (typeof(T).GetConstructor(Type.EmptyTypes) != null)
			{
				return Activator.CreateInstance<T>();
			}
			return default(T);
		}

		public static object GetDefault(Type type)
		{
			if (type.GetConstructor(Type.EmptyTypes) != null)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		public override Node GetNodeAcrossConnection()
		{
			return (this.connections.Count <= 0) ? null : this.connections[0].body;
		}

		private static GUIStyle _defaultStyle;

		public List<NodeInput> connections = new List<NodeInput>();

		[FormerlySerializedAs("type")]
		public string typeID;

		private TypeData _typeData;

		[NonSerialized]
		private object value;

		public bool calculationBlockade;
	}
}
