using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Database;
using Klei;
using Klei.AI;
using UnityEngine;

public static class SimMessages
{
	public unsafe static void AddElementConsumer(int gameCell, ElementConsumer.Configuration configuration, SimHashes element, byte radius, int cb_handle)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		int elementIndex = ElementLoader.GetElementIndex(element);
		SimMessages.AddElementConsumerMessage* ptr = stackalloc SimMessages.AddElementConsumerMessage[checked(1 * sizeof(SimMessages.AddElementConsumerMessage))];
		ptr->cellIdx = gameCell;
		ptr->configuration = (byte)configuration;
		ptr->elementIdx = (byte)elementIndex;
		ptr->radius = radius;
		ptr->callbackIdx = cb_handle;
		Sim.SIM_HandleMessage(2024405073, sizeof(SimMessages.AddElementConsumerMessage), (byte*)ptr);
	}

	public unsafe static void SetElementConsumerData(int sim_handle, float consumptionRate)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			return;
		}
		SimMessages.SetElementConsumerDataMessage* ptr = stackalloc SimMessages.SetElementConsumerDataMessage[checked(1 * sizeof(SimMessages.SetElementConsumerDataMessage))];
		ptr->handle = sim_handle;
		ptr->consumptionRate = consumptionRate;
		Sim.SIM_HandleMessage(1575539738, sizeof(SimMessages.SetElementConsumerDataMessage), (byte*)ptr);
	}

	public unsafe static void RemoveElementConsumer(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			return;
		}
		SimMessages.RemoveElementConsumerMessage* ptr = stackalloc SimMessages.RemoveElementConsumerMessage[checked(1 * sizeof(SimMessages.RemoveElementConsumerMessage))];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(894417742, sizeof(SimMessages.RemoveElementConsumerMessage), (byte*)ptr);
	}

	public unsafe static void AddElementEmitter(float max_pressure, int on_registered, int on_blocked = -1, int on_unblocked = -1)
	{
		SimMessages.AddElementEmitterMessage* ptr = stackalloc SimMessages.AddElementEmitterMessage[checked(1 * sizeof(SimMessages.AddElementEmitterMessage))];
		ptr->maxPressure = max_pressure;
		ptr->callbackIdx = on_registered;
		ptr->onBlockedCB = on_blocked;
		ptr->onUnblockedCB = on_unblocked;
		Sim.SIM_HandleMessage(-505471181, sizeof(SimMessages.AddElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyElementEmitter(int sim_handle, int game_cell, int max_depth, SimHashes element, float emit_interval, float emit_mass, float emit_temperature)
	{
		if (!Grid.IsValidCell(game_cell))
		{
			return;
		}
		int elementIndex = ElementLoader.GetElementIndex(element);
		SimMessages.ModifyElementEmitterMessage* ptr = stackalloc SimMessages.ModifyElementEmitterMessage[checked(1 * sizeof(SimMessages.ModifyElementEmitterMessage))];
		ptr->handle = sim_handle;
		ptr->cellIdx = game_cell;
		ptr->emitInterval = emit_interval;
		ptr->emitMass = emit_mass;
		ptr->emitTemperature = emit_temperature;
		ptr->elementIdx = (byte)elementIndex;
		ptr->maxDepth = (byte)max_depth;
		Sim.SIM_HandleMessage(403589164, sizeof(SimMessages.ModifyElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void RemoveElementEmitter(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			return;
		}
		SimMessages.RemoveElementEmitterMessage* ptr = stackalloc SimMessages.RemoveElementEmitterMessage[checked(1 * sizeof(SimMessages.RemoveElementEmitterMessage))];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(-1524118282, sizeof(SimMessages.RemoveElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void AddElementChunk(int gameCell, SimHashes element, float mass, float temperature, float surface_area, float thickness, int cb_handle)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		global::UnityEngine.Debug.Assert(mass * temperature > 0f, "Tried to add an SimTemperatureTransfer component with 0 mass or temperature. Unsupported! Your game is now in a bad state.");
		if (mass * temperature > 0f)
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			SimMessages.AddElementChunkMessage* ptr = stackalloc SimMessages.AddElementChunkMessage[checked(1 * sizeof(SimMessages.AddElementChunkMessage))];
			ptr->gameCell = gameCell;
			ptr->callbackIdx = cb_handle;
			ptr->mass = mass;
			ptr->temperature = temperature;
			ptr->surfaceArea = surface_area;
			ptr->thickness = thickness;
			ptr->elementIdx = (byte)elementIndex;
			Sim.SIM_HandleMessage(1445724082, sizeof(SimMessages.AddElementChunkMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementChunk(int sim_handle, int cb_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
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
		SimMessages.SetElementChunkDataMessage* ptr = stackalloc SimMessages.SetElementChunkDataMessage[checked(1 * sizeof(SimMessages.SetElementChunkDataMessage))];
		ptr->handle = sim_handle;
		ptr->temperature = temperature;
		ptr->heatCapacity = heat_capacity;
		Sim.SIM_HandleMessage(-435115907, sizeof(SimMessages.SetElementChunkDataMessage), (byte*)ptr);
	}

	public unsafe static void MoveElementChunk(int sim_handle, int cell)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			return;
		}
		SimMessages.MoveElementChunkMessage* ptr = stackalloc SimMessages.MoveElementChunkMessage[checked(1 * sizeof(SimMessages.MoveElementChunkMessage))];
		ptr->handle = sim_handle;
		ptr->gameCell = cell;
		Sim.SIM_HandleMessage(-374911358, sizeof(SimMessages.MoveElementChunkMessage), (byte*)ptr);
	}

	public unsafe static void ModifyElementChunkEnergy(int sim_handle, float delta_kj)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			return;
		}
		SimMessages.ModifyElementChunkEnergyMessage* ptr = stackalloc SimMessages.ModifyElementChunkEnergyMessage[checked(1 * sizeof(SimMessages.ModifyElementChunkEnergyMessage))];
		ptr->handle = sim_handle;
		ptr->deltaKJ = delta_kj;
		Sim.SIM_HandleMessage(1020555667, sizeof(SimMessages.ModifyElementChunkEnergyMessage), (byte*)ptr);
	}

	public unsafe static void AddBuildingHeatExchange(Extents extents, float temperature, float operating_kw, byte element_idx, float mass, int callbackIdx = -1)
	{
		int num = Grid.XYToCell(extents.x, extents.y);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		int num2 = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
		if (!Grid.IsValidCell(num2))
		{
			return;
		}
		SimMessages.AddBuildingHeatExchangeMessage* ptr = stackalloc SimMessages.AddBuildingHeatExchangeMessage[checked(1 * sizeof(SimMessages.AddBuildingHeatExchangeMessage))];
		ptr->callbackIdx = callbackIdx;
		ptr->elemIdx = element_idx;
		ptr->temperature = temperature;
		ptr->overheatTemperature = float.MaxValue;
		ptr->operatingKilowatts = operating_kw;
		ptr->mass = mass;
		ptr->minX = extents.x;
		ptr->minY = extents.y;
		ptr->maxX = extents.x + extents.width;
		ptr->maxY = extents.y + extents.height;
		Sim.SIM_HandleMessage(1739021608, sizeof(SimMessages.AddBuildingHeatExchangeMessage), (byte*)ptr);
	}

	public unsafe static void ModifyBuildingHeatExchange(int sim_handle, Extents extents, float temperature, float overheat_temperature, float operating_kw, byte element_idx, float mass)
	{
		int num = Grid.XYToCell(extents.x, extents.y);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		int num2 = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
		if (!Grid.IsValidCell(num2))
		{
			return;
		}
		SimMessages.ModifyBuildingHeatExchangeMessage* ptr = stackalloc SimMessages.ModifyBuildingHeatExchangeMessage[checked(1 * sizeof(SimMessages.ModifyBuildingHeatExchangeMessage))];
		ptr->handle = sim_handle;
		ptr->elemIdx = element_idx;
		ptr->temperature = temperature;
		ptr->overheatTemperature = overheat_temperature;
		ptr->operatingKilowatts = operating_kw;
		ptr->mass = mass;
		ptr->minX = extents.x;
		ptr->minY = extents.y;
		ptr->maxX = extents.x + extents.width;
		ptr->maxY = extents.y + extents.height;
		SimUtil.CheckValidValue(temperature);
		Sim.SIM_HandleMessage(1818001569, sizeof(SimMessages.ModifyBuildingHeatExchangeMessage), (byte*)ptr);
	}

	public unsafe static void RemoveBuildingHeatExchange(int sim_handle, int callbackIdx = -1)
	{
		SimMessages.RemoveBuildingHeatExchangeMessage* ptr = stackalloc SimMessages.RemoveBuildingHeatExchangeMessage[checked(1 * sizeof(SimMessages.RemoveBuildingHeatExchangeMessage))];
		ptr->handle = sim_handle;
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(-456116629, sizeof(SimMessages.RemoveBuildingHeatExchangeMessage), (byte*)ptr);
	}

	public unsafe static void ModifyBuildingEnergy(int sim_handle, float delta_kj)
	{
		SimMessages.ModifyBuildingEnergyMessage* ptr = stackalloc SimMessages.ModifyBuildingEnergyMessage[checked(1 * sizeof(SimMessages.ModifyBuildingEnergyMessage))];
		ptr->handle = sim_handle;
		ptr->deltaKJ = delta_kj;
		Sim.SIM_HandleMessage(-1348791658, sizeof(SimMessages.ModifyBuildingEnergyMessage), (byte*)ptr);
	}

	public unsafe static void AddDiseaseEmitter(int callbackIdx)
	{
		SimMessages.AddDiseaseEmitterMessage* ptr = stackalloc SimMessages.AddDiseaseEmitterMessage[checked(1 * sizeof(SimMessages.AddDiseaseEmitterMessage))];
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(1486783027, sizeof(SimMessages.AddDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyDiseaseEmitter(int sim_handle, int cell, byte range, byte disease_idx, float emit_interval, int emit_count)
	{
		SimMessages.ModifyDiseaseEmitterMessage* ptr = stackalloc SimMessages.ModifyDiseaseEmitterMessage[checked(1 * sizeof(SimMessages.ModifyDiseaseEmitterMessage))];
		ptr->handle = sim_handle;
		ptr->gameCell = cell;
		ptr->maxDepth = range;
		ptr->diseaseIdx = disease_idx;
		ptr->emitInterval = emit_interval;
		ptr->emitCount = emit_count;
		Sim.SIM_HandleMessage(-1899123924, sizeof(SimMessages.ModifyDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void RemoveDiseaseEmitter(int cb_handle, int sim_handle)
	{
		SimMessages.RemoveDiseaseEmitterMessage* ptr = stackalloc SimMessages.RemoveDiseaseEmitterMessage[checked(1 * sizeof(SimMessages.RemoveDiseaseEmitterMessage))];
		ptr->handle = sim_handle;
		ptr->callbackIdx = cb_handle;
		Sim.SIM_HandleMessage(468135926, sizeof(SimMessages.RemoveDiseaseEmitterMessage), (byte*)ptr);
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
		for (int j = 0; j < elements.Count; j++)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(elements[j].name);
			binaryWriter.Write(bytes.Length);
			binaryWriter.Write(bytes);
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* ptr = (ref buffer != null && buffer.Length != 0 ? ref buffer[0] : ref *null))
		{
			Sim.SIM_HandleMessage(1108437482, buffer.Length, ptr);
		}
	}

	public unsafe static void CreateWorldGenHACKDiseaseTable(List<string> diseaseIds)
	{
		MemoryStream memoryStream = new MemoryStream(1024);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(diseaseIds.Count);
		List<Element> elements = ElementLoader.elements;
		binaryWriter.Write(elements.Count);
		Disease.RangeInfo rangeInfo;
		rangeInfo.maxGrowth = 350f;
		rangeInfo.minGrowth = 250f;
		rangeInfo.minViable = 200f;
		rangeInfo.maxViable = 400f;
		Disease.RangeInfo rangeInfo2;
		rangeInfo2.maxGrowth = float.PositiveInfinity;
		rangeInfo2.minGrowth = float.PositiveInfinity;
		rangeInfo2.minViable = float.PositiveInfinity;
		rangeInfo2.maxViable = float.PositiveInfinity;
		for (int i = 0; i < diseaseIds.Count; i++)
		{
			BinaryWriter binaryWriter2 = binaryWriter;
			HashedString hashedString = new HashedString(diseaseIds[i]);
			binaryWriter2.Write(hashedString.GetHashCode());
			binaryWriter.Write(0f);
			rangeInfo.Write(binaryWriter);
			rangeInfo2.Write(binaryWriter);
			rangeInfo.Write(binaryWriter);
			rangeInfo2.Write(binaryWriter);
			for (int j = 0; j < elements.Count; j++)
			{
				Disease.DEFAULT_GROWTH_INFO.Write(binaryWriter);
			}
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* ptr = (ref buffer != null && buffer.Length != 0 ? ref buffer[0] : ref *null))
		{
			Sim.SIM_HandleMessage(825301935, (int)memoryStream.Length, ptr);
		}
	}

	public unsafe static void CreateDiseaseTable()
	{
		global::Database.Diseases diseases = Db.Get().Diseases;
		MemoryStream memoryStream = new MemoryStream(1024);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(diseases.Count);
		List<Element> elements = ElementLoader.elements;
		binaryWriter.Write(elements.Count);
		for (int i = 0; i < diseases.Count; i++)
		{
			Disease disease = diseases[i];
			binaryWriter.Write(disease.id.GetHashCode());
			binaryWriter.Write(disease.strength);
			disease.temperatureRange.Write(binaryWriter);
			disease.temperatureHalfLives.Write(binaryWriter);
			disease.pressureRange.Write(binaryWriter);
			disease.pressureHalfLives.Write(binaryWriter);
			for (int j = 0; j < elements.Count; j++)
			{
				Disease.ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[j];
				elemGrowthInfo.Write(binaryWriter);
			}
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* ptr = (ref buffer != null && buffer.Length != 0 ? ref buffer[0] : ref *null))
		{
			Sim.SIM_HandleMessage(825301935, (int)memoryStream.Length, ptr);
		}
	}

	public static void SimDataInitializeFromCells(int width, int height, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Cell)) * width * height + Marshal.SizeOf(typeof(float)) * width * height + Marshal.SizeOf(typeof(Sim.DiseaseCell)) * width * height);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(width);
		binaryWriter.Write(height);
		int num = width * height;
		for (int i = 0; i < num; i++)
		{
			cells[i].Write(binaryWriter);
		}
		for (int j = 0; j < num; j++)
		{
			binaryWriter.Write(bgTemp[j]);
		}
		for (int k = 0; k < num; k++)
		{
			dc[k].Write(binaryWriter);
		}
		byte[] buffer = memoryStream.GetBuffer();
		Sim.HandleMessage(SimMessageHashes.SimData_InitializeFromCells, buffer.Length, buffer);
	}

	public unsafe static void Dig(int gameCell, int callbackIdx = -1)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			return;
		}
		SimMessages.DigMessage* ptr = stackalloc SimMessages.DigMessage[checked(1 * sizeof(SimMessages.DigMessage))];
		ptr->cellIdx = gameCell;
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(833038498, sizeof(SimMessages.DigMessage), (byte*)ptr);
	}

	public unsafe static void SetInsulation(int gameCell, float value)
	{
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

	public unsafe static void ModifyCell(int gameCell, int elementIdx, float temperature, float mass, byte disease_idx, int disease_count, SimMessages.ReplaceType replace_type = SimMessages.ReplaceType.None, int callbackIdx = -1)
	{
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
		ptr->replaceType = (byte)replace_type;
		ptr->diseaseIdx = disease_idx;
		ptr->diseaseCount = disease_count;
		SimUtil.CheckValidValue(temperature);
		SimUtil.CheckValidValue(mass);
		Sim.SIM_HandleMessage(-1252920804, sizeof(SimMessages.ModifyCellMessage), (byte*)ptr);
	}

	public unsafe static void ModifyDiseaseOnCell(int gameCell, byte disease_idx, int disease_count)
	{
		SimMessages.CellDiseaseModification* ptr = stackalloc SimMessages.CellDiseaseModification[checked(1 * sizeof(SimMessages.CellDiseaseModification))];
		ptr->cellIdx = gameCell;
		ptr->diseaseIdx = disease_idx;
		ptr->diseaseCount = disease_count;
		Sim.SIM_HandleMessage(-1853671274, sizeof(SimMessages.CellDiseaseModification), (byte*)ptr);
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

	public unsafe static void ConsumeDisease(int game_cell, float percent_to_consume, int max_to_consume, int callback_idx)
	{
		if (!Grid.IsValidCell(game_cell))
		{
			return;
		}
		SimMessages.ConsumeDiseaseMessage* ptr = stackalloc SimMessages.ConsumeDiseaseMessage[checked(1 * sizeof(SimMessages.ConsumeDiseaseMessage))];
		ptr->callbackIdx = callback_idx;
		ptr->gameCell = game_cell;
		ptr->percentToConsume = percent_to_consume;
		ptr->maxToConsume = max_to_consume;
		Sim.SIM_HandleMessage(-1019841536, sizeof(SimMessages.ConsumeDiseaseMessage), (byte*)ptr);
	}

	public static void AddRemoveSubstance(int gameCell, SimHashes new_element, CellAddRemoveSubstanceEvent ev, float mass, float temperature, byte disease_idx, int disease_count, int callbackIdx = -1)
	{
		int elementIndex = SimMessages.GetElementIndex(new_element);
		SimMessages.AddRemoveSubstance(gameCell, elementIndex, ev, mass, temperature, disease_idx, disease_count, callbackIdx);
	}

	public static void AddRemoveSubstance(int gameCell, int elementIdx, CellAddRemoveSubstanceEvent ev, float mass, float temperature, byte disease_idx, int disease_count, int callbackIdx = -1)
	{
		if (elementIdx != -1)
		{
			Element element = ElementLoader.elements[elementIdx];
			float num = ((temperature == -1f) ? element.defaultValues.temperature : temperature);
			SimMessages.ModifyCell(gameCell, elementIdx, num, mass, disease_idx, disease_count, SimMessages.ReplaceType.None, callbackIdx);
			ev.Log(gameCell, ElementLoader.elements[elementIdx].id, mass, callbackIdx);
		}
	}

	public static void ReplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, byte diseaseIdx = 255, int diseaseCount = 0, int callbackIdx = -1)
	{
		int elementIndex = SimMessages.GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float num = ((temperature == -1f) ? element.defaultValues.temperature : temperature);
			SimMessages.ModifyCell(gameCell, elementIndex, num, mass, diseaseIdx, diseaseCount, SimMessages.ReplaceType.Replace, callbackIdx);
		}
	}

	public static void ReplaceAndDisplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, byte disease_idx = 255, int disease_count = 0, int callbackIdx = -1)
	{
		int elementIndex = SimMessages.GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float num = ((temperature == -1f) ? element.defaultValues.temperature : temperature);
			SimMessages.ModifyCell(gameCell, elementIndex, num, mass, disease_idx, disease_count, SimMessages.ReplaceType.ReplaceAndDisplace, callbackIdx);
		}
	}

	public unsafe static void ModifyEnergy(int gameCell, float kilojoules, SimMessages.EnergySourceID id)
	{
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

	public static void ModifyMass(int gameCell, float mass, byte disease_idx, int disease_count, CellModifyMassEvent ev, float temperature = -1f, SimHashes element = SimHashes.Vacuum)
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
				SimMessages.ModifyCell(gameCell, elementIndex, temperature, mass, disease_idx, disease_count, SimMessages.ReplaceType.None, -1);
			}
		}
		else
		{
			SimMessages.ModifyCell(gameCell, 0, temperature, mass, disease_idx, disease_count, SimMessages.ReplaceType.None, -1);
		}
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

	public unsafe static void NewGameFrame(float elapsed_seconds, Vector2I min, Vector2I max)
	{
		min = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, (min.x / 32 - 1) * 32), MathUtil.Clamp(0, Grid.HeightInCells - 1, (min.y / 32 - 1) * 32));
		max = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, ((max.x + 31) / 32 + 1) * 32), MathUtil.Clamp(0, Grid.HeightInCells - 1, ((max.y + 31) / 32 + 1) * 32));
		Sim.NewGameFrame* ptr = stackalloc Sim.NewGameFrame[checked(1 * sizeof(Sim.NewGameFrame))];
		ptr->elapsedSeconds = elapsed_seconds;
		ptr->minX = min.x;
		ptr->minY = min.y;
		ptr->maxX = max.x;
		ptr->maxY = max.y;
		Sim.SIM_HandleMessage(-775326397, sizeof(Sim.NewGameFrame), (byte*)ptr);
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

		public byte radius;

		public byte configuration;

		public byte elementIdx;

		private byte pad0;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetElementConsumerDataMessage
	{
		public int handle;

		public float consumptionRate;
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
		public float maxPressure;

		public int callbackIdx;

		public int onBlockedCB;

		public int onUnblockedCB;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementEmitterMessage
	{
		public int handle;

		public int cellIdx;

		public float emitInterval;

		public float emitMass;

		public float emitTemperature;

		public byte elementIdx;

		public byte maxDepth;

		private byte pad0;

		private byte pad1;
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

		public float surfaceArea;

		public float thickness;

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
	private struct ModifyElementChunkEnergyMessage
	{
		public int handle;

		public float deltaKJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AddBuildingHeatExchangeMessage
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public float overheatTemperature;

		public float operatingKilowatts;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyBuildingHeatExchangeMessage
	{
		public int handle;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public float overheatTemperature;

		public float operatingKilowatts;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyBuildingEnergyMessage
	{
		public int handle;

		public float deltaKJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RemoveBuildingHeatExchangeMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AddDiseaseEmitterMessage
	{
		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyDiseaseEmitterMessage
	{
		public int handle;

		public int gameCell;

		public byte diseaseIdx;

		public byte maxDepth;

		private byte pad0;

		private byte pad1;

		public float emitInterval;

		public int emitCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RemoveDiseaseEmitterMessage
	{
		public int handle;

		public int callbackIdx;
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

		public int diseaseCount;

		public byte elementIdx;

		public byte replaceType;

		public byte diseaseIdx;

		private byte pad0;
	}

	public enum ReplaceType
	{
		None,
		Replace,
		ReplaceAndDisplace
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellDiseaseModification
	{
		public int cellIdx;

		public byte diseaseIdx;

		public int diseaseCount;
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
	private struct ConsumeDiseaseMessage
	{
		public int gameCell;

		public int callbackIdx;

		public float percentToConsume;

		public int maxToConsume;
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
		StructureTemperature,
		Burner
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
