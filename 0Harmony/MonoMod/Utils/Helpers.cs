using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MonoMod.Logs;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class Helpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<[Nullable(2)] T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool Has<T>(this T value, T flag) where T : struct, Enum
		{
			if (Unsafe.SizeOf<T>() == 8)
			{
				long num = *Unsafe.As<T, long>(ref flag);
				return (*Unsafe.As<T, long>(ref value) & num) == num;
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				int num2 = *Unsafe.As<T, int>(ref flag);
				return (*Unsafe.As<T, int>(ref value) & num2) == num2;
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				short num3 = *Unsafe.As<T, short>(ref flag);
				return (*Unsafe.As<T, short>(ref value) & num3) == num3;
			}
			if (Unsafe.SizeOf<T>() == 1)
			{
				byte b = *Unsafe.As<T, byte>(ref flag);
				return (*Unsafe.As<T, byte>(ref value) & b) == b;
			}
			throw new InvalidOperationException("unknown enum size?");
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArgumentNull<T>([global::System.Diagnostics.CodeAnalysis.NotNull] T arg, [Nullable(1)] [CallerArgumentExpression("arg")] string name = "")
		{
			if (arg == null)
			{
				Helpers.ThrowArgumentNull(name);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ThrowIfNull<[Nullable(2)] T>([Nullable(2)] [global::System.Diagnostics.CodeAnalysis.NotNull] T arg, [CallerArgumentExpression("arg")] string name = "")
		{
			if (arg == null)
			{
				Helpers.ThrowArgumentNull(name);
			}
			return arg;
		}

		public static T EventAdd<[Nullable(0)] T>([Nullable(2)] ref T evt, T del) where T : Delegate
		{
			T t;
			T t2;
			do
			{
				t = evt;
				t2 = (T)((object)Delegate.Combine(t, del));
			}
			while (Interlocked.CompareExchange<T>(ref evt, t2, t) != t);
			return t2;
		}

		[return: Nullable(2)]
		public static T EventRemove<[Nullable(0)] T>([Nullable(2)] ref T evt, T del) where T : Delegate
		{
			T t;
			T t2;
			do
			{
				t = evt;
				t2 = (T)((object)Delegate.Remove(t, del));
			}
			while (Interlocked.CompareExchange<T>(ref evt, t2, t) != t);
			return t2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assert([global::System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool value, [Nullable(2)] string message = null, [CallerArgumentExpression("value")] string expr = "")
		{
			if (!value)
			{
				Helpers.ThrowAssertionFailed(message, expr);
			}
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DAssert([global::System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool value, [Nullable(2)] string message = null, [CallerArgumentExpression("value")] string expr = "")
		{
			if (!value)
			{
				Helpers.ThrowAssertionFailed(message, expr);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assert([global::System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool value, [InterpolatedStringHandlerArgument("value")] ref AssertionInterpolatedStringHandler message, [CallerArgumentExpression("value")] string expr = "")
		{
			if (!value)
			{
				Helpers.ThrowAssertionFailed(ref message, expr);
			}
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DAssert([global::System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool value, [InterpolatedStringHandlerArgument("value")] ref AssertionInterpolatedStringHandler message, [CallerArgumentExpression("value")] string expr = "")
		{
			if (!value)
			{
				Helpers.ThrowAssertionFailed(ref message, expr);
			}
		}

		[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
		private static void ThrowArgumentNull(string argName)
		{
			throw new ArgumentNullException(argName);
		}

		[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
		private static void ThrowAssertionFailed([Nullable(2)] string msg, string expr)
		{
			LogLevel logLevel = LogLevel.Assert;
			LogLevel logLevel2 = logLevel;
			bool flag;
			DebugLogInterpolatedStringHandler debugLogInterpolatedStringHandler = new DebugLogInterpolatedStringHandler(19, 2, logLevel, out flag);
			if (flag)
			{
				debugLogInterpolatedStringHandler.AppendLiteral("Assertion failed! ");
				debugLogInterpolatedStringHandler.AppendFormatted(expr);
				debugLogInterpolatedStringHandler.AppendLiteral(" ");
				debugLogInterpolatedStringHandler.AppendFormatted(msg);
			}
			DebugLog.Log("MonoMod.Utils.Assert", logLevel2, ref debugLogInterpolatedStringHandler);
			throw new AssertionFailedException(msg, expr);
		}

		[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
		private static void ThrowAssertionFailed(ref AssertionInterpolatedStringHandler message, string expr)
		{
			string text = message.ToStringAndClear();
			LogLevel logLevel = LogLevel.Assert;
			LogLevel logLevel2 = logLevel;
			bool flag;
			DebugLogInterpolatedStringHandler debugLogInterpolatedStringHandler = new DebugLogInterpolatedStringHandler(19, 2, logLevel, out flag);
			if (flag)
			{
				debugLogInterpolatedStringHandler.AppendLiteral("Assertion failed! ");
				debugLogInterpolatedStringHandler.AppendFormatted(expr);
				debugLogInterpolatedStringHandler.AppendLiteral(" ");
				debugLogInterpolatedStringHandler.AppendFormatted(text);
			}
			DebugLog.Log("MonoMod.Utils.Assert", logLevel2, ref debugLogInterpolatedStringHandler);
			throw new AssertionFailedException(text, expr);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetOrInit<T>([Nullable(2)] ref T location, Func<T> init) where T : class
		{
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValue<T, Func<T>>(ref location, Helpers.FuncInvokeHolder<T>.InvokeFunc, init);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetOrInitWithLock<T>([Nullable(2)] ref T location, object @lock, Func<T> init) where T : class
		{
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValueWithLock<T, Func<T>>(ref location, @lock, Helpers.FuncInvokeHolder<T>.InvokeFunc, init);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetOrInit<[Nullable(2)] TParam, T>([Nullable(2)] ref T location, Func<TParam, T> init, TParam param) where T : class
		{
			Helpers.ThrowIfArgumentNull<Func<TParam, T>>(init, "init");
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValue<T, TParam>(ref location, init, param);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetOrInitWithLock<[Nullable(2)] TParam, T>([Nullable(2)] ref T location, object @lock, Func<TParam, T> init, TParam param) where T : class
		{
			Helpers.ThrowIfArgumentNull<Func<TParam, T>>(init, "init");
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValueWithLock<T, TParam>(ref location, @lock, init, param);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T GetOrInit<T>([Nullable(2)] ref T location, [Nullable(new byte[] { 0, 1 })] delegate*<T> init) where T : class
		{
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValue<T, IntPtr>(ref location, ldftn(TailCallDelegatePtr<T>), (IntPtr)init);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T GetOrInitWithLock<T>([Nullable(2)] ref T location, object @lock, [Nullable(new byte[] { 0, 1 })] delegate*<T> init) where T : class
		{
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValueWithLock<T, IntPtr>(ref location, @lock, ldftn(TailCallDelegatePtr<T>), (IntPtr)init);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T GetOrInit<T, [Nullable(2)] TParam>([Nullable(2)] ref T location, [Nullable(new byte[] { 0, 1, 1 })] delegate*<TParam, T> init, TParam obj) where T : class
		{
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValue<T, TParam>(ref location, init, obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T GetOrInitWithLock<T, [Nullable(2)] TParam>([Nullable(2)] ref T location, object @lock, [Nullable(new byte[] { 0, 1, 1 })] delegate*<TParam, T> init, TParam obj) where T : class
		{
			if (location != null)
			{
				return location;
			}
			return Helpers.InitializeValueWithLock<T, TParam>(ref location, @lock, init, obj);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe static T InitializeValue<T, [Nullable(2)] TParam>([Nullable(2)] ref T location, [Nullable(new byte[] { 0, 1, 1 })] delegate*<TParam, T> init, TParam obj) where T : class
		{
			Interlocked.CompareExchange<T>(ref location, calli(T(TParam), obj, init), default(T));
			return location;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static T InitializeValue<T, [Nullable(2)] TParam>([Nullable(2)] ref T location, Func<TParam, T> init, TParam obj) where T : class
		{
			Interlocked.CompareExchange<T>(ref location, init(obj), default(T));
			return location;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe static T InitializeValueWithLock<T, [Nullable(2)] TParam>([Nullable(2)] ref T location, object @lock, [Nullable(new byte[] { 0, 1, 1 })] delegate*<TParam, T> init, TParam obj) where T : class
		{
			T t;
			lock (@lock)
			{
				if (location != null)
				{
					t = location;
				}
				else
				{
					t = (location = calli(T(TParam), obj, init));
				}
			}
			return t;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static T InitializeValueWithLock<T, [Nullable(2)] TParam>([Nullable(2)] ref T location, object @lock, Func<TParam, T> init, TParam obj) where T : class
		{
			T t;
			lock (@lock)
			{
				if (location != null)
				{
					t = location;
				}
				else
				{
					t = (location = init(obj));
				}
			}
			return t;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MaskedSequenceEqual(global::System.ReadOnlySpan<byte> first, global::System.ReadOnlySpan<byte> second, global::System.ReadOnlySpan<byte> mask)
		{
			if (mask.Length < first.Length || mask.Length < second.Length)
			{
				Helpers.ThrowMaskTooShort();
			}
			return first.Length == second.Length && Helpers.MaskedSequenceEqualCore(global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(first), global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(second), global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(mask), (UIntPtr)((IntPtr)first.Length));
		}

		[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
		private static void ThrowMaskTooShort()
		{
			throw new ArgumentException("Mask too short", "mask");
		}

		private unsafe static bool MaskedSequenceEqualCore(ref byte first, ref byte second, ref byte maskBytes, [NativeInteger] UIntPtr length)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)0;
				if (length >= (UIntPtr)((IntPtr)sizeof(UIntPtr)))
				{
					IntPtr intPtr2 = (IntPtr)(length - (UIntPtr)((IntPtr)sizeof(UIntPtr)));
					UIntPtr uintPtr;
					while (intPtr2 > intPtr)
					{
						uintPtr = Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref maskBytes, intPtr));
						if ((Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr)) & uintPtr) != (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr)) & uintPtr))
						{
							return false;
						}
						intPtr += (IntPtr)sizeof(UIntPtr);
					}
					uintPtr = Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref maskBytes, intPtr));
					return (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) & uintPtr) == (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2)) & uintPtr);
				}
				while (length > (UIntPtr)intPtr)
				{
					byte b = *Unsafe.AddByteOffset<byte>(ref maskBytes, intPtr);
					if ((*Unsafe.AddByteOffset<byte>(ref first, intPtr) & b) != (*Unsafe.AddByteOffset<byte>(ref second, intPtr) & b))
					{
						return false;
					}
					intPtr += (IntPtr)1;
				}
				return true;
			}
			return true;
		}

		public static byte[] ReadAllBytes(string path)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1))
			{
				long length = fileStream.Length;
				if (length > 2147483647L)
				{
					throw new IOException("File is too long (more than 2GB)");
				}
				if (length == 0L)
				{
					array = Helpers.ReadAllBytesUnknownLength(fileStream);
				}
				else
				{
					int num = 0;
					int i = (int)length;
					byte[] array2 = new byte[i];
					while (i > 0)
					{
						int num2 = fileStream.Read(array2, num, i);
						if (num2 == 0)
						{
							throw new IOException("Unexpected end of stream");
						}
						num += num2;
						i -= num2;
					}
					array = array2;
				}
			}
			return array;
		}

		private static byte[] ReadAllBytesUnknownLength(FileStream fs)
		{
			byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(256);
			byte[] array3;
			try
			{
				int num = 0;
				for (;;)
				{
					if (num == array.Length)
					{
						uint num2 = (uint)(array.Length * 2);
						if ((ulong)num2 > (ulong)((long)ArrayEx.MaxLength))
						{
							num2 = (uint)Math.Max(ArrayEx.MaxLength, array.Length + 1);
						}
						byte[] array2 = global::System.Buffers.ArrayPool<byte>.Shared.Rent((int)num2);
						Array.Copy(array, array2, array.Length);
						if (array != null)
						{
							global::System.Buffers.ArrayPool<byte>.Shared.Return(array, false);
						}
						array = array2;
					}
					int num3 = fs.Read(array, num, array.Length - num);
					if (num3 == 0)
					{
						break;
					}
					num += num3;
				}
				array3 = array.AsSpan<byte>(0, num).ToArray();
			}
			finally
			{
				if (array != null)
				{
					global::System.Buffers.ArrayPool<byte>.Shared.Return(array, false);
				}
			}
			return array3;
		}

		[NullableContext(0)]
		private static class FuncInvokeHolder<[Nullable(2)] T>
		{
			[Nullable(1)]
			public static readonly Func<Func<T>, T> InvokeFunc = (Func<T> f) => f();
		}
	}
}
