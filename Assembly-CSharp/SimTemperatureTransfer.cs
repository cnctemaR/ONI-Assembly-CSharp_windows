using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class SimTemperatureTransfer : KMonoBehaviour
{
	public float SurfaceArea
	{
		get
		{
			return this.surfaceArea;
		}
		set
		{
			this.surfaceArea = value;
		}
	}

	public float Thickness
	{
		get
		{
			return this.thickness;
		}
		set
		{
			this.thickness = value;
		}
	}

	public int SimHandle
	{
		get
		{
			return this.simHandle;
		}
	}

	public static void ClearInstanceMap()
	{
		SimTemperatureTransfer.handleInstanceMap.Clear();
	}

	public static void DoStateTransition(int sim_handle)
	{
		SimTemperatureTransfer simTemperatureTransfer = null;
		if (SimTemperatureTransfer.handleInstanceMap.TryGetValue(sim_handle, out simTemperatureTransfer))
		{
			SimTemperatureTransfer simTemperatureTransfer2 = SimTemperatureTransfer.handleInstanceMap[sim_handle];
			if (simTemperatureTransfer2 != null && !simTemperatureTransfer2.HasTag(GameTags.Sealed))
			{
				PrimaryElement component = simTemperatureTransfer2.GetComponent<PrimaryElement>();
				Element element = component.Element;
				if (element.highTempTransitionTarget != SimHashes.Unobtanium)
				{
					int num = Grid.PosToCell(simTemperatureTransfer2.transform.GetPosition());
					SimMessages.AddRemoveSubstance(num, element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, -1);
					Util.KDestroyGameObject(simTemperatureTransfer2.gameObject);
				}
			}
		}
	}

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
		CellChangeMonitor.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged));
		if (component.Element.HasTag(GameTags.Special) || element.specificHeatCapacity == 0f)
		{
			base.enabled = false;
		}
		this.SimRegister();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (Sim.IsValidHandle(this.simHandle))
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			SimTemperatureTransfer.OnSetTemperature(component, component.Temperature);
		}
	}

	protected override void OnCmpDisable()
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			float temperature = component.Temperature;
			component.InternalTemperature = component.Temperature;
			SimMessages.SetElementChunkData(this.simHandle, temperature, 0f);
		}
		base.OnCmpDisable();
	}

	private void OnCellChanged()
	{
		int num = Grid.PosToCell(this);
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.MoveElementChunk(this.simHandle, num);
		}
	}

	protected override void OnCleanUp()
	{
		CellChangeMonitor.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged));
		this.SimUnregister();
		base.OnForcedCleanUp();
	}

	public void ModifyEnergy(float delta_kilojoules)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.ModifyElementChunkEnergy(this.simHandle, delta_kilojoules);
		}
		else
		{
			this.pendingEnergyModifications += delta_kilojoules;
		}
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		float num;
		if (Sim.IsValidHandle(component.simHandle))
		{
			num = Game.Instance.simData.elementChunks[component.simHandle].temperature;
			component.deltaKJ = Game.Instance.simData.elementChunks[component.simHandle].deltaKJ;
		}
		else
		{
			num = primary_element.InternalTemperature;
		}
		return num;
	}

	private unsafe static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		if (temperature <= 0f)
		{
			KCrashReporter.Assert(false, "STT.OnSetTemperature - Tried to set <= 0 degree temperature");
			temperature = 293f;
		}
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		if (Sim.IsValidHandle(component.simHandle))
		{
			float mass = primary_element.Mass;
			float num = ((mass < 0.01f) ? 0f : (mass * primary_element.Element.specificHeatCapacity));
			SimMessages.SetElementChunkData(component.simHandle, temperature, num);
			Game.Instance.simData.elementChunks[component.simHandle].temperature = temperature;
		}
		else
		{
			primary_element.InternalTemperature = temperature;
		}
	}

	private void OnDataChanged(PrimaryElement primary_element)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			float num = ((primary_element.Mass < 0.01f) ? 0f : (primary_element.Mass * primary_element.Element.specificHeatCapacity));
			SimMessages.SetElementChunkData(this.simHandle, primary_element.Temperature, num);
		}
	}

	protected void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1 && base.enabled)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component.Mass > 0f)
			{
				Element element = component.Element;
				if (!element.IsTemperatureInsulated)
				{
					int num = Grid.PosToCell(base.transform.GetPosition());
					this.simHandle = -2;
					HandleVector<Game.ComplexCallbackInfo>.Handle handle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(delegate(object data)
					{
						SimTemperatureTransfer.OnSimRegistered(this, data);
					}));
					float num2 = component.InternalTemperature;
					KCrashReporter.Assert(num2 > 0f, "Invalid temperature");
					KCrashReporter.Assert(component.Mass > 0f);
					if (num2 <= 0f)
					{
						component.InternalTemperature = 293f;
						num2 = 293f;
					}
					SimMessages.AddElementChunk(num, component.ElementID, component.Mass, num2, this.surfaceArea, this.thickness, handle.index);
				}
			}
		}
	}

	protected unsafe void SimUnregister()
	{
		if (this.simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (Sim.IsValidHandle(this.simHandle))
			{
				component.InternalTemperature = Game.Instance.simData.elementChunks[this.simHandle].temperature;
				SimMessages.RemoveElementChunk(this.simHandle, -1);
				SimTemperatureTransfer.handleInstanceMap.Remove(this.simHandle);
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
				KCrashReporter.Assert(false, "Bad temperature");
			}
			SimTemperatureTransfer.handleInstanceMap[instance.simHandle] = instance;
			if (instance.pendingEnergyModifications > 0f)
			{
				instance.ModifyEnergy(instance.pendingEnergyModifications);
				instance.pendingEnergyModifications = 0f;
			}
			if (instance.onSimRegistered != null)
			{
				instance.onSimRegistered(instance);
			}
		}
		else
		{
			SimMessages.RemoveElementChunk(num, -1);
		}
	}

	private const float MIN_MASS_FOR_TEMPERATURE_TRANSFER = 0.01f;

	public float deltaKJ;

	public Action<SimTemperatureTransfer> onSimRegistered;

	protected int simHandle = -1;

	private float pendingEnergyModifications;

	[SerializeField]
	protected float surfaceArea = 10f;

	[SerializeField]
	protected float thickness = 0.01f;

	private static Dictionary<int, SimTemperatureTransfer> handleInstanceMap = new Dictionary<int, SimTemperatureTransfer>();
}
