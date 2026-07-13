using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Mono/DelayedCallAwaitable.h")]
	[AsyncMethodBuilder(typeof(Awaitable.AwaitableAsyncMethodBuilder))]
	[NativeHeader("Runtime/Mono/Awaitable.h")]
	[NativeHeader("Runtime/Mono/AsyncOperationAwaitable.h")]
	public class Awaitable : IEnumerator
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Awaitable FromAsyncOperation(AsyncOperation op, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			IntPtr intPtr = Awaitable.FromAsyncOperationInternal(op.m_Ptr);
			return Awaitable.FromNativeAwaitableHandle(intPtr, cancellationToken);
		}

		[FreeFunction("Scripting::Awaitables::FromAsyncOperation", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FromAsyncOperationInternal(IntPtr asyncOperation);

		[ExcludeFromDocs]
		public Awaitable.Awaiter GetAwaiter()
		{
			return new Awaitable.Awaiter(this);
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private void SetExceptionFromNative(Exception ex)
		{
			bool flag = false;
			try
			{
				this._spinLock.Enter(ref flag);
				this._exceptionToRethrow = ExceptionDispatchInfo.Capture(ex);
			}
			finally
			{
				bool flag2 = flag;
				if (flag2)
				{
					this._spinLock.Exit();
				}
			}
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private void RunContinuation()
		{
			Action action = null;
			bool flag = false;
			try
			{
				this._spinLock.Enter(ref flag);
				action = this._continuation;
				this._continuation = null;
			}
			finally
			{
				bool flag2 = flag;
				if (flag2)
				{
					this._spinLock.Exit();
				}
			}
			if (action != null)
			{
				action();
			}
		}

		[FreeFunction("Scripting::Awaitables::AttachManagedWrapper", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AttachManagedGCHandleToNativeAwaitable(IntPtr nativeAwaitable, UIntPtr gcHandle);

		[FreeFunction("Scripting::Awaitables::Release", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseNativeAwaitable(IntPtr nativeAwaitable);

		[FreeFunction("Scripting::Awaitables::Cancel", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CancelNativeAwaitable(IntPtr nativeAwaitable);

		[FreeFunction("Scripting::Awaitables::IsCompleted", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IsNativeAwaitableCompleted(IntPtr nativeAwaitable);

		private Awaitable()
		{
		}

		internal static Awaitable NewManagedAwaitable()
		{
			Awaitable awaitable = Awaitable._pool.Value.Get();
			awaitable._handle = Awaitable.AwaitableHandle.ManagedHandle;
			return awaitable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static Awaitable FromNativeAwaitableHandle(IntPtr nativeHandle, CancellationToken cancellationToken)
		{
			Awaitable awaitable = Awaitable._pool.Value.Get();
			awaitable._handle = nativeHandle;
			Awaitable.AttachManagedGCHandleToNativeAwaitable(nativeHandle, (UIntPtr)((void*)GCHandle.ToIntPtr(GCHandle.Alloc(awaitable))));
			bool canBeCanceled = cancellationToken.CanBeCanceled;
			if (canBeCanceled)
			{
				Awaitable.WireupCancellation(awaitable, cancellationToken);
			}
			return awaitable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WireupCancellation(Awaitable awaitable, CancellationToken cancellationToken)
		{
			bool flag = awaitable == null;
			if (flag)
			{
				throw new ArgumentNullException("awaitable");
			}
			bool flag2 = false;
			try
			{
				awaitable._spinLock.Enter(ref flag2);
				using (ExecutionContext.SuppressFlow())
				{
					awaitable._cancelTokenRegistration = new CancellationTokenRegistration?(cancellationToken.Register(delegate(object coroutine)
					{
						((Awaitable)coroutine).Cancel();
					}, awaitable, false));
				}
			}
			finally
			{
				bool flag3 = flag2;
				if (flag3)
				{
					awaitable._spinLock.Exit();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool MatchCompletionThreadAffinity(Awaitable.AwaiterCompletionThreadAffinity awaiterCompletionThreadAffinity)
		{
			if (!true)
			{
			}
			bool flag;
			if (awaiterCompletionThreadAffinity != Awaitable.AwaiterCompletionThreadAffinity.MainThread)
			{
				flag = awaiterCompletionThreadAffinity != Awaitable.AwaiterCompletionThreadAffinity.BackgroundThread || Thread.CurrentThread.ManagedThreadId != Awaitable._mainThreadId;
			}
			else
			{
				flag = Thread.CurrentThread.ManagedThreadId == Awaitable._mainThreadId;
			}
			if (!true)
			{
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RaiseManagedCompletion(Exception exception)
		{
			Action action = null;
			bool flag = false;
			Awaitable.AwaiterCompletionThreadAffinity completionThreadAffinity;
			try
			{
				this._spinLock.Enter(ref flag);
				bool flag2 = exception != null;
				if (flag2)
				{
					this._exceptionToRethrow = ExceptionDispatchInfo.Capture(exception);
				}
				this._managedAwaitableDone = true;
				action = this._continuation;
				completionThreadAffinity = this._completionThreadAffinity;
				this._continuation = null;
			}
			finally
			{
				bool flag3 = flag;
				if (flag3)
				{
					this._spinLock.Exit();
				}
			}
			bool flag4 = action != null;
			if (flag4)
			{
				this.RunOrScheduleContinuation(completionThreadAffinity, action);
			}
		}

		private void RunOrScheduleContinuation(Awaitable.AwaiterCompletionThreadAffinity awaiterCompletionThreadAffinity, Action continuation)
		{
			bool flag = Awaitable.MatchCompletionThreadAffinity(awaiterCompletionThreadAffinity);
			if (flag)
			{
				continuation();
			}
			else
			{
				bool flag2 = awaiterCompletionThreadAffinity == Awaitable.AwaiterCompletionThreadAffinity.MainThread;
				if (flag2)
				{
					Awaitable._synchronizationContext.Post(new SendOrPostCallback(Awaitable.DoRunContinuationOnSynchonizationContext), continuation);
				}
				else
				{
					bool flag3 = awaiterCompletionThreadAffinity == Awaitable.AwaiterCompletionThreadAffinity.BackgroundThread;
					if (flag3)
					{
						Task.Run(continuation);
					}
				}
			}
		}

		private static void DoRunContinuationOnSynchonizationContext(object continuation)
		{
			Action action = continuation as Action;
			if (action != null)
			{
				action();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RaiseManagedCompletion()
		{
			Action action = null;
			bool flag = false;
			Awaitable.AwaiterCompletionThreadAffinity completionThreadAffinity;
			try
			{
				this._spinLock.Enter(ref flag);
				this._managedAwaitableDone = true;
				completionThreadAffinity = this._completionThreadAffinity;
				action = this._continuation;
				this._continuation = null;
				this._managedCompletionQueue = null;
			}
			finally
			{
				bool flag2 = flag;
				if (flag2)
				{
					this._spinLock.Exit();
				}
			}
			bool flag3 = action != null;
			if (flag3)
			{
				this.RunOrScheduleContinuation(completionThreadAffinity, action);
			}
		}

		internal void PropagateExceptionAndRelease()
		{
			bool flag = false;
			try
			{
				this._spinLock.Enter(ref flag);
				this.CheckPointerValidity();
				bool flag2 = this._cancelTokenRegistration != null;
				if (flag2)
				{
					this._cancelTokenRegistration.Value.Dispose();
					this._cancelTokenRegistration = null;
				}
				this._managedAwaitableDone = false;
				this._completionThreadAffinity = Awaitable.AwaiterCompletionThreadAffinity.None;
				Awaitable.AwaitableHandle handle = this._handle;
				this._handle = Awaitable.AwaitableHandle.NullHandle;
				ExceptionDispatchInfo exceptionToRethrow = this._exceptionToRethrow;
				this._exceptionToRethrow = null;
				this._managedCompletionQueue = null;
				this._continuation = null;
				bool flag3 = !handle.IsManaged && !handle.IsNull;
				if (flag3)
				{
					Awaitable.ReleaseNativeAwaitable(handle);
				}
				Awaitable._pool.Value.Release(this);
				if (exceptionToRethrow != null)
				{
					exceptionToRethrow.Throw();
				}
			}
			finally
			{
				bool flag4 = flag;
				if (flag4)
				{
					this._spinLock.Exit();
				}
			}
		}

		public void Cancel()
		{
			Awaitable.AwaitableHandle awaitableHandle = this.CheckPointerValidity();
			bool isManaged = awaitableHandle.IsManaged;
			if (isManaged)
			{
				Awaitable.DoubleBufferedAwaitableList managedCompletionQueue = this._managedCompletionQueue;
				if (managedCompletionQueue != null)
				{
					managedCompletionQueue.Remove(this);
				}
				this.RaiseManagedCompletion(new OperationCanceledException());
			}
			else
			{
				Awaitable.CancelNativeAwaitable(awaitableHandle);
			}
		}

		private bool IsCompletedNoLock
		{
			get
			{
				this.CheckPointerValidity();
				bool isManaged = this._handle.IsManaged;
				bool flag;
				if (isManaged)
				{
					flag = this._managedAwaitableDone && Awaitable.MatchCompletionThreadAffinity(this._completionThreadAffinity);
				}
				else
				{
					flag = Awaitable.IsNativeAwaitableCompleted(this._handle) != 0;
				}
				return flag;
			}
		}

		private bool IsLogicallyCompletedNoLock
		{
			get
			{
				this.CheckPointerValidity();
				bool isManaged = this._handle.IsManaged;
				bool flag;
				if (isManaged)
				{
					flag = this._managedAwaitableDone;
				}
				else
				{
					flag = Awaitable.IsNativeAwaitableCompleted(this._handle) != 0;
				}
				return flag;
			}
		}

		public bool IsCompleted
		{
			get
			{
				bool flag = false;
				bool isCompletedNoLock;
				try
				{
					this._spinLock.Enter(ref flag);
					isCompletedNoLock = this.IsCompletedNoLock;
				}
				finally
				{
					bool flag2 = flag;
					if (flag2)
					{
						this._spinLock.Exit();
					}
				}
				return isCompletedNoLock;
			}
		}

		internal bool IsDettachedOrCompleted
		{
			get
			{
				bool flag = false;
				bool flag2;
				try
				{
					this._spinLock.Enter(ref flag);
					bool isNull = this._handle.IsNull;
					if (isNull)
					{
						flag2 = true;
					}
					else
					{
						this.CheckPointerValidity();
						bool isManaged = this._handle.IsManaged;
						if (isManaged)
						{
							flag2 = this._managedAwaitableDone;
						}
						else
						{
							flag2 = Awaitable.IsNativeAwaitableCompleted(this._handle) != 0;
						}
					}
				}
				finally
				{
					bool flag3 = flag;
					if (flag3)
					{
						this._spinLock.Exit();
					}
				}
				return flag2;
			}
		}

		internal Awaitable.AwaiterCompletionThreadAffinity CompletionThreadAffinity
		{
			get
			{
				bool flag = false;
				Awaitable.AwaiterCompletionThreadAffinity completionThreadAffinity;
				try
				{
					this._spinLock.Enter(ref flag);
					completionThreadAffinity = this._completionThreadAffinity;
				}
				finally
				{
					bool flag2 = flag;
					if (flag2)
					{
						this._spinLock.Exit();
					}
				}
				return completionThreadAffinity;
			}
			set
			{
				bool flag = false;
				try
				{
					this._spinLock.Enter(ref flag);
					this._completionThreadAffinity = value;
				}
				finally
				{
					bool flag2 = flag;
					if (flag2)
					{
						this._spinLock.Exit();
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Awaitable.AwaitableHandle CheckPointerValidity()
		{
			Awaitable.AwaitableHandle handle = this._handle;
			bool isNull = handle.IsNull;
			if (isNull)
			{
				throw new InvalidOperationException("Awaitable is in detached state");
			}
			return handle;
		}

		internal void SetContinuation(Action continuation)
		{
			bool flag = false;
			bool flag2 = false;
			Awaitable.AwaiterCompletionThreadAffinity awaiterCompletionThreadAffinity = Awaitable.AwaiterCompletionThreadAffinity.None;
			try
			{
				this._spinLock.Enter(ref flag2);
				bool isLogicallyCompletedNoLock = this.IsLogicallyCompletedNoLock;
				if (isLogicallyCompletedNoLock)
				{
					flag = true;
					awaiterCompletionThreadAffinity = this._completionThreadAffinity;
				}
				else
				{
					this._continuation = continuation;
				}
			}
			finally
			{
				bool flag3 = flag2;
				if (flag3)
				{
					this._spinLock.Exit();
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				this.RunOrScheduleContinuation(awaiterCompletionThreadAffinity, continuation);
			}
		}

		bool IEnumerator.MoveNext()
		{
			bool isCompleted = this.IsCompleted;
			bool flag;
			if (isCompleted)
			{
				this.PropagateExceptionAndRelease();
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		void IEnumerator.Reset()
		{
		}

		object IEnumerator.Current
		{
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowIfNotMainThread()
		{
			bool flag = Thread.CurrentThread.ManagedThreadId != Awaitable._mainThreadId;
			if (flag)
			{
				throw new InvalidOperationException("This operation can only be performed on the main thread.");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Awaitable NextFrameAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Awaitable.ThrowIfNotMainThread();
			cancellationToken.ThrowIfCancellationRequested();
			Awaitable.EnsureDelayedCallWiredUp();
			Awaitable awaitable = Awaitable.NewManagedAwaitable();
			Awaitable._nextFrameAwaitables.Add(awaitable, Time.frameCount + 1);
			awaitable._managedCompletionQueue = Awaitable._nextFrameAwaitables;
			bool canBeCanceled = cancellationToken.CanBeCanceled;
			if (canBeCanceled)
			{
				Awaitable.WireupCancellation(awaitable, cancellationToken);
			}
			return awaitable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Awaitable WaitForSecondsAsync(float seconds, CancellationToken cancellationToken = default(CancellationToken))
		{
			Awaitable.ThrowIfNotMainThread();
			cancellationToken.ThrowIfCancellationRequested();
			IntPtr intPtr = Awaitable.WaitForScondsInternal(seconds);
			return Awaitable.FromNativeAwaitableHandle(intPtr, cancellationToken);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Awaitable FixedUpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Awaitable.ThrowIfNotMainThread();
			cancellationToken.ThrowIfCancellationRequested();
			IntPtr intPtr = Awaitable.FixedUpdateInternal();
			return Awaitable.FromNativeAwaitableHandle(intPtr, cancellationToken);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Awaitable EndOfFrameAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Awaitable.ThrowIfNotMainThread();
			cancellationToken.ThrowIfCancellationRequested();
			Awaitable.EnsureDelayedCallWiredUp();
			Awaitable awaitable = Awaitable.NewManagedAwaitable();
			Awaitable._endOfFrameAwaitables.Add(awaitable, -1);
			awaitable._managedCompletionQueue = Awaitable._endOfFrameAwaitables;
			bool canBeCanceled = cancellationToken.CanBeCanceled;
			if (canBeCanceled)
			{
				Awaitable.WireupCancellation(awaitable, cancellationToken);
			}
			return awaitable;
		}

		private static void EnsureDelayedCallWiredUp()
		{
			bool nextFrameAndEndOfFrameWiredUp = Awaitable._nextFrameAndEndOfFrameWiredUp;
			if (!nextFrameAndEndOfFrameWiredUp)
			{
				Awaitable._nextFrameAndEndOfFrameWiredUp = true;
				Awaitable.WireupNextFrameAndEndOfFrameCallbacks();
				Awaitable._nextFrameAndEndOfFrameWiredUpCTRegistration = Application.exitCancellationToken.Register(new Action(Awaitable.OnDelayedCallManagerCleared));
			}
		}

		[RequiredByNativeCode]
		private static void OnDelayedCallManagerCleared()
		{
			Awaitable._nextFrameAndEndOfFrameWiredUp = false;
			Awaitable._nextFrameAwaitables.Clear();
			Awaitable._endOfFrameAwaitables.Clear();
		}

		[RequiredByNativeCode]
		private static void OnUpdate()
		{
			Awaitable._nextFrameAwaitables.SwapAndComplete();
		}

		[RequiredByNativeCode]
		private static void OnEndOfFrame()
		{
			Awaitable._endOfFrameAwaitables.SwapAndComplete();
		}

		[FreeFunction("Scripting::Awaitables::NextFrameAwaitable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr NextFrameInternal();

		[FreeFunction("Scripting::Awaitables::WaitForSecondsAwaitable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr WaitForScondsInternal(float seconds);

		[FreeFunction("Scripting::Awaitables::FixedUpdateAwaitable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FixedUpdateInternal();

		[FreeFunction("Scripting::Awaitables::EndOfFrameAwaitable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr EndOfFrameInternal();

		[FreeFunction("Scripting::Awaitables::WireupNextFrameAndEndOfFrameCallbacks")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WireupNextFrameAndEndOfFrameCallbacks();

		internal static void SetSynchronizationContext(UnitySynchronizationContext synchronizationContext)
		{
			Awaitable._synchronizationContext = synchronizationContext;
			Awaitable._mainThreadId = synchronizationContext.MainThreadId;
		}

		public static MainThreadAwaitable MainThreadAsync()
		{
			return new MainThreadAwaitable(Awaitable._synchronizationContext, Awaitable._mainThreadId);
		}

		public static BackgroundThreadAwaitable BackgroundThreadAsync()
		{
			return new BackgroundThreadAwaitable(Awaitable._synchronizationContext, Awaitable._mainThreadId);
		}

		private SpinLock _spinLock = default(SpinLock);

		private static readonly ThreadLocal<ObjectPool<Awaitable>> _pool = new ThreadLocal<ObjectPool<Awaitable>>(() => new ObjectPool<Awaitable>(() => new Awaitable(), null, null, null, false, 10, 10000));

		private Awaitable.AwaitableHandle _handle;

		private ExceptionDispatchInfo _exceptionToRethrow;

		private bool _managedAwaitableDone;

		private Awaitable.AwaiterCompletionThreadAffinity _completionThreadAffinity;

		private Action _continuation;

		private CancellationTokenRegistration? _cancelTokenRegistration;

		private Awaitable.DoubleBufferedAwaitableList _managedCompletionQueue;

		private static bool _nextFrameAndEndOfFrameWiredUp = false;

		private static CancellationTokenRegistration _nextFrameAndEndOfFrameWiredUpCTRegistration = default(CancellationTokenRegistration);

		private static readonly Awaitable.DoubleBufferedAwaitableList _nextFrameAwaitables = new Awaitable.DoubleBufferedAwaitableList();

		private static readonly Awaitable.DoubleBufferedAwaitableList _endOfFrameAwaitables = new Awaitable.DoubleBufferedAwaitableList();

		private static SynchronizationContext _synchronizationContext;

		private static int _mainThreadId;

		internal enum AwaiterCompletionThreadAffinity
		{
			None,
			MainThread,
			BackgroundThread
		}

		[ExcludeFromDocs]
		public struct AwaitableAsyncMethodBuilder
		{
			private Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox EnsureStateMachineBox<TStateMachine>() where TStateMachine : IAsyncStateMachine
			{
				bool flag = this._stateMachineBox != null;
				Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox stateMachineBox;
				if (flag)
				{
					stateMachineBox = this._stateMachineBox;
				}
				else
				{
					this._stateMachineBox = Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>.GetOne();
					this._stateMachineBox.ResultingCoroutine = this._resultingCoroutine;
					stateMachineBox = this._stateMachineBox;
				}
				return stateMachineBox;
			}

			public static Awaitable.AwaitableAsyncMethodBuilder Create()
			{
				return default(Awaitable.AwaitableAsyncMethodBuilder);
			}

			public Awaitable Task
			{
				get
				{
					bool flag = this._resultingCoroutine != null;
					Awaitable awaitable;
					if (flag)
					{
						awaitable = this._resultingCoroutine;
					}
					else
					{
						bool flag2 = this._stateMachineBox != null;
						if (flag2)
						{
							Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox stateMachineBox = this._stateMachineBox;
							Awaitable awaitable2;
							if ((awaitable2 = stateMachineBox.ResultingCoroutine) == null)
							{
								awaitable2 = (stateMachineBox.ResultingCoroutine = Awaitable.NewManagedAwaitable());
							}
							awaitable = (this._resultingCoroutine = awaitable2);
						}
						else
						{
							awaitable = (this._resultingCoroutine = Awaitable.NewManagedAwaitable());
						}
					}
					return awaitable;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
			{
				Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox stateMachineBox = this.EnsureStateMachineBox<TStateMachine>();
				Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine> stateMachineBox2 = (Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>)stateMachineBox;
				this.Task.CompletionThreadAffinity = ((Thread.CurrentThread.ManagedThreadId == Awaitable._mainThreadId) ? Awaitable.AwaiterCompletionThreadAffinity.MainThread : Awaitable.AwaiterCompletionThreadAffinity.BackgroundThread);
				stateMachineBox2.StateMachine = stateMachine;
				stateMachineBox2.MoveNext();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
			{
				Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox stateMachineBox = this.EnsureStateMachineBox<TStateMachine>();
				Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine> stateMachineBox2 = (Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>)stateMachineBox;
				stateMachineBox2.StateMachine = stateMachine;
				awaiter.OnCompleted(stateMachineBox.MoveNext);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
			{
				Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox stateMachineBox = this.EnsureStateMachineBox<TStateMachine>();
				Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine> stateMachineBox2 = (Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>)stateMachineBox;
				stateMachineBox2.StateMachine = stateMachine;
				awaiter.UnsafeOnCompleted(stateMachineBox.MoveNext);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetException(Exception e)
			{
				this.Task.RaiseManagedCompletion(e);
				this._stateMachineBox.Dispose();
				this._stateMachineBox = null;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetResult()
			{
				this.Task.RaiseManagedCompletion();
				this._stateMachineBox.Dispose();
				this._stateMachineBox = null;
			}

			private Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox _stateMachineBox;

			private Awaitable _resultingCoroutine;

			private interface IStateMachineBox : IDisposable
			{
				Awaitable ResultingCoroutine { get; set; }

				Action MoveNext { get; }
			}

			private class StateMachineBox<TStateMachine> : Awaitable.AwaitableAsyncMethodBuilder.IStateMachineBox, IDisposable where TStateMachine : IAsyncStateMachine
			{
				public static Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine> GetOne()
				{
					return Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>._pool.Value.Get();
				}

				public void Dispose()
				{
					this.StateMachine = default(TStateMachine);
					this.ResultingCoroutine = null;
					Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>._pool.Value.Release(this);
				}

				public TStateMachine StateMachine { get; set; }

				private void DoMoveNext()
				{
					TStateMachine stateMachine = this.StateMachine;
					stateMachine.MoveNext();
				}

				public Action MoveNext { get; }

				public StateMachineBox()
				{
					this.MoveNext = new Action(this.DoMoveNext);
				}

				public Awaitable ResultingCoroutine { get; set; }

				private static readonly ThreadLocal<ObjectPool<Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>>> _pool = new ThreadLocal<ObjectPool<Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>>>(() => new ObjectPool<Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>>(() => new Awaitable.AwaitableAsyncMethodBuilder.StateMachineBox<TStateMachine>(), null, null, null, false, 10, 10000));
			}
		}

		[ExcludeFromDocs]
		public struct AwaitableAsyncMethodBuilder<T>
		{
			private Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox EnsureStateMachineBox<TStateMachine>() where TStateMachine : IAsyncStateMachine
			{
				bool flag = this._stateMachineBox != null;
				Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox stateMachineBox;
				if (flag)
				{
					stateMachineBox = this._stateMachineBox;
				}
				else
				{
					this._stateMachineBox = Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>.GetOne();
					this._stateMachineBox.ResultingCoroutine = this._resultingCoroutine;
					stateMachineBox = this._stateMachineBox;
				}
				return stateMachineBox;
			}

			public static Awaitable.AwaitableAsyncMethodBuilder<T> Create()
			{
				return default(Awaitable.AwaitableAsyncMethodBuilder<T>);
			}

			public Awaitable<T> Task
			{
				get
				{
					bool flag = this._resultingCoroutine != null;
					Awaitable<T> awaitable;
					if (flag)
					{
						awaitable = this._resultingCoroutine;
					}
					else
					{
						bool flag2 = this._stateMachineBox != null;
						if (flag2)
						{
							Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox stateMachineBox = this._stateMachineBox;
							Awaitable<T> awaitable2;
							if ((awaitable2 = stateMachineBox.ResultingCoroutine) == null)
							{
								awaitable2 = (stateMachineBox.ResultingCoroutine = Awaitable<T>.GetManaged());
							}
							awaitable = (this._resultingCoroutine = awaitable2);
						}
						else
						{
							awaitable = (this._resultingCoroutine = Awaitable<T>.GetManaged());
						}
					}
					return awaitable;
				}
			}

			public void SetResult(T value)
			{
				this.Task.SetResultAndRaiseContinuation(value);
				this._stateMachineBox.Dispose();
				this._stateMachineBox = null;
			}

			public void SetException(Exception e)
			{
				this.Task.SetExceptionAndRaiseContinuation(e);
				this._stateMachineBox.Dispose();
				this._stateMachineBox = null;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
			{
				Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox stateMachineBox = this.EnsureStateMachineBox<TStateMachine>();
				Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine> stateMachineBox2 = (Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>)stateMachineBox;
				this.Task.CompletionThreadAffinity = ((Thread.CurrentThread.ManagedThreadId == Awaitable._mainThreadId) ? Awaitable.AwaiterCompletionThreadAffinity.MainThread : Awaitable.AwaiterCompletionThreadAffinity.BackgroundThread);
				stateMachineBox2.StateMachine = stateMachine;
				stateMachineBox2.MoveNext();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
			{
				Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox stateMachineBox = this.EnsureStateMachineBox<TStateMachine>();
				Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine> stateMachineBox2 = (Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>)stateMachineBox;
				stateMachineBox2.StateMachine = stateMachine;
				awaiter.OnCompleted(stateMachineBox.MoveNext);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
			{
				Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox stateMachineBox = this.EnsureStateMachineBox<TStateMachine>();
				((Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>)stateMachineBox).StateMachine = stateMachine;
				awaiter.UnsafeOnCompleted(stateMachineBox.MoveNext);
			}

			private Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox _stateMachineBox;

			private Awaitable<T> _resultingCoroutine;

			private interface IStateMachineBox : IDisposable
			{
				Awaitable<T> ResultingCoroutine { get; set; }

				Action MoveNext { get; }
			}

			private class StateMachineBox<TStateMachine> : Awaitable.AwaitableAsyncMethodBuilder<T>.IStateMachineBox, IDisposable where TStateMachine : IAsyncStateMachine
			{
				public static Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine> GetOne()
				{
					return Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>._pool.Value.Get();
				}

				public void Dispose()
				{
					this.StateMachine = default(TStateMachine);
					this.ResultingCoroutine = null;
					Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>._pool.Value.Release(this);
				}

				public TStateMachine StateMachine { get; set; }

				public Action MoveNext { get; }

				private void DoMoveNext()
				{
					TStateMachine stateMachine = this.StateMachine;
					stateMachine.MoveNext();
				}

				public StateMachineBox()
				{
					this.MoveNext = new Action(this.DoMoveNext);
				}

				public Awaitable<T> ResultingCoroutine { get; set; }

				private static readonly ThreadLocal<ObjectPool<Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>>> _pool = new ThreadLocal<ObjectPool<Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>>>(() => new ObjectPool<Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>>(() => new Awaitable.AwaitableAsyncMethodBuilder<T>.StateMachineBox<TStateMachine>(), null, null, null, false, 10, 10000));
			}
		}

		[ExcludeFromDocs]
		public struct Awaiter : INotifyCompletion
		{
			internal Awaiter(Awaitable awaited)
			{
				this._awaited = awaited;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void OnCompleted(Action continuation)
			{
				this._awaited.SetContinuation(continuation);
			}

			public bool IsCompleted
			{
				get
				{
					return this._awaited.IsCompleted;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void GetResult()
			{
				this._awaited.PropagateExceptionAndRelease();
			}

			private readonly Awaitable _awaited;
		}

		private readonly struct AwaitableHandle
		{
			public bool IsNull
			{
				get
				{
					return this._handle == IntPtr.Zero;
				}
			}

			public bool IsManaged
			{
				get
				{
					return this._handle == Awaitable.AwaitableHandle.ManagedHandle._handle;
				}
			}

			public AwaitableHandle(IntPtr handle)
			{
				this._handle = handle;
			}

			public static implicit operator IntPtr(Awaitable.AwaitableHandle handle)
			{
				return handle._handle;
			}

			public static implicit operator Awaitable.AwaitableHandle(IntPtr handle)
			{
				return new Awaitable.AwaitableHandle(handle);
			}

			private readonly IntPtr _handle;

			public static Awaitable.AwaitableHandle ManagedHandle = new Awaitable.AwaitableHandle(new IntPtr(-1));

			public static Awaitable.AwaitableHandle NullHandle = new Awaitable.AwaitableHandle(IntPtr.Zero);
		}

		private struct AwaitableAndFrameIndex
		{
			public readonly Awaitable Awaitable { get; }

			public readonly int FrameIndex { get; }

			public AwaitableAndFrameIndex(Awaitable awaitable, int frameIndex)
			{
				this.Awaitable = awaitable;
				this.FrameIndex = frameIndex;
			}
		}

		private class DoubleBufferedAwaitableList
		{
			public void SwapAndComplete()
			{
				List<Awaitable.AwaitableAndFrameIndex> scratch = this._scratch;
				List<Awaitable.AwaitableAndFrameIndex> awaitables = this._awaitables;
				this._awaitables = scratch;
				this._scratch = awaitables;
				try
				{
					foreach (Awaitable.AwaitableAndFrameIndex awaitableAndFrameIndex in awaitables)
					{
						bool flag = !awaitableAndFrameIndex.Awaitable.IsDettachedOrCompleted;
						if (flag)
						{
							bool flag2 = Time.frameCount >= awaitableAndFrameIndex.FrameIndex || awaitableAndFrameIndex.FrameIndex == -1;
							if (flag2)
							{
								awaitableAndFrameIndex.Awaitable.RaiseManagedCompletion();
							}
							else
							{
								scratch.Add(awaitableAndFrameIndex);
							}
						}
					}
				}
				finally
				{
					awaitables.Clear();
				}
			}

			public void Add(Awaitable item, int frameIndex)
			{
				this._awaitables.Add(new Awaitable.AwaitableAndFrameIndex(item, frameIndex));
			}

			public void Remove(Awaitable item)
			{
				this._awaitables.RemoveAll((Awaitable.AwaitableAndFrameIndex x) => x.Awaitable == item);
			}

			public void Clear()
			{
				this._awaitables.Clear();
			}

			private List<Awaitable.AwaitableAndFrameIndex> _awaitables = new List<Awaitable.AwaitableAndFrameIndex>();

			private List<Awaitable.AwaitableAndFrameIndex> _scratch = new List<Awaitable.AwaitableAndFrameIndex>();
		}
	}
}
