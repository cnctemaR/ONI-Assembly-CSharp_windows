using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Scripting/TextAsset.h")]
	public class TextAsset : Object
	{
		public byte[] bytes
		{
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TextAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TextAsset.get_bytes_Injected(intPtr);
			}
		}

		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private byte[] GetPreviewBytes(int maxByteCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TextAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TextAsset.GetPreviewBytes_Injected(intPtr, maxByteCount);
		}

		private unsafe static void Internal_CreateInstance([Writable] TextAsset self, string text)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = text.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				TextAsset.Internal_CreateInstance_Injected(self, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private unsafe static void Internal_CreateInstanceFromBytes([Writable] TextAsset self, ReadOnlySpan<byte> bytes)
		{
			ReadOnlySpan<byte> readOnlySpan = bytes;
			fixed (byte* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				TextAsset.Internal_CreateInstanceFromBytes_Injected(self, ref managedSpanWrapper);
			}
		}

		private IntPtr GetDataPtr()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TextAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TextAsset.GetDataPtr_Injected(intPtr);
		}

		private long GetDataSize()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TextAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TextAsset.GetDataSize_Injected(intPtr);
		}

		public string text
		{
			get
			{
				byte[] bytes = this.bytes;
				return (bytes.Length == 0) ? string.Empty : TextAsset.DecodeString(bytes);
			}
		}

		public long dataSize
		{
			get
			{
				return this.GetDataSize();
			}
		}

		public override string ToString()
		{
			return this.text;
		}

		public TextAsset()
			: this(TextAsset.CreateOptions.CreateNativeObject, null)
		{
		}

		public TextAsset(string text)
			: this(TextAsset.CreateOptions.CreateNativeObject, text)
		{
		}

		public TextAsset(ReadOnlySpan<byte> bytes)
			: this(TextAsset.CreateOptions.CreateNativeObject, bytes)
		{
		}

		internal TextAsset(TextAsset.CreateOptions options, string text)
		{
			bool flag = options == TextAsset.CreateOptions.CreateNativeObject;
			if (flag)
			{
				TextAsset.Internal_CreateInstance(this, text);
			}
		}

		internal TextAsset(TextAsset.CreateOptions options, ReadOnlySpan<byte> bytes)
		{
			bool flag = options == TextAsset.CreateOptions.CreateNativeObject;
			if (flag)
			{
				TextAsset.Internal_CreateInstanceFromBytes(this, bytes);
			}
		}

		public unsafe NativeArray<T> GetData<T>() where T : struct
		{
			long dataSize = this.GetDataSize();
			long num = (long)UnsafeUtility.SizeOf<T>();
			bool flag = dataSize % num != 0L;
			if (flag)
			{
				throw new ArgumentException(string.Format("Type passed to {0} can't capture the asset data. Data size is {1} which is not a multiple of type size {2}", "GetData", dataSize, num));
			}
			long num2 = dataSize / num;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetDataPtr(), (int)num2, Allocator.None);
		}

		internal string GetPreview(int maxChars)
		{
			return TextAsset.DecodeString(this.GetPreviewBytes(maxChars * 4));
		}

		internal static string DecodeString(byte[] bytes)
		{
			int num = TextAsset.EncodingUtility.encodingLookup.Length;
			int i = 0;
			int num2;
			while (i < num)
			{
				byte[] key = TextAsset.EncodingUtility.encodingLookup[i].Key;
				num2 = key.Length;
				bool flag = bytes.Length >= num2;
				if (flag)
				{
					for (int j = 0; j < num2; j++)
					{
						bool flag2 = key[j] != bytes[j];
						if (flag2)
						{
							num2 = -1;
						}
					}
					bool flag3 = num2 < 0;
					if (!flag3)
					{
						try
						{
							Encoding value = TextAsset.EncodingUtility.encodingLookup[i].Value;
							return value.GetString(bytes, num2, bytes.Length - num2);
						}
						catch
						{
						}
					}
				}
				IL_00A2:
				i++;
				continue;
				goto IL_00A2;
			}
			num2 = 0;
			Encoding targetEncoding = TextAsset.EncodingUtility.targetEncoding;
			return targetEncoding.GetString(bytes, num2, bytes.Length - num2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] get_bytes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] GetPreviewBytes_Injected(IntPtr _unity_self, int maxByteCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateInstance_Injected([Writable] TextAsset self, ref ManagedSpanWrapper text);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateInstanceFromBytes_Injected([Writable] TextAsset self, ref ManagedSpanWrapper bytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDataPtr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetDataSize_Injected(IntPtr _unity_self);

		internal enum CreateOptions
		{
			None,
			CreateNativeObject
		}

		private static class EncodingUtility
		{
			static EncodingUtility()
			{
				Encoding encoding = new UTF32Encoding(true, true, true);
				Encoding encoding2 = new UTF32Encoding(false, true, true);
				Encoding encoding3 = new UnicodeEncoding(true, true, true);
				Encoding encoding4 = new UnicodeEncoding(false, true, true);
				Encoding encoding5 = new UTF8Encoding(true, true);
				TextAsset.EncodingUtility.encodingLookup = new KeyValuePair<byte[], Encoding>[]
				{
					new KeyValuePair<byte[], Encoding>(encoding.GetPreamble(), encoding),
					new KeyValuePair<byte[], Encoding>(encoding2.GetPreamble(), encoding2),
					new KeyValuePair<byte[], Encoding>(encoding3.GetPreamble(), encoding3),
					new KeyValuePair<byte[], Encoding>(encoding4.GetPreamble(), encoding4),
					new KeyValuePair<byte[], Encoding>(encoding5.GetPreamble(), encoding5)
				};
			}

			internal static readonly KeyValuePair<byte[], Encoding>[] encodingLookup;

			internal static readonly Encoding targetEncoding = Encoding.GetEncoding(Encoding.UTF8.CodePage, new EncoderReplacementFallback("\ufffd"), new DecoderReplacementFallback("\ufffd"));
		}
	}
}
