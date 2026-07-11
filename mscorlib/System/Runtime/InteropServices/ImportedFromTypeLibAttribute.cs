using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class ImportedFromTypeLibAttribute : Attribute
	{
		public ImportedFromTypeLibAttribute(string tlbFile)
		{
			this.TlbFile = tlbFile;
		}

		public string Value
		{
			get
			{
				return this.TlbFile;
			}
		}

		private string TlbFile;
	}
}
