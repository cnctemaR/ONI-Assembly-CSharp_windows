using System;
using System.Runtime.Serialization;

namespace System.EnterpriseServices
{
	[Serializable]
	public sealed class RegistrationException : SystemException
	{
		[MonoTODO]
		public RegistrationException(string msg)
			: base(msg)
		{
		}

		public RegistrationException()
			: this("Registration error")
		{
		}

		public RegistrationException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public RegistrationErrorInfo[] ErrorInfo
		{
			get
			{
				return this.errorInfo;
			}
		}

		[MonoTODO]
		public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
		{
			throw new NotImplementedException();
		}

		private RegistrationErrorInfo[] errorInfo;
	}
}
