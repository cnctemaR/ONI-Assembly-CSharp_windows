using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.SqlServer.Server
{
	internal class SmiOrderProperty : SmiMetaDataProperty
	{
		internal SmiOrderProperty(IList<SmiOrderProperty.SmiColumnOrder> columnOrders)
		{
			this._columns = new ReadOnlyCollection<SmiOrderProperty.SmiColumnOrder>(columnOrders);
		}

		internal SmiOrderProperty.SmiColumnOrder this[int ordinal]
		{
			get
			{
				if (this._columns.Count <= ordinal)
				{
					return new SmiOrderProperty.SmiColumnOrder
					{
						Order = SortOrder.Unspecified,
						SortOrdinal = -1
					};
				}
				return this._columns[ordinal];
			}
		}

		[Conditional("DEBUG")]
		internal void CheckCount(int countToMatch)
		{
		}

		private IList<SmiOrderProperty.SmiColumnOrder> _columns;

		internal struct SmiColumnOrder
		{
			internal int SortOrdinal;

			internal SortOrder Order;
		}
	}
}
