using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Microsoft.SqlServer.Server
{
	internal class SmiDefaultFieldsProperty : SmiMetaDataProperty
	{
		internal SmiDefaultFieldsProperty(IList<bool> defaultFields)
		{
			this._defaults = new ReadOnlyCollection<bool>(defaultFields);
		}

		internal bool this[int ordinal]
		{
			get
			{
				return this._defaults.Count > ordinal && this._defaults[ordinal];
			}
		}

		[Conditional("DEBUG")]
		internal void CheckCount(int countToMatch)
		{
		}

		private IList<bool> _defaults;
	}
}
