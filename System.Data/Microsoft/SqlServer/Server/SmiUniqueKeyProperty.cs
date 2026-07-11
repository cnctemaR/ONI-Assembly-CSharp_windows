using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Microsoft.SqlServer.Server
{
	internal class SmiUniqueKeyProperty : SmiMetaDataProperty
	{
		internal SmiUniqueKeyProperty(IList<bool> columnIsKey)
		{
			this._columns = new ReadOnlyCollection<bool>(columnIsKey);
		}

		internal bool this[int ordinal]
		{
			get
			{
				return this._columns.Count > ordinal && this._columns[ordinal];
			}
		}

		[Conditional("DEBUG")]
		internal void CheckCount(int countToMatch)
		{
		}

		private IList<bool> _columns;
	}
}
