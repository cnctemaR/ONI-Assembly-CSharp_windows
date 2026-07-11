using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Data.SqlClient
{
	internal class SessionData
	{
		public SessionData(SessionData recoveryData)
		{
			this._initialDatabase = recoveryData._initialDatabase;
			this._initialCollation = recoveryData._initialCollation;
			this._initialLanguage = recoveryData._initialLanguage;
			this._resolvedAliases = recoveryData._resolvedAliases;
			for (int i = 0; i < 256; i++)
			{
				if (recoveryData._initialState[i] != null)
				{
					this._initialState[i] = (byte[])recoveryData._initialState[i].Clone();
				}
			}
		}

		public SessionData()
		{
			this._resolvedAliases = new Dictionary<string, Tuple<string, string>>(2);
		}

		public void Reset()
		{
			this._database = null;
			this._collation = null;
			this._language = null;
			if (this._deltaDirty)
			{
				this._delta = new SessionStateRecord[256];
				this._deltaDirty = false;
			}
			this._unrecoverableStatesCount = 0;
		}

		[Conditional("DEBUG")]
		public void AssertUnrecoverableStateCountIsCorrect()
		{
			byte b = 0;
			foreach (SessionStateRecord sessionStateRecord in this._delta)
			{
				if (sessionStateRecord != null && !sessionStateRecord._recoverable)
				{
					b += 1;
				}
			}
		}

		internal const int _maxNumberOfSessionStates = 256;

		internal uint _tdsVersion;

		internal bool _encrypted;

		internal string _database;

		internal SqlCollation _collation;

		internal string _language;

		internal string _initialDatabase;

		internal SqlCollation _initialCollation;

		internal string _initialLanguage;

		internal byte _unrecoverableStatesCount;

		internal Dictionary<string, Tuple<string, string>> _resolvedAliases;

		internal SessionStateRecord[] _delta = new SessionStateRecord[256];

		internal bool _deltaDirty;

		internal byte[][] _initialState = new byte[256][];
	}
}
