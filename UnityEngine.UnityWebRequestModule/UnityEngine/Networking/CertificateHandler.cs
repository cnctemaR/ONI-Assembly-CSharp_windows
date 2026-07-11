using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Responsible for rejecting or accepting certificates received on https requests.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequest/Public/CertificateHandler/CertificateHandlerScript.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class CertificateHandler : IDisposable
	{
		protected CertificateHandler()
		{
			this.m_Ptr = CertificateHandler.Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(CertificateHandler obj);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Release();

		~CertificateHandler()
		{
			this.Dispose();
		}

		/// <summary>
		///   <para>Callback, invoked for each leaf certificate sent by the remote server.</para>
		/// </summary>
		/// <param name="certificateData">Certificate data in PEM or DER format. If certificate data contains multiple certificates, the first one is the leaf certificate.</param>
		/// <returns>
		///   <para>true if the certificate should be accepted, false if not.</para>
		/// </returns>
		protected virtual bool ValidateCertificate(byte[] certificateData)
		{
			return false;
		}

		[RequiredByNativeCode]
		internal bool ValidateCertificateNative(byte[] certificateData)
		{
			return this.ValidateCertificate(certificateData);
		}

		/// <summary>
		///   <para>Signals that this [CertificateHandler] is no longer being used, and should clean up any resources it is using.</para>
		/// </summary>
		public void Dispose()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				this.Release();
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[NonSerialized]
		internal IntPtr m_Ptr;
	}
}
