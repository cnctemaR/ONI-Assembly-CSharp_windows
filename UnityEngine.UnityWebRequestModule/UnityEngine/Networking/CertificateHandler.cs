using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/CertificateHandler/CertificateHandlerScript.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class CertificateHandler : IDisposable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] CertificateHandler obj);

		[NativeMethod(IsThreadSafe = true)]
		private void ReleaseFromScripting()
		{
			IntPtr intPtr = CertificateHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CertificateHandler.ReleaseFromScripting_Injected(intPtr);
		}

		protected CertificateHandler()
		{
			this.m_Ptr = CertificateHandler.Create(this);
		}

		~CertificateHandler()
		{
			this.Dispose();
		}

		protected virtual bool ValidateCertificate(byte[] certificateData)
		{
			return false;
		}

		[RequiredByNativeCode]
		internal bool ValidateCertificateNative(byte[] certificateData)
		{
			return this.ValidateCertificate(certificateData);
		}

		public void Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				this.ReleaseFromScripting();
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseFromScripting_Injected(IntPtr _unity_self);

		[NonSerialized]
		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(CertificateHandler handler)
			{
				return handler.m_Ptr;
			}
		}
	}
}
