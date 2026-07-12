using System;
using System.Collections.Generic;

public interface IDispenser
{
	List<Tag> DispensedItems();

	Tag SelectedItem();

	void SelectItem(Tag tag);

	void OnOrderDispense();

	void OnCancelDispense();

	bool HasOpenChore();

	event global::System.Action OnStopWorkEvent;
}
