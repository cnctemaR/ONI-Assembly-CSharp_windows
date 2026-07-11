using System;

namespace System.Xml.Schema
{
	public class XmlSchemaInfo : IXmlSchemaInfo
	{
		public XmlSchemaInfo()
		{
			this.Clear();
		}

		internal XmlSchemaInfo(XmlSchemaValidity validity)
			: this()
		{
			this.validity = validity;
		}

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

		public XmlSchemaType SchemaType
		{
			get
			{
				return this.schemaType;
			}
			set
			{
				this.schemaType = value;
				if (this.schemaType != null)
				{
					this.contentType = this.schemaType.SchemaContentType;
					return;
				}
				this.contentType = XmlSchemaContentType.Empty;
			}
		}

		public XmlSchemaElement SchemaElement
		{
			get
			{
				return this.schemaElement;
			}
			set
			{
				this.schemaElement = value;
				if (value != null)
				{
					this.schemaAttribute = null;
				}
			}
		}

		public XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				return this.schemaAttribute;
			}
			set
			{
				this.schemaAttribute = value;
				if (value != null)
				{
					this.schemaElement = null;
				}
			}
		}

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

		internal XmlSchemaType XmlType
		{
			get
			{
				if (this.memberType != null)
				{
					return this.memberType;
				}
				return this.schemaType;
			}
		}

		internal bool HasDefaultValue
		{
			get
			{
				return this.schemaElement != null && this.schemaElement.ElementDecl.DefaultValueTyped != null;
			}
		}

		internal bool IsUnionType
		{
			get
			{
				return this.schemaType != null && this.schemaType.Datatype != null && this.schemaType.Datatype.Variety == XmlSchemaDatatypeVariety.Union;
			}
		}

		internal void Clear()
		{
			this.isNil = false;
			this.isDefault = false;
			this.schemaType = null;
			this.schemaElement = null;
			this.schemaAttribute = null;
			this.memberType = null;
			this.validity = XmlSchemaValidity.NotKnown;
			this.contentType = XmlSchemaContentType.Empty;
		}

		private bool isDefault;

		private bool isNil;

		private XmlSchemaElement schemaElement;

		private XmlSchemaAttribute schemaAttribute;

		private XmlSchemaType schemaType;

		private XmlSchemaSimpleType memberType;

		private XmlSchemaValidity validity;

		private XmlSchemaContentType contentType;
	}
}
