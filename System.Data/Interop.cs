using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	private static class Libraries
	{
		internal const string SystemNative = "System.Native";

		internal const string HttpNative = "System.Net.Http.Native";

		internal const string NetSecurityNative = "System.Net.Security.Native";

		internal const string CryptoNative = "System.Security.Cryptography.Native.OpenSsl";

		internal const string GlobalizationNative = "System.Globalization.Native";

		internal const string CompressionNative = "System.IO.Compression.Native";

		internal const string Libdl = "libdl";
	}

	internal static class NetSecurityNative
	{
		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_ReleaseGssBuffer")]
		internal static extern void ReleaseGssBuffer(IntPtr bufferPtr, ulong length);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_DisplayMinorStatus")]
		internal static extern Interop.NetSecurityNative.Status DisplayMinorStatus(out Interop.NetSecurityNative.Status minorStatus, Interop.NetSecurityNative.Status statusValue, ref Interop.NetSecurityNative.GssBuffer buffer);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_DisplayMajorStatus")]
		internal static extern Interop.NetSecurityNative.Status DisplayMajorStatus(out Interop.NetSecurityNative.Status minorStatus, Interop.NetSecurityNative.Status statusValue, ref Interop.NetSecurityNative.GssBuffer buffer);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_ImportUserName")]
		internal static extern Interop.NetSecurityNative.Status ImportUserName(out Interop.NetSecurityNative.Status minorStatus, string inputName, int inputNameByteCount, out SafeGssNameHandle outputName);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_ImportPrincipalName")]
		internal static extern Interop.NetSecurityNative.Status ImportPrincipalName(out Interop.NetSecurityNative.Status minorStatus, string inputName, int inputNameByteCount, out SafeGssNameHandle outputName);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_ReleaseName")]
		internal static extern Interop.NetSecurityNative.Status ReleaseName(out Interop.NetSecurityNative.Status minorStatus, ref IntPtr inputName);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_InitiateCredSpNego")]
		internal static extern Interop.NetSecurityNative.Status InitiateCredSpNego(out Interop.NetSecurityNative.Status minorStatus, SafeGssNameHandle desiredName, out SafeGssCredHandle outputCredHandle);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_InitiateCredWithPassword")]
		internal static extern Interop.NetSecurityNative.Status InitiateCredWithPassword(out Interop.NetSecurityNative.Status minorStatus, bool isNtlm, SafeGssNameHandle desiredName, string password, int passwordLen, out SafeGssCredHandle outputCredHandle);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_ReleaseCred")]
		internal static extern Interop.NetSecurityNative.Status ReleaseCred(out Interop.NetSecurityNative.Status minorStatus, ref IntPtr credHandle);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_InitSecContext")]
		internal static extern Interop.NetSecurityNative.Status InitSecContext(out Interop.NetSecurityNative.Status minorStatus, SafeGssCredHandle initiatorCredHandle, ref SafeGssContextHandle contextHandle, bool isNtlmOnly, SafeGssNameHandle targetName, uint reqFlags, byte[] inputBytes, int inputLength, ref Interop.NetSecurityNative.GssBuffer token, out uint retFlags, out int isNtlmUsed);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_AcceptSecContext")]
		internal static extern Interop.NetSecurityNative.Status AcceptSecContext(out Interop.NetSecurityNative.Status minorStatus, ref SafeGssContextHandle acceptContextHandle, byte[] inputBytes, int inputLength, ref Interop.NetSecurityNative.GssBuffer token);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_DeleteSecContext")]
		internal static extern Interop.NetSecurityNative.Status DeleteSecContext(out Interop.NetSecurityNative.Status minorStatus, ref IntPtr contextHandle);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_Wrap")]
		private static extern Interop.NetSecurityNative.Status Wrap(out Interop.NetSecurityNative.Status minorStatus, SafeGssContextHandle contextHandle, bool isEncrypt, byte[] inputBytes, int offset, int count, ref Interop.NetSecurityNative.GssBuffer outBuffer);

		[DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_Unwrap")]
		private static extern Interop.NetSecurityNative.Status Unwrap(out Interop.NetSecurityNative.Status minorStatus, SafeGssContextHandle contextHandle, byte[] inputBytes, int offset, int count, ref Interop.NetSecurityNative.GssBuffer outBuffer);

		internal static Interop.NetSecurityNative.Status WrapBuffer(out Interop.NetSecurityNative.Status minorStatus, SafeGssContextHandle contextHandle, bool isEncrypt, byte[] inputBytes, int offset, int count, ref Interop.NetSecurityNative.GssBuffer outBuffer)
		{
			return Interop.NetSecurityNative.Wrap(out minorStatus, contextHandle, isEncrypt, inputBytes, offset, count, ref outBuffer);
		}

		internal static Interop.NetSecurityNative.Status UnwrapBuffer(out Interop.NetSecurityNative.Status minorStatus, SafeGssContextHandle contextHandle, byte[] inputBytes, int offset, int count, ref Interop.NetSecurityNative.GssBuffer outBuffer)
		{
			return Interop.NetSecurityNative.Unwrap(out minorStatus, contextHandle, inputBytes, offset, count, ref outBuffer);
		}

		internal sealed class GssApiException : Exception
		{
			public Interop.NetSecurityNative.Status MinorStatus
			{
				get
				{
					return this._minorStatus;
				}
			}

			public GssApiException(string message)
				: base(message)
			{
			}

			public GssApiException(Interop.NetSecurityNative.Status majorStatus, Interop.NetSecurityNative.Status minorStatus)
				: base(Interop.NetSecurityNative.GssApiException.GetGssApiDisplayStatus(majorStatus, minorStatus))
			{
				base.HResult = (int)majorStatus;
				this._minorStatus = minorStatus;
			}

			private static string GetGssApiDisplayStatus(Interop.NetSecurityNative.Status majorStatus, Interop.NetSecurityNative.Status minorStatus)
			{
				string gssApiDisplayStatus = Interop.NetSecurityNative.GssApiException.GetGssApiDisplayStatus(majorStatus, false);
				string gssApiDisplayStatus2 = Interop.NetSecurityNative.GssApiException.GetGssApiDisplayStatus(minorStatus, true);
				if (gssApiDisplayStatus == null || gssApiDisplayStatus2 == null)
				{
					return SR.Format("GSSAPI operation failed with status: {0} (Minor status: {1}).", majorStatus.ToString("x"), minorStatus.ToString("x"));
				}
				return SR.Format("GSSAPI operation failed with error - {0} ({1}).", gssApiDisplayStatus, gssApiDisplayStatus2);
			}

			private static string GetGssApiDisplayStatus(Interop.NetSecurityNative.Status status, bool isMinor)
			{
				Interop.NetSecurityNative.GssBuffer gssBuffer = default(Interop.NetSecurityNative.GssBuffer);
				string text;
				try
				{
					Interop.NetSecurityNative.Status status2;
					text = (((isMinor ? Interop.NetSecurityNative.DisplayMinorStatus(out status2, status, ref gssBuffer) : Interop.NetSecurityNative.DisplayMajorStatus(out status2, status, ref gssBuffer)) != Interop.NetSecurityNative.Status.GSS_S_COMPLETE) ? null : Marshal.PtrToStringAnsi(gssBuffer._data));
				}
				finally
				{
					gssBuffer.Dispose();
				}
				return text;
			}

			private readonly Interop.NetSecurityNative.Status _minorStatus;
		}

		internal struct GssBuffer : IDisposable
		{
			internal int Copy(byte[] destination, int offset)
			{
				if (this._data == IntPtr.Zero || this._length == 0UL)
				{
					return 0;
				}
				int num = Convert.ToInt32(this._length);
				int num2 = destination.Length - offset;
				if (num > num2)
				{
					throw new Interop.NetSecurityNative.GssApiException(SR.Format("Insufficient buffer space. Required: {0} Actual: {1}.", num, num2));
				}
				Marshal.Copy(this._data, destination, offset, num);
				return num;
			}

			internal byte[] ToByteArray()
			{
				if (this._data == IntPtr.Zero || this._length == 0UL)
				{
					return Array.Empty<byte>();
				}
				int num = Convert.ToInt32(this._length);
				byte[] array = new byte[num];
				Marshal.Copy(this._data, array, 0, num);
				return array;
			}

			public void Dispose()
			{
				if (this._data != IntPtr.Zero)
				{
					Interop.NetSecurityNative.ReleaseGssBuffer(this._data, this._length);
					this._data = IntPtr.Zero;
				}
				this._length = 0UL;
			}

			internal ulong _length;

			internal IntPtr _data;
		}

		internal enum Status : uint
		{
			GSS_S_COMPLETE,
			GSS_S_CONTINUE_NEEDED
		}

		[Flags]
		internal enum GssFlags : uint
		{
			GSS_C_DELEG_FLAG = 1U,
			GSS_C_MUTUAL_FLAG = 2U,
			GSS_C_REPLAY_FLAG = 4U,
			GSS_C_SEQUENCE_FLAG = 8U,
			GSS_C_CONF_FLAG = 16U,
			GSS_C_INTEG_FLAG = 32U,
			GSS_C_ANON_FLAG = 64U,
			GSS_C_PROT_READY_FLAG = 128U,
			GSS_C_TRANS_FLAG = 256U,
			GSS_C_DCE_STYLE = 4096U,
			GSS_C_IDENTIFY_FLAG = 8192U,
			GSS_C_EXTENDED_ERROR_FLAG = 16384U,
			GSS_C_DELEG_POLICY_FLAG = 32768U
		}
	}
}
