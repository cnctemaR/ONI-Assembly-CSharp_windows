using System;
using STRINGS;
using UnityEngine;

public class InfoPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.buttons = base.InstantiateButtons(new Action<int>(this.OnClick), UI.PRIORITYSCREEN.USERMENUPRIORITYTOOLTIP, true);
	}

	public void SetTarget(GameObject target)
	{
		if (this.prioritizable != null)
		{
			Prioritizable prioritizable = this.prioritizable;
			prioritizable.onPriorityChanged = (Action<int>)Delegate.Remove(prioritizable.onPriorityChanged, new Action<int>(this.OnPriorityChanged));
		}
		if (target == null)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			this.prioritizable = target.GetComponent<Prioritizable>();
			if (this.prioritizable != null && this.prioritizable.IsPrioritizable())
			{
				Prioritizable prioritizable2 = this.prioritizable;
				prioritizable2.onPriorityChanged = (Action<int>)Delegate.Combine(prioritizable2.onPriorityChanged, new Action<int>(this.OnPriorityChanged));
				base.gameObject.SetActive(true);
				base.SetScreenPriority(this.prioritizable.GetMasterPriority(), false);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	private void OnClick(int priority)
	{
		if (this.prioritizable != null)
		{
			this.prioritizable.SetMasterPriority(priority);
		}
		foreach (PriorityButton priorityButton in this.buttons)
		{
			priorityButton.toggle.isOn = priorityButton.priority == this.prioritizable.GetMasterPriority();
		}
	}

	private void OnPriorityChanged(int priority)
	{
		base.SetScreenPriority(priority, false);
	}

	private Prioritizable prioritizable;
}
