using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct StringToken
	{
		internal StringToken(int val)
		{
			this.tokValue = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is StringToken;
			if (flag)
			{
				StringToken stringToken = (StringToken)obj;
				flag = this.tokValue == stringToken.tokValue;
			}
			return flag;
		}

		public bool Equals(StringToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		public static bool operator ==(StringToken a, StringToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(StringToken a, StringToken b)
		{
			return !object.Equals(a, b);
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

		internal int tokValue;
	}
}
