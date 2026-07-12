using System;

namespace System.ComponentModel.Design.Serialization
{
	public class ResolveNameEventArgs : EventArgs
	{
		public ResolveNameEventArgs(string name)
		{
			this.Name = name;
			this.Value = null;
		}

		public string Name { get; }

		public object Value { get; set; }
	}
}
