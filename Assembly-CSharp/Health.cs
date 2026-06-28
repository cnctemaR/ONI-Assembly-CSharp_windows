using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Health : KMonoBehaviour, ISaveLoadableJson
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
		this.amountInstance.maxAttribute.Add("Base", new AttributeModifier(this.amountInstance.maxAttribute.Id, this.maxHitPoints, null, false));
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
		if (this.IsDead())
		{
			this.ApplyDeath();
		}
		this.UpdateStatus();
		this.effects = base.GetComponent<Effects>();
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Health.Remove(this);
	}

	protected override void OnCmpEnable()
	{
		if (this.IsDead())
		{
			this.ApplyDeath();
		}
	}

	public void UpdateHealthBar()
	{
		if (this.State == Health.HealthState.Dead || this.State == Health.HealthState.Incapacitated)
		{
			if (NameDisplayScreen.Instance != null)
			{
				NameDisplayScreen.Instance.SetHealthDisplay(base.gameObject, new Func<float>(this.percent), false);
			}
			return;
		}
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetHealthDisplay(base.gameObject, new Func<float>(this.percent), true);
		}
	}

	public bool Bleed(float dt)
	{
		if (this.State == Health.HealthState.Incapacitated)
		{
			this.bleedOutStamina -= dt * Health.baseBleedOutSpeed;
			if (this.bleedOutStamina <= 0f)
			{
				this.State = Health.HealthState.Dead;
				this.Kill(Db.Get().Deaths.Generic);
				return true;
			}
		}
		return false;
	}

	private void Recover()
	{
		this.Trigger(-1256572400, null);
		this.bleedOutStamina = this.maxBleedOutStamina;
	}

	public float GetBleedLifeTime()
	{
		return Mathf.Floor(this.bleedOutStamina / Health.baseBleedOutSpeed);
	}

	public void OnHealthChanged(float delta)
	{
		if (this.State != Health.HealthState.Invincible && this.hitPoints == 0f && !this.IsDefeated())
		{
			if (this.CanBeIncapacitated)
			{
				this.Incapacitate();
			}
			else
			{
				this.Kill(Db.Get().Deaths.Generic);
			}
		}
		this.UpdateStatus();
		this.UpdateWoundEffects();
		this.UpdateHealthBar();
	}

	public void Damage(float amount)
	{
		if (this.State != Health.HealthState.Invincible)
		{
			this.hitPoints = Mathf.Max(0f, this.hitPoints - amount);
		}
		this.Trigger(-2121334874, amount);
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
			if (this.State == Health.HealthState.Incapacitated && healthState != Health.HealthState.Incapacitated && healthState != Health.HealthState.Dead)
			{
				this.Recover();
			}
			if (this.State == Health.HealthState.Perfect)
			{
				this.Trigger(-1491582671, this);
			}
			this.State = healthState;
			KSelectable component = base.GetComponent<KSelectable>();
			if (this.State != Health.HealthState.Dead && this.State != Health.HealthState.Perfect)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, Db.Get().CreatureStatusItems.HealthStatus, this.State);
			}
			else
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, null, null);
			}
		}
	}

	public bool IsDead()
	{
		return this.death.Get() != null;
	}

	public bool IsDefeated()
	{
		return this.State == Health.HealthState.Incapacitated || this.State == Health.HealthState.Dead;
	}

	private void ApplyDeath()
	{
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Dead, this.death.Get());
		Pickupable component = base.GetComponent<Pickupable>();
		if (component != null)
		{
			component.RegisterListeners();
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Corpse);
	}

	public void Incapacitate()
	{
		IncapacitationMonitor.Instance smi = this.GetSMI<IncapacitationMonitor.Instance>();
		smi.Incapacitate();
	}

	public void Kill(Death death)
	{
		DeathMonitor.Instance smi = this.GetSMI<DeathMonitor.Instance>();
		if (smi != null)
		{
			smi.Kill(death);
		}
		else
		{
			this.KillImmediate(death);
		}
		this.hitPoints = 0f;
	}

	public void KillImmediate(Death death)
	{
		Game.Instance.Trigger(1623392196, base.gameObject);
		this.death.Set(death);
		this.ApplyDeath();
		this.Trigger(1623392196, base.gameObject);
	}

	public bool CanBeIncapacitated;

	[Serialize]
	private ResourceRef<Death> death = new ResourceRef<Death>();

	[Serialize]
	private float bleedOutStamina = 100f;

	private static float baseBleedOutSpeed = 0.5f;

	private float maxBleedOutStamina = 100f;

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
		Scuffed,
		Injured,
		Critical,
		Incapacitated,
		Dead,
		Invincible
	}
}
