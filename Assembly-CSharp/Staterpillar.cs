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
		this.dummyElement = new List<Tag> { SimHashes.Unobtanium.CreateTag() };
		this.connectorDef = Assets.GetBuildingDef(this.connectorDefId);
	}

	public void SpawnConnectorBuilding(int targetCell)
	{
		if (this.conduitLayer == ObjectLayer.Wire)
		{
			this.SpawnGenerator(targetCell);
			return;
		}
		this.SpawnConduitConnector(targetCell);
	}

	public void DestroyOrphanedConnectorBuilding()
	{
		KPrefabID building = this.GetConnectorBuilding();
		if (building != null)
		{
			this.connectorRef.Set(null);
			this.cachedGenerator = null;
			this.cachedConduitDispenser = null;
			GameScheduler.Instance.ScheduleNextFrame("Destroy Staterpillar Connector building", delegate(object o)
			{
				if (building != null)
				{
					Util.KDestroyGameObject(building.gameObject);
				}
			}, null, null);
		}
	}

	public void EnableConnector()
	{
		if (this.conduitLayer == ObjectLayer.Wire)
		{
			this.EnableGenerator();
			return;
		}
		this.EnableConduitConnector();
	}

	public bool IsConnectorBuildingSpawned()
	{
		return this.GetConnectorBuilding() != null;
	}

	public bool IsConnected()
	{
		if (this.conduitLayer == ObjectLayer.Wire)
		{
			return this.GetGenerator().CircuitID != ushort.MaxValue;
		}
		return this.GetConduitDispenser().IsConnected;
	}

	public KPrefabID GetConnectorBuilding()
	{
		return this.connectorRef.Get();
	}

	private void SpawnConduitConnector(int targetCell)
	{
		if (this.GetConduitDispenser() == null)
		{
			GameObject gameObject = this.connectorDef.Build(targetCell, Orientation.R180, null, this.dummyElement, base.gameObject.GetComponent<PrimaryElement>().Temperature, true, -1f);
			this.connectorRef = new Ref<KPrefabID>(gameObject.GetComponent<KPrefabID>());
			gameObject.SetActive(true);
			gameObject.GetComponent<BuildingCellVisualizer>().enabled = false;
		}
	}

	private void EnableConduitConnector()
	{
		ConduitDispenser conduitDispenser = this.GetConduitDispenser();
		conduitDispenser.GetComponent<BuildingCellVisualizer>().enabled = true;
		conduitDispenser.storage = base.GetComponent<Storage>();
		conduitDispenser.SetOnState(true);
	}

	public ConduitDispenser GetConduitDispenser()
	{
		if (this.cachedConduitDispenser == null)
		{
			KPrefabID kprefabID = this.connectorRef.Get();
			if (kprefabID != null)
			{
				this.cachedConduitDispenser = kprefabID.GetComponent<ConduitDispenser>();
			}
		}
		return this.cachedConduitDispenser;
	}

	private void DestroyOrphanedConduitDispenserBuilding()
	{
		ConduitDispenser dispenser = this.GetConduitDispenser();
		if (dispenser != null)
		{
			this.connectorRef.Set(null);
			GameScheduler.Instance.ScheduleNextFrame("Destroy Staterpillar Dispenser", delegate(object o)
			{
				if (dispenser != null)
				{
					Util.KDestroyGameObject(dispenser.gameObject);
				}
			}, null, null);
		}
	}

	private void SpawnGenerator(int targetCell)
	{
		StaterpillarGenerator generator = this.GetGenerator();
		GameObject gameObject = null;
		if (generator != null)
		{
			gameObject = generator.gameObject;
		}
		if (!gameObject)
		{
			gameObject = this.connectorDef.Build(targetCell, Orientation.R180, null, this.dummyElement, base.gameObject.GetComponent<PrimaryElement>().Temperature, true, -1f);
			StaterpillarGenerator component = gameObject.GetComponent<StaterpillarGenerator>();
			component.parent = new Ref<Staterpillar>(this);
			this.connectorRef = new Ref<KPrefabID>(component.GetComponent<KPrefabID>());
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
				num = (flag ? 0.1f : 0.025f);
			}
			else if (calories0to <= 0.3f)
			{
				num = 0.5f;
			}
			else if (calories0to <= 0.5f)
			{
				num = 0.75f;
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

	private void EnableGenerator()
	{
		StaterpillarGenerator generator = this.GetGenerator();
		generator.enabled = true;
		generator.GetComponent<BuildingCellVisualizer>().enabled = true;
	}

	public StaterpillarGenerator GetGenerator()
	{
		if (this.cachedGenerator == null)
		{
			KPrefabID kprefabID = this.connectorRef.Get();
			if (kprefabID != null)
			{
				this.cachedGenerator = kprefabID.GetComponent<StaterpillarGenerator>();
			}
		}
		return this.cachedGenerator;
	}

	public ObjectLayer conduitLayer;

	public string connectorDefId;

	private IList<Tag> dummyElement;

	private BuildingDef connectorDef;

	[Serialize]
	private Ref<KPrefabID> connectorRef = new Ref<KPrefabID>();

	private AttributeModifier wildMod = new AttributeModifier(Db.Get().Attributes.GeneratorOutput.Id, -75f, BUILDINGS.PREFABS.STATERPILLARGENERATOR.MODIFIERS.WILD, false, false, true);

	private ConduitDispenser cachedConduitDispenser;

	private StaterpillarGenerator cachedGenerator;
}
