using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct MethodToken
	{
		internal MethodToken(int val)
		{
			this.tokValue = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is MethodToken;
			if (flag)
			{
				MethodToken methodToken = (MethodToken)obj;
				flag = this.tokValue == methodToken.tokValue;
			}
			return flag;
		}

		public bool Equals(MethodToken obj)
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

		public static bool operator ==(MethodToken a, MethodToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(MethodToken a, MethodToken b)
		{
			return !object.Equals(a, b);
		}

		internal int tokValue;

		public static readonly MethodToken Empty = default(MethodToken);
	}
}
