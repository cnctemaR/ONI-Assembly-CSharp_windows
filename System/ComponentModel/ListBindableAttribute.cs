using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ListBindableAttribute : Attribute
	{
		public ListBindableAttribute(bool listBindable)
		{
			this.listBindable = listBindable;
		}

		public ListBindableAttribute(BindableSupport flags)
		{
			this.listBindable = flags > BindableSupport.No;
			this.isDefault = flags == BindableSupport.Default;
		}

		public bool ListBindable
		{
			get
			{
				return this.listBindable;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ListBindableAttribute listBindableAttribute = obj as ListBindableAttribute;
			return listBindableAttribute != null && listBindableAttribute.ListBindable == this.listBindable;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ListBindableAttribute.Default) || this.isDefault;
		}

		public static readonly ListBindableAttribute Yes = new ListBindableAttribute(true);

		public static readonly ListBindableAttribute No = new ListBindableAttribute(false);

		public static readonly ListBindableAttribute Default = ListBindableAttribute.Yes;

		private bool listBindable;

		private bool isDefault;
	}
}
