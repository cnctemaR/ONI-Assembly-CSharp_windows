using System;

namespace System.ComponentModel.Design.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class DesignerSerializerAttribute : Attribute
	{
		public DesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName)
		{
			this.serializerTypeName = serializerTypeName;
			this.baseSerializerTypeName = baseSerializerTypeName;
		}

		public DesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType)
			: this(serializerTypeName, baseSerializerType.AssemblyQualifiedName)
		{
		}

		public DesignerSerializerAttribute(Type serializerType, Type baseSerializerType)
			: this(serializerType.AssemblyQualifiedName, baseSerializerType.AssemblyQualifiedName)
		{
		}

		public string SerializerBaseTypeName
		{
			get
			{
				return this.baseSerializerTypeName;
			}
		}

		public string SerializerTypeName
		{
			get
			{
				return this.serializerTypeName;
			}
		}

		public override object TypeId
		{
			get
			{
				return this.ToString() + this.baseSerializerTypeName;
			}
		}

		private string serializerTypeName;

		private string baseSerializerTypeName;
	}
}
