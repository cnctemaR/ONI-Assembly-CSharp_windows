using System;

namespace System.ComponentModel.Design.Serialization
{
	public class ResolveNameEventArgs : EventArgs
	{
		public ResolveNameEventArgs(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string name;

		private object value;
	}
}
