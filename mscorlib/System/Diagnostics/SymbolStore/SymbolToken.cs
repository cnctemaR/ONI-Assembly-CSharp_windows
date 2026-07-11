using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public struct SymbolToken
	{
		public SymbolToken(int val)
		{
			this._val = val;
		}

		public override bool Equals(object obj)
		{
			return obj is SymbolToken && ((SymbolToken)obj).GetToken() == this._val;
		}

		public bool Equals(SymbolToken obj)
		{
			return obj.GetToken() == this._val;
		}

		public static bool operator ==(SymbolToken a, SymbolToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SymbolToken a, SymbolToken b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return this._val.GetHashCode();
		}

		public int GetToken()
		{
			return this._val;
		}

		private int _val;
	}
}
