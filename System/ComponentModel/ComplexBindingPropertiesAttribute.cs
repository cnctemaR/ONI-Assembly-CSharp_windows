using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ComplexBindingPropertiesAttribute : Attribute
	{
		public ComplexBindingPropertiesAttribute(string dataSource, string dataMember)
		{
			this.data_source = dataSource;
			this.data_member = dataMember;
		}

		public ComplexBindingPropertiesAttribute(string dataSource)
		{
			this.data_source = dataSource;
		}

		public ComplexBindingPropertiesAttribute()
		{
		}

		public string DataMember
		{
			get
			{
				return this.data_member;
			}
		}

		public string DataSource
		{
			get
			{
				return this.data_source;
			}
		}

		public override bool Equals(object obj)
		{
			ComplexBindingPropertiesAttribute complexBindingPropertiesAttribute = obj as ComplexBindingPropertiesAttribute;
			return complexBindingPropertiesAttribute != null && complexBindingPropertiesAttribute.DataMember == this.data_member && complexBindingPropertiesAttribute.DataSource == this.data_source;
		}

		public override int GetHashCode()
		{
			int hashCode = (this.data_source + this.data_member).GetHashCode();
			if (hashCode == 0)
			{
				return base.GetHashCode();
			}
			return hashCode;
		}

		private string data_source;

		private string data_member;

		public static readonly ComplexBindingPropertiesAttribute Default = new ComplexBindingPropertiesAttribute();
	}
}
