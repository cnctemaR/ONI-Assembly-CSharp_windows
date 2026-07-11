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
		public unsafe static bool DoSettleSim(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn, Data data, List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, Action<OfflineWorldGen.ErrorInfo> error_cb, Action<Sim.Cell[], float[], Sim.DiseaseCell[]> onSettleComplete)
		{
			Sim.SIM_Initialize(null);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateWorldGenHACKDiseaseTable(WorldGen.diseaseIds);
			Sim.DiseaseCell[] array = new Sim.DiseaseCell[dcs.Length];
			SimMessages.SimDataInitializeFromCells(Grid.WidthInCells, Grid.HeightInCells, cells, bgTemp, array);
			int num = 500;
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
				SimMessages.NewGameFrame(0.2f, vector2I, vector2I2);
				IntPtr intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, array3.Length, array3);
				updateProgressFn(UI.WORLDGEN.SETTLESIM.key, (float)j / (float)num * 100f, WorldGenProgressStages.Stages.SettleSim);
				if (!(intPtr == IntPtr.Zero))
				{
					Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
					for (int k = 0; k < ptr->numSubstanceChangeInfo; k++)
					{
						Sim.SubstanceChangeInfo substanceChangeInfo = ptr->substanceChangeInfo[k];
						int cellIdx = substanceChangeInfo.cellIdx;
						cells[cellIdx].elementIdx = ptr->elementIdx[cellIdx];
						cells[cellIdx].insulation = ptr->insulation[cellIdx];
						cells[cellIdx].properties = ptr->properties[cellIdx];
						cells[cellIdx].temperature = ptr->temperature[cellIdx];
						cells[cellIdx].mass = ptr->mass[cellIdx];
						cells[cellIdx].strengthInfo = ptr->strengthInfo[cellIdx];
					}
					Cell templateCellData;
					foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair in templateSpawnTargets)
					{
						for (int l = 0; l < keyValuePair.Value.cells.Count; l++)
						{
							templateCellData = keyValuePair.Value.cells[l];
							int num2 = Grid.OffsetCell(Grid.XYToCell(keyValuePair.Key.x, keyValuePair.Key.y), templateCellData.location_x, templateCellData.location_y);
							if (Grid.IsValidCell(num2))
							{
								cells[num2].elementIdx = (byte)ElementLoader.GetElementIndex(templateCellData.element);
								cells[num2].temperature = templateCellData.temperature;
								cells[num2].mass = templateCellData.mass;
								dcs[num2].diseaseIdx = (byte)WorldGen.diseaseIds.FindIndex((string name) => name == templateCellData.diseaseName);
								dcs[num2].elementCount = templateCellData.diseaseCount;
							}
						}
					}
				}
			}
			for (int m = 0; m < Grid.CellCount; m++)
			{
				int num3 = ((m != Grid.CellCount - 1) ? (-1) : 2147481337);
				SimMessages.ModifyCell(m, (int)cells[m].elementIdx, cells[m].temperature, cells[m].mass, dcs[m].diseaseIdx, dcs[m].elementCount, SimMessages.ReplaceType.Replace, false, num3);
			}
			bool flag = false;
			while (!flag)
			{
				SimMessages.NewGameFrame(0.2f, vector2I, vector2I2);
				IntPtr intPtr2 = Sim.HandleMessage(SimMessageHashes.PrepareGameData, array3.Length, array3);
				if (!(intPtr2 == IntPtr.Zero))
				{
					Sim.GameDataUpdate* ptr2 = (Sim.GameDataUpdate*)(void*)intPtr2;
					for (int n = 0; n < ptr2->numCallbackInfo; n++)
					{
						Sim.CallbackInfo callbackInfo = ptr2->callbackInfo[n];
						if (callbackInfo.callbackIdx == 2147481337)
						{
							flag = true;
							break;
						}
					}
				}
			}
			Sim.HandleMessage(SimMessageHashes.SettleWorldGen, 0, null);
			bool flag2 = WorldGenSimUtil.SaveSim(data, error_cb);
			onSettleComplete(cells, bgTemp, dcs);
			Sim.Shutdown();
			return flag2;
		}

		private static bool SaveSim(Data data, Action<OfflineWorldGen.ErrorInfo> error_cb)
		{
			bool flag;
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
				flag = true;
			}
			catch (Exception ex2)
			{
				error_cb(new OfflineWorldGen.ErrorInfo
				{
					errorDesc = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, WorldGen.SIM_SAVE_FILENAME),
					exception = ex2
				});
				Output.LogError(new object[] { "Couldn't write", ex2.Message, ex2.StackTrace });
				flag = false;
			}
			return flag;
		}
	}
}
