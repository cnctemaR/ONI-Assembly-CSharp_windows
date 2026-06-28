using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AdditionalDetailsPanel : TargetScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.detailsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.storagePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
	}

	private void Update()
	{
		this.Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		base.Subscribe(target, -1697596308, new EventSystem.EventHandler(this.OnStorageChange));
		this.Refresh();
		this.RefreshStorage();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
		if (target != null)
		{
			base.Unsubscribe(target, -1697596308, new EventSystem.EventHandler(this.OnStorageChange));
		}
	}

	private void OnStorageChange(object data)
	{
		this.RefreshStorage();
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(this.attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Refresh()
	{
		this.RefreshDetails();
	}

	private void RefreshDetails()
	{
		this.detailsPanel.SetActive(true);
		this.detailsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_DETAILS;
		PrimaryElement component = this.selectedTarget.GetComponent<PrimaryElement>();
		CellSelectionObject component2 = this.selectedTarget.GetComponent<CellSelectionObject>();
		float num;
		float num2;
		float num3;
		if (component != null)
		{
			num = component.Mass;
			num2 = component.Element.specificHeatCapacity;
			num3 = component.Element.thermalConductivity;
		}
		else
		{
			if (!(component2 != null))
			{
				return;
			}
			num = component2.Mass;
			num2 = component2.element.specificHeatCapacity;
			num3 = component2.element.thermalConductivity;
		}
		GameObject gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Mass");
		gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, true, "F1"));
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, true, "F1"));
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "SHC");
		gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", UI.ELEMENTAL.SHC.NAME, num2);
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.SHC.TOOLTIP, num2);
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "THERMALCONDUCTIVITY");
		gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, num3);
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP, num3);
	}

	private void RefreshStorage()
	{
		if (this.selectedTarget == null || this.selectedTarget.GetComponent<Storage>() == null)
		{
			this.storagePanel.gameObject.SetActive(false);
			return;
		}
		this.storagePanel.gameObject.SetActive(true);
		if (this.selectedTarget.GetComponent<MinionIdentity>())
		{
			this.storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS;
		}
		else
		{
			this.storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS;
		}
		Dictionary<string, AdditionalDetailsPanel.StorageEntry> dictionary = new Dictionary<string, AdditionalDetailsPanel.StorageEntry>();
		foreach (Storage storage in this.selectedTarget.GetComponents<Storage>())
		{
			this.CollectItems(storage, ref dictionary);
		}
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.storageLabels)
		{
			keyValuePair.Value.SetActive(false);
		}
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<string, AdditionalDetailsPanel.StorageEntry> keyValuePair2 in dictionary)
			{
				GameObject gameObject = this.AddOrGetLabel(this.storageLabels, this.storagePanel, keyValuePair2.Key);
				PrimaryElement component = keyValuePair2.Value.gameObject.GetComponent<PrimaryElement>();
				if (keyValuePair2.Value.IsMass || component)
				{
					gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", keyValuePair2.Key, GameUtil.GetFormattedMass(keyValuePair2.Value.Amount, GameUtil.TimeSlice.None, true, "F1"));
				}
				else
				{
					gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", keyValuePair2.Key, keyValuePair2.Value.Amount.ToString());
				}
			}
		}
		else
		{
			GameObject gameObject2 = this.AddOrGetLabel(this.storageLabels, this.storagePanel, "empty");
			gameObject2.GetComponent<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
		}
	}

	private void CollectItems(Storage storage, ref Dictionary<string, AdditionalDetailsPanel.StorageEntry> item_counts)
	{
		foreach (GameObject gameObject in storage.items)
		{
			if (!(gameObject == null))
			{
				string text = gameObject.name;
				KSelectable component = gameObject.GetComponent<KSelectable>();
				if (component != null)
				{
					text = component.GetName();
				}
				if (text != null)
				{
					float totalAmount = gameObject.GetComponent<Pickupable>().TotalAmount;
					if (item_counts.ContainsKey(text))
					{
						item_counts[text].Amount = item_counts[text].Amount + totalAmount;
					}
					else
					{
						item_counts[text] = new AdditionalDetailsPanel.StorageEntry
						{
							Amount = totalAmount,
							IsMass = (gameObject.GetComponent<ElementChunk>() != null),
							gameObject = gameObject
						};
					}
				}
			}
		}
	}

	public GameObject attributesLabelTemplate;

	private GameObject detailsPanel;

	private GameObject storagePanel;

	private Dictionary<string, GameObject> detailLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	private class StorageEntry
	{
		public float Amount;

		public bool IsMass;

		public GameObject gameObject;
	}
}
