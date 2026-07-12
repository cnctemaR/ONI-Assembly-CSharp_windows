using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Staterpillar : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.generatorElement = new List<Tag> { SimHashes.Creature.CreateTag() };
		this.generatorDef = Assets.GetBuildingDef(StaterpillarGeneratorConfig.ID);
		base.OnPrefabInit();
	}

	public void SpawnGenerator(int targetCell)
	{
		StaterpillarGenerator staterpillarGenerator = this.generatorRef.Get();
		GameObject gameObject = null;
		if (staterpillarGenerator != null)
		{
			gameObject = staterpillarGenerator.gameObject;
		}
		if (!gameObject)
		{
			gameObject = this.generatorDef.Build(targetCell, Orientation.R180, null, this.generatorElement, base.gameObject.GetComponent<PrimaryElement>().Temperature, true, -1f);
			StaterpillarGenerator component = gameObject.GetComponent<StaterpillarGenerator>();
			component.parent = new Ref<Staterpillar>(this);
			this.generatorRef = new Ref<StaterpillarGenerator>(component);
			gameObject.SetActive(true);
			gameObject.GetComponent<BuildingCellVisualizer>().enabled = false;
			component.enabled = false;
		}
		Attributes attributes = gameObject.gameObject.GetAttributes();
		bool flag = base.gameObject.GetSMI<WildnessMonitor.Instance>().wildness.value > 0f;
		if (flag)
		{
			attributes.Add(this.wildMod);
		}
		bool flag2 = base.gameObject.GetComponent<Effects>().HasEffect("Unhappy");
		CreatureCalorieMonitor.Instance smi = base.gameObject.GetSMI<CreatureCalorieMonitor.Instance>();
		if (smi.IsHungry() || flag2)
		{
			float calories0to = smi.GetCalories0to1();
			float num = 1f;
			if (calories0to <= 0f)
			{
				num = (flag ? 0.1f : 0f);
			}
			else if (calories0to <= 0.3f)
			{
				num = 0.25f;
			}
			else if (calories0to <= 0.6f)
			{
				num = 0.5f;
			}
			else if (calories0to <= 0.9f)
			{
				num = 0.75f;
			}
			if (flag2)
			{
				num *= 0.2f;
			}
			if (num < 1f)
			{
				float num2;
				if (flag)
				{
					num2 = Mathf.Lerp(0f, 25f, 1f - num);
				}
				else
				{
					num2 = (1f - num) * 100f;
				}
				AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Attributes.GeneratorOutput.Id, -num2, BUILDINGS.PREFABS.STATERPILLARGENERATOR.MODIFIERS.HUNGRY, false, false, true);
				attributes.Add(attributeModifier);
			}
		}
	}

	public bool IsConnected()
	{
		return this.GetGenerator().CircuitID != ushort.MaxValue;
	}

	public bool IsNotConnected()
	{
		return !this.IsConnected();
	}

	public void EnableGenerator()
	{
		StaterpillarGenerator generator = this.GetGenerator();
		generator.enabled = true;
		generator.GetComponent<BuildingCellVisualizer>().enabled = true;
	}

	public void DestroyGenerator()
	{
		StaterpillarGenerator generator = this.GetGenerator();
		if (generator != null)
		{
			this.generatorRef.Set(null);
			GameScheduler.Instance.ScheduleNextFrame("Destroy Staterpillar Generator", delegate(object o)
			{
				if (generator != null)
				{
					Util.KDestroyGameObject(generator.gameObject);
				}
			}, null, null);
		}
	}

	public StaterpillarGenerator GetGenerator()
	{
		return this.generatorRef.Get();
	}

	[Serialize]
	private Ref<StaterpillarGenerator> generatorRef = new Ref<StaterpillarGenerator>();

	private AttributeModifier wildMod = new AttributeModifier(Db.Get().Attributes.GeneratorOutput.Id, -75f, BUILDINGS.PREFABS.STATERPILLARGENERATOR.MODIFIERS.WILD, false, false, true);

	private IList<Tag> generatorElement;

	private BuildingDef generatorDef;
}
