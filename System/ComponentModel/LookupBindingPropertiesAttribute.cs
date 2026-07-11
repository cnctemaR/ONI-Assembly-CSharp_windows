using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class LookupBindingPropertiesAttribute : Attribute
	{
		public LookupBindingPropertiesAttribute(string dataSource, string displayMember, string valueMember, string lookupMember)
		{
			this.data_source = dataSource;
			this.display_member = displayMember;
			this.value_member = valueMember;
			this.lookup_member = lookupMember;
		}

		public LookupBindingPropertiesAttribute()
		{
		}

		public override int GetHashCode()
		{
			return ((this.data_source == null) ? 1 : this.data_source.GetHashCode()) << 24 + ((this.display_member == null) ? 1 : this.display_member.GetHashCode()) << 16 + ((this.lookup_member == null) ? 1 : this.lookup_member.GetHashCode()) << 8 + ((this.value_member == null) ? 1 : this.value_member.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			LookupBindingPropertiesAttribute lookupBindingPropertiesAttribute = obj as LookupBindingPropertiesAttribute;
			return lookupBindingPropertiesAttribute != null && !(this.data_source != lookupBindingPropertiesAttribute.data_source) && !(this.display_member != lookupBindingPropertiesAttribute.display_member) && !(this.value_member != lookupBindingPropertiesAttribute.value_member) && !(this.lookup_member != lookupBindingPropertiesAttribute.lookup_member);
		}

		public string DataSource
		{
			get
			{
				return this.data_source;
			}
		}

		public string DisplayMember
		{
			get
			{
				return this.display_member;
			}
		}

		public string LookupMember
		{
			get
			{
				return this.lookup_member;
			}
		}

		public string ValueMember
		{
			get
			{
				return this.value_member;
			}
		}

		private string data_source;

		private string display_member;

		private string value_member;

		private string lookup_member;

		public static readonly LookupBindingPropertiesAttribute Default = new LookupBindingPropertiesAttribute();
	}
}
