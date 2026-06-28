using System;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcError
	{
		internal OdbcError(OdbcConnection connection)
		{
			this._nativeerror = 1;
			this._source = connection.SafeDriver;
			this._message = "Error in " + this._source;
			this._state = string.Empty;
		}

		internal OdbcError(string message, string state, int nativeerror)
		{
			this._message = message;
			this._state = state;
			this._nativeerror = nativeerror;
		}

		public string Message
		{
			get
			{
				return this._message;
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
				return this._source;
			}
		}

		public string SQLState
		{
			get
			{
				return this._state;
			}
		}

		public override string ToString()
		{
			return this.Message;
		}

		internal void SetSource(string source)
		{
			this._source = source;
		}

		private readonly string _message;

		private string _source;

		private readonly string _state;

		private readonly int _nativeerror;
	}
}
