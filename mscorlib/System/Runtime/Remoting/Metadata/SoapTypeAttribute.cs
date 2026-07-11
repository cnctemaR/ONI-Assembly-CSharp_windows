using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	[ComVisible(true)]
	public sealed class SoapTypeAttribute : SoapAttribute
	{
		public SoapOption SoapOptions
		{
			get
			{
				return this._soapOption;
			}
			set
			{
				this._soapOption = value;
			}
		}

		public override bool UseAttribute
		{
			get
			{
				return this._useAttribute;
			}
			set
			{
				this._useAttribute = value;
			}
		}

		public string XmlElementName
		{
			get
			{
				return this._xmlElementName;
			}
			set
			{
				this._isElement = value != null;
				this._xmlElementName = value;
			}
		}

		public XmlFieldOrderOption XmlFieldOrder
		{
			get
			{
				return this._xmlFieldOrder;
			}
			set
			{
				this._xmlFieldOrder = value;
			}
		}

		public override string XmlNamespace
		{
			get
			{
				return this._xmlNamespace;
			}
			set
			{
				this._isElement = value != null;
				this._xmlNamespace = value;
			}
		}

		public string XmlTypeName
		{
			get
			{
				return this._xmlTypeName;
			}
			set
			{
				this._isType = value != null;
				this._xmlTypeName = value;
			}
		}

		public string XmlTypeNamespace
		{
			get
			{
				return this._xmlTypeNamespace;
			}
			set
			{
				this._isType = value != null;
				this._xmlTypeNamespace = value;
			}
		}

		internal bool IsInteropXmlElement
		{
			get
			{
				return this._isElement;
			}
		}

		internal bool IsInteropXmlType
		{
			get
			{
				return this._isType;
			}
		}

		internal override void SetReflectionObject(object reflectionObject)
		{
			Type type = (Type)reflectionObject;
			if (this._xmlElementName == null)
			{
				this._xmlElementName = type.Name;
			}
			if (this._xmlTypeName == null)
			{
				this._xmlTypeName = type.Name;
			}
			if (this._xmlTypeNamespace == null)
			{
				string text;
				if (type.Assembly == typeof(object).Assembly)
				{
					text = string.Empty;
				}
				else
				{
					text = type.Assembly.GetName().Name;
				}
				this._xmlTypeNamespace = SoapServices.CodeXmlNamespaceForClrTypeNamespace(type.Namespace, text);
			}
			if (this._xmlNamespace == null)
			{
				this._xmlNamespace = this._xmlTypeNamespace;
			}
		}

		private SoapOption _soapOption;

		private bool _useAttribute;

		private string _xmlElementName;

		private XmlFieldOrderOption _xmlFieldOrder;

		private string _xmlNamespace;

		private string _xmlTypeName;

		private string _xmlTypeNamespace;

		private bool _isType;

		private bool _isElement;
	}
}
