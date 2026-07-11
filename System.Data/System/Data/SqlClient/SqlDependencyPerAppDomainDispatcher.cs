using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Threading;

namespace System.Data.SqlClient
{
	internal class SqlDependencyPerAppDomainDispatcher : MarshalByRefObject
	{
		private SqlDependencyPerAppDomainDispatcher()
		{
			this._dependencyIdToDependencyHash = new Dictionary<string, SqlDependency>();
			this._notificationIdToDependenciesHash = new Dictionary<string, SqlDependencyPerAppDomainDispatcher.DependencyList>();
			this._commandHashToNotificationId = new Dictionary<string, string>();
			this._timeoutTimer = new Timer(new TimerCallback(SqlDependencyPerAppDomainDispatcher.TimeoutTimerCallback), null, -1, -1);
			this.SubscribeToAppDomainUnload();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		internal void AddDependencyEntry(SqlDependency dep)
		{
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				this._dependencyIdToDependencyHash.Add(dep.Id, dep);
			}
		}

		internal string AddCommandEntry(string commandHash, SqlDependency dep)
		{
			string text = string.Empty;
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				if (this._dependencyIdToDependencyHash.ContainsKey(dep.Id))
				{
					if (this._commandHashToNotificationId.TryGetValue(commandHash, out text))
					{
						SqlDependencyPerAppDomainDispatcher.DependencyList dependencyList = null;
						if (!this._notificationIdToDependenciesHash.TryGetValue(text, out dependencyList))
						{
							throw ADP.InternalError(ADP.InternalErrorCode.SqlDependencyCommandHashIsNotAssociatedWithNotification);
						}
						if (!dependencyList.Contains(dep))
						{
							dependencyList.Add(dep);
						}
					}
					else
					{
						text = string.Format(CultureInfo.InvariantCulture, "{0};{1}", SqlDependency.AppDomainKey, Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
						SqlDependencyPerAppDomainDispatcher.DependencyList dependencyList2 = new SqlDependencyPerAppDomainDispatcher.DependencyList(commandHash);
						dependencyList2.Add(dep);
						this._commandHashToNotificationId.Add(commandHash, text);
						this._notificationIdToDependenciesHash.Add(text, dependencyList2);
					}
				}
			}
			return text;
		}

		internal void InvalidateCommandID(SqlNotification sqlNotification)
		{
			List<SqlDependency> list = null;
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				list = this.LookupCommandEntryWithRemove(sqlNotification.Key);
				if (list != null)
				{
					foreach (SqlDependency sqlDependency in list)
					{
						this.LookupDependencyEntryWithRemove(sqlDependency.Id);
						this.RemoveDependencyFromCommandToDependenciesHash(sqlDependency);
					}
				}
			}
			if (list != null)
			{
				foreach (SqlDependency sqlDependency2 in list)
				{
					try
					{
						sqlDependency2.Invalidate(sqlNotification.Type, sqlNotification.Info, sqlNotification.Source);
					}
					catch (Exception ex)
					{
						if (!ADP.IsCatchableExceptionType(ex))
						{
							throw;
						}
						ADP.TraceExceptionWithoutRethrow(ex);
					}
				}
			}
		}

		internal void InvalidateServer(string server, SqlNotification sqlNotification)
		{
			List<SqlDependency> list = new List<SqlDependency>();
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				foreach (KeyValuePair<string, SqlDependency> keyValuePair in this._dependencyIdToDependencyHash)
				{
					SqlDependency value = keyValuePair.Value;
					if (value.ContainsServer(server))
					{
						list.Add(value);
					}
				}
				foreach (SqlDependency sqlDependency in list)
				{
					this.LookupDependencyEntryWithRemove(sqlDependency.Id);
					this.RemoveDependencyFromCommandToDependenciesHash(sqlDependency);
				}
			}
			foreach (SqlDependency sqlDependency2 in list)
			{
				try
				{
					sqlDependency2.Invalidate(sqlNotification.Type, sqlNotification.Info, sqlNotification.Source);
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
					ADP.TraceExceptionWithoutRethrow(ex);
				}
			}
		}

		internal SqlDependency LookupDependencyEntry(string id)
		{
			if (id == null)
			{
				throw ADP.ArgumentNull("id");
			}
			if (string.IsNullOrEmpty(id))
			{
				throw SQL.SqlDependencyIdMismatch();
			}
			SqlDependency sqlDependency = null;
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				if (this._dependencyIdToDependencyHash.ContainsKey(id))
				{
					sqlDependency = this._dependencyIdToDependencyHash[id];
				}
			}
			return sqlDependency;
		}

		private void LookupDependencyEntryWithRemove(string id)
		{
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				if (this._dependencyIdToDependencyHash.ContainsKey(id))
				{
					this._dependencyIdToDependencyHash.Remove(id);
					if (this._dependencyIdToDependencyHash.Count == 0)
					{
						this._timeoutTimer.Change(-1, -1);
						this._sqlDependencyTimeOutTimerStarted = false;
					}
				}
			}
		}

		private List<SqlDependency> LookupCommandEntryWithRemove(string notificationId)
		{
			SqlDependencyPerAppDomainDispatcher.DependencyList dependencyList = null;
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				if (this._notificationIdToDependenciesHash.TryGetValue(notificationId, out dependencyList))
				{
					this._notificationIdToDependenciesHash.Remove(notificationId);
					this._commandHashToNotificationId.Remove(dependencyList.CommandHash);
				}
			}
			return dependencyList;
		}

		private void RemoveDependencyFromCommandToDependenciesHash(SqlDependency dependency)
		{
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				foreach (KeyValuePair<string, SqlDependencyPerAppDomainDispatcher.DependencyList> keyValuePair in this._notificationIdToDependenciesHash)
				{
					SqlDependencyPerAppDomainDispatcher.DependencyList value = keyValuePair.Value;
					if (value.Remove(dependency) && value.Count == 0)
					{
						list.Add(keyValuePair.Key);
						list2.Add(keyValuePair.Value.CommandHash);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					this._notificationIdToDependenciesHash.Remove(list[i]);
					this._commandHashToNotificationId.Remove(list2[i]);
				}
			}
		}

		internal void StartTimer(SqlDependency dep)
		{
			object instanceLock = this._instanceLock;
			lock (instanceLock)
			{
				if (!this._sqlDependencyTimeOutTimerStarted)
				{
					this._timeoutTimer.Change(15000, 15000);
					this._nextTimeout = dep.ExpirationTime;
					this._sqlDependencyTimeOutTimerStarted = true;
				}
				else if (this._nextTimeout > dep.ExpirationTime)
				{
					this._nextTimeout = dep.ExpirationTime;
				}
			}
		}

		private static void TimeoutTimerCallback(object state)
		{
			object obj = SqlDependencyPerAppDomainDispatcher.SingletonInstance._instanceLock;
			SqlDependency[] array;
			lock (obj)
			{
				if (SqlDependencyPerAppDomainDispatcher.SingletonInstance._dependencyIdToDependencyHash.Count == 0)
				{
					return;
				}
				if (SqlDependencyPerAppDomainDispatcher.SingletonInstance._nextTimeout > DateTime.UtcNow)
				{
					return;
				}
				array = new SqlDependency[SqlDependencyPerAppDomainDispatcher.SingletonInstance._dependencyIdToDependencyHash.Count];
				SqlDependencyPerAppDomainDispatcher.SingletonInstance._dependencyIdToDependencyHash.Values.CopyTo(array, 0);
			}
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = DateTime.MaxValue;
			int i = 0;
			while (i < array.Length)
			{
				if (array[i].ExpirationTime <= utcNow)
				{
					try
					{
						array[i].Invalidate(SqlNotificationType.Change, SqlNotificationInfo.Error, SqlNotificationSource.Timeout);
						goto IL_00E0;
					}
					catch (Exception ex)
					{
						if (!ADP.IsCatchableExceptionType(ex))
						{
							throw;
						}
						ADP.TraceExceptionWithoutRethrow(ex);
						goto IL_00E0;
					}
					goto IL_00C0;
				}
				goto IL_00C0;
				IL_00E0:
				i++;
				continue;
				IL_00C0:
				if (array[i].ExpirationTime < dateTime)
				{
					dateTime = array[i].ExpirationTime;
				}
				array[i] = null;
				goto IL_00E0;
			}
			obj = SqlDependencyPerAppDomainDispatcher.SingletonInstance._instanceLock;
			lock (obj)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != null)
					{
						SqlDependencyPerAppDomainDispatcher.SingletonInstance._dependencyIdToDependencyHash.Remove(array[j].Id);
					}
				}
				if (dateTime < SqlDependencyPerAppDomainDispatcher.SingletonInstance._nextTimeout)
				{
					SqlDependencyPerAppDomainDispatcher.SingletonInstance._nextTimeout = dateTime;
				}
			}
		}

		private void SubscribeToAppDomainUnload()
		{
		}

		internal static readonly SqlDependencyPerAppDomainDispatcher SingletonInstance = new SqlDependencyPerAppDomainDispatcher();

		internal object _instanceLock = new object();

		private Dictionary<string, SqlDependency> _dependencyIdToDependencyHash;

		private Dictionary<string, SqlDependencyPerAppDomainDispatcher.DependencyList> _notificationIdToDependenciesHash;

		private Dictionary<string, string> _commandHashToNotificationId;

		private bool _sqlDependencyTimeOutTimerStarted;

		private DateTime _nextTimeout;

		private Timer _timeoutTimer;

		private sealed class DependencyList : List<SqlDependency>
		{
			internal DependencyList(string commandHash)
			{
				this.CommandHash = commandHash;
			}

			public readonly string CommandHash;
		}
	}
}
