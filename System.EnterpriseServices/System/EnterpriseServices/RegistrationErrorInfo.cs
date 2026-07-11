using System;
using Unity;

namespace System.EnterpriseServices
{
	[Serializable]
	public sealed class RegistrationErrorInfo
	{
		[MonoTODO]
		internal RegistrationErrorInfo(string name, string majorRef, string minorRef, int errorCode)
		{
			this.name = name;
			this.majorRef = majorRef;
			this.minorRef = minorRef;
			this.errorCode = errorCode;
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public string ErrorString
		{
			get
			{
				return this.errorString;
			}
		}

		public string MajorRef
		{
			get
			{
				return this.majorRef;
			}
		}

		public string MinorRef
		{
			get
			{
				return this.minorRef;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		internal RegistrationErrorInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private int errorCode;

		private string errorString;

		private string majorRef;

		private string minorRef;

		private string name;
	}
}
