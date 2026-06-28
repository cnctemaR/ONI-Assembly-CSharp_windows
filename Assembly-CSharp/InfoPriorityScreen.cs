using System;
using STRINGS;
using UnityEngine;

public class InfoPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.buttons = base.InstantiateButtons(new Action<int>(this.OnClick), UI.PRIORITYSCREEN.USERMENUPRIORITYTOOLTIP);
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
			if (this.prioritizable != null)
			{
				Clearable component = this.prioritizable.GetComponent<Clearable>();
				if ((this.prioritizable.GetComponent<MinionIdentity>() == null || this.prioritizable.GetComponent<Health>().IsDead()) && (component == null || component.IsMarkedForClear()))
				{
					Prioritizable prioritizable2 = this.prioritizable;
					prioritizable2.onPriorityChanged = (Action<int>)Delegate.Combine(prioritizable2.onPriorityChanged, new Action<int>(this.OnPriorityChanged));
					base.gameObject.SetActive(true);
					base.SetScreenPriority(this.prioritizable.GetMasterPriority());
				}
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
	}

	private void OnPriorityChanged(int priority)
	{
		base.SetScreenPriority(priority);
	}

	private Prioritizable prioritizable;
}
