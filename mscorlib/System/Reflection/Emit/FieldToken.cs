using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct FieldToken
	{
		internal FieldToken(int val)
		{
			this.tokValue = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is FieldToken;
			if (flag)
			{
				FieldToken fieldToken = (FieldToken)obj;
				flag = this.tokValue == fieldToken.tokValue;
			}
			return flag;
		}

		public bool Equals(FieldToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return this.tokValue;
		}

		public int Token
		{
			get
			{
				return this.tokValue;
			}
		}

		public static bool operator ==(FieldToken a, FieldToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(FieldToken a, FieldToken b)
		{
			return !object.Equals(a, b);
		}

		internal int tokValue;

		public static readonly FieldToken Empty = default(FieldToken);
	}
}
