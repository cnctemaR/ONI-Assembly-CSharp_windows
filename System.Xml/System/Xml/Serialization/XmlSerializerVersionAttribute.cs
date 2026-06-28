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
			this._type = type;
		}

		public string Namespace
		{
			get
			{
				return this._namespace;
			}
			set
			{
				this._namespace = value;
			}
		}

		public string ParentAssemblyId
		{
			get
			{
				return this._parentAssemblyId;
			}
			set
			{
				this._parentAssemblyId = value;
			}
		}

		public Type Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		public string Version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		private string _namespace;

		private string _parentAssemblyId;

		private Type _type;

		private string _version;
	}
}
