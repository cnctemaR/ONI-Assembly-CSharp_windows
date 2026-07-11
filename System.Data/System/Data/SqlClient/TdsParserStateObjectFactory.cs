using System;
using System.Data.SqlClient.SNI;

namespace System.Data.SqlClient
{
	internal sealed class TdsParserStateObjectFactory
	{
		public static bool UseManagedSNI
		{
			get
			{
				return true;
			}
		}

		public EncryptionOptions EncryptionOptions
		{
			get
			{
				return SNILoadHandle.SingletonInstance.Options;
			}
		}

		public uint SNIStatus
		{
			get
			{
				return SNILoadHandle.SingletonInstance.Status;
			}
		}

		public TdsParserStateObject CreateTdsParserStateObject(TdsParser parser)
		{
			return new TdsParserStateObjectManaged(parser);
		}

		internal TdsParserStateObject CreateSessionObject(TdsParser tdsParser, TdsParserStateObject _pMarsPhysicalConObj, bool v)
		{
			return new TdsParserStateObjectManaged(tdsParser, _pMarsPhysicalConObj, true);
		}

		public static readonly TdsParserStateObjectFactory Singleton = new TdsParserStateObjectFactory();
	}
}
