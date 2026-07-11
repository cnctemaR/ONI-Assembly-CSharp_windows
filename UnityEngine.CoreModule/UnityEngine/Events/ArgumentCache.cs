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

		private void TidyAssemblyTypeName()
		{
			bool flag = string.IsNullOrEmpty(this.m_ObjectArgumentAssemblyTypeName);
			if (!flag)
			{
				int num = int.MaxValue;
				int num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", Version=");
				bool flag2 = num2 != -1;
				if (flag2)
				{
					num = Math.Min(num2, num);
				}
				num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", Culture=");
				bool flag3 = num2 != -1;
				if (flag3)
				{
					num = Math.Min(num2, num);
				}
				num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", PublicKeyToken=");
				bool flag4 = num2 != -1;
				if (flag4)
				{
					num = Math.Min(num2, num);
				}
				bool flag5 = num != int.MaxValue;
				if (flag5)
				{
					this.m_ObjectArgumentAssemblyTypeName = this.m_ObjectArgumentAssemblyTypeName.Substring(0, num);
				}
				num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", UnityEngine.");
				bool flag6 = num2 != -1 && this.m_ObjectArgumentAssemblyTypeName.EndsWith("Module");
				if (flag6)
				{
					this.m_ObjectArgumentAssemblyTypeName = this.m_ObjectArgumentAssemblyTypeName.Substring(0, num2) + ", UnityEngine";
				}
			}
		}

		public void OnBeforeSerialize()
		{
			this.TidyAssemblyTypeName();
		}

		public void OnAfterDeserialize()
		{
			this.TidyAssemblyTypeName();
		}

		[SerializeField]
		[FormerlySerializedAs("objectArgument")]
		private Object m_ObjectArgument;

		[SerializeField]
		[FormerlySerializedAs("objectArgumentAssemblyTypeName")]
		private string m_ObjectArgumentAssemblyTypeName;

		[FormerlySerializedAs("intArgument")]
		[SerializeField]
		private int m_IntArgument;

		[FormerlySerializedAs("floatArgument")]
		[SerializeField]
		private float m_FloatArgument;

		[FormerlySerializedAs("stringArgument")]
		[SerializeField]
		private string m_StringArgument;

		[SerializeField]
		private bool m_BoolArgument;
	}
}
