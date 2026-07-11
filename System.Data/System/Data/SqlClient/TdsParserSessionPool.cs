using System;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal class TdsParserSessionPool
	{
		internal TdsParserSessionPool(TdsParser parser)
		{
			this._parser = parser;
			this._cache = new List<TdsParserStateObject>();
			this._freeStateObjects = new TdsParserStateObject[10];
			this._freeStateObjectCount = 0;
		}

		private bool IsDisposed
		{
			get
			{
				return this._freeStateObjects == null;
			}
		}

		internal void Deactivate()
		{
			List<TdsParserStateObject> cache = this._cache;
			lock (cache)
			{
				for (int i = this._cache.Count - 1; i >= 0; i--)
				{
					TdsParserStateObject tdsParserStateObject = this._cache[i];
					if (tdsParserStateObject != null && tdsParserStateObject.IsOrphaned)
					{
						this.PutSession(tdsParserStateObject);
					}
				}
			}
		}

		internal void Dispose()
		{
			List<TdsParserStateObject> cache = this._cache;
			lock (cache)
			{
				for (int i = 0; i < this._freeStateObjectCount; i++)
				{
					if (this._freeStateObjects[i] != null)
					{
						this._freeStateObjects[i].Dispose();
					}
				}
				this._freeStateObjects = null;
				this._freeStateObjectCount = 0;
				for (int j = 0; j < this._cache.Count; j++)
				{
					if (this._cache[j] != null)
					{
						if (this._cache[j].IsOrphaned)
						{
							this._cache[j].Dispose();
						}
						else
						{
							this._cache[j].DecrementPendingCallbacks(false);
						}
					}
				}
				this._cache.Clear();
				this._cachedCount = 0;
			}
		}

		internal TdsParserStateObject GetSession(object owner)
		{
			List<TdsParserStateObject> cache = this._cache;
			TdsParserStateObject tdsParserStateObject;
			lock (cache)
			{
				if (this.IsDisposed)
				{
					throw ADP.ClosedConnectionError();
				}
				if (this._freeStateObjectCount > 0)
				{
					this._freeStateObjectCount--;
					tdsParserStateObject = this._freeStateObjects[this._freeStateObjectCount];
					this._freeStateObjects[this._freeStateObjectCount] = null;
				}
				else
				{
					tdsParserStateObject = this._parser.CreateSession();
					this._cache.Add(tdsParserStateObject);
					this._cachedCount = this._cache.Count;
				}
				tdsParserStateObject.Activate(owner);
			}
			return tdsParserStateObject;
		}

		internal void PutSession(TdsParserStateObject session)
		{
			bool flag = session.Deactivate();
			List<TdsParserStateObject> cache = this._cache;
			lock (cache)
			{
				if (this.IsDisposed)
				{
					session.Dispose();
				}
				else if (flag && this._freeStateObjectCount < 10)
				{
					this._freeStateObjects[this._freeStateObjectCount] = session;
					this._freeStateObjectCount++;
				}
				else
				{
					this._cache.Remove(session);
					this._cachedCount = this._cache.Count;
					session.Dispose();
				}
				session.RemoveOwner();
			}
		}

		internal int ActiveSessionsCount
		{
			get
			{
				return this._cachedCount - this._freeStateObjectCount;
			}
		}

		private const int MaxInactiveCount = 10;

		private readonly TdsParser _parser;

		private readonly List<TdsParserStateObject> _cache;

		private int _cachedCount;

		private TdsParserStateObject[] _freeStateObjects;

		private int _freeStateObjectCount;
	}
}
