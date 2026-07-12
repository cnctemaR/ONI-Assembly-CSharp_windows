using System;

namespace Mono.Btls
{
	internal class MonoBtlsX509Exception : Exception
	{
		public MonoBtlsX509Error ErrorCode { get; private set; }

		public string ErrorMessage { get; private set; }

		public MonoBtlsX509Exception(MonoBtlsX509Error code, string message)
			: base(message)
		{
			this.ErrorCode = code;
			this.ErrorMessage = message;
		}

		public override string ToString()
		{
			return string.Format("[MonoBtlsX509Exception: ErrorCode={0}, ErrorMessage={1}]", this.ErrorCode, this.ErrorMessage);
		}
	}
}
