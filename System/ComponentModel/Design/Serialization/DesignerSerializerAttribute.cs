using System;

namespace System.ComponentModel.Design.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class DesignerSerializerAttribute : Attribute
	{
		public DesignerSerializerAttribute(Type serializerType, Type baseSerializerType)
		{
			this.serializerTypeName = serializerType.AssemblyQualifiedName;
			this.serializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
		}

		public DesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType)
		{
			this.serializerTypeName = serializerTypeName;
			this.serializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
		}

		public DesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName)
		{
			this.serializerTypeName = serializerTypeName;
			this.serializerBaseTypeName = baseSerializerTypeName;
		}

		public string SerializerTypeName
		{
			get
			{
				return this.serializerTypeName;
			}
		}

		public string SerializerBaseTypeName
		{
			get
			{
				return this.serializerBaseTypeName;
			}
		}

		public override object TypeId
		{
			get
			{
				if (this.typeId == null)
				{
					string text = this.serializerBaseTypeName;
					int num = text.IndexOf(',');
					if (num != -1)
					{
						text = text.Substring(0, num);
					}
					this.typeId = base.GetType().FullName + text;
				}
				return this.typeId;
			}
		}

		private string serializerTypeName;

		private string serializerBaseTypeName;

		private string typeId;
	}
}
