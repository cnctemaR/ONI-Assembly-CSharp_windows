using System;
using System.Runtime.InteropServices;

public class ConduitTemperatureManager
{
	public ConduitTemperatureManager()
	{
		ConduitTemperatureManager.ConduitTemperatureManager_Initialize();
	}

	public void Shutdown()
	{
		ConduitTemperatureManager.ConduitTemperatureManager_Shutdown();
	}

	public HandleVector<int>.Handle Allocate(ConduitType conduit_type, int conduit_idx, HandleVector<int>.Handle conduit_structure_temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(conduit_structure_temperature_handle);
		Element element = data.primaryElement.Element;
		BuildingDef def = data.building.Def;
		float num = def.MassForTemperatureModification * element.specificHeatCapacity;
		float num2 = element.thermalConductivity * def.ThermalConductivity;
		int num3 = ConduitTemperatureManager.ConduitTemperatureManager_Add(contents.temperature, contents.mass, (int)contents.element, data.simHandle, num, num2, def.ThermalConductivity < 1f);
		HandleVector<int>.Handle handle = default(HandleVector<int>.Handle);
		handle.index = num3;
		int handleIndex = Sim.GetHandleIndex(num3);
		if (handleIndex + 1 > this.temperatures.Length)
		{
			Array.Resize<float>(ref this.temperatures, (handleIndex + 1) * 2);
			Array.Resize<ConduitTemperatureManager.ConduitInfo>(ref this.conduitInfo, (handleIndex + 1) * 2);
		}
		this.temperatures[handleIndex] = contents.temperature;
		this.conduitInfo[handleIndex] = new ConduitTemperatureManager.ConduitInfo
		{
			type = conduit_type,
			idx = conduit_idx
		};
		return handle;
	}

	public void SetData(HandleVector<int>.Handle handle, ref ConduitFlow.ConduitContents contents)
	{
		if (!handle.IsValid())
		{
			return;
		}
		this.temperatures[Sim.GetHandleIndex(handle.index)] = contents.temperature;
		ConduitTemperatureManager.ConduitTemperatureManager_Set(handle.index, contents.temperature, contents.mass, (int)contents.element);
	}

	public void Free(HandleVector<int>.Handle handle)
	{
		if (handle.IsValid())
		{
			int handleIndex = Sim.GetHandleIndex(handle.index);
			this.temperatures[handleIndex] = -1f;
			this.conduitInfo[handleIndex] = new ConduitTemperatureManager.ConduitInfo
			{
				type = ConduitType.None,
				idx = -1
			};
			ConduitTemperatureManager.ConduitTemperatureManager_Remove(handle.index);
		}
	}

	public void Clear()
	{
		ConduitTemperatureManager.ConduitTemperatureManager_Clear();
	}

	public unsafe void Sim200ms(float dt)
	{
		IntPtr intPtr = ConduitTemperatureManager.ConduitTemperatureManager_Update(dt, (IntPtr)((void*)Game.Instance.simData.buildingTemperatures));
		ConduitTemperatureManager.ConduitTemperatureUpdateData* ptr = (ConduitTemperatureManager.ConduitTemperatureUpdateData*)(void*)intPtr;
		int numEntries = ptr->numEntries;
		if (numEntries > 0)
		{
			Marshal.Copy((IntPtr)((void*)ptr->temperatures), this.temperatures, 0, numEntries);
		}
		for (int i = 0; i < ptr->numFrozenHandles; i++)
		{
			int num = ptr->frozenHandles[i];
			int handleIndex = Sim.GetHandleIndex(num);
			ConduitTemperatureManager.ConduitInfo conduitInfo = this.conduitInfo[handleIndex];
			ConduitFlow flowManager = Conduit.GetFlowManager(conduitInfo.type);
			flowManager.FreezeConduitContents(conduitInfo.idx);
		}
		for (int j = 0; j < ptr->numMeltedHandles; j++)
		{
			int num2 = ptr->meltedHandles[j];
			int handleIndex2 = Sim.GetHandleIndex(num2);
			ConduitTemperatureManager.ConduitInfo conduitInfo2 = this.conduitInfo[handleIndex2];
			ConduitFlow flowManager2 = Conduit.GetFlowManager(conduitInfo2.type);
			flowManager2.MeltConduitContents(conduitInfo2.idx);
		}
	}

	public float GetTemperature(HandleVector<int>.Handle handle)
	{
		return this.temperatures[Sim.GetHandleIndex(handle.index)];
	}

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Initialize();

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Shutdown();

	[DllImport("SimDLL")]
	private static extern int ConduitTemperatureManager_Add(float contents_temperature, float contents_mass, int contents_element_hash, int conduit_structure_temperature_handle, float conduit_heat_capacity, float conduit_thermal_conductivity, bool conduit_insulated);

	[DllImport("SimDLL")]
	private static extern int ConduitTemperatureManager_Set(int handle, float contents_temperature, float contents_mass, int contents_element_hash);

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Remove(int handle);

	[DllImport("SimDLL")]
	private static extern IntPtr ConduitTemperatureManager_Update(float dt, IntPtr building_conductivity_data);

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Clear();

	private float[] temperatures = new float[0];

	private ConduitTemperatureManager.ConduitInfo[] conduitInfo = new ConduitTemperatureManager.ConduitInfo[0];

	private struct ConduitInfo
	{
		public ConduitType type;

		public int idx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ConduitTemperatureUpdateData
	{
		public int numEntries;

		public unsafe float* temperatures;

		public int numFrozenHandles;

		public unsafe int* frozenHandles;

		public int numMeltedHandles;

		public unsafe int* meltedHandles;
	}
}
