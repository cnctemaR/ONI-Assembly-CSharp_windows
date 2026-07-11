using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ComplexBindingPropertiesAttribute : Attribute
	{
		public ComplexBindingPropertiesAttribute()
		{
			this.dataSource = null;
			this.dataMember = null;
		}

		public ComplexBindingPropertiesAttribute(string dataSource)
		{
			this.dataSource = dataSource;
			this.dataMember = null;
		}

		public ComplexBindingPropertiesAttribute(string dataSource, string dataMember)
		{
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}

		public string DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public string DataMember
		{
			get
			{
				return this.dataMember;
			}
		}

		public override bool Equals(object obj)
		{
			ComplexBindingPropertiesAttribute complexBindingPropertiesAttribute = obj as ComplexBindingPropertiesAttribute;
			return complexBindingPropertiesAttribute != null && complexBindingPropertiesAttribute.DataSource == this.dataSource && complexBindingPropertiesAttribute.DataMember == this.dataMember;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly string dataSource;

		private readonly string dataMember;

		public static readonly ComplexBindingPropertiesAttribute Default = new ComplexBindingPropertiesAttribute();
	}
}
