using System;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class XmlSerializerVersionAttribute : Attribute
	{
		public XmlSerializerVersionAttribute()
		{
		}

		public XmlSerializerVersionAttribute(Type type)
		{
			this.type = type;
		}

		public string ParentAssemblyId
		{
			get
			{
				return this.mvid;
			}
			set
			{
				this.mvid = value;
			}
		}

		public string Version
		{
			get
			{
				return this.serializerVersion;
			}
			set
			{
				this.serializerVersion = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		private string mvid;

		private string serializerVersion;

		private string ns;

		private Type type;
	}
}
