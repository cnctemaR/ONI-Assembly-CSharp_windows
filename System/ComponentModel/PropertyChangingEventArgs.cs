using System;

namespace System.ComponentModel
{
	public class PropertyChangingEventArgs : EventArgs
	{
		public PropertyChangingEventArgs(string propertyName)
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
