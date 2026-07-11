using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class ImportedFromTypeLibAttribute : Attribute
	{
		public ImportedFromTypeLibAttribute(string tlbFile)
		{
			this._val = tlbFile;
		}

		public string Value
		{
			get
			{
				return this._val;
			}
		}

		internal string _val;
	}
}
