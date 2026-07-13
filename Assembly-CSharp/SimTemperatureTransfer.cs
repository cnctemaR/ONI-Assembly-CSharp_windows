using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			return this.groundTransferScale;
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
		PrimaryElement primaryElement = simTemperatureTransfer.pe;
		Element element = primaryElement.Element;
		bool flag = primaryElement.Temperature >= element.highTemp;
		bool flag2 = primaryElement.Temperature <= element.lowTemp;
		if (!flag && !flag2)
		{
			return;
		}
		if (flag && element.highTempTransitionTarget == SimHashes.Unobtanium)
		{
			return;
		}
		if (flag2 && element.lowTempTransitionTarget == SimHashes.Unobtanium)
		{
			return;
		}
		if (primaryElement.Mass > 0f)
		{
			int num = Grid.PosToCell(simTemperatureTransfer.transform.GetPosition());
			float num2 = primaryElement.Mass;
			int num3 = primaryElement.DiseaseCount;
			SimHashes simHashes = (flag ? element.highTempTransitionTarget : element.lowTempTransitionTarget);
			SimHashes simHashes2 = (flag ? element.highTempTransitionOreID : element.lowTempTransitionOreID);
			float num4 = (flag ? element.highTempTransitionOreMassConversion : element.lowTempTransitionOreMassConversion);
			if (simHashes2 != (SimHashes)0)
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
						GameObject gameObject = element2.substance.SpawnResource(simTemperatureTransfer.transform.GetPosition(), num5, primaryElement.Temperature, primaryElement.DiseaseIdx, num6, true, false, true);
						element2.substance.ActivateSubstanceGameObject(gameObject, primaryElement.DiseaseIdx, num6);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num, element2.id, CellEventLogger.Instance.OreMelted, num5, primaryElement.Temperature, primaryElement.DiseaseIdx, num6, true, -1);
					}
				}
			}
			SimMessages.AddRemoveSubstance(num, simHashes, CellEventLogger.Instance.OreMelted, num2, primaryElement.Temperature, primaryElement.DiseaseIdx, num3, true, -1);
		}
		simTemperatureTransfer.OnCleanUp();
		Util.KDestroyGameObject(simTemperatureTransfer.gameObject);
	}

	protected override void OnPrefabInit()
	{
		this.pe.sttOptimizationHook = this;
		this.pe.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(SimTemperatureTransfer.OnGetTemperature);
		this.pe.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(SimTemperatureTransfer.OnSetTemperature);
		PrimaryElement primaryElement = this.pe;
		primaryElement.onDataChanged = (Action<PrimaryElement>)Delegate.Combine(primaryElement.onDataChanged, new Action<PrimaryElement>(this.OnDataChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Element element = this.pe.Element;
		this.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, SimTemperatureTransfer.OnCellChangedDispatcher, this, "SimTemperatureTransfer.OnSpawn");
		if (!Grid.IsValidCell(Grid.PosToCell(this)) || this.pe.Element.HasTag(GameTags.Special) || element.specificHeatCapacity == 0f)
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
			SimTemperatureTransfer.OnSetTemperature(this.pe, this.pe.Temperature);
		}
	}

	protected override void OnCmpDisable()
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			float temperature = this.pe.Temperature;
			this.pe.InternalTemperature = this.pe.Temperature;
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
			return;
		}
		this.forceDataSyncOnRegister = true;
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangedHandlerID);
		this.SimUnregister();
		base.OnForcedCleanUp();
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimTemperatureTransfer sttOptimizationHook = primary_element.sttOptimizationHook;
		float num;
		if (Sim.IsValidHandle(sttOptimizationHook.simHandle))
		{
			int handleIndex = Sim.GetHandleIndex(sttOptimizationHook.simHandle);
			num = Game.Instance.simData.elementChunks[handleIndex].temperature;
			sttOptimizationHook.deltaKJ = Game.Instance.simData.elementChunks[handleIndex].deltaKJ;
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
			KCrashReporter.Assert(false, "STT.OnSetTemperature - Tried to set <= 0 degree temperature", null);
			temperature = 293f;
		}
		primary_element.InternalTemperature = temperature;
		SimTemperatureTransfer sttOptimizationHook = primary_element.sttOptimizationHook;
		if (Sim.IsValidHandle(sttOptimizationHook.simHandle))
		{
			float mass = primary_element.Mass;
			float num = ((mass >= 0.01f) ? (mass * primary_element.Element.specificHeatCapacity) : 0f);
			SimMessages.SetElementChunkData(sttOptimizationHook.simHandle, temperature, num);
			int handleIndex = Sim.GetHandleIndex(sttOptimizationHook.simHandle);
			Game.Instance.simData.elementChunks[handleIndex].temperature = temperature;
		}
	}

	private void OnDataChanged(PrimaryElement primary_element)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			float num = ((primary_element.Mass >= 0.01f) ? (primary_element.Mass * primary_element.Element.specificHeatCapacity) : 0f);
			SimMessages.SetElementChunkData(this.simHandle, primary_element.Temperature, num);
			return;
		}
		this.forceDataSyncOnRegister = true;
	}

	protected void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1 && base.enabled && this.pe.Mass > 0f && !this.pe.Element.IsTemperatureInsulated)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			this.simHandle = -2;
			HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle = Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(SimTemperatureTransfer.OnSimRegisteredCallback), this, "SimTemperatureTransfer.SimRegister");
			float num2 = this.pe.InternalTemperature;
			if (num2 <= 0f)
			{
				this.pe.InternalTemperature = 293f;
				num2 = 293f;
			}
			this.forceDataSyncOnRegister = false;
			SimMessages.AddElementChunk(num, this.pe.ElementID, this.pe.Mass, num2, this.surfaceArea, this.thickness, this.groundTransferScale, handle.index);
		}
	}

	protected unsafe void SimUnregister()
	{
		if (this.simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			if (Sim.IsValidHandle(this.simHandle))
			{
				int handleIndex = Sim.GetHandleIndex(this.simHandle);
				this.pe.InternalTemperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
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
			float temperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
			float internalTemperature = this.pe.InternalTemperature;
			if (temperature <= 0f)
			{
				KCrashReporter.Assert(false, "Bad temperature", null);
			}
			SimTemperatureTransfer.handleInstanceMap[this.simHandle] = this;
			if (this.forceDataSyncOnRegister || Mathf.Abs(temperature - internalTemperature) > 0.1f)
			{
				float num = ((this.pe.Mass >= 0.01f) ? (this.pe.Mass * this.pe.Element.specificHeatCapacity) : 0f);
				SimMessages.SetElementChunkData(this.simHandle, internalTemperature, num);
				SimMessages.MoveElementChunk(this.simHandle, Grid.PosToCell(this));
				Game.Instance.simData.elementChunks[handleIndex].temperature = internalTemperature;
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

	[MyCmpReq]
	public PrimaryElement pe;

	private const float SIM_FREEZE_SPAWN_ORE_PERCENT = 0.8f;

	public const float MIN_MASS_FOR_TEMPERATURE_TRANSFER = 0.01f;

	public float deltaKJ;

	public Action<SimTemperatureTransfer> onSimRegistered;

	protected int simHandle = -1;

	protected bool forceDataSyncOnRegister;

	[SerializeField]
	protected float surfaceArea = 10f;

	[SerializeField]
	protected float thickness = 0.01f;

	[SerializeField]
	protected float groundTransferScale = 0.0625f;

	private static Dictionary<int, SimTemperatureTransfer> handleInstanceMap = new Dictionary<int, SimTemperatureTransfer>();

	private ulong cellChangedHandlerID;

	private static readonly Action<object> OnCellChangedDispatcher = delegate(object obj)
	{
		Unsafe.As<SimTemperatureTransfer>(obj).OnCellChanged();
	};
}
