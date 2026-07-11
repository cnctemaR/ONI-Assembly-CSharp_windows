using System;
using Unity;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcError
	{
		internal OdbcError(string source, string message, string state, int nativeerror)
		{
			this._source = source;
			this._message = message;
			this._state = state;
			this._nativeerror = nativeerror;
		}

		public string Message
		{
			get
			{
				if (this._message == null)
				{
					return string.Empty;
				}
				return this._message;
			}
		}

		public string SQLState
		{
			get
			{
				return this._state;
			}
		}

		public int NativeError
		{
			get
			{
				return this._nativeerror;
			}
		}

		public string Source
		{
			get
			{
				if (this._source == null)
				{
					return string.Empty;
				}
				return this._source;
			}
		}

		internal void SetSource(string Source)
		{
			this._source = Source;
		}

		public override string ToString()
		{
			return this.Message;
		}

		internal OdbcError()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal string _message;

		internal string _state;

		internal int _nativeerror;

		internal string _source;
	}
}
