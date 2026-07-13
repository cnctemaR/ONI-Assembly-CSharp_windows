using System;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Unsafe/UTF8StringView.bindings.h")]
	internal readonly struct UTF8StringView
	{
		public unsafe UTF8StringView(ReadOnlySpan<byte> prefixUtf8Span)
		{
			fixed (byte* ptr = prefixUtf8Span[0])
			{
				byte* ptr2 = ptr;
				this.utf8Ptr = new IntPtr((void*)ptr2);
			}
			this.utf8Length = prefixUtf8Span.Length;
		}

		public UTF8StringView(IntPtr ptr, int lengthUtf8)
		{
			this.utf8Ptr = ptr;
			this.utf8Length = lengthUtf8;
		}

		public unsafe UTF8StringView(byte* ptr, int lengthUtf8)
		{
			this.utf8Ptr = new IntPtr((void*)ptr);
			this.utf8Length = lengthUtf8;
		}

		public unsafe override string ToString()
		{
			return Encoding.UTF8.GetString((byte*)this.utf8Ptr.ToPointer(), this.utf8Length);
		}

		public readonly IntPtr utf8Ptr;

		public readonly int utf8Length;
	}
}
