using System;
using System.Collections.Generic;
using KSerialization;

public class WiltCondition : KMonoBehaviour
{
	public bool IsWilting()
	{
		return this.wilting;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.WiltConditions.Add("Temperature", true);
		this.WiltConditions.Add("Pressure", true);
		this.WiltConditions.Add("Drowning", true);
		this.WiltConditions.Add("DryingOut", true);
		this.Subscribe(-107174716, delegate(object data)
		{
			this.SetCondition("Temperature", false);
		});
		this.Subscribe(-1234705021, delegate(object data)
		{
			this.SetCondition("Temperature", false);
		});
		this.Subscribe(115888613, delegate(object data)
		{
			this.SetCondition("Temperature", true);
		});
		this.Subscribe(-1175525437, delegate(object data)
		{
			this.SetCondition("Pressure", false);
		});
		this.Subscribe(-907106982, delegate(object data)
		{
			this.SetCondition("Pressure", true);
		});
		this.Subscribe(1949704522, delegate(object data)
		{
			this.SetCondition("Drowning", false);
		});
		this.Subscribe(99949694, delegate(object data)
		{
			this.SetCondition("Drowning", true);
		});
		this.Subscribe(-2057657673, delegate(object data)
		{
			this.SetCondition("DryingOut", false);
		});
		this.Subscribe(1555379996, delegate(object data)
		{
			this.SetCondition("DryingOut", true);
		});
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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
		this.wiltSchedulerHandler.Clear();
		this.recoverSchedulerHandler.Clear();
		base.OnCleanUp();
	}

	private void SetCondition(string condition, bool satisfiedState)
	{
		if (!this.WiltConditions.ContainsKey(condition))
		{
			return;
		}
		this.WiltConditions[condition] = satisfiedState;
		this.CheckShouldWilt();
	}

	private void CheckShouldWilt()
	{
		bool flag = false;
		foreach (KeyValuePair<string, bool> keyValuePair in this.WiltConditions)
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
			this.recoverSchedulerHandler.Clear();
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
			this.wiltSchedulerHandler.Clear();
			if (!this.recoverSchedulerHandler.IsValid)
			{
				this.recoverSchedulerHandler = GameScheduler.Instance.Schedule("Recover", this.RecoveryDelay, new Action<object>(this.DoRecover), null, null);
			}
		}
	}

	private void DoWilt(object obj)
	{
		this.wiltSchedulerHandler.Clear();
		KSelectable component = base.GetComponent<KSelectable>();
		this.wilting = true;
		this.Trigger(-724860998, null);
		if (this.growing != null && this.growing.Replanted)
		{
			component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingDomestic, null);
		}
		else
		{
			component.AddStatusItem(Db.Get().CreatureStatusItems.Wilting, null);
		}
	}

	private void DoRecover(object obj)
	{
		this.recoverSchedulerHandler.Clear();
		KSelectable component = base.GetComponent<KSelectable>();
		this.wilting = false;
		this.Trigger(712767498, null);
		if (this.growing != null && this.growing.Replanted)
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingDomestic, false);
		}
		else
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.Wilting, false);
		}
	}

	[MyCmpGet]
	private Growing growing;

	[Serialize]
	private bool goingToWilt;

	[Serialize]
	private bool wilting;

	private Dictionary<string, bool> WiltConditions = new Dictionary<string, bool>();

	public float WiltDelay = 5f;

	public float RecoveryDelay = 1f;

	private SchedulerHandle wiltSchedulerHandler;

	private SchedulerHandle recoverSchedulerHandler;
}
