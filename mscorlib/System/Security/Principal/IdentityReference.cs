using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public abstract class IdentityReference
	{
		internal IdentityReference()
		{
		}

		public abstract string Value { get; }

		public abstract override bool Equals(object o);

		public abstract override int GetHashCode();

		public abstract bool IsValidTargetType(Type targetType);

		public abstract override string ToString();

		public abstract IdentityReference Translate(Type targetType);

		public static bool operator ==(IdentityReference left, IdentityReference right)
		{
			if (left == null)
			{
				return right == null;
			}
			return right != null && left.Value == right.Value;
		}

		public static bool operator !=(IdentityReference left, IdentityReference right)
		{
			if (left == null)
			{
				return right != null;
			}
			return right == null || left.Value != right.Value;
		}
	}
}
