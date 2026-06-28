using System;

namespace System.Xml.Schema
{
	[MonoTODO]
	public class XmlSchemaInfo : IXmlSchemaInfo
	{
		public XmlSchemaInfo()
		{
		}

		internal XmlSchemaInfo(IXmlSchemaInfo info)
		{
			this.isDefault = info.IsDefault;
			this.isNil = info.IsNil;
			this.memberType = info.MemberType;
			this.attr = info.SchemaAttribute;
			this.elem = info.SchemaElement;
			this.type = info.SchemaType;
			this.validity = info.Validity;
		}

		[MonoTODO]
		public XmlSchemaContentType ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		[MonoTODO]
		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
			set
			{
				this.isDefault = value;
			}
		}

		[MonoTODO]
		public bool IsNil
		{
			get
			{
				return this.isNil;
			}
			set
			{
				this.isNil = value;
			}
		}

		[MonoTODO]
		public XmlSchemaSimpleType MemberType
		{
			get
			{
				return this.memberType;
			}
			set
			{
				this.memberType = value;
			}
		}

		[MonoTODO]
		public XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				return this.attr;
			}
			set
			{
				this.attr = value;
			}
		}

		[MonoTODO]
		public XmlSchemaElement SchemaElement
		{
			get
			{
				return this.elem;
			}
			set
			{
				this.elem = value;
			}
		}

		[MonoTODO]
		public XmlSchemaType SchemaType
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

		[MonoTODO]
		public XmlSchemaValidity Validity
		{
			get
			{
				return this.validity;
			}
			set
			{
				this.validity = value;
			}
		}

		private bool isDefault;

		private bool isNil;

		private XmlSchemaSimpleType memberType;

		private XmlSchemaAttribute attr;

		private XmlSchemaElement elem;

		private XmlSchemaType type;

		private XmlSchemaValidity validity;

		private XmlSchemaContentType contentType;
	}
}
