using System;

namespace System.ComponentModel
{
	public class PropertyChangedEventArgs : EventArgs
	{
		public PropertyChangedEventArgs(string name)
		{
			this.propertyName = name;
		}

		public virtual string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private string propertyName;
	}
}
