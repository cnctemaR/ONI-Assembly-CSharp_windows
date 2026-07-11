using System;
using System.Collections.Generic;

namespace System.Xml.Schema
{
	internal abstract class SchemaDeclBase
	{
		protected SchemaDeclBase(XmlQualifiedName name, string prefix)
		{
			this.name = name;
			this.prefix = prefix;
			this.maxLength = -1L;
			this.minLength = -1L;
		}

		protected SchemaDeclBase()
		{
		}

		internal XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal string Prefix
		{
			get
			{
				if (this.prefix != null)
				{
					return this.prefix;
				}
				return string.Empty;
			}
			set
			{
				this.prefix = value;
			}
		}

		internal bool IsDeclaredInExternal
		{
			get
			{
				return this.isDeclaredInExternal;
			}
			set
			{
				this.isDeclaredInExternal = value;
			}
		}

		internal SchemaDeclBase.Use Presence
		{
			get
			{
				return this.presence;
			}
			set
			{
				this.presence = value;
			}
		}

		internal long MaxLength
		{
			get
			{
				return this.maxLength;
			}
			set
			{
				this.maxLength = value;
			}
		}

		internal long MinLength
		{
			get
			{
				return this.minLength;
			}
			set
			{
				this.minLength = value;
			}
		}

		internal XmlSchemaType SchemaType
		{
			get
			{
				return this.schemaType;
			}
			set
			{
				this.schemaType = value;
			}
		}

		internal XmlSchemaDatatype Datatype
		{
			get
			{
				return this.datatype;
			}
			set
			{
				this.datatype = value;
			}
		}

		internal void AddValue(string value)
		{
			if (this.values == null)
			{
				this.values = new List<string>();
			}
			this.values.Add(value);
		}

		internal List<string> Values
		{
			get
			{
				return this.values;
			}
			set
			{
				this.values = value;
			}
		}

		internal string DefaultValueRaw
		{
			get
			{
				if (this.defaultValueRaw == null)
				{
					return string.Empty;
				}
				return this.defaultValueRaw;
			}
			set
			{
				this.defaultValueRaw = value;
			}
		}

		internal object DefaultValueTyped
		{
			get
			{
				return this.defaultValueTyped;
			}
			set
			{
				this.defaultValueTyped = value;
			}
		}

		internal bool CheckEnumeration(object pVal)
		{
			return (this.datatype.TokenizedType != XmlTokenizedType.NOTATION && this.datatype.TokenizedType != XmlTokenizedType.ENUMERATION) || this.values.Contains(pVal.ToString());
		}

		internal bool CheckValue(object pVal)
		{
			return (this.presence != SchemaDeclBase.Use.Fixed && this.presence != SchemaDeclBase.Use.RequiredFixed) || (this.defaultValueTyped != null && this.datatype.IsEqual(pVal, this.defaultValueTyped));
		}

		protected XmlQualifiedName name = XmlQualifiedName.Empty;

		protected string prefix;

		protected bool isDeclaredInExternal;

		protected SchemaDeclBase.Use presence;

		protected XmlSchemaType schemaType;

		protected XmlSchemaDatatype datatype;

		protected string defaultValueRaw;

		protected object defaultValueTyped;

		protected long maxLength;

		protected long minLength;

		protected List<string> values;

		internal enum Use
		{
			Default,
			Required,
			Implied,
			Fixed,
			RequiredFixed
		}
	}
}
