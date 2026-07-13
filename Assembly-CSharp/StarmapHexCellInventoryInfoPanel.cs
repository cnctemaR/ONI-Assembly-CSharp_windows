using System;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarmapHexCellInventoryInfoPanel : SimpleInfoPanel
{
	public StarmapHexCellInventoryInfoPanel(SimpleInfoScreen simpleInfoScreen)
		: base(simpleInfoScreen)
	{
	}

	public override void Refresh(CollapsibleDetailContentPanel panel, GameObject selectedTarget)
	{
		StarmapHexCellInventory starmapHexCellInventory;
		if (!this.IsValidTarget(selectedTarget, out starmapHexCellInventory))
		{
			panel.gameObject.SetActive(false);
			return;
		}
		panel.SetTitle(UI.CLUSTERMAP.HEXCELL_INVENTORY.UI_PANEL.TITLE);
		this.RefreshElements(panel, starmapHexCellInventory);
		panel.gameObject.SetActive(true);
	}

	private void RefreshElements(CollapsibleDetailContentPanel panel, StarmapHexCellInventory hexCellInventory)
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.itemRows)
		{
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.SetActive(false);
			}
		}
		if (hexCellInventory == null)
		{
			return;
		}
		List<StarmapHexCellInventory.SerializedItem> list = new List<StarmapHexCellInventory.SerializedItem>(hexCellInventory.Items);
		list.Sort((StarmapHexCellInventory.SerializedItem a, StarmapHexCellInventory.SerializedItem b) => b.Mass.CompareTo(a.Mass));
		foreach (StarmapHexCellInventory.SerializedItem serializedItem in list)
		{
			Tag id = serializedItem.ID;
			GameObject gameObject;
			if (!this.itemRows.TryGetValue(id, out gameObject))
			{
				gameObject = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, panel.Content.gameObject, true);
				this.itemRows.Add(id, gameObject);
			}
			gameObject.SetActive(true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(id, "ui", false);
			component.GetReference<Image>("Icon").sprite = uisprite.first;
			component.GetReference<Image>("Icon").color = uisprite.second;
			component.GetReference<LocText>("NameLabel").text = (serializedItem.IsEntity ? serializedItem.ID.ProperName() : ElementLoader.GetElement(id).name);
			component.GetReference<LocText>("ValueLabel").text = (serializedItem.IsEntity ? GameUtil.GetFormattedUnits(serializedItem.Mass, GameUtil.TimeSlice.None, true, "") : GameUtil.GetFormattedMass(serializedItem.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		}
	}

	public bool IsValidTarget(GameObject go, out StarmapHexCellInventory hexCellInventory)
	{
		hexCellInventory = null;
		if (go == null)
		{
			return false;
		}
		hexCellInventory = go.GetComponent<StarmapHexCellInventory>();
		return hexCellInventory != null;
	}

	private Dictionary<Tag, GameObject> itemRows = new Dictionary<Tag, GameObject>();
}
