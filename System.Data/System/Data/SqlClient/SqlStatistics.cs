using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal sealed class SqlStatistics
	{
		internal static SqlStatistics StartTimer(SqlStatistics statistics)
		{
			if (statistics != null && !statistics.RequestExecutionTimer())
			{
				statistics = null;
			}
			return statistics;
		}

		internal static void StopTimer(SqlStatistics statistics)
		{
			if (statistics != null)
			{
				statistics.ReleaseAndUpdateExecutionTimer();
			}
		}

		internal bool WaitForDoneAfterRow
		{
			get
			{
				return this._waitForDoneAfterRow;
			}
			set
			{
				this._waitForDoneAfterRow = value;
			}
		}

		internal bool WaitForReply
		{
			get
			{
				return this._waitForReply;
			}
		}

		internal SqlStatistics()
		{
		}

		internal void ContinueOnNewConnection()
		{
			this._startExecutionTimestamp = 0L;
			this._startFetchTimestamp = 0L;
			this._waitForDoneAfterRow = false;
			this._waitForReply = false;
		}

		internal IDictionary GetDictionary()
		{
			return new SqlStatistics.StatisticsDictionary(18)
			{
				{ "BuffersReceived", this._buffersReceived },
				{ "BuffersSent", this._buffersSent },
				{ "BytesReceived", this._bytesReceived },
				{ "BytesSent", this._bytesSent },
				{ "CursorOpens", this._cursorOpens },
				{ "IduCount", this._iduCount },
				{ "IduRows", this._iduRows },
				{ "PreparedExecs", this._preparedExecs },
				{ "Prepares", this._prepares },
				{ "SelectCount", this._selectCount },
				{ "SelectRows", this._selectRows },
				{ "ServerRoundtrips", this._serverRoundtrips },
				{ "SumResultSets", this._sumResultSets },
				{ "Transactions", this._transactions },
				{ "UnpreparedExecs", this._unpreparedExecs },
				{
					"ConnectionTime",
					ADP.TimerToMilliseconds(this._connectionTime)
				},
				{
					"ExecutionTime",
					ADP.TimerToMilliseconds(this._executionTime)
				},
				{
					"NetworkServerTime",
					ADP.TimerToMilliseconds(this._networkServerTime)
				}
			};
		}

		internal bool RequestExecutionTimer()
		{
			if (this._startExecutionTimestamp == 0L)
			{
				ADP.TimerCurrent(out this._startExecutionTimestamp);
				return true;
			}
			return false;
		}

		internal void RequestNetworkServerTimer()
		{
			if (this._startNetworkServerTimestamp == 0L)
			{
				ADP.TimerCurrent(out this._startNetworkServerTimestamp);
			}
			this._waitForReply = true;
		}

		internal void ReleaseAndUpdateExecutionTimer()
		{
			if (this._startExecutionTimestamp > 0L)
			{
				this._executionTime += ADP.TimerCurrent() - this._startExecutionTimestamp;
				this._startExecutionTimestamp = 0L;
			}
		}

		internal void ReleaseAndUpdateNetworkServerTimer()
		{
			if (this._waitForReply && this._startNetworkServerTimestamp > 0L)
			{
				this._networkServerTime += ADP.TimerCurrent() - this._startNetworkServerTimestamp;
				this._startNetworkServerTimestamp = 0L;
			}
			this._waitForReply = false;
		}

		internal void Reset()
		{
			this._buffersReceived = 0L;
			this._buffersSent = 0L;
			this._bytesReceived = 0L;
			this._bytesSent = 0L;
			this._connectionTime = 0L;
			this._cursorOpens = 0L;
			this._executionTime = 0L;
			this._iduCount = 0L;
			this._iduRows = 0L;
			this._networkServerTime = 0L;
			this._preparedExecs = 0L;
			this._prepares = 0L;
			this._selectCount = 0L;
			this._selectRows = 0L;
			this._serverRoundtrips = 0L;
			this._sumResultSets = 0L;
			this._transactions = 0L;
			this._unpreparedExecs = 0L;
			this._waitForDoneAfterRow = false;
			this._waitForReply = false;
			this._startExecutionTimestamp = 0L;
			this._startNetworkServerTimestamp = 0L;
		}

		internal void SafeAdd(ref long value, long summand)
		{
			if (9223372036854775807L - value > summand)
			{
				value += summand;
				return;
			}
			value = long.MaxValue;
		}

		internal long SafeIncrement(ref long value)
		{
			if (value < 9223372036854775807L)
			{
				value += 1L;
			}
			return value;
		}

		internal void UpdateStatistics()
		{
			if (this._closeTimestamp >= this._openTimestamp)
			{
				this.SafeAdd(ref this._connectionTime, this._closeTimestamp - this._openTimestamp);
				return;
			}
			this._connectionTime = long.MaxValue;
		}

		internal long _closeTimestamp;

		internal long _openTimestamp;

		internal long _startExecutionTimestamp;

		internal long _startFetchTimestamp;

		internal long _startNetworkServerTimestamp;

		internal long _buffersReceived;

		internal long _buffersSent;

		internal long _bytesReceived;

		internal long _bytesSent;

		internal long _connectionTime;

		internal long _cursorOpens;

		internal long _executionTime;

		internal long _iduCount;

		internal long _iduRows;

		internal long _networkServerTime;

		internal long _preparedExecs;

		internal long _prepares;

		internal long _selectCount;

		internal long _selectRows;

		internal long _serverRoundtrips;

		internal long _sumResultSets;

		internal long _transactions;

		internal long _unpreparedExecs;

		private bool _waitForDoneAfterRow;

		private bool _waitForReply;

		private sealed class StatisticsDictionary : Dictionary<object, object>, IDictionary, ICollection, IEnumerable
		{
			public StatisticsDictionary(int capacity)
				: base(capacity)
			{
			}

			ICollection IDictionary.Keys
			{
				get
				{
					SqlStatistics.StatisticsDictionary.Collection collection;
					if ((collection = this._keys) == null)
					{
						collection = (this._keys = new SqlStatistics.StatisticsDictionary.Collection(this, base.Keys));
					}
					return collection;
				}
			}

			ICollection IDictionary.Values
			{
				get
				{
					SqlStatistics.StatisticsDictionary.Collection collection;
					if ((collection = this._values) == null)
					{
						collection = (this._values = new SqlStatistics.StatisticsDictionary.Collection(this, base.Values));
					}
					return collection;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IDictionary)this).GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int arrayIndex)
			{
				this.ValidateCopyToArguments(array, arrayIndex);
				foreach (KeyValuePair<object, object> keyValuePair in this)
				{
					DictionaryEntry dictionaryEntry = new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
					array.SetValue(dictionaryEntry, arrayIndex++);
				}
			}

			private void CopyKeys(Array array, int arrayIndex)
			{
				this.ValidateCopyToArguments(array, arrayIndex);
				foreach (KeyValuePair<object, object> keyValuePair in this)
				{
					array.SetValue(keyValuePair.Key, arrayIndex++);
				}
			}

			private void CopyValues(Array array, int arrayIndex)
			{
				this.ValidateCopyToArguments(array, arrayIndex);
				foreach (KeyValuePair<object, object> keyValuePair in this)
				{
					array.SetValue(keyValuePair.Value, arrayIndex++);
				}
			}

			private void ValidateCopyToArguments(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number required.");
				}
				if (array.Length - arrayIndex < base.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
			}

			private SqlStatistics.StatisticsDictionary.Collection _keys;

			private SqlStatistics.StatisticsDictionary.Collection _values;

			private sealed class Collection : ICollection, IEnumerable
			{
				public Collection(SqlStatistics.StatisticsDictionary dictionary, ICollection collection)
				{
					this._dictionary = dictionary;
					this._collection = collection;
				}

				int ICollection.Count
				{
					get
					{
						return this._collection.Count;
					}
				}

				bool ICollection.IsSynchronized
				{
					get
					{
						return this._collection.IsSynchronized;
					}
				}

				object ICollection.SyncRoot
				{
					get
					{
						return this._collection.SyncRoot;
					}
				}

				void ICollection.CopyTo(Array array, int arrayIndex)
				{
					if (this._collection is Dictionary<object, object>.KeyCollection)
					{
						this._dictionary.CopyKeys(array, arrayIndex);
						return;
					}
					this._dictionary.CopyValues(array, arrayIndex);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this._collection.GetEnumerator();
				}

				private readonly SqlStatistics.StatisticsDictionary _dictionary;

				private readonly ICollection _collection;
			}
		}
	}
}
