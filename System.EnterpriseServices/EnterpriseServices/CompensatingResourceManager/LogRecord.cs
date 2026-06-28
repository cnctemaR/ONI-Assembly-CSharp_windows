using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	public sealed class LogRecord
	{
		[MonoTODO]
		internal LogRecord()
		{
		}

		[MonoTODO]
		internal LogRecord(_LogRecord logRecord)
		{
			this.flags = (LogRecordFlags)logRecord.dwCrmFlags;
			this.sequence = logRecord.dwSequenceNumber;
			this.record = logRecord.blobUserData;
		}

		public LogRecordFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public object Record
		{
			get
			{
				return this.record;
			}
		}

		public int Sequence
		{
			get
			{
				return this.sequence;
			}
		}

		private LogRecordFlags flags;

		private object record;

		private int sequence;
	}
}
