using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipes
{
	internal abstract class PipeCompletionSource<TResult> : TaskCompletionSource<TResult>
	{
		protected unsafe PipeCompletionSource(ThreadPoolBoundHandle handle, ReadOnlyMemory<byte> bufferToPin)
			: base(TaskCreationOptions.RunContinuationsAsynchronously)
		{
			this._threadPoolBinding = handle;
			this._state = 0;
			this._pinnedMemory = bufferToPin.Pin();
			this._overlapped = this._threadPoolBinding.AllocateNativeOverlapped(delegate(uint errorCode, uint numBytes, NativeOverlapped* pOverlapped)
			{
				((PipeCompletionSource<TResult>)ThreadPoolBoundHandle.GetNativeOverlappedState(pOverlapped)).AsyncCallback(errorCode, numBytes);
			}, this, null);
		}

		internal unsafe NativeOverlapped* Overlapped
		{
			get
			{
				return this._overlapped;
			}
		}

		internal void RegisterForCancellation(CancellationToken cancellationToken)
		{
			if (cancellationToken.CanBeCanceled && this.Overlapped != null)
			{
				int num = Interlocked.CompareExchange(ref this._state, 4, 0);
				if (num == 0)
				{
					this._cancellationRegistration = cancellationToken.Register(delegate(object thisRef)
					{
						((PipeCompletionSource<TResult>)thisRef).Cancel();
					}, this);
					num = Interlocked.Exchange(ref this._state, 0);
				}
				else if (num != 8)
				{
					num = Interlocked.Exchange(ref this._state, 0);
				}
				if ((num & 3) != 0)
				{
					this.CompleteCallback(num);
				}
			}
		}

		internal void ReleaseResources()
		{
			this._cancellationRegistration.Dispose();
			if (this._overlapped != null)
			{
				this._threadPoolBinding.FreeNativeOverlapped(this.Overlapped);
				this._overlapped = null;
			}
			this._pinnedMemory.Dispose();
		}

		internal abstract void SetCompletedSynchronously();

		protected virtual void AsyncCallback(uint errorCode, uint numBytes)
		{
			int num;
			if (errorCode == 0U)
			{
				num = 1;
			}
			else
			{
				num = 2;
				this._errorCode = (int)errorCode;
			}
			if (Interlocked.Exchange(ref this._state, num) == 0 && Interlocked.Exchange(ref this._state, 8) != 0)
			{
				this.CompleteCallback(num);
			}
		}

		protected abstract void HandleError(int errorCode);

		private unsafe void Cancel()
		{
			SafeHandle handle = this._threadPoolBinding.Handle;
			NativeOverlapped* overlapped = this.Overlapped;
			if (!handle.IsInvalid && !global::Interop.Kernel32.CancelIoEx(handle, overlapped))
			{
				Marshal.GetLastWin32Error();
			}
		}

		protected virtual void HandleUnexpectedCancellation()
		{
			base.TrySetCanceled();
		}

		private void CompleteCallback(int resultState)
		{
			CancellationToken token = this._cancellationRegistration.Token;
			this.ReleaseResources();
			if (resultState != 2)
			{
				this.SetCompletedSynchronously();
				return;
			}
			if (this._errorCode != 995)
			{
				this.HandleError(this._errorCode);
				return;
			}
			if (token.CanBeCanceled && !token.IsCancellationRequested)
			{
				this.HandleUnexpectedCancellation();
				return;
			}
			base.TrySetCanceled(token);
		}

		private const int NoResult = 0;

		private const int ResultSuccess = 1;

		private const int ResultError = 2;

		private const int RegisteringCancellation = 4;

		private const int CompletedCallback = 8;

		private readonly ThreadPoolBoundHandle _threadPoolBinding;

		private CancellationTokenRegistration _cancellationRegistration;

		private int _errorCode;

		private unsafe NativeOverlapped* _overlapped;

		private MemoryHandle _pinnedMemory;

		private int _state;
	}
}
