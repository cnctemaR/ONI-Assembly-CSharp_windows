using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class PriorityScreen : KScreen
{
	public List<PriorityButton> InstantiateButtons(Action<int> on_click, string tooltip_str, bool playSelectionSound = true)
	{
		List<PriorityButton> list = new List<PriorityButton>();
		for (int i = 1; i <= 9; i++)
		{
			int idx = i;
			PriorityButton priorityButton = global::Util.KInstantiateUI<PriorityButton>(this.buttonPrefab.gameObject, base.gameObject, false);
			list.Add(priorityButton);
			priorityButton.toggle.onClick += delegate
			{
				if (playSelectionSound)
				{
					this.PlayPriorityConfirmSound(idx);
				}
				on_click(idx);
			};
			priorityButton.text.text = i.ToString();
			priorityButton.priority = idx;
			string text = string.Format(tooltip_str, i);
			priorityButton.tooltip.SetSimpleTooltip(text);
		}
		this.buttonPrefab.gameObject.SetActive(false);
		return list;
	}

	protected void SetScreenPriority(int priority)
	{
		this.priority = priority;
		foreach (PriorityButton priorityButton in this.buttons)
		{
			priorityButton.toggle.isOn = priority == priorityButton.priority;
		}
	}

	public int GetScreenPriority()
	{
		return this.priority;
	}

	public void PlayPriorityConfirmSound(int priority)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Priority_Tool_Confirm", false), Vector3.zero);
		if (eventInstance != null)
		{
			eventInstance.setParameterValue("priority", (float)priority);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	[SerializeField]
	protected PriorityButton buttonPrefab;

	protected List<PriorityButton> buttons = new List<PriorityButton>();

	private int priority;
}
