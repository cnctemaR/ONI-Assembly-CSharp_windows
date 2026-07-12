using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public readonly struct EventToken : IEquatable<EventToken>
	{
		internal EventToken(int val)
		{
			this.tokValue = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is EventToken;
			if (flag)
			{
				EventToken eventToken = (EventToken)obj;
				flag = this.tokValue == eventToken.tokValue;
			}
			return flag;
		}

		public bool Equals(EventToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		public static bool operator ==(EventToken a, EventToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(EventToken a, EventToken b)
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

		internal readonly int tokValue;

		public static readonly EventToken Empty;
	}
}
