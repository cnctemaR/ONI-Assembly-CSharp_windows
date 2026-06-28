using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using KSerialization;
using STRINGS;
using TemplateClasses;

namespace ProcGenGame
{
	public static class WorldGenSimUtil
	{
		public unsafe static bool DoSettleSim(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn, Data data, List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets)
		{
			Sim.SIM_Initialize(null);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateWorldGenHACKDiseaseTable(WorldGen.diseaseIds);
			Sim.DiseaseCell[] array = new Sim.DiseaseCell[dcs.Length];
			SimMessages.SimDataInitializeFromCells(Grid.WidthInCells, Grid.HeightInCells, cells, bgTemp, array);
			int num = 300;
			updateProgressFn(UI.WORLDGEN.SETTLESIM.key, 0f, WorldGenProgressStages.Stages.SettleSim);
			Vector2I vector2I = new Vector2I(0, 0);
			Vector2I vector2I2 = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			byte[] array2 = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					try
					{
						Sim.Save(binaryWriter);
					}
					catch (Exception ex)
					{
						string message = ex.Message;
						string stackTrace = ex.StackTrace;
						WorldGenLogger.LogException(message, stackTrace);
						return updateProgressFn(new StringKey("Exception in Sim Save"), -1f, WorldGenProgressStages.Stages.Failure);
					}
				}
				array2 = memoryStream.ToArray();
			}
			FastReader fastReader = new FastReader(array2);
			if (Sim.Load(fastReader) != 0)
			{
				updateProgressFn(UI.WORLDGEN.FAILED.key, -1f, WorldGenProgressStages.Stages.Failure);
				return true;
			}
			byte[] array3 = new byte[Grid.CellCount];
			for (int i = 0; i < Grid.CellCount; i++)
			{
				array3[i] = byte.MaxValue;
			}
			for (int j = 0; j < num; j++)
			{
				SimMessages.NewGameFrame(0.25f, vector2I, vector2I2);
				IntPtr intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, array3.Length, array3);
				updateProgressFn(UI.WORLDGEN.SETTLESIM.key, (float)j / (float)num * 100f, WorldGenProgressStages.Stages.SettleSim);
				if (!(intPtr == IntPtr.Zero))
				{
					Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
					for (int k = 0; k < ptr->numSubstanceChangeInfo; k++)
					{
						Sim.SubstanceChangeInfo substanceChangeInfo = ptr->substanceChangeInfo[k];
						int cellIdx = substanceChangeInfo.cellIdx;
						cells[cellIdx].elementIdx = ptr->cells[cellIdx].elementIdx;
						cells[cellIdx].insulation = ptr->cells[cellIdx].insulation;
						cells[cellIdx].properties = ptr->cells[cellIdx].properties;
						cells[cellIdx].temperature = ptr->cells[cellIdx].temperature;
						cells[cellIdx].mass = ptr->cells[cellIdx].mass;
						cells[cellIdx].strengthInfo = ptr->cells[cellIdx].strengthInfo;
					}
					Cell templateCellData;
					foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair in templateSpawnTargets)
					{
						for (int l = 0; l < keyValuePair.Value.cells.Count; l++)
						{
							templateCellData = keyValuePair.Value.cells[l];
							int num2 = Grid.OffsetCell(Grid.XYToCell(keyValuePair.Key.x, keyValuePair.Key.y), templateCellData.location_x, templateCellData.location_y);
							cells[num2].elementIdx = (byte)ElementLoader.GetElementIndex(templateCellData.element);
							cells[num2].temperature = templateCellData.temperature;
							cells[num2].mass = templateCellData.mass;
							dcs[num2].diseaseIdx = (byte)WorldGen.diseaseIds.FindIndex((string name) => name == templateCellData.diseaseName);
							dcs[num2].elementCount = templateCellData.diseaseCount;
						}
					}
					if (j == num - 2)
					{
						for (int m = 0; m < Grid.CellCount; m++)
						{
							SimMessages.ModifyCell(m, (int)cells[m].elementIdx, cells[m].temperature, cells[m].mass, dcs[m].diseaseIdx, dcs[m].elementCount, SimMessages.ReplaceType.Replace, -1);
						}
					}
				}
			}
			WorldGenSimUtil.SaveSim(data);
			Sim.Shutdown();
			return true;
		}

		private static void SaveSim(Data data)
		{
			try
			{
				Manager.Clear();
				SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
				for (int i = 0; i < data.overworldCells.Count; i++)
				{
					simSaveFileStructure.worldDetail.overworldCells.Add(new WorldDetailSave.OverworldCell(data.overworldCells[i]));
				}
				simSaveFileStructure.worldDetail.globalWorldSeed = data.globalWorldSeed;
				simSaveFileStructure.worldDetail.globalWorldLayoutSeed = data.globalWorldLayoutSeed;
				simSaveFileStructure.worldDetail.globalTerrainSeed = data.globalTerrainSeed;
				simSaveFileStructure.worldDetail.globalNoiseSeed = data.globalNoiseSeed;
				simSaveFileStructure.WidthInCells = Grid.WidthInCells;
				simSaveFileStructure.HeightInCells = Grid.HeightInCells;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						Sim.Save(binaryWriter);
					}
					simSaveFileStructure.Sim = memoryStream.ToArray();
				}
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2))
					{
						try
						{
							Serializer.Serialize(simSaveFileStructure, binaryWriter2);
						}
						catch (Exception ex)
						{
							Output.LogError(new object[] { "Couldn't serialize", ex.Message, ex.StackTrace });
						}
					}
					using (BinaryWriter binaryWriter3 = new BinaryWriter(File.Open(WorldGen.SIM_SAVE_FILENAME, FileMode.Create)))
					{
						Manager.SerializeDirectory(binaryWriter3);
						binaryWriter3.Write(memoryStream2.ToArray());
					}
				}
			}
			catch (Exception ex2)
			{
				Output.LogError(new object[] { "Couldn't write", ex2.Message, ex2.StackTrace });
			}
		}
	}
}
