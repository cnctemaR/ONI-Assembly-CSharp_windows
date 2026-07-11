using System;

namespace System.ComponentModel
{
	public class PropertyChangingEventArgs : EventArgs
	{
		public PropertyChangingEventArgs(string propertyName)
		{
			this.name = propertyName;
		}

		public virtual string PropertyName
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
