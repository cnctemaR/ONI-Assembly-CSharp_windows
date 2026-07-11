using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class ListBindableAttribute : Attribute
	{
		public ListBindableAttribute(bool listBindable)
		{
			this.bindable = listBindable;
		}

		public ListBindableAttribute(BindableSupport flags)
		{
			if (flags == BindableSupport.No)
			{
				this.bindable = false;
			}
			else
			{
				this.bindable = true;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ListBindableAttribute && ((ListBindableAttribute)obj).ListBindable.Equals(this.bindable);
		}

		public override int GetHashCode()
		{
			return this.bindable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ListBindableAttribute.Default);
		}

		public bool ListBindable
		{
			get
			{
				return this.bindable;
			}
		}

		public static readonly ListBindableAttribute Default = new ListBindableAttribute(true);

		public static readonly ListBindableAttribute No = new ListBindableAttribute(false);

		public static readonly ListBindableAttribute Yes = new ListBindableAttribute(true);

		private bool bindable;
	}
}
