using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Runtime
{
	internal class SynchronizedPool<T> where T : class
	{
		public SynchronizedPool(int maxCount)
		{
			int num = maxCount;
			int num2 = 16 + SynchronizedPool<T>.SynchronizedPoolHelper.ProcessorCount;
			if (num > num2)
			{
				num = num2;
			}
			this.maxCount = maxCount;
			this.entries = new SynchronizedPool<T>.Entry[num];
			this.pending = new SynchronizedPool<T>.PendingEntry[4];
			this.globalPool = new SynchronizedPool<T>.GlobalPool(maxCount);
		}

		private object ThisLock
		{
			get
			{
				return this;
			}
		}

		public void Clear()
		{
			SynchronizedPool<T>.Entry[] array = this.entries;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].value = default(T);
			}
			this.globalPool.Clear();
		}

		private void HandlePromotionFailure(int thisThreadID)
		{
			int num = this.promotionFailures + 1;
			if (num >= 64)
			{
				object thisLock = this.ThisLock;
				lock (thisLock)
				{
					this.entries = new SynchronizedPool<T>.Entry[this.entries.Length];
					this.globalPool.MaxCount = this.maxCount;
				}
				this.PromoteThread(thisThreadID);
				return;
			}
			this.promotionFailures = num;
		}

		private bool PromoteThread(int thisThreadID)
		{
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				for (int i = 0; i < this.entries.Length; i++)
				{
					int threadID = this.entries[i].threadID;
					if (threadID == thisThreadID)
					{
						return true;
					}
					if (threadID == 0)
					{
						this.globalPool.DecrementMaxCount();
						this.entries[i].threadID = thisThreadID;
						return true;
					}
				}
			}
			return false;
		}

		private void RecordReturnToGlobalPool(int thisThreadID)
		{
			SynchronizedPool<T>.PendingEntry[] array = this.pending;
			int i = 0;
			while (i < array.Length)
			{
				int threadID = array[i].threadID;
				if (threadID == thisThreadID)
				{
					int num = array[i].returnCount + 1;
					if (num < 64)
					{
						array[i].returnCount = num;
						return;
					}
					array[i].returnCount = 0;
					if (!this.PromoteThread(thisThreadID))
					{
						this.HandlePromotionFailure(thisThreadID);
						return;
					}
					break;
				}
				else
				{
					if (threadID == 0)
					{
						break;
					}
					i++;
				}
			}
		}

		private void RecordTakeFromGlobalPool(int thisThreadID)
		{
			SynchronizedPool<T>.PendingEntry[] array = this.pending;
			for (int i = 0; i < array.Length; i++)
			{
				int threadID = array[i].threadID;
				if (threadID == thisThreadID)
				{
					return;
				}
				if (threadID == 0)
				{
					SynchronizedPool<T>.PendingEntry[] array2 = array;
					lock (array2)
					{
						if (array[i].threadID == 0)
						{
							array[i].threadID = thisThreadID;
							return;
						}
					}
				}
			}
			if (array.Length >= 128)
			{
				this.pending = new SynchronizedPool<T>.PendingEntry[array.Length];
				return;
			}
			SynchronizedPool<T>.PendingEntry[] array3 = new SynchronizedPool<T>.PendingEntry[array.Length * 2];
			Array.Copy(array, array3, array.Length);
			this.pending = array3;
		}

		public bool Return(T value)
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			return managedThreadId != 0 && (this.ReturnToPerThreadPool(managedThreadId, value) || this.ReturnToGlobalPool(managedThreadId, value));
		}

		private bool ReturnToPerThreadPool(int thisThreadID, T value)
		{
			SynchronizedPool<T>.Entry[] array = this.entries;
			int i = 0;
			while (i < array.Length)
			{
				int threadID = array[i].threadID;
				if (threadID == thisThreadID)
				{
					if (array[i].value == null)
					{
						array[i].value = value;
						return true;
					}
					return false;
				}
				else
				{
					if (threadID == 0)
					{
						break;
					}
					i++;
				}
			}
			return false;
		}

		private bool ReturnToGlobalPool(int thisThreadID, T value)
		{
			this.RecordReturnToGlobalPool(thisThreadID);
			return this.globalPool.Return(value);
		}

		public T Take()
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			if (managedThreadId == 0)
			{
				return default(T);
			}
			T t = this.TakeFromPerThreadPool(managedThreadId);
			if (t != null)
			{
				return t;
			}
			return this.TakeFromGlobalPool(managedThreadId);
		}

		private T TakeFromPerThreadPool(int thisThreadID)
		{
			SynchronizedPool<T>.Entry[] array = this.entries;
			int i = 0;
			while (i < array.Length)
			{
				int threadID = array[i].threadID;
				if (threadID == thisThreadID)
				{
					T value = array[i].value;
					if (value != null)
					{
						array[i].value = default(T);
						return value;
					}
					return default(T);
				}
				else
				{
					if (threadID == 0)
					{
						break;
					}
					i++;
				}
			}
			return default(T);
		}

		private T TakeFromGlobalPool(int thisThreadID)
		{
			this.RecordTakeFromGlobalPool(thisThreadID);
			return this.globalPool.Take();
		}

		private const int maxPendingEntries = 128;

		private const int maxPromotionFailures = 64;

		private const int maxReturnsBeforePromotion = 64;

		private const int maxThreadItemsPerProcessor = 16;

		private SynchronizedPool<T>.Entry[] entries;

		private SynchronizedPool<T>.GlobalPool globalPool;

		private int maxCount;

		private SynchronizedPool<T>.PendingEntry[] pending;

		private int promotionFailures;

		private struct Entry
		{
			public int threadID;

			public T value;
		}

		private struct PendingEntry
		{
			public int returnCount;

			public int threadID;
		}

		private static class SynchronizedPoolHelper
		{
			[SecuritySafeCritical]
			[EnvironmentPermission(SecurityAction.Assert, Read = "NUMBER_OF_PROCESSORS")]
			private static int GetProcessorCount()
			{
				return Environment.ProcessorCount;
			}

			public static readonly int ProcessorCount = SynchronizedPool<T>.SynchronizedPoolHelper.GetProcessorCount();
		}

		private class GlobalPool
		{
			public GlobalPool(int maxCount)
			{
				this.items = new Stack<T>();
				this.maxCount = maxCount;
			}

			public int MaxCount
			{
				get
				{
					return this.maxCount;
				}
				set
				{
					object thisLock = this.ThisLock;
					lock (thisLock)
					{
						while (this.items.Count > value)
						{
							this.items.Pop();
						}
						this.maxCount = value;
					}
				}
			}

			private object ThisLock
			{
				get
				{
					return this;
				}
			}

			public void DecrementMaxCount()
			{
				object thisLock = this.ThisLock;
				lock (thisLock)
				{
					if (this.items.Count == this.maxCount)
					{
						this.items.Pop();
					}
					this.maxCount--;
				}
			}

			public T Take()
			{
				if (this.items.Count > 0)
				{
					object thisLock = this.ThisLock;
					lock (thisLock)
					{
						if (this.items.Count > 0)
						{
							return this.items.Pop();
						}
					}
				}
				return default(T);
			}

			public bool Return(T value)
			{
				if (this.items.Count < this.MaxCount)
				{
					object thisLock = this.ThisLock;
					lock (thisLock)
					{
						if (this.items.Count < this.MaxCount)
						{
							this.items.Push(value);
							return true;
						}
					}
					return false;
				}
				return false;
			}

			public void Clear()
			{
				object thisLock = this.ThisLock;
				lock (thisLock)
				{
					this.items.Clear();
				}
			}

			private Stack<T> items;

			private int maxCount;
		}
	}
}
