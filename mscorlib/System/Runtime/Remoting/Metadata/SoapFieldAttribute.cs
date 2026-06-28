using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SoapFieldAttribute : SoapAttribute
	{
		public int Order
		{
			get
			{
				return this._order;
			}
			set
			{
				this._order = value;
			}
		}

		public string XmlElementName
		{
			get
			{
				return this._elementName;
			}
			set
			{
				this._isElement = value != null;
				this._elementName = value;
			}
		}

		public bool IsInteropXmlElement()
		{
			return this._isElement;
		}

		internal override void SetReflectionObject(object reflectionObject)
		{
			FieldInfo fieldInfo = (FieldInfo)reflectionObject;
			if (this._elementName == null)
			{
				this._elementName = fieldInfo.Name;
			}
		}

		private int _order;

		private string _elementName;

		private bool _isElement;
	}
}
