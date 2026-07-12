using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SimTemperatureTransfer")]
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

	public float GroundTransferScale
	{
		get
		{
			return this.GroundTransferScale;
		}
		set
		{
			this.groundTransferScale = value;
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

	public static void DoOreMeltTransition(int sim_handle)
	{
		SimTemperatureTransfer simTemperatureTransfer = null;
		if (!SimTemperatureTransfer.handleInstanceMap.TryGetValue(sim_handle, out simTemperatureTransfer))
		{
			return;
		}
		if (simTemperatureTransfer == null)
		{
			return;
		}
		if (simTemperatureTransfer.HasTag(GameTags.Sealed))
		{
			return;
		}
		PrimaryElement component = simTemperatureTransfer.GetComponent<PrimaryElement>();
		Element element = component.Element;
		bool flag = component.Temperature >= element.highTemp;
		bool flag2 = component.Temperature <= element.lowTemp;
		DebugUtil.DevAssert(flag || flag2, "An ore got a melt message from the sim but it's still the correct temperature for its state!", component);
		if (flag && element.highTempTransitionTarget == SimHashes.Unobtanium)
		{
			return;
		}
		if (flag2 && element.lowTempTransitionTarget == SimHashes.Unobtanium)
		{
			return;
		}
		if (component.Mass > 0f)
		{
			int num = Grid.PosToCell(simTemperatureTransfer.transform.GetPosition());
			float num2 = component.Mass;
			int num3 = component.DiseaseCount;
			SimHashes simHashes = (flag ? element.highTempTransitionTarget : element.lowTempTransitionTarget);
			SimHashes simHashes2 = (flag ? element.highTempTransitionOreID : element.lowTempTransitionOreID);
			float num4 = (flag ? element.highTempTransitionOreMassConversion : element.lowTempTransitionOreMassConversion);
			if ((byte)simHashes2 != 255)
			{
				float num5 = num2 * num4;
				int num6 = (int)((float)num3 * num4);
				if (num5 > 0.001f)
				{
					num2 -= num5;
					num3 -= num6;
					Element element2 = ElementLoader.FindElementByHash(simHashes2);
					if (element2.IsSolid)
					{
						GameObject gameObject = element2.substance.SpawnResource(simTemperatureTransfer.transform.GetPosition(), num5, component.Temperature, component.DiseaseIdx, num6, true, false, true);
						element2.substance.ActivateSubstanceGameObject(gameObject, component.DiseaseIdx, num6);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num, element2.id, CellEventLogger.Instance.OreMelted, num5, component.Temperature, component.DiseaseIdx, num6, true, -1);
					}
				}
			}
			SimMessages.AddRemoveSubstance(num, simHashes, CellEventLogger.Instance.OreMelted, num2, component.Temperature, component.DiseaseIdx, num3, true, -1);
		}
		simTemperatureTransfer.OnCleanUp();
		Util.KDestroyGameObject(simTemperatureTransfer.gameObject);
	}

	protected override void OnPrefabInit()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(SimTemperatureTransfer.OnGetTemperature);
		component.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(SimTemperatureTransfer.OnSetTemperature);
		component.onDataChanged = (Action<PrimaryElement>)Delegate.Combine(component.onDataChanged, new Action<PrimaryElement>(this.OnDataChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Element element = component.Element;
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged), "SimTemperatureTransfer.OnSpawn");
		if (!Grid.IsValidCell(Grid.PosToCell(this)) || component.Element.HasTag(GameTags.Special) || element.specificHeatCapacity == 0f)
		{
			base.enabled = false;
		}
		this.SimRegister();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.SimRegister();
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
		if (!Grid.IsValidCell(num))
		{
			base.enabled = false;
			return;
		}
		this.SimRegister();
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.MoveElementChunk(this.simHandle, num);
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged));
		this.SimUnregister();
		base.OnForcedCleanUp();
	}

	public void ModifyEnergy(float delta_kilojoules)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.ModifyElementChunkEnergy(this.simHandle, delta_kilojoules);
			return;
		}
		this.pendingEnergyModifications += delta_kilojoules;
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		float num;
		if (Sim.IsValidHandle(component.simHandle))
		{
			int handleIndex = Sim.GetHandleIndex(component.simHandle);
			num = Game.Instance.simData.elementChunks[handleIndex].temperature;
			component.deltaKJ = Game.Instance.simData.elementChunks[handleIndex].deltaKJ;
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
			float num = ((mass >= 0.01f) ? (mass * primary_element.Element.specificHeatCapacity) : 0f);
			SimMessages.SetElementChunkData(component.simHandle, temperature, num);
			int handleIndex = Sim.GetHandleIndex(component.simHandle);
			Game.Instance.simData.elementChunks[handleIndex].temperature = temperature;
			return;
		}
		primary_element.InternalTemperature = temperature;
	}

	private void OnDataChanged(PrimaryElement primary_element)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			float num = ((primary_element.Mass >= 0.01f) ? (primary_element.Mass * primary_element.Element.specificHeatCapacity) : 0f);
			SimMessages.SetElementChunkData(this.simHandle, primary_element.Temperature, num);
		}
	}

	protected void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1 && base.enabled)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component.Mass > 0f && !component.Element.IsTemperatureInsulated)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				this.simHandle = -2;
				HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle = Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(SimTemperatureTransfer.OnSimRegisteredCallback), this, "SimTemperatureTransfer.SimRegister");
				float num2 = component.InternalTemperature;
				if (num2 <= 0f)
				{
					component.InternalTemperature = 293f;
					num2 = 293f;
				}
				SimMessages.AddElementChunk(num, component.ElementID, component.Mass, num2, this.surfaceArea, this.thickness, this.groundTransferScale, handle.index);
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
				int handleIndex = Sim.GetHandleIndex(this.simHandle);
				component.InternalTemperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
				SimMessages.RemoveElementChunk(this.simHandle, -1);
				SimTemperatureTransfer.handleInstanceMap.Remove(this.simHandle);
			}
			this.simHandle = -1;
		}
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((SimTemperatureTransfer)data).OnSimRegistered(handle);
	}

	private unsafe void OnSimRegistered(int handle)
	{
		if (this != null && this.simHandle == -2)
		{
			this.simHandle = handle;
			int handleIndex = Sim.GetHandleIndex(handle);
			if (Game.Instance.simData.elementChunks[handleIndex].temperature <= 0f)
			{
				KCrashReporter.Assert(false, "Bad temperature");
			}
			SimTemperatureTransfer.handleInstanceMap[this.simHandle] = this;
			if (this.pendingEnergyModifications > 0f)
			{
				this.ModifyEnergy(this.pendingEnergyModifications);
				this.pendingEnergyModifications = 0f;
			}
			if (this.onSimRegistered != null)
			{
				this.onSimRegistered(this);
			}
			if (!base.enabled)
			{
				this.OnCmpDisable();
				return;
			}
		}
		else
		{
			SimMessages.RemoveElementChunk(handle, -1);
		}
	}

	private const float SIM_FREEZE_SPAWN_ORE_PERCENT = 0.8f;

	public const float MIN_MASS_FOR_TEMPERATURE_TRANSFER = 0.01f;

	public float deltaKJ;

	public Action<SimTemperatureTransfer> onSimRegistered;

	protected int simHandle = -1;

	private float pendingEnergyModifications;

	[SerializeField]
	protected float surfaceArea = 10f;

	[SerializeField]
	protected float thickness = 0.01f;

	[SerializeField]
	protected float groundTransferScale = 0.0625f;

	private static Dictionary<int, SimTemperatureTransfer> handleInstanceMap = new Dictionary<int, SimTemperatureTransfer>();
}
