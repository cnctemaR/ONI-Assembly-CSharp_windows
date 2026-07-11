using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	internal sealed class SpanDebugView<T>
	{
		public SpanDebugView(Span<T> collection)
		{
			this._pinnable = (T[])collection.Pinnable;
			this._byteOffset = collection.ByteOffset;
			this._length = collection.Length;
		}

		public SpanDebugView(ReadOnlySpan<T> collection)
		{
			this._pinnable = (T[])collection.Pinnable;
			this._byteOffset = collection.ByteOffset;
			this._length = collection.Length;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public unsafe T[] Items
		{
			get
			{
				int num = (typeof(T).GetTypeInfo().IsValueType ? Unsafe.SizeOf<T>() : IntPtr.Size);
				T[] array = new T[this._length];
				if (this._pinnable == null)
				{
					byte* ptr = (byte*)this._byteOffset.ToPointer();
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Unsafe.Read<T>((void*)ptr);
						ptr += num;
					}
				}
				else
				{
					long num2 = this._byteOffset.ToInt64();
					long num3 = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.ToInt64();
					int num4 = (int)((num2 - num3) / (long)num);
					Array.Copy(this._pinnable, num4, array, 0, this._length);
				}
				return array;
			}
		}

		private readonly T[] _pinnable;

		private readonly IntPtr _byteOffset;

		private readonly int _length;
	}
}
