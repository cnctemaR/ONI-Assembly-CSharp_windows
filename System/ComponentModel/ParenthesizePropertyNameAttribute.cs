using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ParenthesizePropertyNameAttribute : Attribute
	{
		public ParenthesizePropertyNameAttribute()
			: this(false)
		{
		}

		public ParenthesizePropertyNameAttribute(bool needParenthesis)
		{
			this.needParenthesis = needParenthesis;
		}

		public bool NeedParenthesis
		{
			get
			{
				return this.needParenthesis;
			}
		}

		public override bool Equals(object o)
		{
			return o is ParenthesizePropertyNameAttribute && ((ParenthesizePropertyNameAttribute)o).NeedParenthesis == this.needParenthesis;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ParenthesizePropertyNameAttribute.Default);
		}

		public static readonly ParenthesizePropertyNameAttribute Default = new ParenthesizePropertyNameAttribute();

		private bool needParenthesis;
	}
}
