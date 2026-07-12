using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureToStructureTemperature : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<StructureToStructureTemperature>(-1555603773, StructureToStructureTemperature.OnStructureTemperatureRegisteredDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.DefineConductiveCells();
		GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.contactConductiveLayer, new Action<int, object>(this.OnAnyBuildingChanged));
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.RemoveGlobalLayerListener(GameScenePartitioner.Instance.contactConductiveLayer, new Action<int, object>(this.OnAnyBuildingChanged));
		this.UnregisterToSIM();
		base.OnCleanUp();
	}

	private void OnStructureTemperatureRegistered(object _sim_handle)
	{
		int num = (int)_sim_handle;
		this.RegisterToSIM(num);
	}

	private void RegisterToSIM(int sim_handle)
	{
		string name = this.building.Def.Name;
		SimMessages.RegisterBuildingToBuildingHeatExchange(sim_handle, Game.Instance.simComponentCallbackManager.Add(delegate(int sim_handle, object callback_data)
		{
			this.OnSimRegistered(sim_handle);
		}, null, "StructureToStructureTemperature.SimRegister").index);
	}

	private void OnSimRegistered(int sim_handle)
	{
		if (sim_handle != -1)
		{
			this.selfHandle = sim_handle;
			this.hasBeenRegister = true;
			if (this.buildingDestroyed)
			{
				this.UnregisterToSIM();
				return;
			}
			this.Refresh_InContactBuildings();
		}
	}

	private void UnregisterToSIM()
	{
		if (this.hasBeenRegister)
		{
			SimMessages.RemoveBuildingToBuildingHeatExchange(this.selfHandle, -1);
		}
		this.buildingDestroyed = true;
	}

	private void DefineConductiveCells()
	{
		this.conductiveCells = new List<int>(this.building.PlacementCells);
		this.conductiveCells.Remove(this.building.GetUtilityInputCell());
		this.conductiveCells.Remove(this.building.GetUtilityOutputCell());
	}

	private void Add(StructureToStructureTemperature.InContactBuildingData buildingData)
	{
		if (this.inContactBuildings.Add(buildingData.buildingInContact))
		{
			SimMessages.AddBuildingToBuildingHeatExchange(this.selfHandle, buildingData.buildingInContact, buildingData.cellsInContact);
		}
	}

	private void Remove(int building)
	{
		if (this.inContactBuildings.Contains(building))
		{
			this.inContactBuildings.Remove(building);
			SimMessages.RemoveBuildingInContactFromBuildingToBuildingHeatExchange(this.selfHandle, building);
		}
	}

	private void OnAnyBuildingChanged(int _cell, object _data)
	{
		if (this.hasBeenRegister)
		{
			StructureToStructureTemperature.BuildingChangedObj buildingChangedObj = (StructureToStructureTemperature.BuildingChangedObj)_data;
			bool flag = false;
			int num = 0;
			for (int i = 0; i < buildingChangedObj.building.PlacementCells.Length; i++)
			{
				int num2 = buildingChangedObj.building.PlacementCells[i];
				if (this.conductiveCells.Contains(num2))
				{
					flag = true;
					num++;
				}
			}
			if (flag)
			{
				int simHandler = buildingChangedObj.simHandler;
				StructureToStructureTemperature.BuildingChangeType changeType = buildingChangedObj.changeType;
				if (changeType == StructureToStructureTemperature.BuildingChangeType.Created)
				{
					StructureToStructureTemperature.InContactBuildingData inContactBuildingData = new StructureToStructureTemperature.InContactBuildingData
					{
						buildingInContact = simHandler,
						cellsInContact = num
					};
					this.Add(inContactBuildingData);
					return;
				}
				if (changeType != StructureToStructureTemperature.BuildingChangeType.Destroyed)
				{
					return;
				}
				this.Remove(simHandler);
			}
		}
	}

	private void Refresh_InContactBuildings()
	{
		foreach (StructureToStructureTemperature.InContactBuildingData inContactBuildingData in this.GetAll_InContact_Buildings())
		{
			this.Add(inContactBuildingData);
		}
	}

	private List<StructureToStructureTemperature.InContactBuildingData> GetAll_InContact_Buildings()
	{
		Dictionary<Building, int> dictionary = new Dictionary<Building, int>();
		List<StructureToStructureTemperature.InContactBuildingData> list = new List<StructureToStructureTemperature.InContactBuildingData>();
		List<GameObject> buildingsInCell = new List<GameObject>();
		using (List<int>.Enumerator enumerator = this.conductiveCells.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int cell = enumerator.Current;
				buildingsInCell.Clear();
				Action<int> action = delegate(int layer)
				{
					GameObject gameObject = Grid.Objects[cell, layer];
					if (gameObject != null && !buildingsInCell.Contains(gameObject))
					{
						buildingsInCell.Add(gameObject);
					}
				};
				action(1);
				action(26);
				action(27);
				action(31);
				action(32);
				action(30);
				action(12);
				action(13);
				action(16);
				action(17);
				action(24);
				action(2);
				for (int i = 0; i < buildingsInCell.Count; i++)
				{
					Building building = ((buildingsInCell[i] == null) ? null : buildingsInCell[i].GetComponent<Building>());
					if (building != null && building.Def.UseStructureTemperature)
					{
						if (!dictionary.ContainsKey(building))
						{
							dictionary.Add(building, 0);
						}
						Dictionary<Building, int> dictionary2 = dictionary;
						Building building2 = building;
						int num = dictionary2[building2];
						dictionary2[building2] = num + 1;
					}
				}
			}
		}
		foreach (Building building3 in dictionary.Keys)
		{
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(building3);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				int simHandleCopy = GameComps.StructureTemperatures.GetPayload(handle).simHandleCopy;
				StructureToStructureTemperature.InContactBuildingData inContactBuildingData = new StructureToStructureTemperature.InContactBuildingData
				{
					buildingInContact = simHandleCopy,
					cellsInContact = dictionary[building3]
				};
				list.Add(inContactBuildingData);
			}
		}
		return list;
	}

	[MyCmpGet]
	private Building building;

	private List<int> conductiveCells;

	private HashSet<int> inContactBuildings = new HashSet<int>();

	private bool hasBeenRegister;

	private bool buildingDestroyed;

	private int selfHandle;

	protected static readonly EventSystem.IntraObjectHandler<StructureToStructureTemperature> OnStructureTemperatureRegisteredDelegate = new EventSystem.IntraObjectHandler<StructureToStructureTemperature>(delegate(StructureToStructureTemperature component, object data)
	{
		component.OnStructureTemperatureRegistered(data);
	});

	public enum BuildingChangeType
	{
		Created,
		Destroyed,
		Moved
	}

	public struct InContactBuildingData
	{
		public int buildingInContact;

		public int cellsInContact;
	}

	public struct BuildingChangedObj
	{
		public BuildingChangedObj(StructureToStructureTemperature.BuildingChangeType _changeType, Building _building, int sim_handler)
		{
			this.changeType = _changeType;
			this.building = _building;
			this.simHandler = sim_handler;
		}

		public StructureToStructureTemperature.BuildingChangeType changeType;

		public int simHandler;

		public Building building;
	}
}
