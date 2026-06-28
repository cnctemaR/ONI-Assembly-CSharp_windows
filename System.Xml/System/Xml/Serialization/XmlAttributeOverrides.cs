using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace System.Xml.Serialization
{
	public class XmlAttributeOverrides
	{
		public XmlAttributeOverrides()
		{
			this.overrides = new Hashtable();
		}

		public XmlAttributes this[Type type]
		{
			get
			{
				return this[type, string.Empty];
			}
		}

		public XmlAttributes this[Type type, string member]
		{
			get
			{
				return (XmlAttributes)this.overrides[this.GetKey(type, member)];
			}
		}

		public void Add(Type type, XmlAttributes attributes)
		{
			this.Add(type, string.Empty, attributes);
		}

		public void Add(Type type, string member, XmlAttributes attributes)
		{
			if (this.overrides[this.GetKey(type, member)] != null)
			{
				throw new Exception("The attributes for the given type and Member already exist in the collection");
			}
			this.overrides.Add(this.GetKey(type, member), attributes);
		}

		private TypeMember GetKey(Type type, string member)
		{
			return new TypeMember(type, member);
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XAO ");
			foreach (object obj in this.overrides)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				XmlAttributes xmlAttributes = (XmlAttributes)dictionaryEntry.Value;
				IFormattable formattable = dictionaryEntry.Key as IFormattable;
				sb.Append((formattable == null) ? dictionaryEntry.Key.ToString() : formattable.ToString(null, CultureInfo.InvariantCulture)).Append(' ');
				xmlAttributes.AddKeyHash(sb);
			}
			sb.Append("|");
		}

		private Hashtable overrides;
	}
}
