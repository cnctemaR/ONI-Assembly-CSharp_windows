using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class EntityTypeSet : ResourceSet<EntityType>
{
	public EntityTypeSet()
	{
		this.human = this.CreateHuman("Human", DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME);
	}

	public static EntityTypeSet Instance
	{
		get
		{
			return Singleton<EntityTypeSet>.Instance;
		}
	}

	public static void Destroy()
	{
		Singleton<EntityTypeSet>.Destroy();
	}

	private EntityType CreateHuman(string id, string name)
	{
		EntityType entityType = base.Add(new EntityType(id, name));
		foreach (Klei.AI.Attribute attribute in Db.Get().Attributes)
		{
			if (!entityType.attributes.Contains(attribute))
			{
				entityType.attributes.Add(attribute);
			}
		}
		entityType.amounts.Add(Db.Get().Amounts.Stamina);
		entityType.amounts.Add(Db.Get().Amounts.Calories);
		entityType.amounts.Add(Db.Get().Amounts.ImmuneLevel);
		entityType.amounts.Add(Db.Get().Amounts.Breath);
		entityType.amounts.Add(Db.Get().Amounts.Stress);
		entityType.amounts.Add(Db.Get().Amounts.Toxicity);
		entityType.amounts.Add(Db.Get().Amounts.Bladder);
		Trait trait = Db.Get().CreateTrait(id + "BaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, -0.11666667f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.6666f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.AirConsumptionRate.Id, 0.1f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, 0.16666667f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.MaxUnderwaterTravelCost.Id, 8f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, -35f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, -1f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, 1f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.RoomTemperaturePreference.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.025f, name, false, false, true));
		foreach (Disease disease in Db.Get().Diseases)
		{
			entityType.amounts.Add(disease.amount);
			entityType.attributes.Add(disease.cureSpeedBase);
		}
		DuplicantNoiseLevels.SetupNoiseLevels();
		entityType.baseTraits.Add(trait);
		EntityPrefabs.Instance.MinionPrefab.GetComponent<Health>().SetMaxHitPoints(100f);
		if (!EntityTypeSet.dupeInitHackHasRun)
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
				component3.transitionDriver.overrideLayers.Add(new TubeTransitionLayer(component3));
				component3.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(component3));
				component3.transitionDriver.overrideLayers.Add(new ReactableTransitionLayer(component3));
				component3.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(component3));
				ThreatMonitor.Instance smi = go.GetSMI<ThreatMonitor.Instance>();
				if (smi != null)
				{
					smi.sm.FleeThresholdState = Health.HealthState.Critical;
				}
			};
			EntityTypeSet.dupeInitHackHasRun = true;
		}
		return entityType;
	}

	public EntityType human;

	private static bool dupeInitHackHasRun;
}
