using System;
using System.Collections;
using System.Text;

namespace System.Xml.Serialization
{
	public class SoapAttributeOverrides
	{
		public SoapAttributeOverrides()
		{
			this.overrides = new Hashtable();
		}

		public SoapAttributes this[Type type]
		{
			get
			{
				return this[type, string.Empty];
			}
		}

		public SoapAttributes this[Type type, string member]
		{
			get
			{
				return (SoapAttributes)this.overrides[this.GetKey(type, member)];
			}
		}

		public void Add(Type type, SoapAttributes attributes)
		{
			this.Add(type, string.Empty, attributes);
		}

		public void Add(Type type, string member, SoapAttributes attributes)
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
			sb.Append("SAO ");
			foreach (object obj in this.overrides)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				SoapAttributes soapAttributes = (SoapAttributes)this.overrides[dictionaryEntry.Key];
				sb.Append(dictionaryEntry.Key.ToString()).Append(' ');
				soapAttributes.AddKeyHash(sb);
			}
			sb.Append("|");
		}

		private Hashtable overrides;
	}
}
