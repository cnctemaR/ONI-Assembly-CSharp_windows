using System;

namespace UnityEngine.UIElements
{
	public abstract class AbstractGenericMenu
	{
		public abstract void AddItem(string itemName, bool isChecked, Action action);

		public abstract void AddItem(string itemName, bool isChecked, Action<object> action, object data);

		public abstract void AddDisabledItem(string itemName, bool isChecked);

		public abstract void AddSeparator(string path);

		public abstract void DropDown(Rect position, VisualElement targetElement, DropdownMenuSizeMode dropdownMenuSizeMode = DropdownMenuSizeMode.Auto);
	}
}
