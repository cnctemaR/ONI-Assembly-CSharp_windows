using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;

public class RestRestoreHealth : KMonoBehaviour, IEffectDescriptor
{
	public RestRestoreHealth()
	{
		this.notification = new Notification(MISC.NOTIFICATIONS.HEALINGTRAITGAIN.NAME, NotificationType.Bad, HashedString.Invalid, new Func<List<Notification>, object, string>(RestRestoreHealth.OnTraitGainTooltip), null, true, 0f, null, null, null);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.healthModifier = new AttributeModifier("Health", this.HitPointsPerDay / 600f, DUPLICANTS.MODIFIERS.RESTING.NAME, false, false);
		this.caloriesModifier = new AttributeModifier("Calories", this.CaloriesPerDay / 600f, DUPLICANTS.MODIFIERS.INTRAVENOUS_NUTRITION.NAME, false, false);
	}

	public void StartHealing(Worker worker)
	{
		Health component = worker.GetComponent<Health>();
		this.targetPreviousState = component.State;
		component.GetAmountInstance.deltaAttribute.Add(global::STRINGS.CREATURES.STATUSITEMS.HEALTHSTATUS.NAME, this.healthModifier);
		if (this.caloriesModifier.Value > 0f)
		{
			worker.GetAmounts().Get("Calories").deltaAttribute.Add(global::STRINGS.CREATURES.STATUSITEMS.HEALTHSTATUS.NAME, this.caloriesModifier);
		}
		Diseases diseases = worker.GetComponent<MinionModifiers>().diseases;
		diseases.AddCure(DUPLICANTS.DISEASES.RECUPERATING, 1.1f);
	}

	public void StopHealing(Worker worker)
	{
		worker.GetComponent<Health>().GetAmountInstance.deltaAttribute.Remove(this.healthModifier);
		worker.GetAmounts().Get("Calories").deltaAttribute.Remove(this.caloriesModifier);
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.HitPointsPerDay > 0f)
		{
			Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.HIT_POINTS_PER_CYCLE, this.HitPointsPerDay), string.Format(UI.BUILDINGEFFECTS.HIT_POINTS_PER_CYCLE, this.HitPointsPerDay), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
		}
		if (this.CaloriesPerDay > 0f)
		{
			Descriptor descriptor2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.KCAL_PER_CYCLE, GameUtil.GetFormattedCalories(this.CaloriesPerDay, GameUtil.TimeSlice.None, true)), string.Format(UI.BUILDINGEFFECTS.KCAL_PER_CYCLE, GameUtil.GetFormattedCalories(this.CaloriesPerDay, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor2);
		}
		return list;
	}

	public float ChanceForNewTraitWhenChangeState;

	public float HitPointsPerDay = 100f;

	public float CaloriesPerDay = 2000000f;

	public Notification notification;

	private AttributeModifier healthModifier;

	private AttributeModifier caloriesModifier;

	private Health.HealthState targetPreviousState;
}
