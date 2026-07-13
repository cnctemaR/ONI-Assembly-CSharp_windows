using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandler.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class UploadHandler : IDisposable
	{
		[NativeMethod(IsThreadSafe = true)]
		private void ReleaseFromScripting()
		{
			IntPtr intPtr = UploadHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			UploadHandler.ReleaseFromScripting_Injected(intPtr);
		}

		internal UploadHandler()
		{
		}

		~UploadHandler()
		{
			this.Dispose();
		}

		public virtual void Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				this.ReleaseFromScripting();
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public byte[] data
		{
			get
			{
				return this.GetData();
			}
		}

		public string contentType
		{
			get
			{
				return this.GetContentType();
			}
			set
			{
				this.SetContentType(value);
			}
		}

		public float progress
		{
			get
			{
				return this.GetProgress();
			}
		}

		internal virtual byte[] GetData()
		{
			return null;
		}

		internal virtual string GetContentType()
		{
			return this.InternalGetContentType();
		}

		internal virtual void SetContentType(string newContentType)
		{
			this.InternalSetContentType(newContentType);
		}

		internal virtual float GetProgress()
		{
			return this.InternalGetProgress();
		}

		[NativeMethod("GetContentType")]
		private string InternalGetContentType()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = UploadHandler.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				UploadHandler.InternalGetContentType_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeMethod("SetContentType")]
		private unsafe void InternalSetContentType(string newContentType)
		{
			try
			{
				IntPtr intPtr = UploadHandler.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(newContentType, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = newContentType.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				UploadHandler.InternalSetContentType_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeMethod("GetProgress")]
		private float InternalGetProgress()
		{
			IntPtr intPtr = UploadHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UploadHandler.InternalGetProgress_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseFromScripting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetContentType_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetContentType_Injected(IntPtr _unity_self, ref ManagedSpanWrapper newContentType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float InternalGetProgress_Injected(IntPtr _unity_self);

		[NonSerialized]
		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(UploadHandler uploadHandler)
			{
				return uploadHandler.m_Ptr;
			}
		}
	}
}
