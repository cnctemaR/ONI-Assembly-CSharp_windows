using System;
using System.Diagnostics;

namespace FileHelpers.Options
{
	public sealed class FixedRecordOptions : RecordOptions
	{
		internal FixedRecordOptions(IRecordInfo info)
			: base(info)
		{
		}

		public FixedMode FixedMode
		{
			get
			{
				return ((FixedLengthField)this.mRecordInfo.Fields[0]).FixedMode;
			}
			set
			{
				for (int i = 0; i < this.mRecordInfo.FieldCount; i++)
				{
					((FixedLengthField)this.mRecordInfo.Fields[i]).FixedMode = value;
				}
			}
		}

		public int RecordLength
		{
			get
			{
				if (this.mRecordLength != -2147483648)
				{
					return this.mRecordLength;
				}
				this.mRecordLength = 0;
				foreach (FixedLengthField fixedLengthField in this.mRecordInfo.Fields)
				{
					this.mRecordLength += fixedLengthField.FieldLength;
				}
				return this.mRecordLength;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int mRecordLength = int.MinValue;
	}
}
