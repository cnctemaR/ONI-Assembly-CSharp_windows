using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms
{
	internal sealed class SimpleNativeDetour : IDisposable
	{
		public global::System.ReadOnlyMemory<byte> DetourBackup
		{
			get
			{
				return this.backup;
			}
		}

		public IntPtr Source
		{
			get
			{
				return this.detourInfo.From;
			}
		}

		public IntPtr Destination
		{
			get
			{
				return this.detourInfo.To;
			}
		}

		internal SimpleNativeDetour([Nullable(1)] PlatformTriple triple, NativeDetourInfo detourInfo, global::System.Memory<byte> backup, [Nullable(2)] IDisposable allocHandle)
		{
			this.triple = triple;
			this.detourInfo = detourInfo;
			this.backup = backup;
			this.AllocHandle = allocHandle;
		}

		public unsafe void ChangeTarget(IntPtr newTarget)
		{
			this.CheckDisposed();
			bool flag;
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(47, 3, out flag);
			if (flag)
			{
				debugLogTraceStringHandler.AppendLiteral("Retargeting simple detour 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Source, "x16");
				debugLogTraceStringHandler.AppendLiteral(" => 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Destination, "x16");
				debugLogTraceStringHandler.AppendLiteral(" to target 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(newTarget, "x16");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			NativeDetourInfo nativeDetourInfo = this.triple.Architecture.ComputeRetargetInfo(this.detourInfo, newTarget, this.detourInfo.Size);
			int size = nativeDetourInfo.Size;
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)size], size);
			IDisposable disposable;
			bool flag2;
			bool flag3;
			this.triple.Architecture.GetRetargetBytes(this.detourInfo, nativeDetourInfo, span, out disposable, out flag2, out flag3);
			if (flag2)
			{
				byte[] array = null;
				if (nativeDetourInfo.Size > this.backup.Length)
				{
					array = new byte[nativeDetourInfo.Size];
				}
				this.triple.System.PatchData(PatchTargetKind.Executable, this.Source, span, array);
				if (array != null)
				{
					this.backup.Span.CopyTo(array);
					this.backup = array;
				}
			}
			this.detourInfo = nativeDetourInfo;
			IDisposable allocHandle = this.AllocHandle;
			IDisposable disposable2 = disposable;
			disposable = allocHandle;
			this.AllocHandle = disposable2;
			if (flag3 && disposable != null)
			{
				disposable.Dispose();
			}
		}

		public void Undo()
		{
			this.CheckDisposed();
			this.UndoCore(true);
		}

		private void CheckDisposed()
		{
			if (this.disposedValue)
			{
				throw new ObjectDisposedException("SimpleNativeDetour");
			}
		}

		private void UndoCore(bool disposing)
		{
			bool flag;
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(30, 2, out flag);
			if (flag)
			{
				debugLogTraceStringHandler.AppendLiteral("Undoing simple detour 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Source, "x16");
				debugLogTraceStringHandler.AppendLiteral(" => 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Destination, "x16");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			this.triple.System.PatchData(PatchTargetKind.Executable, this.Source, this.DetourBackup.Span, default(global::System.Span<byte>));
			if (disposing)
			{
				this.Cleanup();
			}
			this.disposedValue = true;
		}

		private void Cleanup()
		{
			IDisposable allocHandle = this.AllocHandle;
			if (allocHandle == null)
			{
				return;
			}
			allocHandle.Dispose();
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				this.UndoCore(disposing);
				this.disposedValue = true;
			}
		}

		~SimpleNativeDetour()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool disposedValue;

		[Nullable(1)]
		private readonly PlatformTriple triple;

		private NativeDetourInfo detourInfo;

		private global::System.Memory<byte> backup;

		[Nullable(2)]
		private IDisposable AllocHandle;
	}
}
