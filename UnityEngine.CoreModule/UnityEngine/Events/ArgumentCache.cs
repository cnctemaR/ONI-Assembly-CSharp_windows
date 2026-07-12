using System;
using UnityEngine.Serialization;

namespace UnityEngine.Events
{
	[Serializable]
	internal class ArgumentCache : ISerializationCallbackReceiver
	{
		public Object unityObjectArgument
		{
			get
			{
				return this.m_ObjectArgument;
			}
			set
			{
				this.m_ObjectArgument = value;
				this.m_ObjectArgumentAssemblyTypeName = ((value != null) ? value.GetType().AssemblyQualifiedName : string.Empty);
			}
		}

		public string unityObjectArgumentAssemblyTypeName
		{
			get
			{
				return this.m_ObjectArgumentAssemblyTypeName;
			}
		}

		public int intArgument
		{
			get
			{
				return this.m_IntArgument;
			}
			set
			{
				this.m_IntArgument = value;
			}
		}

		public float floatArgument
		{
			get
			{
				return this.m_FloatArgument;
			}
			set
			{
				this.m_FloatArgument = value;
			}
		}

		public string stringArgument
		{
			get
			{
				return this.m_StringArgument;
			}
			set
			{
				this.m_StringArgument = value;
			}
		}

		public bool boolArgument
		{
			get
			{
				return this.m_BoolArgument;
			}
			set
			{
				this.m_BoolArgument = value;
			}
		}

		public void OnBeforeSerialize()
		{
			this.m_ObjectArgumentAssemblyTypeName = UnityEventTools.TidyAssemblyTypeName(this.m_ObjectArgumentAssemblyTypeName);
		}

		public void OnAfterDeserialize()
		{
			this.m_ObjectArgumentAssemblyTypeName = UnityEventTools.TidyAssemblyTypeName(this.m_ObjectArgumentAssemblyTypeName);
		}

		[SerializeField]
		[FormerlySerializedAs("objectArgument")]
		private Object m_ObjectArgument;

		[SerializeField]
		[FormerlySerializedAs("objectArgumentAssemblyTypeName")]
		private string m_ObjectArgumentAssemblyTypeName;

		[SerializeField]
		[FormerlySerializedAs("intArgument")]
		private int m_IntArgument;

		[FormerlySerializedAs("floatArgument")]
		[SerializeField]
		private float m_FloatArgument;

		[SerializeField]
		[FormerlySerializedAs("stringArgument")]
		private string m_StringArgument;

		[SerializeField]
		private bool m_BoolArgument;
	}
}
