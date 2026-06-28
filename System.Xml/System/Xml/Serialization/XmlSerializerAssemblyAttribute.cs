using System;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public sealed class XmlSerializerAssemblyAttribute : Attribute
	{
		public XmlSerializerAssemblyAttribute()
		{
		}

		public XmlSerializerAssemblyAttribute(string assemblyName)
		{
			this._assemblyName = assemblyName;
		}

		public XmlSerializerAssemblyAttribute(string assemblyName, string codeBase)
			: this(assemblyName)
		{
			this._codeBase = codeBase;
		}

		public string AssemblyName
		{
			get
			{
				return this._assemblyName;
			}
			set
			{
				this._assemblyName = value;
			}
		}

		public string CodeBase
		{
			get
			{
				return this._codeBase;
			}
			set
			{
				this._codeBase = value;
			}
		}

		private string _assemblyName;

		private string _codeBase;
	}
}
