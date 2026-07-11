using System;

namespace System.ComponentModel.Design.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	[Obsolete("Use DesignerSerializerAttribute instead")]
	public sealed class RootDesignerSerializerAttribute : Attribute
	{
		public RootDesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName, bool reloadable)
		{
			this.serializer = serializerTypeName;
			this.baseserializer = baseSerializerTypeName;
			this.reload = reloadable;
		}

		public RootDesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType, bool reloadable)
			: this(serializerTypeName, baseSerializerType.AssemblyQualifiedName, reloadable)
		{
		}

		public RootDesignerSerializerAttribute(Type serializerType, Type baseSerializerType, bool reloadable)
			: this(serializerType.AssemblyQualifiedName, baseSerializerType.AssemblyQualifiedName, reloadable)
		{
		}

		public bool Reloadable
		{
			get
			{
				return this.reload;
			}
		}

		public string SerializerBaseTypeName
		{
			get
			{
				return this.baseserializer;
			}
		}

		public string SerializerTypeName
		{
			get
			{
				return this.serializer;
			}
		}

		public override object TypeId
		{
			get
			{
				return this.ToString() + this.baseserializer;
			}
		}

		private string serializer;

		private string baseserializer;

		private bool reload;
	}
}
