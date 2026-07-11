using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public class XmlTypeAttribute : Attribute
	{
		public XmlTypeAttribute()
		{
		}

		public XmlTypeAttribute(string typeName)
		{
			this.typeName = typeName;
		}

		public bool AnonymousType
		{
			get
			{
				return this.anonymousType;
			}
			set
			{
				this.anonymousType = value;
			}
		}

		public bool IncludeInSchema
		{
			get
			{
				return this.includeInSchema;
			}
			set
			{
				this.includeInSchema = value;
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

		public string TypeName
		{
			get
			{
				if (this.typeName == null)
				{
					return string.Empty;
				}
				return this.typeName;
			}
			set
			{
				this.typeName = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XTA ");
			KeyHelper.AddField(sb, 1, this.ns);
			KeyHelper.AddField(sb, 2, this.typeName);
			KeyHelper.AddField(sb, 4, this.includeInSchema);
			sb.Append('|');
		}

		private bool includeInSchema = true;

		private string ns;

		private string typeName;

		private bool anonymousType;
	}
}
