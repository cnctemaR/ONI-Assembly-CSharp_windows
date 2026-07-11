using System;

namespace System.Data
{
	public sealed class StateChangeEventArgs : EventArgs
	{
		public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
		{
		}

		public ConnectionState CurrentState
		{
			get
			{
				throw null;
			}
		}

		public ConnectionState OriginalState
		{
			get
			{
				throw null;
			}
		}
	}
}
