using System;
using System.Collections;

namespace System.ComponentModel
{
	public interface INotifyDataErrorInfo
	{
		bool HasErrors { get; }

		IEnumerable GetErrors(string propertyName);

		event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
	}
}
