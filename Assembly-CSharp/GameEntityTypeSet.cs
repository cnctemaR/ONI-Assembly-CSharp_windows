using System;
using Klei.AI;

public class GameEntityTypeSet : EntityTypeSet
{
	public GameEntityTypeSet()
	{
		EntityTypeSet.Instance = this;
		Db db = Db.Get();
		this.creatureEntityTypes = this.AddTypes<CreatureEntityTypeSet>(new CreatureEntityTypeSet(db));
		this.humanEntityTypes = this.AddTypes<HumanEntityTypeSet>(new HumanEntityTypeSet(db));
	}

	public new static GameEntityTypeSet Instance
	{
		get
		{
			return Singleton<GameEntityTypeSet>.Instance;
		}
	}

	public static void Destroy()
	{
		Singleton<GameEntityTypeSet>.Destroy();
	}

	public T AddTypes<T>(T entity_types) where T : EntityTypeSet
	{
		foreach (EntityType entityType in entity_types)
		{
			base.Add(entityType);
		}
		return entity_types;
	}

	public static Trait CreateLivingEntityBaseTrait(string id, string name, float stamina_per_day, float calories_per_day, float air_per_second, float toxicity_recovery_per_day, float bladder_increase_per_day, float max_underwater_travel_cost, Db modifier_set)
	{
		Trait trait = Db.Get().CreateTrait(id + "BaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, stamina_per_day / 600f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, calories_per_day / 600f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, toxicity_recovery_per_day / 600f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Attributes.AirConsumptionRate.Id, air_per_second, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, bladder_increase_per_day / 600f, name, false, false));
		trait.Add(new AttributeModifier(Db.Get().Attributes.MaxUnderwaterTravelCost.Id, max_underwater_travel_cost, name, false, false));
		return trait;
	}

	public override void RegisterPrefab(KPrefabID prefab)
	{
		Assets.AddPrefab(prefab);
	}

	public CreatureEntityTypeSet creatureEntityTypes;

	public HumanEntityTypeSet humanEntityTypes;
}
