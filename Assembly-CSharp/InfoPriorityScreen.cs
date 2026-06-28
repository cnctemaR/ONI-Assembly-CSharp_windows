using System;
using UnityEngine;

public class InfoPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.InstantiateButtons(new Action<PriorityScreen.PriorityClass, int>(this.OnClick), "STRINGS.UI.PRIORITYSCREEN.USERMENUPRIORITYTOOLTIP", true);
	}

	public void SetTarget(GameObject target)
	{
		if (this.prioritizable != null)
		{
			Prioritizable prioritizable = this.prioritizable;
			prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Remove(prioritizable.onPriorityChanged, new Action<PrioritySetting>(this.OnPriorityChanged));
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
				prioritizable2.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable2.onPriorityChanged, new Action<PrioritySetting>(this.OnPriorityChanged));
				base.gameObject.SetActive(true);
				base.SetScreenPriority(this.prioritizable.GetMasterPriority().priority_class, this.prioritizable.GetMasterPriority().priority_value, false);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	private void OnClick(PriorityScreen.PriorityClass priorityClass, int priority)
	{
		if (this.prioritizable != null)
		{
			this.prioritizable.SetMasterPriority(new PrioritySetting(priorityClass, priority));
		}
		foreach (PriorityButton priorityButton in this.buttons_basic)
		{
			priorityButton.toggle.isOn = priorityButton.priority.Equals(this.prioritizable.GetMasterPriority());
		}
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		base.SetScreenPriority(priority.priority_class, priority.priority_value, false);
	}

	private Prioritizable prioritizable;
}
