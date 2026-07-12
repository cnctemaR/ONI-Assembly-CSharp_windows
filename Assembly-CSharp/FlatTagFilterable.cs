using System;
using System.Collections.Generic;
using KSerialization;

public class FlatTagFilterable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		component.filterByStorageCategoriesOnSpawn = false;
		component.UpdateFilters(this.selectedTags);
	}

	public void SelectTag(Tag tag, bool state)
	{
		Debug.Assert(this.tagOptions.Contains(tag), "The tag " + tag.Name + " is not valid for this filterable - it must be added to tagOptions");
		if (state)
		{
			if (!this.selectedTags.Contains(tag))
			{
				this.selectedTags.Add(tag);
			}
		}
		else if (this.selectedTags.Contains(tag))
		{
			this.selectedTags.Remove(tag);
		}
		base.GetComponent<TreeFilterable>().UpdateFilters(this.selectedTags);
	}

	public void ToggleTag(Tag tag)
	{
		this.SelectTag(tag, !this.selectedTags.Contains(tag));
	}

	public string GetHeaderText()
	{
		return this.headerText;
	}

	[Serialize]
	public List<Tag> selectedTags = new List<Tag>();

	public List<Tag> tagOptions = new List<Tag>();

	public string headerText;
}
