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
		foreach (KeyValuePair<WiltCondition.Condition, bool> keyValuePair in this.WiltConditions)
		{
			if (!keyValuePair.Value)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.WiltConditions.Add(WiltCondition.Condition.Temperature, true);
		this.WiltConditions.Add(WiltCondition.Condition.Pressure, true);
		this.WiltConditions.Add(WiltCondition.Condition.AtmosphereElement, true);
		this.WiltConditions.Add(WiltCondition.Condition.Drowning, true);
		this.WiltConditions.Add(WiltCondition.Condition.Fertilized, true);
		this.WiltConditions.Add(WiltCondition.Condition.DryingOut, true);
		this.WiltConditions.Add(WiltCondition.Condition.Irrigation, true);
		this.WiltConditions.Add(WiltCondition.Condition.IlluminationComfort, true);
		this.WiltConditions.Add(WiltCondition.Condition.Receptacle, true);
		this.WiltConditions.Add(WiltCondition.Condition.Entombed, true);
		this.Subscribe(-107174716, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, false);
		});
		this.Subscribe(-1234705021, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, false);
		});
		this.Subscribe(115888613, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Temperature, true);
		});
		this.Subscribe(-593125877, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		this.Subscribe(-1175525437, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		this.Subscribe(-907106982, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, true);
		});
		this.Subscribe(103243573, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		this.Subscribe(646131325, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Pressure, false);
		});
		this.Subscribe(221594799, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.AtmosphereElement, false);
		});
		this.Subscribe(777259436, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.AtmosphereElement, true);
		});
		this.Subscribe(1949704522, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Drowning, false);
		});
		this.Subscribe(99949694, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Drowning, true);
		});
		this.Subscribe(-2057657673, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.DryingOut, false);
		});
		this.Subscribe(1555379996, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.DryingOut, true);
		});
		this.Subscribe(-370379773, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Irrigation, false);
		});
		this.Subscribe(207387507, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Irrigation, true);
		});
		this.Subscribe(-1073674739, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Fertilized, false);
		});
		this.Subscribe(-1396791468, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Fertilized, true);
		});
		this.Subscribe(1113102781, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.IlluminationComfort, true);
		});
		this.Subscribe(1387626797, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.IlluminationComfort, false);
		});
		this.Subscribe(1628751838, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Receptacle, true);
		});
		this.Subscribe(960378201, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Receptacle, false);
		});
		this.Subscribe(-1089732772, delegate(object data)
		{
			this.SetCondition(WiltCondition.Condition.Entombed, !(bool)data);
		});
	}

	private void Update()
	{
		if (this.wilt_condition_dirty)
		{
			this.CheckShouldWilt();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CheckShouldWilt();
		if (this.wilting)
		{
			this.DoWilt(null);
			if (!this.goingToWilt)
			{
				this.goingToWilt = true;
				this.Recover();
			}
		}
		else
		{
			this.DoRecover(null);
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
		if (!this.WiltConditions.ContainsKey(condition))
		{
			return;
		}
		this.WiltConditions[condition] = satisfiedState;
		this.wilt_condition_dirty = true;
	}

	private void CheckShouldWilt()
	{
		this.wilt_condition_dirty = false;
		bool flag = false;
		foreach (KeyValuePair<WiltCondition.Condition, bool> keyValuePair in this.WiltConditions)
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
				this.wiltSchedulerHandler = GameScheduler.Instance.Schedule("Wilt", this.WiltDelay, new Action<object>(this.DoWilt), null, null);
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
				this.recoverSchedulerHandler = GameScheduler.Instance.Schedule("Recover", this.RecoveryDelay, new Action<object>(this.DoRecover), null, null);
			}
		}
	}

	private void DoWilt(object obj)
	{
		this.wiltSchedulerHandler.ClearScheduler();
		KSelectable component = base.GetComponent<KSelectable>();
		if (!this.wilting)
		{
			this.wilting = true;
			this.Trigger(-724860998, null);
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
				if (this.WiltConditions.ContainsKey(condition))
				{
					if (!this.WiltConditions[condition])
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

	private void DoRecover(object obj)
	{
		this.recoverSchedulerHandler.ClearScheduler();
		KSelectable component = base.GetComponent<KSelectable>();
		this.wilting = false;
		this.Trigger(712767498, null);
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

	private bool wilt_condition_dirty;

	private Dictionary<WiltCondition.Condition, bool> WiltConditions = new Dictionary<WiltCondition.Condition, bool>();

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
