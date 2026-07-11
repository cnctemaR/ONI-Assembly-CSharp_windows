using System;

namespace System.Data
{
	public sealed class StateChangeEventArgs : EventArgs
	{
		public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
		{
			this._originalState = originalState;
			this._currentState = currentState;
		}

		public ConnectionState CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		public ConnectionState OriginalState
		{
			get
			{
				return this._originalState;
			}
		}

		private ConnectionState _originalState;

		private ConnectionState _currentState;
	}
}
