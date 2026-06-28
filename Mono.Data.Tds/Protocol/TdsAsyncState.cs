using System;

namespace Mono.Data.Tds.Protocol
{
	internal class TdsAsyncState
	{
		public TdsAsyncState(object userState)
		{
			this._userState = userState;
		}

		public object UserState
		{
			get
			{
				return this._userState;
			}
			set
			{
				this._userState = value;
			}
		}

		public bool WantResults
		{
			get
			{
				return this._wantResults;
			}
			set
			{
				this._wantResults = value;
			}
		}

		private object _userState;

		private bool _wantResults;
	}
}
