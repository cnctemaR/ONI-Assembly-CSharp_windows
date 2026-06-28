using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SimTemperatureTransfer : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(SimTemperatureTransfer.OnGetTemperature);
		component.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(SimTemperatureTransfer.OnSetTemperature);
		PrimaryElement primaryElement = component;
		primaryElement.onDataChanged = (Action<PrimaryElement>)Delegate.Combine(primaryElement.onDataChanged, new Action<PrimaryElement>(this.OnDataChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Element element = component.Element;
		CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChanged), false);
		if (component.Element.HasTag(GameTags.Special) || element.specificHeatCapacity == 0f)
		{
			base.enabled = false;
		}
		this.SimRegister();
	}

	private void OnCellChanged(int previous_cell, int cell)
	{
		if (cell == previous_cell)
		{
			return;
		}
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.MoveElementChunk(this.simHandle, cell);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.SimRegister();
	}

	protected override void OnCmpDisable()
	{
		this.SimUnregister();
		base.OnCmpDisable();
	}

	protected override void OnCleanUp()
	{
		CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChanged), false);
		this.SimUnregister();
		base.OnForcedCleanUp();
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		float num;
		if (Sim.IsValidHandle(component.simHandle))
		{
			num = Game.Instance.simData.elementChunks[component.simHandle].temperature;
		}
		else
		{
			num = primary_element.InternalTemperature;
		}
		return num;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		if (Sim.IsValidHandle(component.simHandle))
		{
			float num = primary_element.Mass * primary_element.Element.specificHeatCapacity;
			SimMessages.SetElementChunkData(component.simHandle, temperature, num);
		}
	}

	private void OnDataChanged(PrimaryElement primary_element)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			float num = primary_element.Mass * primary_element.Element.specificHeatCapacity;
			SimMessages.SetElementChunkData(this.simHandle, primary_element.Temperature, num);
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1 && base.isSpawned && base.enabled)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component.Mass > 0f)
			{
				Element element = component.Element;
				if (!element.IsTemperatureInsulated)
				{
					int num = Grid.PosToCell(this.transform.position);
					this.simHandle = -2;
					HandleVector<Action<object>>.Handle handle = Game.Instance.complexCallbackManager.Add(delegate(object data)
					{
						SimTemperatureTransfer.OnSimRegistered(this, data);
					}, "SimTempTransfer");
					float temperature = component.Temperature;
					Debug.Assert(temperature > 0f, "Invalid temperature");
					Debug.Assert(component.Mass > 0f);
					SimMessages.AddElementChunk(num, component.ElementID, component.Mass, temperature, handle.index);
				}
			}
		}
	}

	private unsafe void SimUnregister()
	{
		if (base.isSpawned && this.simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			if (Sim.IsValidHandle(this.simHandle))
			{
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				component.InternalTemperature = Game.Instance.simData.elementChunks[this.simHandle].temperature;
				SimMessages.RemoveElementChunk(-1, this.simHandle);
			}
			this.simHandle = -1;
		}
	}

	private unsafe static void OnSimRegistered(SimTemperatureTransfer instance, object data)
	{
		int num = (int)data;
		if (instance != null && instance.simHandle == -2)
		{
			instance.simHandle = num;
			float temperature = Game.Instance.simData.elementChunks[instance.simHandle].temperature;
			if (temperature <= 0f)
			{
				Debug.Assert(false, "Bad temperature");
			}
		}
		else
		{
			SimMessages.RemoveElementChunk(-1, num);
		}
	}

	private int simHandle = -1;
}
