using System;

namespace FileHelpers.Options
{
	public sealed class DelimitedRecordOptions : RecordOptions
	{
		internal DelimitedRecordOptions(IRecordInfo info)
			: base(info)
		{
		}

		public string Delimiter
		{
			get
			{
				return ((DelimitedField)this.mRecordInfo.Fields[0]).Separator;
			}
			set
			{
				for (int i = 0; i < this.mRecordInfo.FieldCount; i++)
				{
					((DelimitedField)this.mRecordInfo.Fields[i]).Separator = value;
				}
			}
		}
	}
}
