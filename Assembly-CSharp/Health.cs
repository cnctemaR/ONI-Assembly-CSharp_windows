using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Health : KMonoBehaviour, ISaveLoadable
{
	public float percent()
	{
		return this.hitPoints / this.maxHitPoints;
	}

	public AmountInstance GetAmountInstance
	{
		get
		{
			return this.amountInstance;
		}
	}

	public float hitPoints
	{
		get
		{
			return this.amountInstance.value;
		}
		set
		{
			this.amountInstance.value = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Health.Add(this);
		this.amountInstance = new AmountInstance(Db.Get().Amounts.HitPoints, base.gameObject);
		this.amountInstance.value = this.maxHitPoints;
		base.gameObject.GetAmounts().Add(this.amountInstance);
		AmountInstance amountInstance = this.amountInstance;
		amountInstance.OnDelta = (Action<float>)Delegate.Combine(amountInstance.OnDelta, new Action<float>(this.OnHealthChanged));
	}

	public void SetMaxHitPoints(float _max)
	{
		this.maxHitPoints = _max;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.State == Health.HealthState.Incapacitated || this.hitPoints == 0f)
		{
			if (this.CanBeIncapacitated)
			{
				this.Incapacitate(Db.Get().Deaths.Slain);
			}
			else
			{
				global::Debug.Log("death " + base.gameObject.name, null);
				this.Kill();
			}
		}
		if (this.State != Health.HealthState.Incapacitated && this.State != Health.HealthState.Dead)
		{
			this.UpdateStatus();
		}
		this.effects = base.GetComponent<Effects>();
		this.UpdateHealthBar();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Health.Remove(this);
	}

	public void UpdateHealthBar()
	{
		if (NameDisplayScreen.Instance == null)
		{
			return;
		}
		bool flag = this.State == Health.HealthState.Dead || this.State == Health.HealthState.Incapacitated || this.hitPoints >= this.maxHitPoints;
		NameDisplayScreen.Instance.SetHealthDisplay(base.gameObject, new Func<float>(this.percent), !flag);
	}

	private void Recover()
	{
		base.GetComponent<KPrefabID>().RemoveTag(GameTags.HitPointsDepleted);
	}

	public void OnHealthChanged(float delta)
	{
		if (this.State != Health.HealthState.Invincible)
		{
			if (this.hitPoints == 0f && !this.IsDefeated())
			{
				if (this.CanBeIncapacitated)
				{
					this.Incapacitate(Db.Get().Deaths.Slain);
				}
				else
				{
					this.Kill();
				}
			}
			else
			{
				base.GetComponent<KPrefabID>().RemoveTag(GameTags.HitPointsDepleted);
			}
		}
		this.UpdateStatus();
		this.UpdateWoundEffects();
		this.UpdateHealthBar();
	}

	[ContextMenu("DoDamage")]
	public void DoDamage()
	{
		this.Damage(1f);
	}

	public void Damage(float amount)
	{
		if (this.State != Health.HealthState.Invincible)
		{
			this.hitPoints = Mathf.Max(0f, this.hitPoints - amount);
		}
		base.Trigger(-2121334874, amount);
		this.OnHealthChanged(-amount);
	}

	private void UpdateWoundEffects()
	{
		if (!this.effects)
		{
			return;
		}
		switch (this.State)
		{
		case Health.HealthState.Perfect:
			this.effects.Remove("LightWounds");
			this.effects.Remove("ModerateWounds");
			this.effects.Remove("SevereWounds");
			break;
		case Health.HealthState.Alright:
			this.effects.Remove("LightWounds");
			this.effects.Remove("ModerateWounds");
			this.effects.Remove("SevereWounds");
			break;
		case Health.HealthState.Scuffed:
			this.effects.Remove("ModerateWounds");
			this.effects.Remove("SevereWounds");
			if (!this.effects.HasEffect("LightWounds"))
			{
				this.effects.Add("LightWounds", true);
			}
			break;
		case Health.HealthState.Injured:
			this.effects.Remove("LightWounds");
			this.effects.Remove("SevereWounds");
			if (!this.effects.HasEffect("ModerateWounds"))
			{
				this.effects.Add("ModerateWounds", true);
			}
			break;
		case Health.HealthState.Critical:
			this.effects.Remove("LightWounds");
			this.effects.Remove("ModerateWounds");
			if (!this.effects.HasEffect("SevereWounds"))
			{
				this.effects.Add("SevereWounds", true);
			}
			break;
		case Health.HealthState.Incapacitated:
			this.effects.Remove("LightWounds");
			this.effects.Remove("ModerateWounds");
			this.effects.Remove("SevereWounds");
			break;
		case Health.HealthState.Dead:
			this.effects.Remove("LightWounds");
			this.effects.Remove("ModerateWounds");
			this.effects.Remove("SevereWounds");
			break;
		}
	}

	private void UpdateStatus()
	{
		float num = this.hitPoints / this.maxHitPoints;
		Health.HealthState healthState;
		if (this.State == Health.HealthState.Invincible)
		{
			healthState = Health.HealthState.Invincible;
		}
		else if (num >= 1f)
		{
			healthState = Health.HealthState.Perfect;
		}
		else if (num >= 0.85f)
		{
			healthState = Health.HealthState.Alright;
		}
		else if (num >= 0.66f)
		{
			healthState = Health.HealthState.Scuffed;
		}
		else if ((double)num >= 0.33)
		{
			healthState = Health.HealthState.Injured;
		}
		else if (num > 0f)
		{
			healthState = Health.HealthState.Critical;
		}
		else if (num == 0f)
		{
			healthState = Health.HealthState.Incapacitated;
		}
		else
		{
			healthState = Health.HealthState.Dead;
		}
		if (this.State != healthState)
		{
			if (this.State == Health.HealthState.Incapacitated && healthState != Health.HealthState.Dead)
			{
				this.Recover();
			}
			if (healthState == Health.HealthState.Perfect)
			{
				base.Trigger(-1491582671, this);
			}
			this.State = healthState;
			KSelectable component = base.GetComponent<KSelectable>();
			if (this.State != Health.HealthState.Dead && this.State != Health.HealthState.Perfect && this.State != Health.HealthState.Alright)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, Db.Get().CreatureStatusItems.HealthStatus, this.State);
			}
			else
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, null, null);
			}
		}
	}

	public bool IsIncapacitated()
	{
		return this.State == Health.HealthState.Incapacitated;
	}

	public bool IsDefeated()
	{
		return this.State == Health.HealthState.Incapacitated || this.State == Health.HealthState.Dead;
	}

	public void Incapacitate(Death source_of_death)
	{
		this.State = Health.HealthState.Incapacitated;
		base.GetComponent<KPrefabID>().AddTag(GameTags.HitPointsDepleted);
	}

	private void Kill()
	{
		DeathMonitor.Instance smi = base.gameObject.GetSMI<DeathMonitor.Instance>();
		if (smi != null)
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Slain);
		}
	}

	[Serialize]
	public bool CanBeIncapacitated;

	[Serialize]
	public float maxHitPoints = 100f;

	[Serialize]
	public Health.HealthState State;

	public HealthBar healthBar;

	private Effects effects;

	private AmountInstance amountInstance;

	public enum HealthState
	{
		Perfect,
		Alright,
		Scuffed,
		Injured,
		Critical,
		Incapacitated,
		Dead,
		Invincible
	}
}
