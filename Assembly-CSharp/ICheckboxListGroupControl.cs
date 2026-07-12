using System;

public interface ICheckboxListGroupControl
{
	string Title { get; }

	string Description { get; }

	ICheckboxListGroupControl.ListGroup[] GetData();

	bool SidescreenEnabled();

	int CheckboxSideScreenSortOrder();

	public struct ListGroup
	{
		public ListGroup(string title, ICheckboxListGroupControl.CheckboxItem[] checkboxItems, Func<string, string> resolveTitleCallback = null, global::System.Action onItemClicked = null)
		{
			this.title = title;
			this.checkboxItems = checkboxItems;
			this.resolveTitleCallback = resolveTitleCallback;
			this.onItemClicked = onItemClicked;
		}

		public Func<string, string> resolveTitleCallback;

		public global::System.Action onItemClicked;

		public string title;

		public ICheckboxListGroupControl.CheckboxItem[] checkboxItems;
	}

	public struct CheckboxItem
	{
		public string text;

		public string tooltip;

		public bool isOn;

		public Func<string, bool> overrideLinkActions;

		public Func<string, object, string> resolveTooltipCallback;
	}
}
