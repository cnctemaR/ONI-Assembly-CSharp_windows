using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;

public class RestRestoreHealth : KMonoBehaviour
{
	public RestRestoreHealth()
	{
		Func<List<Notification>, object, string> func = new Func<List<Notification>, object, string>(RestRestoreHealth.OnTraitGainTooltip);
		this.notification = new Notification(MISC.NOTIFICATIONS.HEALINGTRAITGAIN.NAME, NotificationType.Bad, null, func, null, true, 0f, null, null, null);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.healthModifier = new AttributeModifier("Health", this.HitPointsPerDay / 600f, DUPLICANTS.MODIFIERS.RESTING.NAME, false);
	}

	public void StartHealing(Worker worker)
	{
		Health component = worker.GetComponent<Health>();
		this.targetPreviousState = component.State;
		component.GetAmountInstance.deltaAttribute.Add(global::STRINGS.CREATURES.STATUSITEMS.HEALTHSTATUS.NAME, this.healthModifier);
		Diseases diseases = worker.GetComponent<MinionModifiers>().diseases;
		diseases.AddCure(DUPLICANTS.DISEASES.RECUPERATING, 1.1f);
	}

	public void StopHealing(Worker worker)
	{
		worker.GetComponent<Health>().GetAmountInstance.deltaAttribute.Remove(this.healthModifier);
		Diseases diseases = worker.GetComponent<MinionModifiers>().diseases;
		diseases.RemoveCure(DUPLICANTS.DISEASES.RECUPERATING, 1.1f);
		worker.Trigger(-1527662329, null);
	}

	public void OnHealTick(Worker worker)
	{
		Health component = worker.GetComponent<Health>();
		Health.HealthState state = component.State;
		if (state != this.targetPreviousState)
		{
			this.targetPreviousState = state;
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, MISC.NOTIFICATIONS.HEALING.NAME, component.transform, 1.5f, false);
			float num = (float)Util.RandomInt(0, 100) / 100f;
			if (this.ChanceForNewTraitWhenChangeState > num)
			{
				List<string> list = new List<string>(DUPLICANTSTATS.CONTRACTEDTRAITS_HEALING);
				list.Shuffle<string>();
				Trait trait = Db.Get().traits.TryGet(list[0]);
				Traits component2 = component.GetComponent<Traits>();
				component2.Add(trait);
				component.GetComponent<Notifier>().Add(this.notification, string.Format(MISC.NOTIFICATIONS.HEALINGTRAITGAIN.SUFFIX, trait.Name));
			}
		}
	}

	private static string OnTraitGainTooltip(List<Notification> notifications, object data)
	{
		return MISC.NOTIFICATIONS.HEALINGTRAITGAIN.TOOLTIP + notifications.ReduceMessages(false);
	}

	public float ChanceForNewTraitWhenChangeState;

	public float HitPointsPerDay = 100f;

	public Notification notification;

	private AttributeModifier healthModifier;

	private Health.HealthState targetPreviousState;
}
