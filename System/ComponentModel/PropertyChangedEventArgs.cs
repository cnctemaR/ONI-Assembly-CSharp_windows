using System;

namespace System.ComponentModel
{
	public class PropertyChangedEventArgs : EventArgs
	{
		public PropertyChangedEventArgs(string propertyName)
		{
			this._propertyName = propertyName;
		}

		public virtual string PropertyName
		{
			get
			{
				return this._propertyName;
			}
		}

		private readonly string _propertyName;
	}
}
