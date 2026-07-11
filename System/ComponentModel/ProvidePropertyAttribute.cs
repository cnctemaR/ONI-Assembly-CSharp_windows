using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ProvidePropertyAttribute : Attribute
	{
		public ProvidePropertyAttribute(string propertyName, Type receiverType)
		{
			this.propertyName = propertyName;
			this.receiverTypeName = receiverType.AssemblyQualifiedName;
		}

		public ProvidePropertyAttribute(string propertyName, string receiverTypeName)
		{
			this.propertyName = propertyName;
			this.receiverTypeName = receiverTypeName;
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public string ReceiverTypeName
		{
			get
			{
				return this.receiverTypeName;
			}
		}

		public override object TypeId
		{
			get
			{
				return base.GetType().FullName + this.propertyName;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ProvidePropertyAttribute providePropertyAttribute = obj as ProvidePropertyAttribute;
			return providePropertyAttribute != null && providePropertyAttribute.propertyName == this.propertyName && providePropertyAttribute.receiverTypeName == this.receiverTypeName;
		}

		public override int GetHashCode()
		{
			return this.propertyName.GetHashCode() ^ this.receiverTypeName.GetHashCode();
		}

		private readonly string propertyName;

		private readonly string receiverTypeName;
	}
}
