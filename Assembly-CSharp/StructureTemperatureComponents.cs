using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class StructureTemperatureComponents : KGameObjectComponentManager<StructureTemperatureData>
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		return base.Add(go, new StructureTemperatureData(go));
	}

	public static void ClearInstanceMap()
	{
		StructureTemperatureComponents.handleInstanceMap.Clear();
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		this.InitializeStatusItem();
		base.OnPrefabInit(handle);
		StructureTemperatureData data = base.GetData(handle);
		data.primaryElement.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(StructureTemperatureComponents.OnGetTemperature);
		data.primaryElement.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(StructureTemperatureComponents.OnSetTemperature);
		data.isActiveBuilding = data.building.Def.OperatingKilowatts > 0f || data.ExhaustKilowatts > 0f;
		base.SetData(handle, data);
	}

	private void InitializeStatusItem()
	{
		if (this.operatingEnergyStatusItem != null)
		{
			return;
		}
		this.operatingEnergyStatusItem = new StatusItem("OperatingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 14334);
		this.operatingEnergyStatusItem.resolveStringCallback = delegate(string str, object ev_data)
		{
			int num = (int)ev_data;
			HandleVector<int>.Handle handle = StructureTemperatureComponents.handleInstanceMap[num];
			StructureTemperatureData data = base.GetData(handle);
			if (str != BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP)
			{
				try
				{
					str = string.Format(str, GameUtil.GetFormattedWattage(data.TotalEnergyProducedKW * 1000f * 0.005f, GameUtil.WattageFormatterUnit.Automatic));
				}
				catch (Exception ex)
				{
					global::Debug.LogWarning(ex, null);
					global::Debug.LogWarning(BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP, null);
					global::Debug.LogWarning(str, null);
				}
			}
			else
			{
				string text = string.Empty;
				foreach (StructureTemperatureData.EnergySource energySource in data.energySourcesKW)
				{
					text += string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, energySource.source, GameUtil.GetFormattedWattage(energySource.value * 1000f * 0.005f, GameUtil.WattageFormatterUnit.Automatic));
				}
				str = string.Format(str, GameUtil.GetFormattedWattage(data.TotalEnergyProducedKW * 1000f * 0.005f, GameUtil.WattageFormatterUnit.Automatic), text);
			}
			return str;
		};
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = base.GetData(handle);
		if (data.operational != null && data.isActiveBuilding)
		{
			data.primaryElement.Subscribe(824508782, delegate(object ev_data)
			{
				StructureTemperatureComponents.OnActiveChanged(handle, ev_data);
			});
		}
		base.SetData(handle, data);
		this.SimRegister(handle, ref data);
	}

	private static void OnActiveChanged(HandleVector<int>.Handle handle, object ev_data)
	{
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		float temperature = data.Temperature;
		data.primaryElement.InternalTemperature = temperature;
		data.dirty = true;
		GameComps.StructureTemperatures.SetData(handle, data);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		this.SimUnregister(handle);
		base.OnCleanUp(handle);
	}

	public override void SimUpdate(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			StructureTemperatureData structureTemperatureData = this.data[i];
			if (Sim.IsValidHandle(structureTemperatureData.simHandle))
			{
				StructureTemperatureComponents.UpdateSimState(structureTemperatureData);
				structureTemperatureData.dirty = false;
				structureTemperatureData.ApplyPendingEnergyModifications();
				if (!structureTemperatureData.isActiveBuilding)
				{
					this.data[i] = structureTemperatureData;
				}
				else
				{
					if (structureTemperatureData.operational.IsActive)
					{
						structureTemperatureData.selectable.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, this.operatingEnergyStatusItem, structureTemperatureData.simHandle);
						structureTemperatureData.energySourcesKW = this.AccumulateProducedEnergyKW(structureTemperatureData.energySourcesKW, structureTemperatureData.OperatingKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING);
						if (structureTemperatureData.ExhaustKilowatts > 0f)
						{
							int num = structureTemperatureData.building.Def.WidthInCells * structureTemperatureData.building.Def.HeightInCells;
							float num2 = structureTemperatureData.ExhaustKilowatts * dt / (float)num;
							Extents extents = structureTemperatureData.building.GetExtents();
							for (int j = 0; j < extents.height; j++)
							{
								int num3 = extents.y + j;
								for (int k = 0; k < extents.width; k++)
								{
									int num4 = extents.x + k;
									int num5 = num3 * Grid.WidthInCells + num4;
									float mass = Grid.Cell[num5].mass;
									float num6 = Mathf.Min(mass, 1.5f) / 1.5f;
									float num7 = num2 * num6;
									SimMessages.ModifyEnergy(num5, num7, SimMessages.EnergySourceID.StructureTemperature);
								}
							}
							structureTemperatureData.energySourcesKW = this.AccumulateProducedEnergyKW(structureTemperatureData.energySourcesKW, structureTemperatureData.ExhaustKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING);
						}
					}
					else
					{
						structureTemperatureData.selectable.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, null, null);
					}
					this.data[i] = structureTemperatureData;
				}
			}
		}
	}

	private static void UpdateSimState(StructureTemperatureData data)
	{
		if (!data.dirty)
		{
			return;
		}
		if (!Sim.IsValidHandle(data.simHandle))
		{
			return;
		}
		float internalTemperature = data.primaryElement.InternalTemperature;
		float num = data.building.Def.MassForTemperatureModification;
		float operatingKilowatts = data.OperatingKilowatts;
		Overheatable component = data.primaryElement.GetComponent<Overheatable>();
		float num2 = ((!(component != null)) ? 10000f : component.OverheatTemperature);
		global::UnityEngine.Debug.Assert(internalTemperature > 0f, "Invalid temperature");
		global::UnityEngine.Debug.Assert(num > 0f);
		if (!data.enabled)
		{
			num = 0f;
		}
		Extents extents = data.GetExtents();
		byte b = (byte)ElementLoader.elements.IndexOf(data.primaryElement.Element);
		SimMessages.ModifyBuildingHeatExchange(data.simHandle, extents, internalTemperature, num2, operatingKilowatts, b, num);
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		float num;
		if (Sim.IsValidHandle(data.simHandle))
		{
			if (data.enabled)
			{
				num = Game.Instance.simData.buildingTemperatures[data.simHandle].temperature;
			}
			else
			{
				int num2 = Grid.PosToCell(data.primaryElement.transform.position);
				num = Grid.Temperature[num2];
			}
		}
		else
		{
			num = data.primaryElement.InternalTemperature;
		}
		return num;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		data.primaryElement.InternalTemperature = temperature;
		data.dirty = true;
		if (!data.isActiveBuilding && Sim.IsValidHandle(data.simHandle))
		{
			StructureTemperatureComponents.UpdateSimState(data);
			data.ApplyPendingEnergyModifications();
			data.dirty = false;
		}
		GameComps.StructureTemperatures.SetData(handle, data);
	}

	public void ProduceEnergy(HandleVector<int>.Handle handle, float delta_kilojoules, string source, float display_dt)
	{
		StructureTemperatureData data = base.GetData(handle);
		data.ModifyEnergy(delta_kilojoules);
		data.energySourcesKW = this.AccumulateProducedEnergyKW(data.energySourcesKW, delta_kilojoules / display_dt, source);
	}

	private List<StructureTemperatureData.EnergySource> AccumulateProducedEnergyKW(List<StructureTemperatureData.EnergySource> sources, float kw, string source)
	{
		if (sources == null)
		{
			sources = new List<StructureTemperatureData.EnergySource>();
		}
		bool flag = false;
		for (int i = 0; i < sources.Count; i++)
		{
			if (sources[i].source == source)
			{
				sources[i].Accumulate(kw);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			sources.Add(new StructureTemperatureData.EnergySource(kw, source));
		}
		return sources;
	}

	public static void DoStateTransition(int sim_handle)
	{
		HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
		if (StructureTemperatureComponents.handleInstanceMap.TryGetValue(sim_handle, out invalidHandle))
		{
			StructureTemperatureData data = GameComps.StructureTemperatures.GetData(invalidHandle);
			Element element = data.primaryElement.Element;
			if (element.highTempTransitionTarget != SimHashes.Unobtanium)
			{
				int num = Grid.PosToCell(data.primaryElement.transform.position);
				SimMessages.AddRemoveSubstance(num, element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, data.primaryElement.Mass, data.primaryElement.Temperature, data.primaryElement.DiseaseIdx, data.primaryElement.DiseaseCount, -1);
				Util.KDestroyGameObject(data.primaryElement.gameObject);
			}
		}
	}

	public static void DoOverheat(int sim_handle)
	{
		HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
		if (StructureTemperatureComponents.handleInstanceMap.TryGetValue(sim_handle, out invalidHandle))
		{
			GameComps.StructureTemperatures.GetData(invalidHandle).primaryElement.gameObject.Trigger(1832602615, null);
		}
	}

	public static void DoNoLongerOverheated(int sim_handle)
	{
		HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
		if (StructureTemperatureComponents.handleInstanceMap.TryGetValue(sim_handle, out invalidHandle))
		{
			GameComps.StructureTemperatures.GetData(invalidHandle).primaryElement.gameObject.Trigger(171119937, null);
		}
	}

	public bool IsEnabled(HandleVector<int>.Handle handle)
	{
		return base.GetData(handle).enabled;
	}

	public void Enable(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = base.GetData(handle);
		data.enabled = true;
		data.dirty = true;
		base.SetData(handle, data);
	}

	public void Disable(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = base.GetData(handle);
		data.enabled = false;
		data.dirty = true;
		base.SetData(handle, data);
	}

	protected void SimRegister(HandleVector<int>.Handle handle, ref StructureTemperatureData data)
	{
		if (data.simHandle == -1)
		{
			PrimaryElement primaryElement = data.primaryElement;
			if (primaryElement.Mass > 0f)
			{
				Element element = primaryElement.Element;
				if (!element.IsTemperatureInsulated)
				{
					data.simHandle = -2;
					HandleVector<Game.ComplexCallbackInfo>.Handle handle2 = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(delegate(object ev_data)
					{
						StructureTemperatureComponents.OnSimRegistered(handle, ev_data);
					}));
					float internalTemperature = primaryElement.InternalTemperature;
					float massForTemperatureModification = primaryElement.GetComponent<Building>().Def.MassForTemperatureModification;
					float operatingKilowatts = data.OperatingKilowatts;
					global::UnityEngine.Debug.Assert(internalTemperature > 0f, "Invalid temperature");
					global::UnityEngine.Debug.Assert(primaryElement.Mass > 0f);
					global::UnityEngine.Debug.Assert(massForTemperatureModification > 0f);
					Extents extents = data.GetExtents();
					byte b = (byte)ElementLoader.elements.IndexOf(primaryElement.Element);
					SimMessages.AddBuildingHeatExchange(extents, internalTemperature, operatingKilowatts, b, massForTemperatureModification, handle2.index);
					base.SetData(handle, data);
				}
			}
		}
	}

	private static void OnSimRegistered(HandleVector<int>.Handle handle, object ev_data)
	{
		int num = (int)ev_data;
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		if (data.simHandle == -2)
		{
			data.simHandle = num;
			StructureTemperatureComponents.handleInstanceMap[num] = handle;
			GameComps.StructureTemperatures.SetData(handle, data);
		}
		else
		{
			SimMessages.RemoveBuildingHeatExchange(num, -1);
		}
	}

	protected unsafe void SimUnregister(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = base.GetData(handle);
		if (data.simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			if (Sim.IsValidHandle(data.simHandle))
			{
				data.primaryElement.InternalTemperature = Game.Instance.simData.buildingTemperatures[data.simHandle].temperature;
				SimMessages.RemoveBuildingHeatExchange(data.simHandle, -1);
				StructureTemperatureComponents.handleInstanceMap.Remove(data.simHandle);
			}
			data.simHandle = -1;
			base.SetData(handle, data);
		}
	}

	private const float MAX_PRESSURE = 1.5f;

	private static Dictionary<int, HandleVector<int>.Handle> handleInstanceMap = new Dictionary<int, HandleVector<int>.Handle>();

	private StatusItem operatingEnergyStatusItem;
}
