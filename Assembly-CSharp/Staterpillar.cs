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
		KPrefabID kprefabID = this.generatorRef.Get();
		GameObject gameObject = null;
		if (kprefabID != null)
		{
			gameObject = kprefabID.gameObject;
		}
		if (!gameObject)
		{
			gameObject = this.generatorDef.Build(targetCell, Orientation.R180, null, this.generatorElement, base.gameObject.GetComponent<PrimaryElement>().Temperature, true, -1f);
			gameObject.SetActive(true);
			this.generatorRef = new Ref<KPrefabID>(gameObject.GetComponent<KPrefabID>());
			gameObject.GetComponent<BuildingCellVisualizer>().enabled = false;
			gameObject.GetComponent<StaterpillarGenerator>().enabled = false;
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
		return this.GetGenerator().GetComponent<Generator>().CircuitID != ushort.MaxValue;
	}

	public bool IsNotConnected()
	{
		return !this.IsConnected();
	}

	public void EnableGenerator()
	{
		KPrefabID generator = this.GetGenerator();
		generator.GetComponent<StaterpillarGenerator>().enabled = true;
		generator.GetComponent<BuildingCellVisualizer>().enabled = true;
	}

	public void DestroyGenerator()
	{
		KPrefabID generator = this.GetGenerator();
		if (generator != null)
		{
			this.generatorRef.Set(null);
			GameScheduler.Instance.ScheduleNextFrame("Destroy Staterpillar Generator", delegate(object o)
			{
				Util.KDestroyGameObject(generator.gameObject);
			}, null, null);
		}
	}

	public KPrefabID GetGenerator()
	{
		if (!(this.generatorRef.Get() != null))
		{
			return null;
		}
		return this.generatorRef.Get();
	}

	[Serialize]
	private Ref<KPrefabID> generatorRef = new Ref<KPrefabID>();

	private AttributeModifier wildMod = new AttributeModifier(Db.Get().Attributes.GeneratorOutput.Id, -75f, BUILDINGS.PREFABS.STATERPILLARGENERATOR.MODIFIERS.WILD, false, false, true);

	private IList<Tag> generatorElement;

	private BuildingDef generatorDef;
}
