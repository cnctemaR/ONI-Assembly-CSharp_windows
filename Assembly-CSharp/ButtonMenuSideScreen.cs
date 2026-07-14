using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenuSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		ISidescreenButtonControl sidescreenButtonControl = target.GetComponent<ISidescreenButtonControl>();
		if (sidescreenButtonControl == null)
		{
			sidescreenButtonControl = target.GetSMI<ISidescreenButtonControl>();
		}
		return sidescreenButtonControl != null && sidescreenButtonControl.SidescreenEnabled();
	}

	public override int GetSideScreenSortOrder()
	{
		if (this.targets == null)
		{
			return 20;
		}
		return this.targets[0].ButtonSideScreenSortOrder();
	}

	public override string GetTitle()
	{
		if (this.targets != null)
		{
			foreach (ISidescreenButtonControl sidescreenButtonControl in this.targets)
			{
				string sidescreenTitle = sidescreenButtonControl.SidescreenTitle;
				if (sidescreenTitle != null)
				{
					return sidescreenTitle;
				}
			}
		}
		return base.GetTitle();
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.targets = new_target.GetAllSMI<ISidescreenButtonControl>();
		this.targets.AddRange(new_target.GetComponents<ISidescreenButtonControl>());
		this.Refresh();
	}

	public GameObject GetHorizontalGroup(int id)
	{
		if (!this.horizontalGroups.ContainsKey(id))
		{
			this.horizontalGroups.Add(id, Util.KInstantiateUI(this.horizontalGroupPrefab, this.buttonContainer.gameObject, true));
		}
		return this.horizontalGroups[id];
	}

	public void CopyLayoutSettings(LayoutElement to, LayoutElement from)
	{
		to.ignoreLayout = from.ignoreLayout;
		to.minWidth = from.minWidth;
		to.minHeight = from.minHeight;
		to.preferredWidth = from.preferredWidth;
		to.preferredHeight = from.preferredHeight;
		to.flexibleWidth = from.flexibleWidth;
		to.flexibleHeight = from.flexibleHeight;
		to.layoutPriority = from.layoutPriority;
	}

	private void Refresh()
	{
		while (this.liveButtons.Count < this.targets.Count)
		{
			this.liveButtons.Add(Util.KInstantiateUI(this.buttonPrefab.gameObject, this.buttonContainer.gameObject, true));
		}
		foreach (int num in this.horizontalGroups.Keys)
		{
			this.horizontalGroups[num].SetActive(false);
		}
		for (int i = 0; i < this.liveButtons.Count; i++)
		{
			if (i >= this.targets.Count)
			{
				this.liveButtons[i].SetActive(false);
			}
			else
			{
				if (!this.liveButtons[i].activeSelf)
				{
					this.liveButtons[i].SetActive(true);
				}
				int num2 = this.targets[i].HorizontalGroupID();
				LayoutElement component = this.liveButtons[i].GetComponent<LayoutElement>();
				KButton componentInChildren = this.liveButtons[i].GetComponentInChildren<KButton>();
				ToolTip componentInChildren2 = this.liveButtons[i].GetComponentInChildren<ToolTip>();
				LocText componentInChildren3 = this.liveButtons[i].GetComponentInChildren<LocText>();
				if (num2 >= 0)
				{
					GameObject horizontalGroup = this.GetHorizontalGroup(num2);
					horizontalGroup.SetActive(true);
					this.liveButtons[i].transform.SetParent(horizontalGroup.transform, false);
					this.CopyLayoutSettings(component, this.horizontalButtonPrefab);
				}
				else
				{
					this.liveButtons[i].transform.SetParent(this.buttonContainer, false);
					this.CopyLayoutSettings(component, this.buttonPrefab);
				}
				componentInChildren.isInteractable = this.targets[i].SidescreenButtonInteractable();
				componentInChildren.ClearOnClick();
				componentInChildren.onClick += this.targets[i].OnSidescreenButtonPressed;
				componentInChildren.onClick += this.Refresh;
				componentInChildren3.SetText(this.targets[i].SidescreenButtonText);
				componentInChildren2.SetSimpleTooltip(this.targets[i].SidescreenButtonTooltip);
			}
		}
	}

	public const int DefaultButtonMenuSideScreenSortOrder = 20;

	public LayoutElement buttonPrefab;

	public LayoutElement horizontalButtonPrefab;

	public GameObject horizontalGroupPrefab;

	public RectTransform buttonContainer;

	private List<GameObject> liveButtons = new List<GameObject>();

	private Dictionary<int, GameObject> horizontalGroups = new Dictionary<int, GameObject>();

	private List<ISidescreenButtonControl> targets;
}
