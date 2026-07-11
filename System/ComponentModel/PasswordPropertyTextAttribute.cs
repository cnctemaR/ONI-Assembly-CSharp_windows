using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class PasswordPropertyTextAttribute : Attribute
	{
		public PasswordPropertyTextAttribute()
			: this(false)
		{
		}

		public PasswordPropertyTextAttribute(bool password)
		{
			this._password = password;
		}

		public bool Password
		{
			get
			{
				return this._password;
			}
		}

		public override bool Equals(object o)
		{
			return o is PasswordPropertyTextAttribute && ((PasswordPropertyTextAttribute)o).Password == this._password;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(PasswordPropertyTextAttribute.Default);
		}

		public static readonly PasswordPropertyTextAttribute Yes = new PasswordPropertyTextAttribute(true);

		public static readonly PasswordPropertyTextAttribute No = new PasswordPropertyTextAttribute(false);

		public static readonly PasswordPropertyTextAttribute Default = PasswordPropertyTextAttribute.No;

		private bool _password;
	}
}
