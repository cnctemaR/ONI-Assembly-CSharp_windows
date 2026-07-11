using System;
using System.ComponentModel;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataViewRowStateEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Flags]
	public enum DataViewRowState
	{
		Added = 4,
		CurrentRows = 22,
		Deleted = 8,
		ModifiedCurrent = 16,
		ModifiedOriginal = 32,
		None = 0,
		OriginalRows = 42,
		Unchanged = 2
	}
}
