using System;
using System.Collections.Generic;
using UnityEngine;

public class SingleItemSelectionSideScreen : SingleItemSelectionSideScreenBase
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<StorageTile.Instance>() != null && target.GetComponent<TreeFilterable>() != null;
	}

	private Tag GetTargetCurrentSelectedTag()
	{
		if (this.CurrentTarget != null)
		{
			return this.CurrentTarget.TargetTag;
		}
		return this.INVALID_OPTION_TAG;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.CurrentTarget = target.GetSMI<StorageTile.Instance>();
		if (this.CurrentTarget != null)
		{
			Dictionary<Tag, HashSet<Tag>> dictionary = new Dictionary<Tag, HashSet<Tag>>();
			foreach (Tag tag in new HashSet<Tag>(this.CurrentTarget.GetComponent<Storage>().storageFilters))
			{
				HashSet<Tag> discoveredResourcesFromTag = DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(tag);
				if (discoveredResourcesFromTag != null && discoveredResourcesFromTag.Count > 0)
				{
					dictionary.Add(tag, discoveredResourcesFromTag);
				}
			}
			this.SetData(dictionary);
			SingleItemSelectionSideScreenBase.Category category = null;
			if (!this.categories.TryGetValue(this.INVALID_OPTION_TAG, out category))
			{
				category = base.GetCategoryWithItem(this.INVALID_OPTION_TAG, false);
			}
			if (category != null)
			{
				category.SetProihibedState(true);
			}
			this.CreateNoneOption();
			Tag targetCurrentSelectedTag = this.GetTargetCurrentSelectedTag();
			if (targetCurrentSelectedTag != this.INVALID_OPTION_TAG)
			{
				this.SetSelectedItem(targetCurrentSelectedTag);
				base.GetCategoryWithItem(targetCurrentSelectedTag, false).SetUnfoldedState(SingleItemSelectionSideScreenBase.Category.UnfoldedStates.Unfolded);
			}
			else
			{
				this.SetSelectedItem(this.noneOptionRow);
			}
			this.selectedItemLabel.SetItem(targetCurrentSelectedTag);
		}
	}

	private void CreateNoneOption()
	{
		if (this.noneOptionRow == null)
		{
			this.noneOptionRow = this.GetOrCreateItemRow(this.INVALID_OPTION_TAG);
		}
		this.noneOptionRow.transform.SetAsFirstSibling();
	}

	public override void ItemRowClicked(SingleItemSelectionRow rowClicked)
	{
		base.ItemRowClicked(rowClicked);
		this.selectedItemLabel.SetItem(rowClicked.tag);
		Tag targetCurrentSelectedTag = this.GetTargetCurrentSelectedTag();
		if (this.CurrentTarget != null && targetCurrentSelectedTag != rowClicked.tag)
		{
			this.CurrentTarget.SetTargetItem(rowClicked.tag);
		}
	}

	[SerializeField]
	private SingleItemSelectionSideScreen_SelectedItemSection selectedItemLabel;

	private StorageTile.Instance CurrentTarget;

	private SingleItemSelectionRow noneOptionRow;

	private Tag INVALID_OPTION_TAG = GameTags.Void;
}
