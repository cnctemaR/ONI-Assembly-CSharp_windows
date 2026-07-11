using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Net.Sockets
{
	[Serializable]
	public class SocketException : Win32Exception
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int WSAGetLastError_internal();

		public SocketException()
			: base(SocketException.WSAGetLastError_internal())
		{
		}

		internal SocketException(int error, string message)
			: base(error, message)
		{
		}

		internal SocketException(EndPoint endPoint)
			: base(Marshal.GetLastWin32Error())
		{
			this.m_EndPoint = endPoint;
		}

		public SocketException(int errorCode)
			: base(errorCode)
		{
		}

		internal SocketException(int errorCode, EndPoint endPoint)
			: base(errorCode)
		{
			this.m_EndPoint = endPoint;
		}

		internal SocketException(SocketError socketError)
			: base((int)socketError)
		{
		}

		protected SocketException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public override int ErrorCode
		{
			get
			{
				return base.NativeErrorCode;
			}
		}

		public override string Message
		{
			get
			{
				if (this.m_EndPoint == null)
				{
					return base.Message;
				}
				return base.Message + " " + this.m_EndPoint.ToString();
			}
		}

		public SocketError SocketErrorCode
		{
			get
			{
				return (SocketError)base.NativeErrorCode;
			}
		}

		[NonSerialized]
		private EndPoint m_EndPoint;
	}
}
