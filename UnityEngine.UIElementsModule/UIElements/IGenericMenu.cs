using System;

namespace UnityEngine.UIElements
{
	internal interface IGenericMenu
	{
		void AddItem(string itemName, bool isChecked, Action action);

		void AddItem(string itemName, bool isChecked, Action<object> action, object data);

		void AddDisabledItem(string itemName, bool isChecked);

		void AddSeparator(string path);

		void DropDown(Rect position, VisualElement targetElement = null, bool anchored = false);
	}
}
