using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MinionEquipmentPanel : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.roomPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.roomPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.POSSESSIONS.GROUPNAME_ROOMS;
		this.roomPanel.SetActive(true);
		this.ownablePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.ownablePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.POSSESSIONS.GROUPNAME_OWNABLE;
		this.ownablePanel.SetActive(true);
	}

	public void SetSelectedMinion(GameObject minion)
	{
		if (this.SelectedMinion != null)
		{
			this.SelectedMinion.Unsubscribe(-1195989806, new Action<object>(this.Refresh));
			this.SelectedMinion.Unsubscribe(-272419061, new Action<object>(this.Refresh));
			this.SelectedMinion.Unsubscribe(-1585839766, new Action<object>(this.Refresh));
		}
		this.SelectedMinion = minion;
		this.SelectedMinion.Subscribe(-1195989806, new Action<object>(this.Refresh));
		this.SelectedMinion.Subscribe(-272419061, new Action<object>(this.Refresh));
		this.SelectedMinion.Subscribe(-1585839766, new Action<object>(this.Refresh));
		this.Refresh(null);
	}

	private void Update()
	{
	}

	public void Refresh(object data = null)
	{
		if (this.SelectedMinion == null)
		{
			return;
		}
		this.Build();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.SelectedMinion != null)
		{
			this.SelectedMinion.Unsubscribe(-1195989806, new Action<object>(this.Refresh));
			this.SelectedMinion.Unsubscribe(-272419061, new Action<object>(this.Refresh));
			this.SelectedMinion.Unsubscribe(-1585839766, new Action<object>(this.Refresh));
		}
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
			gameObject = Util.KInstantiate(this.labelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Build()
	{
		this.ShowAssignables(this.SelectedMinion.GetComponent<Ownables>(), this.roomPanel);
		this.ShowAssignables(this.SelectedMinion.GetComponent<Equipment>(), this.ownablePanel);
	}

	private void ShowAssignables(Assignables assignables, GameObject panel)
	{
		foreach (AssignableSlotInstance assignableSlotInstance in assignables)
		{
			if (assignableSlotInstance.slot.showInUI)
			{
				GameObject gameObject = this.AddOrGetLabel(this.labels, panel, assignableSlotInstance.slot.Name);
				string text = ((!assignableSlotInstance.IsAssigned()) ? UI.DETAILTABS.POSSESSIONS.UNASSIGNED.text : assignableSlotInstance.assignable.GetComponent<KSelectable>().GetName());
				gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", assignableSlotInstance.slot.Name, text);
				if (assignableSlotInstance.IsAssigned())
				{
					gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.POSSESSIONS.ASSIGNED_TOOLTIP, text, this.GetAssignedEffectsString(assignableSlotInstance));
				}
				else
				{
					gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.POSSESSIONS.UNASSIGNED_TOOLTIP, assignableSlotInstance.slot.Name);
				}
			}
		}
	}

	private string GetAssignedEffectsString(AssignableSlotInstance slot)
	{
		string text = string.Empty;
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(GameUtil.GetGameObjectEffects(slot.assignable.gameObject, false));
		if (list.Count > 0)
		{
			text += "\n";
			foreach (Descriptor descriptor in list)
			{
				text = text + descriptor.IndentedText() + "\n";
			}
		}
		return text;
	}

	public GameObject SelectedMinion;

	public GameObject labelTemplate;

	private GameObject roomPanel;

	private GameObject ownablePanel;

	private Storage storage;

	private Dictionary<string, GameObject> labels = new Dictionary<string, GameObject>();
}
