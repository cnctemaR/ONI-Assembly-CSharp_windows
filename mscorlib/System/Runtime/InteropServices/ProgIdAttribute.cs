using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[ComVisible(true)]
	public sealed class ProgIdAttribute : Attribute
	{
		public ProgIdAttribute(string progId)
		{
			this.pid = progId;
		}

		public string Value
		{
			get
			{
				return this.pid;
			}
		}

		private string pid;
	}
}
