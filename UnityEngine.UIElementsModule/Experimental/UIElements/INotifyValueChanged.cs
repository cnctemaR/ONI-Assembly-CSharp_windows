using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface INotifyValueChanged<T>
	{
		T value { get; set; }

		void SetValueAndNotify(T newValue);

		void OnValueChanged(EventCallback<ChangeEvent<T>> callback);
	}
}
