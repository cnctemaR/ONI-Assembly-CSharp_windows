using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct PropertyToken
	{
		internal PropertyToken(int val)
		{
			this.tokValue = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is PropertyToken;
			if (flag)
			{
				PropertyToken propertyToken = (PropertyToken)obj;
				flag = this.tokValue == propertyToken.tokValue;
			}
			return flag;
		}

		public bool Equals(PropertyToken obj)
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

		public static bool operator ==(PropertyToken a, PropertyToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(PropertyToken a, PropertyToken b)
		{
			return !object.Equals(a, b);
		}

		internal int tokValue;

		public static readonly PropertyToken Empty = default(PropertyToken);
	}
}
