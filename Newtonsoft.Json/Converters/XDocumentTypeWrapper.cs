using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XDocumentTypeWrapper : XObjectWrapper, IXmlDocumentType, IXmlNode
	{
		public XDocumentTypeWrapper(XDocumentType documentType)
			: base(documentType)
		{
			this._documentType = documentType;
		}

		public string Name
		{
			get
			{
				return this._documentType.Name;
			}
		}

		public string System
		{
			get
			{
				return this._documentType.SystemId;
			}
		}

		public string Public
		{
			get
			{
				return this._documentType.PublicId;
			}
		}

		public string InternalSubset
		{
			get
			{
				return this._documentType.InternalSubset;
			}
		}

		public override string LocalName
		{
			get
			{
				return "DOCTYPE";
			}
		}

		private readonly XDocumentType _documentType;
	}
}
