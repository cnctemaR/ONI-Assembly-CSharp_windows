using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class LookupBindingPropertiesAttribute : Attribute
	{
		public LookupBindingPropertiesAttribute()
		{
			this.dataSource = null;
			this.displayMember = null;
			this.valueMember = null;
			this.lookupMember = null;
		}

		public LookupBindingPropertiesAttribute(string dataSource, string displayMember, string valueMember, string lookupMember)
		{
			this.dataSource = dataSource;
			this.displayMember = displayMember;
			this.valueMember = valueMember;
			this.lookupMember = lookupMember;
		}

		public string DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public string DisplayMember
		{
			get
			{
				return this.displayMember;
			}
		}

		public string ValueMember
		{
			get
			{
				return this.valueMember;
			}
		}

		public string LookupMember
		{
			get
			{
				return this.lookupMember;
			}
		}

		public override bool Equals(object obj)
		{
			LookupBindingPropertiesAttribute lookupBindingPropertiesAttribute = obj as LookupBindingPropertiesAttribute;
			return lookupBindingPropertiesAttribute != null && lookupBindingPropertiesAttribute.DataSource == this.dataSource && lookupBindingPropertiesAttribute.displayMember == this.displayMember && lookupBindingPropertiesAttribute.valueMember == this.valueMember && lookupBindingPropertiesAttribute.lookupMember == this.lookupMember;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly string dataSource;

		private readonly string displayMember;

		private readonly string valueMember;

		private readonly string lookupMember;

		public static readonly LookupBindingPropertiesAttribute Default = new LookupBindingPropertiesAttribute();
	}
}
