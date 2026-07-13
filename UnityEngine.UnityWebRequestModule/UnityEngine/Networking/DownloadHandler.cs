using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandler.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class DownloadHandler : IDisposable
	{
		[NativeMethod(IsThreadSafe = true)]
		private void ReleaseFromScripting()
		{
			IntPtr intPtr = DownloadHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			DownloadHandler.ReleaseFromScripting_Injected(intPtr);
		}

		[VisibleToOtherModules]
		internal DownloadHandler()
		{
		}

		~DownloadHandler()
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

		public bool isDone
		{
			get
			{
				return this.IsDone();
			}
		}

		private bool IsDone()
		{
			IntPtr intPtr = DownloadHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return DownloadHandler.IsDone_Injected(intPtr);
		}

		public string error
		{
			get
			{
				return this.GetErrorMsg();
			}
		}

		private string GetErrorMsg()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = DownloadHandler.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				DownloadHandler.GetErrorMsg_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public NativeArray<byte>.ReadOnly nativeData
		{
			get
			{
				return this.GetNativeData().AsReadOnly();
			}
		}

		public byte[] data
		{
			get
			{
				return this.GetData();
			}
		}

		public string text
		{
			get
			{
				return this.GetText();
			}
		}

		protected virtual NativeArray<byte> GetNativeData()
		{
			return default(NativeArray<byte>);
		}

		protected virtual byte[] GetData()
		{
			return DownloadHandler.InternalGetByteArray(this);
		}

		protected unsafe virtual string GetText()
		{
			NativeArray<byte> nativeData = this.GetNativeData();
			bool flag = nativeData.IsCreated && nativeData.Length > 0;
			string text;
			if (flag)
			{
				text = new string((sbyte*)nativeData.GetUnsafeReadOnlyPtr<byte>(), 0, nativeData.Length, this.GetTextEncoder());
			}
			else
			{
				text = "";
			}
			return text;
		}

		private Encoding GetTextEncoder()
		{
			string contentType = this.GetContentType();
			bool flag = !string.IsNullOrEmpty(contentType);
			if (flag)
			{
				int num = contentType.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
				bool flag2 = num > -1;
				if (flag2)
				{
					int num2 = contentType.IndexOf('=', num);
					bool flag3 = num2 > -1;
					if (flag3)
					{
						string text = contentType.Substring(num2 + 1).Trim().Trim(new char[] { '\'', '"' })
							.Trim();
						int num3 = text.IndexOf(';');
						bool flag4 = num3 > -1;
						if (flag4)
						{
							text = text.Substring(0, num3);
						}
						try
						{
							return Encoding.GetEncoding(text);
						}
						catch (ArgumentException ex)
						{
							Debug.LogWarning(string.Format("Unsupported encoding '{0}': {1}", text, ex.Message));
						}
						catch (NotSupportedException ex2)
						{
							Debug.LogWarning(string.Format("Unsupported encoding '{0}': {1}", text, ex2.Message));
						}
					}
				}
			}
			return Encoding.UTF8;
		}

		private string GetContentType()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = DownloadHandler.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				DownloadHandler.GetContentType_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[RequiredByNativeCode]
		protected virtual bool ReceiveData(byte[] data, int dataLength)
		{
			return true;
		}

		[RequiredByNativeCode]
		protected virtual void ReceiveContentLengthHeader(ulong contentLength)
		{
			this.ReceiveContentLength((int)contentLength);
		}

		[Obsolete("Use ReceiveContentLengthHeader")]
		protected virtual void ReceiveContentLength(int contentLength)
		{
		}

		[RequiredByNativeCode]
		protected virtual void CompleteContent()
		{
		}

		[RequiredByNativeCode]
		protected virtual float GetProgress()
		{
			return 0f;
		}

		protected static T GetCheckedDownloader<T>(UnityWebRequest www) where T : DownloadHandler
		{
			bool flag = www == null;
			if (flag)
			{
				throw new NullReferenceException("Cannot get content from a null UnityWebRequest object");
			}
			bool flag2 = !www.isDone;
			if (flag2)
			{
				throw new InvalidOperationException("Cannot get content from an unfinished UnityWebRequest object");
			}
			bool flag3 = www.result == UnityWebRequest.Result.ProtocolError;
			if (flag3)
			{
				throw new InvalidOperationException(www.error);
			}
			return (T)((object)www.downloadHandler);
		}

		[VisibleToOtherModules]
		[NativeThrows]
		internal unsafe static byte* InternalGetByteArray(DownloadHandler dh, out int length)
		{
			return DownloadHandler.InternalGetByteArray_Injected((dh == null) ? ((IntPtr)0) : DownloadHandler.BindingsMarshaller.ConvertToNative(dh), out length);
		}

		internal static byte[] InternalGetByteArray(DownloadHandler dh)
		{
			NativeArray<byte> nativeData = dh.GetNativeData();
			bool isCreated = nativeData.IsCreated;
			byte[] array;
			if (isCreated)
			{
				array = nativeData.ToArray();
			}
			else
			{
				array = null;
			}
			return array;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UnityWebRequestAudioModule", "UnityEngine.UnityWebRequestTextureModule" })]
		internal unsafe static NativeArray<byte> InternalGetNativeArray(DownloadHandler dh, ref NativeArray<byte> nativeArray)
		{
			int num;
			byte* ptr = DownloadHandler.InternalGetByteArray(dh, out num);
			bool isCreated = nativeArray.IsCreated;
			if (isCreated)
			{
				bool flag = nativeArray.Length == num;
				if (flag)
				{
					return nativeArray;
				}
				DownloadHandler.DisposeNativeArray(ref nativeArray);
			}
			DownloadHandler.CreateNativeArrayForNativeData(ref nativeArray, ptr, num);
			return nativeArray;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UnityWebRequestAudioModule", "UnityEngine.UnityWebRequestTextureModule" })]
		internal static void DisposeNativeArray(ref NativeArray<byte> data)
		{
			bool flag = !data.IsCreated;
			if (!flag)
			{
				data = default(NativeArray<byte>);
			}
		}

		internal unsafe static void CreateNativeArrayForNativeData(ref NativeArray<byte> data, byte* bytes, int length)
		{
			data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)bytes, length, Allocator.Persistent);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseFromScripting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDone_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetErrorMsg_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetContentType_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern byte* InternalGetByteArray_Injected(IntPtr dh, out int length);

		[VisibleToOtherModules]
		[NonSerialized]
		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(DownloadHandler handler)
			{
				return handler.m_Ptr;
			}
		}
	}
}
