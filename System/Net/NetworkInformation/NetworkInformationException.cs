using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Net.NetworkInformation
{
	[Serializable]
	public class NetworkInformationException : global::System.ComponentModel.Win32Exception
	{
		public NetworkInformationException()
		{
		}

		public NetworkInformationException(int errorCode)
			: base(errorCode)
		{
		}

		protected NetworkInformationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.error_code = info.GetInt32("ErrorCode");
		}

		public override int ErrorCode
		{
			get
			{
				return this.error_code;
			}
		}

		private int error_code;
	}
}
