using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ExtenderProvidedPropertyAttribute : Attribute
	{
		internal static ExtenderProvidedPropertyAttribute Create(PropertyDescriptor extenderProperty, Type receiverType, IExtenderProvider provider)
		{
			return new ExtenderProvidedPropertyAttribute
			{
				extenderProperty = extenderProperty,
				receiverType = receiverType,
				provider = provider
			};
		}

		public PropertyDescriptor ExtenderProperty
		{
			get
			{
				return this.extenderProperty;
			}
		}

		public IExtenderProvider Provider
		{
			get
			{
				return this.provider;
			}
		}

		public Type ReceiverType
		{
			get
			{
				return this.receiverType;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ExtenderProvidedPropertyAttribute extenderProvidedPropertyAttribute = obj as ExtenderProvidedPropertyAttribute;
			return extenderProvidedPropertyAttribute != null && extenderProvidedPropertyAttribute.extenderProperty.Equals(this.extenderProperty) && extenderProvidedPropertyAttribute.provider.Equals(this.provider) && extenderProvidedPropertyAttribute.receiverType.Equals(this.receiverType);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.receiverType == null;
		}

		private PropertyDescriptor extenderProperty;

		private IExtenderProvider provider;

		private Type receiverType;
	}
}
