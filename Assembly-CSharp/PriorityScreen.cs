using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityScreen : KScreen
{
	public List<PriorityButton> InstantiateButtons(Action<int> on_click, string tooltip_str)
	{
		List<PriorityButton> list = new List<PriorityButton>();
		for (int i = 1; i <= 9; i++)
		{
			int idx = i;
			PriorityButton priorityButton = Util.KInstantiateUI<PriorityButton>(this.buttonPrefab.gameObject, base.gameObject, false);
			list.Add(priorityButton);
			priorityButton.toggle.onClick += delegate
			{
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
			bool flag = priority == priorityButton.priority;
			priorityButton.toggle.isOn = flag;
			if (flag)
			{
				priorityButton.GetComponent<ImageToggleState>().SetActive();
			}
			else
			{
				priorityButton.GetComponent<ImageToggleState>().SetInactive();
			}
		}
	}

	public int GetScreenPriority()
	{
		return this.priority;
	}

	[SerializeField]
	protected PriorityButton buttonPrefab;

	protected List<PriorityButton> buttons = new List<PriorityButton>();

	private int priority;
}
