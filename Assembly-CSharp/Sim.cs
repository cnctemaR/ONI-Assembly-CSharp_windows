using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Sim
{
	public static bool IsValidHandle(int h)
	{
		return h >= 0;
	}

	[DllImport("SimDLL")]
	public static extern void SIM_Initialize(Sim.GAME_MessageHandler callback);

	[DllImport("SimDLL")]
	public static extern void SIM_Shutdown();

	[DllImport("SimDLL")]
	public unsafe static extern IntPtr SIM_HandleMessage(int sim_msg_id, int msg_length, byte* msg);

	[DllImport("SimDLL")]
	private unsafe static extern byte* SIM_BeginSave(int* size);

	[DllImport("SimDLL")]
	private static extern void SIM_EndSave();

	public unsafe static IntPtr HandleMessage(SimMessageHashes sim_msg_id, int msg_length, byte[] msg)
	{
		IntPtr intPtr;
		fixed (byte* ptr = (ref msg != null && msg.Length != 0 ? ref msg[0] : ref *null))
		{
			intPtr = Sim.SIM_HandleMessage((int)sim_msg_id, msg_length, ptr);
		}
		return intPtr;
	}

	public unsafe static void Save(BinaryWriter writer)
	{
		int num;
		byte* ptr = Sim.SIM_BeginSave(&num);
		byte[] array = new byte[num];
		Marshal.Copy((IntPtr)((void*)ptr), array, 0, num);
		Sim.SIM_EndSave();
		writer.Write(num);
		writer.Write(array);
	}

	public unsafe static int Load(FastReader reader)
	{
		int num = reader.ReadInt32();
		byte[] array = reader.ReadBytes(num);
		IntPtr intPtr;
		fixed (byte* ptr = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
		{
			intPtr = Sim.SIM_HandleMessage(-672538170, num, ptr);
		}
		if (intPtr == IntPtr.Zero)
		{
			return -1;
		}
		Sim.GameDataUpdate* ptr2 = (Sim.GameDataUpdate*)(void*)intPtr;
		Grid.CellValues = ptr2->cells;
		Grid.PropertyTextureFlowValues = ptr2->propertyTextureFlow;
		Grid.AccumulatedFlowValues = ptr2->accumulatedFlow;
		Grid.InitializeCells(ptr2->cells);
		return 0;
	}

	public static void Shutdown()
	{
		Sim.SIM_Shutdown();
		Grid.CellValues = null;
	}

	[DllImport("SimDLL")]
	public unsafe static extern char* SYSINFO_Acquire();

	[DllImport("SimDLL")]
	public static extern void SYSINFO_Release();

	public const int InvalidHandle = -1;

	public const int QueuedRegisterHandle = -2;

	public const int ChunkEdgeSize = 32;

	public const float StateTransitionEnergy = 3f;

	public const float ZeroDegreesCentigrade = 273.15f;

	public const float StandardTemperature = 293.15f;

	public const float StandardPressure = 101.3f;

	public const float Epsilon = 0.0001f;

	public const float MaxTemperature = 10000f;

	public const float MinTemperature = 0f;

	public const float MaxMass = 10000f;

	public const float MinMass = 1.0001f;

	private const int PressureUpdateInterval = 1;

	private const int TemperatureUpdateInterval = 1;

	private const int LiquidUpdateInterval = 1;

	private const int LifeUpdateInterval = 1;

	public const int PACKING_ALIGNMENT = 4;

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PhysicsData
	{
		public void Write(BinaryWriter writer)
		{
			writer.Write(this.temperature);
			writer.Write(this.mass);
			writer.Write(this.pressure);
		}

		public float temperature;

		public float mass;

		public float pressure;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Cell
	{
		public void Write(BinaryWriter writer, List<global::Element> elements)
		{
			writer.Write(this.elementIdx);
			writer.Write(0);
			writer.Write(this.insulation);
			writer.Write(0);
			writer.Write(this.temperature);
			writer.Write(this.mass);
		}

		public void SetValues(global::Element elem, List<global::Element> elements)
		{
			this.SetValues(elem, elem.defaultValues, elements);
		}

		public void SetValues(global::Element elem, Sim.PhysicsData pd, List<global::Element> elements)
		{
			this.elementIdx = (byte)elements.IndexOf(elem);
			this.temperature = pd.temperature;
			this.mass = pd.mass;
			this.insulation = byte.MaxValue;
		}

		public byte elementIdx;

		public byte properties;

		public byte insulation;

		public byte strengthInfo;

		public float temperature;

		public float mass;

		public enum Properties
		{
			GasImpermeable = 1,
			LiquidImpermeable,
			SolidImpermeable = 4
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Element
	{
		public Element(global::Element e, List<global::Element> elements)
		{
			this.id = e.id;
			this.state = (byte)e.state;
			if (e.HasTag(GameTags.Unstable))
			{
				this.state |= 8;
			}
			this.lowTempTransitionIdx = (sbyte)elements.IndexOf(e.lowTempTransition);
			this.highTempTransitionIdx = (sbyte)elements.IndexOf(e.highTempTransition);
			this.elementsTableIdx = (byte)elements.IndexOf(e);
			this.specificHeatCapacity = e.specificHeatCapacity;
			this.thermalConductivity = e.thermalConductivity;
			this.molarMass = e.molarMass;
			this.strength = e.strength;
			this.flow = e.flow;
			this.viscosity = e.viscosity;
			this.minHorizontalLiquidFlow = e.minHorizontalLiquidFlow;
			this.minVerticalLiquidFlow = e.minVerticalLiquidFlow;
			this.maxMass = e.maxMass;
			this.lowTemp = e.lowTemp;
			this.highTemp = e.highTemp;
			this.highTempTransitionOreID = e.highTempTransitionOreID;
			this.highTempTransitionOreMassConversion = e.highTempTransitionOreMassConversion;
			this.defaultValues = e.defaultValues;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((int)this.id);
			writer.Write(this.state);
			writer.Write(this.lowTempTransitionIdx);
			writer.Write(this.highTempTransitionIdx);
			writer.Write(this.elementsTableIdx);
			writer.Write(this.specificHeatCapacity);
			writer.Write(this.thermalConductivity);
			writer.Write(this.molarMass);
			writer.Write(this.flow);
			writer.Write(this.viscosity);
			writer.Write(this.minHorizontalLiquidFlow);
			writer.Write(this.minVerticalLiquidFlow);
			writer.Write(this.maxMass);
			writer.Write(this.lowTemp);
			writer.Write(this.highTemp);
			writer.Write(this.strength);
			writer.Write((int)this.highTempTransitionOreID);
			writer.Write(this.highTempTransitionOreMassConversion);
			this.defaultValues.Write(writer);
		}

		public SimHashes id;

		public byte state;

		public sbyte lowTempTransitionIdx;

		public sbyte highTempTransitionIdx;

		public byte elementsTableIdx;

		public float specificHeatCapacity;

		public float thermalConductivity;

		public float molarMass;

		public float flow;

		public float viscosity;

		public float minHorizontalLiquidFlow;

		public float minVerticalLiquidFlow;

		public float maxMass;

		public float lowTemp;

		public float highTemp;

		public float strength;

		public SimHashes highTempTransitionOreID;

		public float highTempTransitionOreMassConversion;

		public Sim.PhysicsData defaultValues;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SolidInfo
	{
		public int cellIdx;

		public int isSolid;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct LiquidChangeInfo
	{
		public int cellIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SolidSubstanceChangeInfo
	{
		public int cellIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SubstanceChangeInfo
	{
		public int cellIdx;

		public byte oldElemIdx;

		public byte newElemIdx;

		private byte pad0;

		private byte pad1;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CallbackInfo
	{
		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GameDataUpdate
	{
		public unsafe Sim.Cell* cells;

		public int numSolidInfo;

		public unsafe Sim.SolidInfo* solidInfo;

		public int numLiquidChangeInfo;

		public unsafe Sim.LiquidChangeInfo* liquidChangeInfo;

		public int numSolidSubstanceChangeInfo;

		public unsafe Sim.SolidSubstanceChangeInfo* solidSubstanceChangeInfo;

		public int numSubstanceChangeInfo;

		public unsafe Sim.SubstanceChangeInfo* substanceChangeInfo;

		public int numCallbackInfo;

		public unsafe Sim.CallbackInfo* callbackInfo;

		public int numSpawnFallingLiquidInfo;

		public unsafe Sim.SpawnFallingLiquidInfo* spawnFallingLiquidInfo;

		public int numSpawnOreInfo;

		public unsafe Sim.SpawnOreInfo* spawnOreInfo;

		public int numSpawnFXInfo;

		public unsafe Sim.SpawnFXInfo* spawnFXInfo;

		public int numUnstableCellInfo;

		public unsafe Sim.UnstableCellInfo* unstableCellInfo;

		public int numWorldDamageInfo;

		public unsafe Sim.WorldDamageInfo* worldDamageInfo;

		public int numBuildingTemperatures;

		public unsafe Sim.BuildingTemperatureInfo* buildingTemperatures;

		public int numGasPipeTemperatureChanges;

		public unsafe Sim.PipeTemperatureChange* gasPipeTemperatureChanges;

		public int numLiquidPipeTemperatureChanges;

		public unsafe Sim.PipeTemperatureChange* liquidPipeTemperatureChanges;

		public int numMassConsumptionCallbacks;

		public unsafe Sim.MassConsumptionCallback* massConsumptionCallbacks;

		public int numComponentStateChangedMessages;

		public unsafe Sim.ComponentStateChangedMessage* componentStateChangedMessages;

		public int numRemovedMassEntries;

		public unsafe Sim.MassChangeInfo* removedMassEntries;

		public int numEmittedMassEntries;

		public unsafe Sim.MassChangeInfo* emittedMassEntries;

		public int numElementChunkInfos;

		public unsafe Sim.ElementChunkInfo* elementChunkInfos;

		public unsafe float* accumulatedFlow;

		public unsafe Vector2* propertyTextureFlow;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnFallingLiquidInfo
	{
		public int cellIdx;

		public float mass;

		public float temperature;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnOreInfo
	{
		public int cellIdx;

		public float mass;

		public float temperature;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnFXInfo
	{
		public int cellIdx;

		public int fxHash;

		public float rotation;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct UnstableCellInfo
	{
		public int cellIdx;

		public byte fallingInfo;

		public byte elemIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public enum FallingInfo
		{
			StartedFalling,
			StoppedFalling
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ActiveRegion
	{
		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct WorldDamageInfo
	{
		public int gameCell;

		public int damageSourceOffset;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BuildingHeatExchangeMessage
	{
		public int buildingID;

		public int callbackIdx;

		public byte add;

		public byte elemIdx;

		public byte pipeLayer;

		private byte pad0;

		public float mass;

		public float temperature;

		public float minTemperature;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BuildingTemperatureInfo
	{
		public int id;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PipeTemperatureChange
	{
		public int cellIdx;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MassConsumptionCallback
	{
		public int callbackIdx;

		public float mass;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ComponentStateChangedMessage
	{
		public int callbackIdx;

		public int simHandle;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DebugProperties
	{
		public float buildingTemperatureScale;

		public float contaminatedOxygenEmitProbability;

		public float contaminatedOxygenConversionPercent;

		public float biomeTemperatureLerpRate;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MassChangeInfo
	{
		public byte removedElemIdx;

		public float mass;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ElementChunkInfo
	{
		public float temperature;
	}

	public delegate int GAME_MessageHandler(int message_id, IntPtr data);

	public delegate void GAME_Callback();
}
