using System;

namespace System.Data
{
	public sealed class StateChangeEventArgs : EventArgs
	{
		public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
		{
			this.originalState = originalState;
			this.currentState = currentState;
		}

		public ConnectionState CurrentState
		{
			get
			{
				return this.currentState;
			}
		}

		public ConnectionState OriginalState
		{
			get
			{
				return this.originalState;
			}
		}

		private ConnectionState originalState;

		private ConnectionState currentState;
	}
}
