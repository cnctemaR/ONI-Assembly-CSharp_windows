using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ParenthesizePropertyNameAttribute : Attribute
	{
		public ParenthesizePropertyNameAttribute()
		{
			this.parenthesis = false;
		}

		public ParenthesizePropertyNameAttribute(bool needParenthesis)
		{
			this.parenthesis = needParenthesis;
		}

		public bool NeedParenthesis
		{
			get
			{
				return this.parenthesis;
			}
		}

		public override bool Equals(object o)
		{
			return o is ParenthesizePropertyNameAttribute && (o == this || ((ParenthesizePropertyNameAttribute)o).NeedParenthesis == this.parenthesis);
		}

		public override int GetHashCode()
		{
			return this.parenthesis.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.parenthesis == ParenthesizePropertyNameAttribute.Default.NeedParenthesis;
		}

		private bool parenthesis;

		public static readonly ParenthesizePropertyNameAttribute Default = new ParenthesizePropertyNameAttribute();
	}
}
