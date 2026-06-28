using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Klei;
using UnityEngine;

public static class SimMessages
{
	public unsafe static void AddElementConsumer(int gameCell, ElementConsumer.Configuration configuration, SimHashes element, float consumption_rate, byte radius, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		int elementIndex = ElementLoader.GetElementIndex(element);
		SimMessages.AddElementConsumerMessage* ptr = stackalloc SimMessages.AddElementConsumerMessage[checked(1 * sizeof(SimMessages.AddElementConsumerMessage))];
		ptr->cellIdx = gameCell;
		ptr->consumptionRate = consumption_rate;
		ptr->configuration = (byte)configuration;
		ptr->elementIdx = (byte)elementIndex;
		ptr->radius = radius;
		ptr->callbackIdx = cb_handle;
		Sim.SIM_HandleMessage(2024405073, sizeof(SimMessages.AddElementConsumerMessage), (byte*)ptr);
	}

	public unsafe static void RemoveElementConsumer(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
			return;
		}
		SimMessages.RemoveElementConsumerMessage* ptr = stackalloc SimMessages.RemoveElementConsumerMessage[checked(1 * sizeof(SimMessages.RemoveElementConsumerMessage))];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(894417742, sizeof(SimMessages.RemoveElementConsumerMessage), (byte*)ptr);
	}

	public unsafe static void AddElementEmitter(int gameCell, SimHashes element, float emit_interval, float emit_mass, float emit_temperature, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		int elementIndex = ElementLoader.GetElementIndex(element);
		SimMessages.AddElementEmitterMessage* ptr = stackalloc SimMessages.AddElementEmitterMessage[checked(1 * sizeof(SimMessages.AddElementEmitterMessage))];
		ptr->cellIdx = gameCell;
		ptr->callbackIdx = cb_handle;
		ptr->emitInterval = emit_interval;
		ptr->emitMass = emit_mass;
		ptr->emitTemperature = emit_temperature;
		ptr->elementIdx = (byte)elementIndex;
		Sim.SIM_HandleMessage(-505471181, sizeof(SimMessages.AddElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void RemoveElementEmitter(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
			return;
		}
		SimMessages.RemoveElementEmitterMessage* ptr = stackalloc SimMessages.RemoveElementEmitterMessage[checked(1 * sizeof(SimMessages.RemoveElementEmitterMessage))];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(-1524118282, sizeof(SimMessages.RemoveElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void AddElementChunk(int gameCell, SimHashes element, float mass, float temperature, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		Debug.Assert(mass * temperature > 0f, "Tried to add an SimTemperatureTransfer component with 0 mass or temperature. Unsupported! Your game is now in a bad state.");
		if (mass * temperature > 0f)
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			SimMessages.AddElementChunkMessage* ptr = stackalloc SimMessages.AddElementChunkMessage[checked(1 * sizeof(SimMessages.AddElementChunkMessage))];
			ptr->gameCell = gameCell;
			ptr->callbackIdx = cb_handle;
			ptr->mass = mass;
			ptr->temperature = temperature;
			ptr->elementIdx = (byte)elementIndex;
			Sim.SIM_HandleMessage(1445724082, sizeof(SimMessages.AddElementChunkMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementChunk(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
			return;
		}
		SimMessages.RemoveElementChunkMessage* ptr = stackalloc SimMessages.RemoveElementChunkMessage[checked(1 * sizeof(SimMessages.RemoveElementChunkMessage))];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(-912908555, sizeof(SimMessages.RemoveElementChunkMessage), (byte*)ptr);
	}

	public unsafe static void SetElementChunkData(int sim_handle, float temperature, float heat_capacity)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			return;
		}
		Debug.Assert(heat_capacity > 0f, "Tried to set SimTemperatureTransfer's heat capacity to zero. This is an invalid thing to do!");
		if (heat_capacity > 0f)
		{
			SimMessages.SetElementChunkDataMessage* ptr = stackalloc SimMessages.SetElementChunkDataMessage[checked(1 * sizeof(SimMessages.SetElementChunkDataMessage))];
			ptr->handle = sim_handle;
			ptr->temperature = temperature;
			ptr->heatCapacity = heat_capacity;
			Sim.SIM_HandleMessage(-435115907, sizeof(SimMessages.SetElementChunkDataMessage), (byte*)ptr);
		}
	}

	public unsafe static void MoveElementChunk(int sim_handle, int cell)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
			return;
		}
		SimMessages.MoveElementChunkMessage* ptr = stackalloc SimMessages.MoveElementChunkMessage[checked(1 * sizeof(SimMessages.MoveElementChunkMessage))];
		ptr->handle = sim_handle;
		ptr->gameCell = cell;
		Sim.SIM_HandleMessage(-374911358, sizeof(SimMessages.MoveElementChunkMessage), (byte*)ptr);
	}

	public unsafe static void CreateSimElementsTable(List<Element> elements)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Element)) * elements.Count);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(elements.Count);
		for (int i = 0; i < elements.Count; i++)
		{
			Sim.Element element = new Sim.Element(elements[i], elements);
			element.Write(binaryWriter);
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* ptr = (ref buffer != null && buffer.Length != 0 ? ref buffer[0] : ref *null))
		{
			Sim.SIM_HandleMessage(1108437482, buffer.Length, ptr);
		}
	}

	public static void SimDataInitializeFromCells(int width, int height, Sim.Cell[] cells, float[] bgTemp, List<Element> elements)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Cell)) * width * height + Marshal.SizeOf(typeof(float)) * width * height);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(width);
		binaryWriter.Write(height);
		int num = width * height;
		for (int i = 0; i < num; i++)
		{
			cells[i].Write(binaryWriter, elements);
		}
		for (int j = 0; j < num; j++)
		{
			binaryWriter.Write(bgTemp[j]);
		}
		byte[] buffer = memoryStream.GetBuffer();
		Sim.HandleMessage(SimMessageHashes.SimData_InitializeFromCells, buffer.Length, buffer);
	}

	public unsafe static void Dig(int gameCell, int callbackIdx = -1)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.DigMessage* ptr = stackalloc SimMessages.DigMessage[checked(1 * sizeof(SimMessages.DigMessage))];
		ptr->cellIdx = gameCell;
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(833038498, sizeof(SimMessages.DigMessage), (byte*)ptr);
		CellEventLogger.Instance.Dig.Log(gameCell, callbackIdx);
	}

	public unsafe static void SetInsulation(int gameCell, float value)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.SetCellFloatValueMessage* ptr = stackalloc SimMessages.SetCellFloatValueMessage[checked(1 * sizeof(SimMessages.SetCellFloatValueMessage))];
		ptr->cellIdx = gameCell;
		ptr->value = value;
		Sim.SIM_HandleMessage(-898773121, sizeof(SimMessages.SetCellFloatValueMessage), (byte*)ptr);
	}

	public unsafe static void SetStrength(int gameCell, int weight, float strengthMultiplier)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.SetCellFloatValueMessage* ptr = stackalloc SimMessages.SetCellFloatValueMessage[checked(1 * sizeof(SimMessages.SetCellFloatValueMessage))];
		ptr->cellIdx = gameCell;
		int num = (int)(strengthMultiplier * 4f) & 127;
		int num2 = ((weight & 1) << 7) | num;
		ptr->value = (float)((byte)num2);
		Sim.SIM_HandleMessage(1593243982, sizeof(SimMessages.SetCellFloatValueMessage), (byte*)ptr);
	}

	public unsafe static void SetCellProperties(int gameCell, byte properties)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.CellPropertiesMessage* ptr = stackalloc SimMessages.CellPropertiesMessage[checked(1 * sizeof(SimMessages.CellPropertiesMessage))];
		ptr->cellIdx = gameCell;
		ptr->properties = properties;
		ptr->set = 1;
		Sim.SIM_HandleMessage(-469311643, sizeof(SimMessages.CellPropertiesMessage), (byte*)ptr);
	}

	public unsafe static void ClearCellProperties(int gameCell, byte properties)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.CellPropertiesMessage* ptr = stackalloc SimMessages.CellPropertiesMessage[checked(1 * sizeof(SimMessages.CellPropertiesMessage))];
		ptr->cellIdx = gameCell;
		ptr->properties = properties;
		ptr->set = 0;
		Sim.SIM_HandleMessage(-469311643, sizeof(SimMessages.CellPropertiesMessage), (byte*)ptr);
	}

	public unsafe static void ModifyCell(int gameCell, int elementIdx, float temperature, float mass, bool replace = false, int callbackIdx = -1)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.ModifyCellMessage* ptr = stackalloc SimMessages.ModifyCellMessage[checked(1 * sizeof(SimMessages.ModifyCellMessage))];
		ptr->cellIdx = gameCell;
		ptr->callbackIdx = callbackIdx;
		ptr->temperature = temperature;
		ptr->mass = mass;
		ptr->elementIdx = (byte)elementIdx;
		ptr->replace = replace;
		SimUtil.CheckValidValue(temperature);
		SimUtil.CheckValidValue(mass);
		Sim.SIM_HandleMessage(-1252920804, sizeof(SimMessages.ModifyCellMessage), (byte*)ptr);
	}

	public static int GetElementIndex(SimHashes element)
	{
		int num = -1;
		List<Element> elements = ElementLoader.elements;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i].id == element)
			{
				num = i;
				break;
			}
		}
		return num;
	}

	public unsafe static void ConsumeMass(int gameCell, SimHashes element, float mass, byte radius, int callbackIdx = -1)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		int elementIndex = ElementLoader.GetElementIndex(element);
		SimMessages.MassConsumptionMessage* ptr = stackalloc SimMessages.MassConsumptionMessage[checked(1 * sizeof(SimMessages.MassConsumptionMessage))];
		ptr->cellIdx = gameCell;
		ptr->callbackIdx = callbackIdx;
		ptr->mass = mass;
		ptr->elementIdx = (byte)elementIndex;
		ptr->radius = radius;
		Sim.SIM_HandleMessage(1727657959, sizeof(SimMessages.MassConsumptionMessage), (byte*)ptr);
	}

	public static void AddRemoveSubstance(int gameCell, SimHashes new_element, CellAddRemoveSubstanceEvent ev, float mass, float temperature, int callbackIdx = -1)
	{
		int elementIndex = SimMessages.GetElementIndex(new_element);
		SimMessages.AddRemoveSubstance(gameCell, elementIndex, ev, mass, temperature, callbackIdx);
	}

	public static void AddRemoveSubstance(int gameCell, int elementIdx, CellAddRemoveSubstanceEvent ev, float mass, float temperature, int callbackIdx = -1)
	{
		if (elementIdx != -1)
		{
			Element element = ElementLoader.elements[elementIdx];
			float num = ((temperature == -1f) ? element.defaultValues.temperature : temperature);
			SimMessages.ModifyCell(gameCell, elementIdx, num, mass, false, callbackIdx);
			ev.Log(gameCell, ElementLoader.elements[elementIdx].id, mass, callbackIdx);
		}
	}

	public static void ReplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, int callbackIdx = -1)
	{
		int elementIndex = SimMessages.GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float num = ((temperature == -1f) ? element.defaultValues.temperature : temperature);
			SimMessages.ModifyCell(gameCell, elementIndex, num, mass, true, callbackIdx);
			ev.Log(gameCell, new_element, callbackIdx);
		}
	}

	public unsafe static void BuildingHeatExchange(bool add, int building_id, Extents extents, float mass, float temperature, float min_temperature, byte element_idx, byte pipe_layer, int callbackIdx = -1)
	{
		int num = Grid.XYToCell(extents.x, extents.y);
		Debug.Assert(Grid.IsValidCell(num));
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		int num2 = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
		Debug.Assert(Grid.IsValidCell(num2));
		if (!Grid.IsValidCell(num2))
		{
			return;
		}
		Sim.BuildingHeatExchangeMessage* ptr = stackalloc Sim.BuildingHeatExchangeMessage[checked(1 * sizeof(Sim.BuildingHeatExchangeMessage))];
		ptr->add = ((!add) ? 0 : 1);
		ptr->buildingID = building_id;
		ptr->callbackIdx = callbackIdx;
		ptr->mass = mass;
		ptr->temperature = temperature;
		ptr->minTemperature = min_temperature;
		ptr->elemIdx = element_idx;
		ptr->pipeLayer = pipe_layer;
		ptr->minX = extents.x;
		ptr->minY = extents.y;
		ptr->maxX = extents.x + extents.width;
		ptr->maxY = extents.y + extents.height;
		SimUtil.CheckValidValue(temperature);
		SimUtil.CheckValidValue(min_temperature);
		Sim.SIM_HandleMessage(-514856633, sizeof(Sim.BuildingHeatExchangeMessage), (byte*)ptr);
	}

	public unsafe static void ModifyEnergy(int gameCell, float kilojoules, SimMessages.EnergySourceID id)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimUtil.CheckValidValue(kilojoules);
		SimMessages.ModifyCellEnergyMessage* ptr = stackalloc SimMessages.ModifyCellEnergyMessage[checked(1 * sizeof(SimMessages.ModifyCellEnergyMessage))];
		ptr->cellIdx = gameCell;
		ptr->kilojoules = kilojoules;
		ptr->id = (int)id;
		Sim.SIM_HandleMessage(818320644, sizeof(SimMessages.ModifyCellEnergyMessage), (byte*)ptr);
	}

	public static void ModifyMass(int gameCell, float mass, CellModifyMassEvent ev, float temperature = -1f, SimHashes element = SimHashes.Vacuum)
	{
		if (element != SimHashes.Vacuum)
		{
			int elementIndex = SimMessages.GetElementIndex(element);
			if (elementIndex != -1)
			{
				if (temperature == -1f)
				{
					temperature = ElementLoader.elements[elementIndex].defaultValues.temperature;
				}
				SimMessages.ModifyCell(gameCell, elementIndex, temperature, mass, false, -1);
			}
		}
		else
		{
			Debug.Assert(mass < 0f);
			SimMessages.ModifyCell(gameCell, 0, temperature, mass, false, -1);
		}
		ev.Log(gameCell, element, mass);
	}

	public unsafe static void CreateElementInteractions(SimMessages.ElementInteraction[] interactions)
	{
		fixed (SimMessages.ElementInteraction* ptr = (ref interactions != null && interactions.Length != 0 ? ref interactions[0] : ref *null))
		{
			SimMessages.CreateElementInteractionsMsg* ptr2 = stackalloc SimMessages.CreateElementInteractionsMsg[checked(1 * sizeof(SimMessages.CreateElementInteractionsMsg))];
			ptr2->numInteractions = interactions.Length;
			ptr2->interactions = ptr;
			Sim.SIM_HandleMessage(-930289787, sizeof(SimMessages.CreateElementInteractionsMsg), (byte*)ptr2);
		}
	}

	public unsafe static void NewGameFrame(Vector2I min, Vector2I max)
	{
		min = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, (min.x / 32 - 1) * 32), MathUtil.Clamp(0, Grid.HeightInCells - 1, (min.y / 32 - 1) * 32));
		max = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, ((max.x + 31) / 32 + 1) * 32), MathUtil.Clamp(0, Grid.HeightInCells - 1, ((max.y + 31) / 32 + 1) * 32));
		Sim.ActiveRegion* ptr = stackalloc Sim.ActiveRegion[checked(1 * sizeof(Sim.ActiveRegion))];
		ptr->minX = min.x;
		ptr->minY = min.y;
		ptr->maxX = max.x;
		ptr->maxY = max.y;
		Sim.SIM_HandleMessage(-775326397, sizeof(Sim.ActiveRegion), (byte*)ptr);
	}

	public unsafe static void SubmitPipeChanges(SimMessages.PipeChange[] pipe_changes, int num_changes)
	{
		fixed (SimMessages.PipeChange* ptr = (ref pipe_changes != null && pipe_changes.Length != 0 ? ref pipe_changes[0] : ref *null))
		{
			Sim.SIM_HandleMessage(84465086, num_changes, (byte*)ptr);
		}
	}

	public unsafe static void SetDebugProperties(Sim.DebugProperties properties)
	{
		SimUtil.CheckValidValue(properties.buildingTemperatureScale);
		Sim.DebugProperties* ptr = stackalloc Sim.DebugProperties[checked(1 * sizeof(Sim.DebugProperties))];
		*ptr = properties;
		ptr->buildingTemperatureScale = properties.buildingTemperatureScale;
		Sim.SIM_HandleMessage(-1683118492, sizeof(Sim.DebugProperties), (byte*)ptr);
	}

	public const int InvalidCallback = -1;

	public const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementConsumerMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float consumptionRate;

		public byte radius;

		public byte configuration;

		public byte elementIdx;

		private byte pad0;

		private byte pad1;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementConsumerMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementEmitterMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float emitInterval;

		public float emitMass;

		public float emitTemperature;

		public byte elementIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementEmitterMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementChunkMessage
	{
		public int gameCell;

		public int callbackIdx;

		public float mass;

		public float temperature;

		public byte elementIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementChunkMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetElementChunkDataMessage
	{
		public int handle;

		public float temperature;

		public float heatCapacity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MoveElementChunkMessage
	{
		public int handle;

		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct DigMessage
	{
		public int cellIdx;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetCellFloatValueMessage
	{
		public int cellIdx;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellPropertiesMessage
	{
		public int cellIdx;

		public byte properties;

		public byte set;

		public byte pad0;

		public byte pad1;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetInsulationValueMessage
	{
		public int cellIdx;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyCellMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float temperature;

		public float mass;

		public byte elementIdx;

		public bool replace;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MassConsumptionMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float mass;

		public byte elementIdx;

		public byte radius;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyCellEnergyMessage
	{
		public int cellIdx;

		public float kilojoules;

		public int id;
	}

	public enum EnergySourceID
	{
		DebugHeat = 1000,
		DebugCool,
		FierySkin,
		Overheatable,
		LiquidCooledFan,
		ConduitTemperatureManager,
		Excavator,
		HeatBulb,
		WarmBlooded,
		StructureTemperature
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct VisibleCells
	{
		public Vector2I min;

		public Vector2I max;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct WakeCellMessage
	{
		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ElementInteraction
	{
		public uint interactionType;

		public byte elemIdx1;

		public byte elemIdx2;

		public byte elemResultIdx;

		public byte pad;

		public float minMass;

		public float interactionProbability;

		public float elem1MassDestructionPercent;

		public float elem2MassRequiredMultiplier;

		public float elemResultMassCreationMultiplier;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CreateElementInteractionsMsg
	{
		public int numInteractions;

		public unsafe SimMessages.ElementInteraction* interactions;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PipeChange
	{
		public int cell;

		public byte layer;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public int elementHash;
	}
}
