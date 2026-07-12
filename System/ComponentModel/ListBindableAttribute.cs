using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ListBindableAttribute : Attribute
	{
		public ListBindableAttribute(bool listBindable)
		{
			this.ListBindable = listBindable;
		}

		public ListBindableAttribute(BindableSupport flags)
		{
			this.ListBindable = flags > BindableSupport.No;
			this._isDefault = flags == BindableSupport.Default;
		}

		public bool ListBindable { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ListBindableAttribute listBindableAttribute = obj as ListBindableAttribute;
			return listBindableAttribute != null && listBindableAttribute.ListBindable == this.ListBindable;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ListBindableAttribute.Default) || this._isDefault;
		}

		public static readonly ListBindableAttribute Yes = new ListBindableAttribute(true);

		public static readonly ListBindableAttribute No = new ListBindableAttribute(false);

		public static readonly ListBindableAttribute Default = ListBindableAttribute.Yes;

		private bool _isDefault;
	}
}
