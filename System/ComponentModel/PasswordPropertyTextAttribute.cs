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
			return o is PasswordPropertyTextAttribute && ((PasswordPropertyTextAttribute)o).Password == this.Password;
		}

		public override int GetHashCode()
		{
			return this.Password.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return PasswordPropertyTextAttribute.Default.Equals(this);
		}

		public static readonly PasswordPropertyTextAttribute Default = PasswordPropertyTextAttribute.No;

		public static readonly PasswordPropertyTextAttribute No = new PasswordPropertyTextAttribute(false);

		public static readonly PasswordPropertyTextAttribute Yes = new PasswordPropertyTextAttribute(true);

		private bool _password;
	}
}
