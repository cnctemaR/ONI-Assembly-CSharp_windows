using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class FlatTagFilterable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		component.filterByStorageCategoriesOnSpawn = false;
		component.UpdateFilters(new HashSet<Tag>(this.selectedTags));
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	public void SelectTag(Tag tag, bool state)
	{
		global::Debug.Assert(this.tagOptions.Contains(tag), "The tag " + tag.Name + " is not valid for this filterable - it must be added to tagOptions");
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
		base.GetComponent<TreeFilterable>().UpdateFilters(new HashSet<Tag>(this.selectedTags));
	}

	public void ToggleTag(Tag tag)
	{
		this.SelectTag(tag, !this.selectedTags.Contains(tag));
	}

	public string GetHeaderText()
	{
		return this.headerText;
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (base.GetComponent<KPrefabID>().PrefabID() != gameObject.GetComponent<KPrefabID>().PrefabID())
		{
			return;
		}
		this.selectedTags.Clear();
		foreach (Tag tag in gameObject.GetComponent<FlatTagFilterable>().selectedTags)
		{
			this.SelectTag(tag, true);
		}
		base.GetComponent<TreeFilterable>().UpdateFilters(new HashSet<Tag>(this.selectedTags));
	}

	[Serialize]
	public List<Tag> selectedTags = new List<Tag>();

	public List<Tag> tagOptions = new List<Tag>();

	public string headerText;

	public bool displayOnlyDiscoveredTags = true;
}
