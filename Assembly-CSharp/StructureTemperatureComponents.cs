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
		data.isActiveBuilding = data.building.Def.SelfHeatKilowattsWhenActive != 0f || data.ExhaustKilowatts != 0f;
		base.SetData(handle, data);
	}

	private void InitializeStatusItem()
	{
		if (this.operatingEnergyStatusItem != null)
		{
			return;
		}
		this.operatingEnergyStatusItem = new StatusItem("OperatingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
		this.operatingEnergyStatusItem.resolveStringCallback = delegate(string str, object ev_data)
		{
			int num = (int)ev_data;
			HandleVector<int>.Handle handle = StructureTemperatureComponents.handleInstanceMap[num];
			StructureTemperatureData data = base.GetData(handle);
			if (str != BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP)
			{
				try
				{
					str = string.Format(str, GameUtil.GetFormattedHeatEnergy(data.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
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
					text += string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, energySource.source, GameUtil.GetFormattedHeatEnergy(energySource.value * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
				}
				str = string.Format(str, GameUtil.GetFormattedHeatEnergy(data.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.None), text);
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
				StructureTemperatureComponents.OnActiveChanged(handle);
			});
		}
		Overheatable component = data.primaryElement.GetComponent<Overheatable>();
		data.maxTemperature = ((!(component != null)) ? 10000f : component.OverheatTemperature);
		if (data.maxTemperature <= 0f)
		{
			Output.LogError(new object[] { "invalid max temperature" });
		}
		base.SetData(handle, data);
		this.SimRegister(handle, ref data);
	}

	private static void OnActiveChanged(HandleVector<int>.Handle handle)
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

	public override void Sim200ms(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			StructureTemperatureData structureTemperatureData = this.data[i];
			if (Sim.IsValidHandle(structureTemperatureData.simHandle))
			{
				StructureTemperatureComponents.UpdateSimState(ref structureTemperatureData);
				structureTemperatureData.ApplyPendingEnergyModifications();
				if (!structureTemperatureData.isActiveBuilding)
				{
					this.data[i] = structureTemperatureData;
				}
				else
				{
					if (structureTemperatureData.operational == null || structureTemperatureData.operational.IsActive)
					{
						if (!structureTemperatureData.isActiveStatusItemSet)
						{
							structureTemperatureData.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, this.operatingEnergyStatusItem, structureTemperatureData.simHandle);
							structureTemperatureData.isActiveStatusItemSet = true;
						}
						structureTemperatureData.energySourcesKW = this.AccumulateProducedEnergyKW(structureTemperatureData.energySourcesKW, structureTemperatureData.OperatingKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING);
						if (structureTemperatureData.ExhaustKilowatts != 0f)
						{
							Extents extents = structureTemperatureData.GetExtents();
							int num = extents.width * extents.height;
							float num2 = structureTemperatureData.ExhaustKilowatts * dt / (float)num;
							for (int j = 0; j < extents.height; j++)
							{
								int num3 = extents.y + j;
								for (int k = 0; k < extents.width; k++)
								{
									int num4 = extents.x + k;
									int num5 = num3 * Grid.WidthInCells + num4;
									float num6 = Grid.Mass[num5];
									float num7 = Mathf.Min(num6, 1.5f) / 1.5f;
									float num8 = num2 * num7;
									SimMessages.ModifyEnergy(num5, num8, structureTemperatureData.maxTemperature, SimMessages.EnergySourceID.StructureTemperature);
								}
							}
							structureTemperatureData.energySourcesKW = this.AccumulateProducedEnergyKW(structureTemperatureData.energySourcesKW, structureTemperatureData.ExhaustKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING);
						}
					}
					else if (structureTemperatureData.isActiveStatusItemSet)
					{
						structureTemperatureData.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, null, null);
						structureTemperatureData.isActiveStatusItemSet = false;
					}
					this.data[i] = structureTemperatureData;
				}
			}
		}
	}

	private static void UpdateSimState(ref StructureTemperatureData data)
	{
		if (!data.dirty)
		{
			return;
		}
		if (!Sim.IsValidHandle(data.simHandle))
		{
			return;
		}
		data.dirty = false;
		float internalTemperature = data.primaryElement.InternalTemperature;
		BuildingDef def = data.building.Def;
		float num = def.MassForTemperatureModification;
		float operatingKilowatts = data.OperatingKilowatts;
		Overheatable component = data.primaryElement.GetComponent<Overheatable>();
		float num2 = ((!(component != null)) ? 10000f : component.OverheatTemperature);
		if (!data.enabled)
		{
			num = 0f;
		}
		Extents extents = data.GetExtents();
		byte idx = data.primaryElement.Element.idx;
		SimMessages.ModifyBuildingHeatExchange(data.simHandle, extents, num, internalTemperature, def.ThermalConductivity, num2, operatingKilowatts, idx);
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
				int handleIndex = Sim.GetHandleIndex(data.simHandle);
				num = Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
			}
			else
			{
				int num2 = Grid.PosToCell(data.primaryElement.transform.GetPosition());
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
			StructureTemperatureComponents.UpdateSimState(ref data);
			data.ApplyPendingEnergyModifications();
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
			StructureTemperatureComponents.DoMelt(GameComps.StructureTemperatures.GetData(invalidHandle).primaryElement);
		}
	}

	public static void DoMelt(PrimaryElement primary_element)
	{
		Element element = primary_element.Element;
		if (element.highTempTransitionTarget != SimHashes.Unobtanium)
		{
			int num = Grid.PosToCell(primary_element.transform.GetPosition());
			SimMessages.AddRemoveSubstance(num, element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, primary_element.Mass, primary_element.Element.highTemp, primary_element.DiseaseIdx, primary_element.DiseaseCount, true, -1);
			Util.KDestroyGameObject(primary_element.gameObject);
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
					string dbg_name = primaryElement.name;
					HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle2 = Game.Instance.simComponentCallbackManager.Add(delegate(int sim_handle, object callback_data)
					{
						StructureTemperatureComponents.OnSimRegistered(handle, sim_handle, dbg_name);
					}, null, "StructureTemperature.SimRegister");
					BuildingDef def = primaryElement.GetComponent<Building>().Def;
					float internalTemperature = primaryElement.InternalTemperature;
					float massForTemperatureModification = def.MassForTemperatureModification;
					float operatingKilowatts = data.OperatingKilowatts;
					Extents extents = data.GetExtents();
					byte b = (byte)ElementLoader.elements.IndexOf(primaryElement.Element);
					SimMessages.AddBuildingHeatExchange(extents, massForTemperatureModification, internalTemperature, def.ThermalConductivity, operatingKilowatts, b, handle2.index);
					base.SetData(handle, data);
				}
			}
		}
	}

	private static void OnSimRegistered(HandleVector<int>.Handle handle, int sim_handle, string dbg_name)
	{
		if (!GameComps.StructureTemperatures.IsValid(handle))
		{
			return;
		}
		if (!GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			return;
		}
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		if (data.simHandle == -2)
		{
			data.simHandle = sim_handle;
			StructureTemperatureComponents.handleInstanceMap[sim_handle] = handle;
			GameComps.StructureTemperatures.SetData(handle, data);
			data.primaryElement.Trigger(-1555603773, null);
		}
		else
		{
			SimMessages.RemoveBuildingHeatExchange(sim_handle, -1);
		}
	}

	protected unsafe void SimUnregister(HandleVector<int>.Handle handle)
	{
		if (!GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			KCrashReporter.Assert(false, "Handle version mismatch in StructureTemperature.SimUnregister");
			return;
		}
		StructureTemperatureData data = base.GetData(handle);
		if (data.simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			if (Sim.IsValidHandle(data.simHandle))
			{
				int handleIndex = Sim.GetHandleIndex(data.simHandle);
				data.primaryElement.InternalTemperature = Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
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
