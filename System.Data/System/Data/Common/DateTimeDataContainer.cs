using System;

namespace System.Data.Common
{
	internal sealed class DateTimeDataContainer : ObjectDataContainer
	{
		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			base.SetValue(index, record.GetDateTimeSafe(field));
		}

		protected override void SetValue(int index, object value)
		{
			base.SetValue(index, base.GetContainerData(value));
		}
	}
}
