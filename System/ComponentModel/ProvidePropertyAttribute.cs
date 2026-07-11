using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class ProvidePropertyAttribute : Attribute
	{
		public ProvidePropertyAttribute(string propertyName, string receiverTypeName)
		{
			this.Property = propertyName;
			this.Receiver = receiverTypeName;
		}

		public ProvidePropertyAttribute(string propertyName, Type receiverType)
		{
			this.Property = propertyName;
			this.Receiver = receiverType.AssemblyQualifiedName;
		}

		public string PropertyName
		{
			get
			{
				return this.Property;
			}
		}

		public string ReceiverTypeName
		{
			get
			{
				return this.Receiver;
			}
		}

		public override object TypeId
		{
			get
			{
				return base.TypeId + this.Property;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ProvidePropertyAttribute && (obj == this || (((ProvidePropertyAttribute)obj).PropertyName == this.Property && ((ProvidePropertyAttribute)obj).ReceiverTypeName == this.Receiver));
		}

		public override int GetHashCode()
		{
			return (this.Property + this.Receiver).GetHashCode();
		}

		private string Property;

		private string Receiver;
	}
}
