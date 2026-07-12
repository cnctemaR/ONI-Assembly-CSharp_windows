using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DispenserSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IDispenser>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetDispenser = target.GetComponent<IDispenser>();
		this.Refresh();
	}

	private void Refresh()
	{
		this.dispenseButton.ClearOnClick();
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		this.rows.Clear();
		foreach (Tag tag in this.targetDispenser.DispensedItems())
		{
			GameObject gameObject = Util.KInstantiateUI(this.itemRowPrefab, this.itemRowContainer.gameObject, true);
			this.rows.Add(tag, gameObject);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<Image>("Icon").sprite = Def.GetUISprite(tag, "ui", false).first;
			component.GetReference<LocText>("Label").text = Assets.GetPrefab(tag).GetProperName();
			gameObject.GetComponent<MultiToggle>().ChangeState((tag == this.targetDispenser.SelectedItem()) ? 0 : 1);
		}
		if (this.targetDispenser.HasOpenChore())
		{
			this.dispenseButton.onClick += delegate
			{
				this.targetDispenser.OnCancelDispense();
				this.Refresh();
			};
			this.dispenseButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.DISPENSERSIDESCREEN.BUTTON_CANCEL;
		}
		else
		{
			this.dispenseButton.onClick += delegate
			{
				this.targetDispenser.OnOrderDispense();
				this.Refresh();
			};
			this.dispenseButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.DISPENSERSIDESCREEN.BUTTON_DISPENSE;
		}
		this.targetDispenser.OnStopWorkEvent -= this.Refresh;
		this.targetDispenser.OnStopWorkEvent += this.Refresh;
	}

	private void SelectTag(Tag tag)
	{
		this.targetDispenser.SelectItem(tag);
		this.Refresh();
	}

	[SerializeField]
	private KButton dispenseButton;

	[SerializeField]
	private RectTransform itemRowContainer;

	[SerializeField]
	private GameObject itemRowPrefab;

	private IDispenser targetDispenser;

	private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();
}
