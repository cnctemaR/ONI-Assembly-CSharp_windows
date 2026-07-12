using System;

namespace System.ComponentModel
{
	public class DataErrorsChangedEventArgs : EventArgs
	{
		public DataErrorsChangedEventArgs(string propertyName)
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
