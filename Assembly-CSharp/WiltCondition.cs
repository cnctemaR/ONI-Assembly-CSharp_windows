using System;
using System.Collections.Generic;
using KSerialization;

public class WiltCondition : KMonoBehaviour
{
	public bool IsWilting()
	{
		return this.wilting;
	}

	public List<WiltCondition.Condition> CurrentWiltSources()
	{
		List<WiltCondition.Condition> list = new List<WiltCondition.Condition>();
		foreach (KeyValuePair<int, bool> keyValuePair in this.WiltConditions)
		{
			if (!keyValuePair.Value)
			{
				list.Add((WiltCondition.Condition)keyValuePair.Key);
			}
		}
		return list;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.WiltConditions.Add(0, true);
		this.WiltConditions.Add(1, true);
		this.WiltConditions.Add(2, true);
		this.WiltConditions.Add(3, true);
		this.WiltConditions.Add(4, true);
		this.WiltConditions.Add(5, true);
		this.WiltConditions.Add(6, true);
		this.WiltConditions.Add(7, true);
		this.WiltConditions.Add(9, true);
		this.WiltConditions.Add(10, true);
		base.Subscribe(-107174716, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, false);
		});
		base.Subscribe(-1758196852, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, false);
		});
		base.Subscribe(-1234705021, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, false);
		});
		base.Subscribe(-55477301, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, false);
		});
		base.Subscribe(115888613, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, true);
		});
		base.Subscribe(-593125877, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		base.Subscribe(-1175525437, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		base.Subscribe(-907106982, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, true);
		});
		base.Subscribe(103243573, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		base.Subscribe(646131325, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		base.Subscribe(221594799, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.AtmosphereElement, false);
		});
		base.Subscribe(777259436, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.AtmosphereElement, true);
		});
		base.Subscribe(1949704522, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Drowning, false);
		});
		base.Subscribe(99949694, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Drowning, true);
		});
		base.Subscribe(-2057657673, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.DryingOut, false);
		});
		base.Subscribe(1555379996, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.DryingOut, true);
		});
		base.Subscribe(-370379773, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Irrigation, false);
		});
		base.Subscribe(207387507, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Irrigation, true);
		});
		base.Subscribe(-1073674739, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Fertilized, false);
		});
		base.Subscribe(-1396791468, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Fertilized, true);
		});
		base.Subscribe(1113102781, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.IlluminationComfort, true);
		});
		base.Subscribe(1387626797, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.IlluminationComfort, false);
		});
		base.Subscribe(1628751838, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Receptacle, true);
		});
		base.Subscribe(960378201, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Receptacle, false);
		});
		base.Subscribe(-1089732772, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Entombed, !(bool)data);
		});
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CheckShouldWilt();
		if (this.wilting)
		{
			this.DoWilt();
			if (!this.goingToWilt)
			{
				this.goingToWilt = true;
				this.Recover();
			}
		}
		else
		{
			this.DoRecover();
			if (this.goingToWilt)
			{
				this.goingToWilt = false;
				this.Wilt();
			}
		}
	}

	protected override void OnCleanUp()
	{
		this.wiltSchedulerHandler.ClearScheduler();
		this.recoverSchedulerHandler.ClearScheduler();
		base.OnCleanUp();
	}

	private void SetCondition(WiltCondition.Condition condition, bool satisfiedState)
	{
		if (!this.WiltConditions.ContainsKey((int)condition))
		{
			return;
		}
		this.WiltConditions[(int)condition] = satisfiedState;
		this.CheckShouldWilt();
	}

	private void CheckShouldWilt()
	{
		bool flag = false;
		foreach (KeyValuePair<int, bool> keyValuePair in this.WiltConditions)
		{
			if (!keyValuePair.Value)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (!this.goingToWilt)
			{
				this.Wilt();
			}
		}
		else if (this.goingToWilt)
		{
			this.Recover();
		}
	}

	private void Wilt()
	{
		if (!this.goingToWilt)
		{
			this.goingToWilt = true;
			this.recoverSchedulerHandler.ClearScheduler();
			if (!this.wiltSchedulerHandler.IsValid)
			{
				this.wiltSchedulerHandler = GameScheduler.Instance.Schedule("Wilt", this.WiltDelay, new Action<object>(WiltCondition.DoWiltCallback), this, null);
			}
		}
	}

	private void Recover()
	{
		if (this.goingToWilt)
		{
			this.goingToWilt = false;
			this.wiltSchedulerHandler.ClearScheduler();
			if (!this.recoverSchedulerHandler.IsValid)
			{
				this.recoverSchedulerHandler = GameScheduler.Instance.Schedule("Recover", this.RecoveryDelay, new Action<object>(WiltCondition.DoRecoverCallback), this, null);
			}
		}
	}

	private static void DoWiltCallback(object data)
	{
		((WiltCondition)data).DoWilt();
	}

	private void DoWilt()
	{
		this.wiltSchedulerHandler.ClearScheduler();
		KSelectable component = base.GetComponent<KSelectable>();
		if (!this.wilting)
		{
			this.wilting = true;
			base.Trigger(-724860998, null);
		}
		if (this.growing != null)
		{
			if (this.growing.Replanted)
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingDomestic, base.GetComponent<Growing>());
			}
			else
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.Wilting, base.GetComponent<Growing>());
			}
		}
		else
		{
			ReceptacleMonitor.StatesInstance smi = component.GetSMI<ReceptacleMonitor.StatesInstance>();
			if (smi != null && !smi.IsInsideState(smi.sm.wild))
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowingDomestic, this);
			}
			else
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowing, this);
			}
		}
		component.GetComponent<KPrefabID>().AddTag(GameTags.Wilting);
	}

	public string WiltCausesString()
	{
		string text = string.Empty;
		List<IWiltCause> allSMI = this.GetAllSMI<IWiltCause>();
		allSMI.AddRange(base.GetComponents<IWiltCause>());
		foreach (IWiltCause wiltCause in allSMI)
		{
			foreach (WiltCondition.Condition condition in wiltCause.Conditions)
			{
				if (this.WiltConditions.ContainsKey((int)condition))
				{
					if (!this.WiltConditions[(int)condition])
					{
						text += "\n";
						text += wiltCause.WiltStateString;
						break;
					}
				}
			}
		}
		return text;
	}

	private static void DoRecoverCallback(object data)
	{
		((WiltCondition)data).DoRecover();
	}

	private void DoRecover()
	{
		this.recoverSchedulerHandler.ClearScheduler();
		KSelectable component = base.GetComponent<KSelectable>();
		this.wilting = false;
		base.Trigger(712767498, null);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingDomestic, false);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.Wilting, false);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowing, false);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowingDomestic, false);
		component.GetComponent<KPrefabID>().RemoveTag(GameTags.Wilting);
	}

	[MyCmpGet]
	private Growing growing;

	[Serialize]
	private bool goingToWilt;

	[Serialize]
	private bool wilting;

	private Dictionary<int, bool> WiltConditions = new Dictionary<int, bool>();

	public float WiltDelay = 1f;

	public float RecoveryDelay = 1f;

	private SchedulerHandle wiltSchedulerHandler;

	private SchedulerHandle recoverSchedulerHandler;

	public enum Condition
	{
		Temperature,
		Pressure,
		AtmosphereElement,
		Drowning,
		Fertilized,
		DryingOut,
		Irrigation,
		IlluminationComfort,
		Darkness,
		Receptacle,
		Entombed,
		Count
	}
}
