using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ExtenderProvidedPropertyAttribute : Attribute
	{
		internal static ExtenderProvidedPropertyAttribute CreateAttribute(PropertyDescriptor extenderProperty, IExtenderProvider provider, Type receiverType)
		{
			return new ExtenderProvidedPropertyAttribute
			{
				extender = extenderProperty,
				receiver = receiverType,
				extenderProvider = provider
			};
		}

		public PropertyDescriptor ExtenderProperty
		{
			get
			{
				return this.extender;
			}
		}

		public IExtenderProvider Provider
		{
			get
			{
				return this.extenderProvider;
			}
		}

		public Type ReceiverType
		{
			get
			{
				return this.receiver;
			}
		}

		public override bool IsDefaultAttribute()
		{
			return this.extender == null && this.extenderProvider == null && this.receiver == null;
		}

		public override bool Equals(object obj)
		{
			return obj is ExtenderProvidedPropertyAttribute && (obj == this || (((ExtenderProvidedPropertyAttribute)obj).ExtenderProperty.Equals(this.extender) && ((ExtenderProvidedPropertyAttribute)obj).Provider.Equals(this.extenderProvider) && ((ExtenderProvidedPropertyAttribute)obj).ReceiverType.Equals(this.receiver)));
		}

		public override int GetHashCode()
		{
			return this.extender.GetHashCode() ^ this.extenderProvider.GetHashCode() ^ this.receiver.GetHashCode();
		}

		private PropertyDescriptor extender;

		private IExtenderProvider extenderProvider;

		private Type receiver;
	}
}
