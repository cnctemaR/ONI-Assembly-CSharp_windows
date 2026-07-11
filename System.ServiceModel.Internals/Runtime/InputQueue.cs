using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Runtime
{
	internal sealed class InputQueue<T> : IDisposable where T : class
	{
		public InputQueue()
		{
			this.itemQueue = new InputQueue<T>.ItemQueue();
			this.readerQueue = new Queue<InputQueue<T>.IQueueReader>();
			this.waiterList = new List<InputQueue<T>.IQueueWaiter>();
			this.queueState = InputQueue<T>.QueueState.Open;
		}

		public InputQueue(Func<Action<AsyncCallback, IAsyncResult>> asyncCallbackGenerator)
			: this()
		{
			this.AsyncCallbackGenerator = asyncCallbackGenerator;
		}

		public int PendingCount
		{
			get
			{
				object thisLock = this.ThisLock;
				int itemCount;
				lock (thisLock)
				{
					itemCount = this.itemQueue.ItemCount;
				}
				return itemCount;
			}
		}

		public Action<T> DisposeItemCallback { get; set; }

		private Func<Action<AsyncCallback, IAsyncResult>> AsyncCallbackGenerator { get; set; }

		private object ThisLock
		{
			get
			{
				return this.itemQueue;
			}
		}

		public IAsyncResult BeginDequeue(TimeSpan timeout, AsyncCallback callback, object state)
		{
			InputQueue<T>.Item item = default(InputQueue<T>.Item);
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState == InputQueue<T>.QueueState.Open)
				{
					if (!this.itemQueue.HasAvailableItem)
					{
						InputQueue<T>.AsyncQueueReader asyncQueueReader = new InputQueue<T>.AsyncQueueReader(this, timeout, callback, state);
						this.readerQueue.Enqueue(asyncQueueReader);
						return asyncQueueReader;
					}
					item = this.itemQueue.DequeueAvailableItem();
				}
				else if (this.queueState == InputQueue<T>.QueueState.Shutdown)
				{
					if (this.itemQueue.HasAvailableItem)
					{
						item = this.itemQueue.DequeueAvailableItem();
					}
					else if (this.itemQueue.HasAnyItem)
					{
						InputQueue<T>.AsyncQueueReader asyncQueueReader2 = new InputQueue<T>.AsyncQueueReader(this, timeout, callback, state);
						this.readerQueue.Enqueue(asyncQueueReader2);
						return asyncQueueReader2;
					}
				}
			}
			InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
			return new CompletedAsyncResult<T>(item.GetValue(), callback, state);
		}

		public IAsyncResult BeginWaitForItem(TimeSpan timeout, AsyncCallback callback, object state)
		{
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState == InputQueue<T>.QueueState.Open)
				{
					if (!this.itemQueue.HasAvailableItem)
					{
						InputQueue<T>.AsyncQueueWaiter asyncQueueWaiter = new InputQueue<T>.AsyncQueueWaiter(timeout, callback, state);
						this.waiterList.Add(asyncQueueWaiter);
						return asyncQueueWaiter;
					}
				}
				else if (this.queueState == InputQueue<T>.QueueState.Shutdown && !this.itemQueue.HasAvailableItem && this.itemQueue.HasAnyItem)
				{
					InputQueue<T>.AsyncQueueWaiter asyncQueueWaiter2 = new InputQueue<T>.AsyncQueueWaiter(timeout, callback, state);
					this.waiterList.Add(asyncQueueWaiter2);
					return asyncQueueWaiter2;
				}
			}
			return new CompletedAsyncResult<bool>(true, callback, state);
		}

		public void Close()
		{
			this.Dispose();
		}

		public T Dequeue(TimeSpan timeout)
		{
			T t;
			if (!this.Dequeue(timeout, out t))
			{
				throw Fx.Exception.AsError(new TimeoutException(InternalSR.TimeoutInputQueueDequeue(timeout)));
			}
			return t;
		}

		public bool Dequeue(TimeSpan timeout, out T value)
		{
			InputQueue<T>.WaitQueueReader waitQueueReader = null;
			InputQueue<T>.Item item = default(InputQueue<T>.Item);
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState == InputQueue<T>.QueueState.Open)
				{
					if (this.itemQueue.HasAvailableItem)
					{
						item = this.itemQueue.DequeueAvailableItem();
					}
					else
					{
						waitQueueReader = new InputQueue<T>.WaitQueueReader(this);
						this.readerQueue.Enqueue(waitQueueReader);
					}
				}
				else
				{
					if (this.queueState != InputQueue<T>.QueueState.Shutdown)
					{
						value = default(T);
						return true;
					}
					if (this.itemQueue.HasAvailableItem)
					{
						item = this.itemQueue.DequeueAvailableItem();
					}
					else
					{
						if (!this.itemQueue.HasAnyItem)
						{
							value = default(T);
							return true;
						}
						waitQueueReader = new InputQueue<T>.WaitQueueReader(this);
						this.readerQueue.Enqueue(waitQueueReader);
					}
				}
			}
			if (waitQueueReader != null)
			{
				return waitQueueReader.Wait(timeout, out value);
			}
			InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
			value = item.GetValue();
			return true;
		}

		public void Dispatch()
		{
			InputQueue<T>.IQueueReader queueReader = null;
			InputQueue<T>.Item item = default(InputQueue<T>.Item);
			InputQueue<T>.IQueueReader[] array = null;
			InputQueue<T>.IQueueWaiter[] array2 = null;
			bool flag = true;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				flag = this.queueState != InputQueue<T>.QueueState.Closed && this.queueState != InputQueue<T>.QueueState.Shutdown;
				this.GetWaiters(out array2);
				if (this.queueState != InputQueue<T>.QueueState.Closed)
				{
					this.itemQueue.MakePendingItemAvailable();
					if (this.readerQueue.Count > 0)
					{
						item = this.itemQueue.DequeueAvailableItem();
						queueReader = this.readerQueue.Dequeue();
						if (this.queueState == InputQueue<T>.QueueState.Shutdown && this.readerQueue.Count > 0 && this.itemQueue.ItemCount == 0)
						{
							array = new InputQueue<T>.IQueueReader[this.readerQueue.Count];
							this.readerQueue.CopyTo(array, 0);
							this.readerQueue.Clear();
							flag = false;
						}
					}
				}
			}
			if (array != null)
			{
				if (InputQueue<T>.completeOutstandingReadersCallback == null)
				{
					InputQueue<T>.completeOutstandingReadersCallback = new Action<object>(InputQueue<T>.CompleteOutstandingReadersCallback);
				}
				ActionItem.Schedule(InputQueue<T>.completeOutstandingReadersCallback, array);
			}
			if (array2 != null)
			{
				InputQueue<T>.CompleteWaitersLater(flag, array2);
			}
			if (queueReader != null)
			{
				InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
				queueReader.Set(item);
			}
		}

		public bool EndDequeue(IAsyncResult result, out T value)
		{
			if (result is CompletedAsyncResult<T>)
			{
				value = CompletedAsyncResult<T>.End(result);
				return true;
			}
			return InputQueue<T>.AsyncQueueReader.End(result, out value);
		}

		public T EndDequeue(IAsyncResult result)
		{
			T t;
			if (!this.EndDequeue(result, out t))
			{
				throw Fx.Exception.AsError(new TimeoutException());
			}
			return t;
		}

		public bool EndWaitForItem(IAsyncResult result)
		{
			if (result is CompletedAsyncResult<bool>)
			{
				return CompletedAsyncResult<bool>.End(result);
			}
			return InputQueue<T>.AsyncQueueWaiter.End(result);
		}

		public void EnqueueAndDispatch(T item)
		{
			this.EnqueueAndDispatch(item, null);
		}

		public void EnqueueAndDispatch(T item, Action dequeuedCallback)
		{
			this.EnqueueAndDispatch(item, dequeuedCallback, true);
		}

		public void EnqueueAndDispatch(Exception exception, Action dequeuedCallback, bool canDispatchOnThisThread)
		{
			this.EnqueueAndDispatch(new InputQueue<T>.Item(exception, dequeuedCallback), canDispatchOnThisThread);
		}

		public void EnqueueAndDispatch(T item, Action dequeuedCallback, bool canDispatchOnThisThread)
		{
			this.EnqueueAndDispatch(new InputQueue<T>.Item(item, dequeuedCallback), canDispatchOnThisThread);
		}

		public bool EnqueueWithoutDispatch(T item, Action dequeuedCallback)
		{
			return this.EnqueueWithoutDispatch(new InputQueue<T>.Item(item, dequeuedCallback));
		}

		public bool EnqueueWithoutDispatch(Exception exception, Action dequeuedCallback)
		{
			return this.EnqueueWithoutDispatch(new InputQueue<T>.Item(exception, dequeuedCallback));
		}

		public void Shutdown()
		{
			this.Shutdown(null);
		}

		public void Shutdown(Func<Exception> pendingExceptionGenerator)
		{
			InputQueue<T>.IQueueReader[] array = null;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState == InputQueue<T>.QueueState.Shutdown)
				{
					return;
				}
				if (this.queueState == InputQueue<T>.QueueState.Closed)
				{
					return;
				}
				this.queueState = InputQueue<T>.QueueState.Shutdown;
				if (this.readerQueue.Count > 0 && this.itemQueue.ItemCount == 0)
				{
					array = new InputQueue<T>.IQueueReader[this.readerQueue.Count];
					this.readerQueue.CopyTo(array, 0);
					this.readerQueue.Clear();
				}
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					Exception ex = ((pendingExceptionGenerator != null) ? pendingExceptionGenerator() : null);
					array[i].Set(new InputQueue<T>.Item(ex, null));
				}
			}
		}

		public bool WaitForItem(TimeSpan timeout)
		{
			InputQueue<T>.WaitQueueWaiter waitQueueWaiter = null;
			bool flag = false;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState == InputQueue<T>.QueueState.Open)
				{
					if (this.itemQueue.HasAvailableItem)
					{
						flag = true;
					}
					else
					{
						waitQueueWaiter = new InputQueue<T>.WaitQueueWaiter();
						this.waiterList.Add(waitQueueWaiter);
					}
				}
				else
				{
					if (this.queueState != InputQueue<T>.QueueState.Shutdown)
					{
						return true;
					}
					if (this.itemQueue.HasAvailableItem)
					{
						flag = true;
					}
					else
					{
						if (!this.itemQueue.HasAnyItem)
						{
							return true;
						}
						waitQueueWaiter = new InputQueue<T>.WaitQueueWaiter();
						this.waiterList.Add(waitQueueWaiter);
					}
				}
			}
			if (waitQueueWaiter != null)
			{
				return waitQueueWaiter.Wait(timeout);
			}
			return flag;
		}

		public void Dispose()
		{
			bool flag = false;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState != InputQueue<T>.QueueState.Closed)
				{
					this.queueState = InputQueue<T>.QueueState.Closed;
					flag = true;
				}
			}
			if (flag)
			{
				while (this.readerQueue.Count > 0)
				{
					this.readerQueue.Dequeue().Set(default(InputQueue<T>.Item));
				}
				while (this.itemQueue.HasAnyItem)
				{
					InputQueue<T>.Item item = this.itemQueue.DequeueAnyItem();
					this.DisposeItem(item);
					InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
				}
			}
		}

		private void DisposeItem(InputQueue<T>.Item item)
		{
			T value = item.Value;
			if (value != null)
			{
				if (value is IDisposable)
				{
					((IDisposable)((object)value)).Dispose();
					return;
				}
				Action<T> disposeItemCallback = this.DisposeItemCallback;
				if (disposeItemCallback != null)
				{
					disposeItemCallback(value);
				}
			}
		}

		private static void CompleteOutstandingReadersCallback(object state)
		{
			InputQueue<T>.IQueueReader[] array = (InputQueue<T>.IQueueReader[])state;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Set(default(InputQueue<T>.Item));
			}
		}

		private static void CompleteWaiters(bool itemAvailable, InputQueue<T>.IQueueWaiter[] waiters)
		{
			for (int i = 0; i < waiters.Length; i++)
			{
				waiters[i].Set(itemAvailable);
			}
		}

		private static void CompleteWaitersFalseCallback(object state)
		{
			InputQueue<T>.CompleteWaiters(false, (InputQueue<T>.IQueueWaiter[])state);
		}

		private static void CompleteWaitersLater(bool itemAvailable, InputQueue<T>.IQueueWaiter[] waiters)
		{
			if (itemAvailable)
			{
				if (InputQueue<T>.completeWaitersTrueCallback == null)
				{
					InputQueue<T>.completeWaitersTrueCallback = new Action<object>(InputQueue<T>.CompleteWaitersTrueCallback);
				}
				ActionItem.Schedule(InputQueue<T>.completeWaitersTrueCallback, waiters);
				return;
			}
			if (InputQueue<T>.completeWaitersFalseCallback == null)
			{
				InputQueue<T>.completeWaitersFalseCallback = new Action<object>(InputQueue<T>.CompleteWaitersFalseCallback);
			}
			ActionItem.Schedule(InputQueue<T>.completeWaitersFalseCallback, waiters);
		}

		private static void CompleteWaitersTrueCallback(object state)
		{
			InputQueue<T>.CompleteWaiters(true, (InputQueue<T>.IQueueWaiter[])state);
		}

		private static void InvokeDequeuedCallback(Action dequeuedCallback)
		{
			if (dequeuedCallback != null)
			{
				dequeuedCallback();
			}
		}

		private static void InvokeDequeuedCallbackLater(Action dequeuedCallback)
		{
			if (dequeuedCallback != null)
			{
				if (InputQueue<T>.onInvokeDequeuedCallback == null)
				{
					InputQueue<T>.onInvokeDequeuedCallback = new Action<object>(InputQueue<T>.OnInvokeDequeuedCallback);
				}
				ActionItem.Schedule(InputQueue<T>.onInvokeDequeuedCallback, dequeuedCallback);
			}
		}

		private static void OnDispatchCallback(object state)
		{
			((InputQueue<T>)state).Dispatch();
		}

		private static void OnInvokeDequeuedCallback(object state)
		{
			((Action)state)();
		}

		private void EnqueueAndDispatch(InputQueue<T>.Item item, bool canDispatchOnThisThread)
		{
			bool flag = false;
			InputQueue<T>.IQueueReader queueReader = null;
			bool flag2 = false;
			InputQueue<T>.IQueueWaiter[] array = null;
			bool flag3 = true;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				flag3 = this.queueState != InputQueue<T>.QueueState.Closed && this.queueState != InputQueue<T>.QueueState.Shutdown;
				this.GetWaiters(out array);
				if (this.queueState == InputQueue<T>.QueueState.Open)
				{
					if (canDispatchOnThisThread)
					{
						if (this.readerQueue.Count == 0)
						{
							this.itemQueue.EnqueueAvailableItem(item);
						}
						else
						{
							queueReader = this.readerQueue.Dequeue();
						}
					}
					else if (this.readerQueue.Count == 0)
					{
						this.itemQueue.EnqueueAvailableItem(item);
					}
					else
					{
						this.itemQueue.EnqueuePendingItem(item);
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (array != null)
			{
				if (canDispatchOnThisThread)
				{
					InputQueue<T>.CompleteWaiters(flag3, array);
				}
				else
				{
					InputQueue<T>.CompleteWaitersLater(flag3, array);
				}
			}
			if (queueReader != null)
			{
				InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
				queueReader.Set(item);
			}
			if (flag2)
			{
				if (InputQueue<T>.onDispatchCallback == null)
				{
					InputQueue<T>.onDispatchCallback = new Action<object>(InputQueue<T>.OnDispatchCallback);
				}
				ActionItem.Schedule(InputQueue<T>.onDispatchCallback, this);
				return;
			}
			if (flag)
			{
				InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
				this.DisposeItem(item);
			}
		}

		private bool EnqueueWithoutDispatch(InputQueue<T>.Item item)
		{
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState != InputQueue<T>.QueueState.Closed && this.queueState != InputQueue<T>.QueueState.Shutdown)
				{
					if (this.readerQueue.Count == 0 && this.waiterList.Count == 0)
					{
						this.itemQueue.EnqueueAvailableItem(item);
						return false;
					}
					this.itemQueue.EnqueuePendingItem(item);
					return true;
				}
			}
			this.DisposeItem(item);
			InputQueue<T>.InvokeDequeuedCallbackLater(item.DequeuedCallback);
			return false;
		}

		private void GetWaiters(out InputQueue<T>.IQueueWaiter[] waiters)
		{
			if (this.waiterList.Count > 0)
			{
				waiters = this.waiterList.ToArray();
				this.waiterList.Clear();
				return;
			}
			waiters = null;
		}

		private bool RemoveReader(InputQueue<T>.IQueueReader reader)
		{
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.queueState == InputQueue<T>.QueueState.Open || this.queueState == InputQueue<T>.QueueState.Shutdown)
				{
					bool flag2 = false;
					for (int i = this.readerQueue.Count; i > 0; i--)
					{
						InputQueue<T>.IQueueReader queueReader = this.readerQueue.Dequeue();
						if (queueReader == reader)
						{
							flag2 = true;
						}
						else
						{
							this.readerQueue.Enqueue(queueReader);
						}
					}
					return flag2;
				}
			}
			return false;
		}

		private static Action<object> completeOutstandingReadersCallback;

		private static Action<object> completeWaitersFalseCallback;

		private static Action<object> completeWaitersTrueCallback;

		private static Action<object> onDispatchCallback;

		private static Action<object> onInvokeDequeuedCallback;

		private InputQueue<T>.QueueState queueState;

		private InputQueue<T>.ItemQueue itemQueue;

		private Queue<InputQueue<T>.IQueueReader> readerQueue;

		private List<InputQueue<T>.IQueueWaiter> waiterList;

		private enum QueueState
		{
			Open,
			Shutdown,
			Closed
		}

		private interface IQueueReader
		{
			void Set(InputQueue<T>.Item item);
		}

		private interface IQueueWaiter
		{
			void Set(bool itemAvailable);
		}

		private struct Item
		{
			public Item(T value, Action dequeuedCallback)
			{
				this = new InputQueue<T>.Item(value, null, dequeuedCallback);
			}

			public Item(Exception exception, Action dequeuedCallback)
			{
				this = new InputQueue<T>.Item(default(T), exception, dequeuedCallback);
			}

			private Item(T value, Exception exception, Action dequeuedCallback)
			{
				this.value = value;
				this.exception = exception;
				this.dequeuedCallback = dequeuedCallback;
			}

			public Action DequeuedCallback
			{
				get
				{
					return this.dequeuedCallback;
				}
			}

			public Exception Exception
			{
				get
				{
					return this.exception;
				}
			}

			public T Value
			{
				get
				{
					return this.value;
				}
			}

			public T GetValue()
			{
				if (this.exception != null)
				{
					throw Fx.Exception.AsError(this.exception);
				}
				return this.value;
			}

			private Action dequeuedCallback;

			private Exception exception;

			private T value;
		}

		private class AsyncQueueReader : AsyncResult, InputQueue<T>.IQueueReader
		{
			public AsyncQueueReader(InputQueue<T> inputQueue, TimeSpan timeout, AsyncCallback callback, object state)
				: base(callback, state)
			{
				if (inputQueue.AsyncCallbackGenerator != null)
				{
					base.VirtualCallback = inputQueue.AsyncCallbackGenerator();
				}
				this.inputQueue = inputQueue;
				if (timeout != TimeSpan.MaxValue)
				{
					this.timer = new IOThreadTimer(InputQueue<T>.AsyncQueueReader.timerCallback, this, false);
					this.timer.Set(timeout);
				}
			}

			public static bool End(IAsyncResult result, out T value)
			{
				InputQueue<T>.AsyncQueueReader asyncQueueReader = AsyncResult.End<InputQueue<T>.AsyncQueueReader>(result);
				if (asyncQueueReader.expired)
				{
					value = default(T);
					return false;
				}
				value = asyncQueueReader.item;
				return true;
			}

			public void Set(InputQueue<T>.Item item)
			{
				this.item = item.Value;
				if (this.timer != null)
				{
					this.timer.Cancel();
				}
				base.Complete(false, item.Exception);
			}

			private static void TimerCallback(object state)
			{
				InputQueue<T>.AsyncQueueReader asyncQueueReader = (InputQueue<T>.AsyncQueueReader)state;
				if (asyncQueueReader.inputQueue.RemoveReader(asyncQueueReader))
				{
					asyncQueueReader.expired = true;
					asyncQueueReader.Complete(false);
				}
			}

			private static Action<object> timerCallback = new Action<object>(InputQueue<T>.AsyncQueueReader.TimerCallback);

			private bool expired;

			private InputQueue<T> inputQueue;

			private T item;

			private IOThreadTimer timer;
		}

		private class AsyncQueueWaiter : AsyncResult, InputQueue<T>.IQueueWaiter
		{
			public AsyncQueueWaiter(TimeSpan timeout, AsyncCallback callback, object state)
				: base(callback, state)
			{
				if (timeout != TimeSpan.MaxValue)
				{
					this.timer = new IOThreadTimer(InputQueue<T>.AsyncQueueWaiter.timerCallback, this, false);
					this.timer.Set(timeout);
				}
			}

			private object ThisLock
			{
				get
				{
					return this.thisLock;
				}
			}

			public static bool End(IAsyncResult result)
			{
				return AsyncResult.End<InputQueue<T>.AsyncQueueWaiter>(result).itemAvailable;
			}

			public void Set(bool itemAvailable)
			{
				object obj = this.ThisLock;
				bool flag2;
				lock (obj)
				{
					flag2 = this.timer == null || this.timer.Cancel();
					this.itemAvailable = itemAvailable;
				}
				if (flag2)
				{
					base.Complete(false);
				}
			}

			private static void TimerCallback(object state)
			{
				((InputQueue<T>.AsyncQueueWaiter)state).Complete(false);
			}

			private static Action<object> timerCallback = new Action<object>(InputQueue<T>.AsyncQueueWaiter.TimerCallback);

			private bool itemAvailable;

			private object thisLock = new object();

			private IOThreadTimer timer;
		}

		private class ItemQueue
		{
			public ItemQueue()
			{
				this.items = new InputQueue<T>.Item[1];
			}

			public bool HasAnyItem
			{
				get
				{
					return this.totalCount > 0;
				}
			}

			public bool HasAvailableItem
			{
				get
				{
					return this.totalCount > this.pendingCount;
				}
			}

			public int ItemCount
			{
				get
				{
					return this.totalCount;
				}
			}

			public InputQueue<T>.Item DequeueAnyItem()
			{
				if (this.pendingCount == this.totalCount)
				{
					this.pendingCount--;
				}
				return this.DequeueItemCore();
			}

			public InputQueue<T>.Item DequeueAvailableItem()
			{
				Fx.AssertAndThrow(this.totalCount != this.pendingCount, "ItemQueue does not contain any available items");
				return this.DequeueItemCore();
			}

			public void EnqueueAvailableItem(InputQueue<T>.Item item)
			{
				this.EnqueueItemCore(item);
			}

			public void EnqueuePendingItem(InputQueue<T>.Item item)
			{
				this.EnqueueItemCore(item);
				this.pendingCount++;
			}

			public void MakePendingItemAvailable()
			{
				Fx.AssertAndThrow(this.pendingCount != 0, "ItemQueue does not contain any pending items");
				this.pendingCount--;
			}

			private InputQueue<T>.Item DequeueItemCore()
			{
				Fx.AssertAndThrow(this.totalCount != 0, "ItemQueue does not contain any items");
				InputQueue<T>.Item item = this.items[this.head];
				this.items[this.head] = default(InputQueue<T>.Item);
				this.totalCount--;
				this.head = (this.head + 1) % this.items.Length;
				return item;
			}

			private void EnqueueItemCore(InputQueue<T>.Item item)
			{
				if (this.totalCount == this.items.Length)
				{
					InputQueue<T>.Item[] array = new InputQueue<T>.Item[this.items.Length * 2];
					for (int i = 0; i < this.totalCount; i++)
					{
						array[i] = this.items[(this.head + i) % this.items.Length];
					}
					this.head = 0;
					this.items = array;
				}
				int num = (this.head + this.totalCount) % this.items.Length;
				this.items[num] = item;
				this.totalCount++;
			}

			private int head;

			private InputQueue<T>.Item[] items;

			private int pendingCount;

			private int totalCount;
		}

		private class WaitQueueReader : InputQueue<T>.IQueueReader
		{
			public WaitQueueReader(InputQueue<T> inputQueue)
			{
				this.inputQueue = inputQueue;
				this.waitEvent = new ManualResetEvent(false);
			}

			public void Set(InputQueue<T>.Item item)
			{
				lock (this)
				{
					this.exception = item.Exception;
					this.item = item.Value;
					this.waitEvent.Set();
				}
			}

			public bool Wait(TimeSpan timeout, out T value)
			{
				bool flag = false;
				try
				{
					if (!TimeoutHelper.WaitOne(this.waitEvent, timeout))
					{
						if (this.inputQueue.RemoveReader(this))
						{
							value = default(T);
							flag = true;
							return false;
						}
						this.waitEvent.WaitOne();
					}
					flag = true;
				}
				finally
				{
					if (flag)
					{
						this.waitEvent.Close();
					}
				}
				if (this.exception != null)
				{
					throw Fx.Exception.AsError(this.exception);
				}
				value = this.item;
				return true;
			}

			private Exception exception;

			private InputQueue<T> inputQueue;

			private T item;

			private ManualResetEvent waitEvent;
		}

		private class WaitQueueWaiter : InputQueue<T>.IQueueWaiter
		{
			public WaitQueueWaiter()
			{
				this.waitEvent = new ManualResetEvent(false);
			}

			public void Set(bool itemAvailable)
			{
				lock (this)
				{
					this.itemAvailable = itemAvailable;
					this.waitEvent.Set();
				}
			}

			public bool Wait(TimeSpan timeout)
			{
				return TimeoutHelper.WaitOne(this.waitEvent, timeout) && this.itemAvailable;
			}

			private bool itemAvailable;

			private ManualResetEvent waitEvent;
		}
	}
}
