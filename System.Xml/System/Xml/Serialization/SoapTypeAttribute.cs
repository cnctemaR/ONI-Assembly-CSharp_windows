using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public class SoapTypeAttribute : Attribute
	{
		public SoapTypeAttribute()
		{
		}

		public SoapTypeAttribute(string typeName)
		{
			this.typeName = typeName;
		}

		public SoapTypeAttribute(string typeName, string ns)
		{
			this.typeName = typeName;
			this.ns = ns;
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
			sb.Append("STA ");
			KeyHelper.AddField(sb, 1, this.ns);
			KeyHelper.AddField(sb, 2, this.typeName);
			KeyHelper.AddField(sb, 3, this.includeInSchema);
			sb.Append('|');
		}

		private string ns;

		private string typeName;

		private bool includeInSchema = true;
	}
}
