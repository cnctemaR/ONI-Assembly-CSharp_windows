using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandlerRaw.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UploadHandlerRaw : UploadHandler
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] UploadHandlerRaw self, byte* data, int dataLength);

		public UploadHandlerRaw(byte[] data)
			: this((data == null || data.Length == 0) ? default(NativeArray<byte>) : new NativeArray<byte>(data, Allocator.Persistent), true)
		{
		}

		public unsafe UploadHandlerRaw(NativeArray<byte> data, bool transferOwnership)
		{
			bool flag = !data.IsCreated || data.Length == 0;
			if (flag)
			{
				this.m_Ptr = UploadHandlerRaw.Create(this, null, 0);
			}
			else
			{
				if (transferOwnership)
				{
					this.m_Payload = data;
				}
				this.m_Ptr = UploadHandlerRaw.Create(this, (byte*)data.GetUnsafeReadOnlyPtr<byte>(), data.Length);
			}
		}

		public unsafe UploadHandlerRaw(NativeArray<byte>.ReadOnly data)
		{
			bool flag = !data.IsCreated || data.Length == 0;
			if (flag)
			{
				this.m_Ptr = UploadHandlerRaw.Create(this, null, 0);
			}
			else
			{
				bool flag2 = data.Length == 0;
				if (flag2)
				{
					this.m_Ptr = UploadHandlerRaw.Create(this, null, 0);
				}
				else
				{
					this.m_Ptr = UploadHandlerRaw.Create(this, (byte*)data.GetUnsafeReadOnlyPtr<byte>(), data.Length);
				}
			}
		}

		internal override byte[] GetData()
		{
			bool isCreated = this.m_Payload.IsCreated;
			byte[] array;
			if (isCreated)
			{
				array = this.m_Payload.ToArray();
			}
			else
			{
				array = null;
			}
			return array;
		}

		public override void Dispose()
		{
			bool isCreated = this.m_Payload.IsCreated;
			if (isCreated)
			{
				this.m_Payload.Dispose();
			}
			base.Dispose();
		}

		private NativeArray<byte> m_Payload;

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(UploadHandlerRaw uploadHandler)
			{
				return uploadHandler.m_Ptr;
			}
		}
	}
}
