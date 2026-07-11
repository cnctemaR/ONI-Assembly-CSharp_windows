using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct ParameterToken
	{
		internal ParameterToken(int val)
		{
			this.tokValue = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is ParameterToken;
			if (flag)
			{
				ParameterToken parameterToken = (ParameterToken)obj;
				flag = this.tokValue == parameterToken.tokValue;
			}
			return flag;
		}

		public bool Equals(ParameterToken obj)
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

		public static bool operator ==(ParameterToken a, ParameterToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(ParameterToken a, ParameterToken b)
		{
			return !object.Equals(a, b);
		}

		internal int tokValue;

		public static readonly ParameterToken Empty = default(ParameterToken);
	}
}
