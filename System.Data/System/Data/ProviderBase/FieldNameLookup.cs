using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace System.Data.ProviderBase
{
	internal sealed class FieldNameLookup : BasicFieldNameLookup
	{
		public FieldNameLookup(string[] fieldNames, int defaultLocaleID)
			: base(fieldNames)
		{
			this._defaultLocaleID = defaultLocaleID;
		}

		public FieldNameLookup(ReadOnlyCollection<string> columnNames, int defaultLocaleID)
			: base(columnNames)
		{
			this._defaultLocaleID = defaultLocaleID;
		}

		public FieldNameLookup(IDataReader reader, int defaultLocaleID)
			: base(reader)
		{
			this._defaultLocaleID = defaultLocaleID;
		}

		protected override CompareInfo GetCompareInfo()
		{
			CompareInfo compareInfo = null;
			if (-1 != this._defaultLocaleID)
			{
				compareInfo = CompareInfo.GetCompareInfo(this._defaultLocaleID);
			}
			if (compareInfo == null)
			{
				compareInfo = base.GetCompareInfo();
			}
			return compareInfo;
		}

		private readonly int _defaultLocaleID;
	}
}
