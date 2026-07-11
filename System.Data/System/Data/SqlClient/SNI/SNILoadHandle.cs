using System;
using System.Threading;

namespace System.Data.SqlClient.SNI
{
	internal class SNILoadHandle
	{
		public SNIError LastError
		{
			get
			{
				return this._lastError.Value;
			}
			set
			{
				this._lastError.Value = value;
			}
		}

		public uint Status
		{
			get
			{
				return this._status;
			}
		}

		public EncryptionOptions Options
		{
			get
			{
				return this._encryptionOption;
			}
		}

		public static readonly SNILoadHandle SingletonInstance = new SNILoadHandle();

		public readonly EncryptionOptions _encryptionOption;

		public ThreadLocal<SNIError> _lastError = new ThreadLocal<SNIError>(() => new SNIError(SNIProviders.INVALID_PROV, 0U, 0U, string.Empty));

		private readonly uint _status;
	}
}
