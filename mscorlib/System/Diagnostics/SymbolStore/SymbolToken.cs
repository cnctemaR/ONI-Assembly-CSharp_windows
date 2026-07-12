using System;

namespace System.Diagnostics.SymbolStore
{
	public readonly struct SymbolToken
	{
		public SymbolToken(int val)
		{
			this._token = val;
		}

		public int GetToken()
		{
			return this._token;
		}

		public override int GetHashCode()
		{
			return this._token;
		}

		public override bool Equals(object obj)
		{
			return obj is SymbolToken && this.Equals((SymbolToken)obj);
		}

		public bool Equals(SymbolToken obj)
		{
			return obj._token == this._token;
		}

		public static bool operator ==(SymbolToken a, SymbolToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SymbolToken a, SymbolToken b)
		{
			return !(a == b);
		}

		private readonly int _token;
	}
}
