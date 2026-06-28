using System;
using System.ComponentModel;
using System.Diagnostics;

namespace FileHelpers.MasterDetail
{
	[DebuggerDisplay("Master: {Master.ToString()} - Details: {Details.Length}")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MasterDetails : MasterDetails<object, object>
	{
		public MasterDetails(object master, object[] details)
			: base(master, details)
		{
		}
	}
}
