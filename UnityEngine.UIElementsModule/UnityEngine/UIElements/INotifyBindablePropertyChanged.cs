using System;

namespace UnityEngine.UIElements
{
	public interface INotifyBindablePropertyChanged
	{
		event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
	}
}
