using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class HumanEntityTypeSet : EntityTypeSet
{
	public HumanEntityTypeSet(Db modifier_set)
	{
		this.human = this.CreateHuman("Human", DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, modifier_set);
	}

	private EntityType CreateHuman(string id, string name, Db modifier_set)
	{
		EntityType entityType = base.Add(new EntityType(id, name));
		entityType.amounts.Add(Db.Get().Amounts.Stress);
		entityType.amounts.Add(Db.Get().Amounts.Stamina);
		entityType.amounts.Add(Db.Get().Amounts.Calories);
		entityType.amounts.Add(Db.Get().Amounts.Temperature);
		entityType.amounts.Add(Db.Get().Amounts.ExternalTemperature);
		entityType.amounts.Add(Db.Get().Amounts.Breath);
		entityType.amounts.Add(Db.Get().Amounts.Toxicity);
		entityType.amounts.Add(Db.Get().Amounts.Bladder);
		entityType.amounts.Add(Db.Get().Amounts.Decor);
		entityType.amounts.Add(Db.Get().Amounts.ImmuneLevel);
		Trait trait = GameEntityTypeSet.CreateLivingEntityBaseTrait(id, name, -0.11666667f, -1666.6666f, 0.1f, 0f, 0.16666667f, 8f, modifier_set);
		trait.Add(new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, -35f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, -3f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, 1f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Attributes.RoomTemperaturePreference.Id, 0f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 0f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.025f, name, false, false));
		foreach (Disease disease in Db.Get().Diseases)
		{
			entityType.amounts.Add(disease.amount);
			entityType.attributes.Add(disease.cureSpeedBase);
		}
		entityType.attributes.Add(Db.Get().Attributes.DoctoredLevel);
		DuplicantNoiseLevels.SetupNoiseLevels();
		entityType.baseTraits.Add(trait);
		EntityPrefabs.Instance.MinionPrefab.GetComponent<Health>().SetMaxHitPoints(100f);
		if (!HumanEntityTypeSet.dupeInitHackHasRun)
		{
			KPrefabID componentInChildren = EntityPrefabs.Instance.MinionPrefab.GetComponentInChildren<KPrefabID>(true);
			componentInChildren.prefabSpawnFn += delegate(GameObject go)
			{
				Sensors component = go.GetComponent<Sensors>();
				component.Add(new PathProberSensor(component));
				component.Add(new SafeCellSensor(component));
				component.Add(new IdleCellSensor(component));
				component.Add(new PickupableSensor(component));
				component.Add(new ClosestEdibleSensor(component));
				component.Add(new ToxicantSensor(component));
				component.Add(new BreathableAreaSensor(component));
				component.Add(new AssignableReachabilitySensor(component));
				component.Add(new ToiletSensor(component));
				StateMachineController component2 = go.GetComponent<StateMachineController>();
				RationalAi.Instance instance = new RationalAi.Instance(component2);
				instance.StartSM();
				if (go.GetComponent<OxygenBreather>().GetGasProvider() == null)
				{
					go.GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
				}
				Navigator component3 = go.GetComponent<Navigator>();
				component3.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(component3, 3.325f, 2.5f));
				component3.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component3));
				component3.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(component3));
				component3.transitionDriver.overrideLayers.Add(new ReactableTransitionLayer(component3));
				component3.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(component3));
				ThreatMonitor.Instance smi = go.GetSMI<ThreatMonitor.Instance>();
				if (smi != null)
				{
					smi.sm.FleeThresholdState = Health.HealthState.Critical;
				}
			};
			HumanEntityTypeSet.dupeInitHackHasRun = true;
		}
		return entityType;
	}

	public EntityType human;

	private static bool dupeInitHackHasRun;
}
