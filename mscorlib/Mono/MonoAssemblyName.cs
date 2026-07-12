using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono
{
	internal struct MonoAssemblyName
	{
		private const int MONO_PUBLIC_KEY_TOKEN_LENGTH = 17;

		internal IntPtr name;

		internal IntPtr culture;

		internal IntPtr hash_value;

		internal IntPtr public_key;

		[FixedBuffer(typeof(byte), 17)]
		internal MonoAssemblyName.<public_key_token>e__FixedBuffer public_key_token;

		internal uint hash_alg;

		internal uint hash_len;

		internal uint flags;

		internal ushort major;

		internal ushort minor;

		internal ushort build;

		internal ushort revision;

		internal ushort arch;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 17)]
		public struct <public_key_token>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
