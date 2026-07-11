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

	[DllImport("SimDLL")]
	public static extern void SIM_DebugCrash();

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
		Grid.elementIdx = ptr2->elementIdx;
		Grid.temperature = ptr2->temperature;
		Grid.mass = ptr2->mass;
		Grid.properties = ptr2->properties;
		Grid.strengthInfo = ptr2->strengthInfo;
		Grid.insulation = ptr2->insulation;
		Grid.diseaseIdx = ptr2->diseaseIdx;
		Grid.diseaseCount = ptr2->diseaseCount;
		Grid.AccumulatedFlowValues = ptr2->accumulatedFlow;
		PropertyTextures.externalFlowTex = ptr2->propertyTextureFlow;
		PropertyTextures.externalLiquidTex = ptr2->propertyTextureLiquid;
		PropertyTextures.externalExposedToSunlight = ptr2->propertyTextureExposedToSunlight;
		Grid.InitializeCells();
		return 0;
	}

	public static void Shutdown()
	{
		Sim.SIM_Shutdown();
		Grid.mass = null;
	}

	[DllImport("SimDLL")]
	public unsafe static extern char* SYSINFO_Acquire();

	[DllImport("SimDLL")]
	public static extern void SYSINFO_Release();

	public unsafe static int DLL_MessageHandler(int message_id, IntPtr data)
	{
		if (message_id == 1)
		{
			string text = Marshal.PtrToStringAnsi(data);
			text = "SimDLL: " + text;
			KCrashReporter.ReportDLLCrash(text, text, null);
			return 0;
		}
		if (message_id != 0)
		{
			return -1;
		}
		Sim.DLLCrash* ptr = (Sim.DLLCrash*)(void*)data;
		string text2 = Marshal.PtrToStringAnsi(ptr->callstack);
		string text3 = Marshal.PtrToStringAnsi(ptr->dmpFilename);
		KCrashReporter.ReportDLLCrash(text2, text2, text3);
		return 0;
	}

	public const int InvalidHandle = -1;

	public const int QueuedRegisterHandle = -2;

	public const byte InvalidDiseaseIdx = 255;

	public const byte InvalidElementIdx = 255;

	public const byte SpaceZoneID = 255;

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

	public delegate int GAME_MessageHandler(int message_id, IntPtr data);

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DLLCrash
	{
		public IntPtr callstack;

		public IntPtr dmpFilename;
	}

	private enum GameHandledMessages
	{
		ExceptionHandler,
		ReportMessage
	}

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
		public void Write(BinaryWriter writer)
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

		public void SetValues(byte new_elem_idx, float new_temperature, float new_mass)
		{
			this.elementIdx = new_elem_idx;
			this.temperature = new_temperature;
			this.mass = new_mass;
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
			SolidImpermeable = 4,
			Unbreakable = 8,
			Transparent = 16,
			Foundation = 32
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
			int num = elements.FindIndex((global::Element ele) => ele.id == e.lowTempTransitionTarget);
			int num2 = elements.FindIndex((global::Element ele) => ele.id == e.highTempTransitionTarget);
			this.lowTempTransitionIdx = (byte)((num < 0) ? 255 : num);
			this.highTempTransitionIdx = (byte)((num2 < 0) ? 255 : num2);
			this.elementsTableIdx = (byte)elements.IndexOf(e);
			this.specificHeatCapacity = e.specificHeatCapacity;
			this.thermalConductivity = e.thermalConductivity;
			this.solidSurfaceAreaMultiplier = e.solidSurfaceAreaMultiplier;
			this.liquidSurfaceAreaMultiplier = e.liquidSurfaceAreaMultiplier;
			this.gasSurfaceAreaMultiplier = e.gasSurfaceAreaMultiplier;
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
			this.sublimateIndex = (sbyte)elements.FindIndex((global::Element ele) => ele.id == e.sublimateId);
			this.convertIndex = (sbyte)elements.FindIndex((global::Element ele) => ele.id == e.convertId);
			this.pack0 = 0;
			this.pack1 = 0;
			if (e.substance == null)
			{
				this.colour = 0U;
			}
			else
			{
				Color32 color = e.substance.colour;
				this.colour = (uint)(((int)color.a << 24) | ((int)color.b << 16) | ((int)color.g << 8) | (int)color.r);
			}
			this.sublimateFX = e.sublimateFX;
			this.defaultValues = e.defaultValues;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((int)this.id);
			writer.Write(this.state);
			writer.Write((sbyte)this.lowTempTransitionIdx);
			writer.Write((sbyte)this.highTempTransitionIdx);
			writer.Write(this.elementsTableIdx);
			writer.Write(this.specificHeatCapacity);
			writer.Write(this.thermalConductivity);
			writer.Write(this.molarMass);
			writer.Write(this.solidSurfaceAreaMultiplier);
			writer.Write(this.liquidSurfaceAreaMultiplier);
			writer.Write(this.gasSurfaceAreaMultiplier);
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
			writer.Write(this.sublimateIndex);
			writer.Write(this.convertIndex);
			writer.Write(this.pack0);
			writer.Write(this.pack1);
			writer.Write(this.colour);
			writer.Write((int)this.sublimateFX);
			this.defaultValues.Write(writer);
		}

		public SimHashes id;

		public byte state;

		public byte lowTempTransitionIdx;

		public byte highTempTransitionIdx;

		public byte elementsTableIdx;

		public float specificHeatCapacity;

		public float thermalConductivity;

		public float molarMass;

		public float solidSurfaceAreaMultiplier;

		public float liquidSurfaceAreaMultiplier;

		public float gasSurfaceAreaMultiplier;

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

		public sbyte sublimateIndex;

		public sbyte convertIndex;

		public byte pack0;

		public byte pack1;

		public uint colour;

		public SpawnFXHashes sublimateFX;

		public Sim.PhysicsData defaultValues;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseCell
	{
		public void Write(BinaryWriter writer)
		{
			writer.Write(this.diseaseIdx);
			writer.Write(this.reservedInfestationTickCount);
			writer.Write(this.pad1);
			writer.Write(this.pad2);
			writer.Write(this.elementCount);
			writer.Write(this.reservedAccumulatedError);
		}

		public byte diseaseIdx;

		private byte reservedInfestationTickCount;

		private byte pad1;

		private byte pad2;

		public int elementCount;

		private float reservedAccumulatedError;

		public static Sim.DiseaseCell Invalid = new Sim.DiseaseCell
		{
			diseaseIdx = byte.MaxValue,
			elementCount = 0
		};
	}

	public delegate void GAME_Callback();

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
		public int numFramesProcessed;

		public unsafe byte* elementIdx;

		public unsafe float* temperature;

		public unsafe float* mass;

		public unsafe byte* properties;

		public unsafe byte* insulation;

		public unsafe byte* strengthInfo;

		public unsafe byte* diseaseIdx;

		public unsafe int* diseaseCount;

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

		public int numDigInfo;

		public unsafe Sim.SpawnOreInfo* digInfo;

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

		public int numMassConsumedCallbacks;

		public unsafe Sim.MassConsumedCallback* massConsumedCallbacks;

		public int numMassEmittedCallbacks;

		public unsafe Sim.MassEmittedCallback* massEmittedCallbacks;

		public int numDiseaseConsumptionCallbacks;

		public unsafe Sim.DiseaseConsumptionCallback* diseaseConsumptionCallbacks;

		public int numComponentStateChangedMessages;

		public unsafe Sim.ComponentStateChangedMessage* componentStateChangedMessages;

		public int numRemovedMassEntries;

		public unsafe Sim.ConsumedMassInfo* removedMassEntries;

		public int numEmittedMassEntries;

		public unsafe Sim.EmittedMassInfo* emittedMassEntries;

		public int numElementChunkInfos;

		public unsafe Sim.ElementChunkInfo* elementChunkInfos;

		public int numElementChunkMeltedInfos;

		public unsafe Sim.MeltedInfo* elementChunkMeltedInfos;

		public int numBuildingOverheatInfos;

		public unsafe Sim.MeltedInfo* buildingOverheatInfos;

		public int numBuildingNoLongerOverheatedInfos;

		public unsafe Sim.MeltedInfo* buildingNoLongerOverheatedInfos;

		public int numBuildingMeltedInfos;

		public unsafe Sim.MeltedInfo* buildingMeltedInfos;

		public int numDiseaseEmittedInfos;

		public unsafe Sim.DiseaseEmittedInfo* diseaseEmittedInfos;

		public int numDiseaseConsumedInfos;

		public unsafe Sim.DiseaseConsumedInfo* diseaseConsumedInfos;

		public unsafe float* accumulatedFlow;

		public IntPtr propertyTextureFlow;

		public IntPtr propertyTextureLiquid;

		public IntPtr propertyTextureExposedToSunlight;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnFallingLiquidInfo
	{
		public int cellIdx;

		public byte elemIdx;

		public byte diseaseIdx;

		public byte pad0;

		public byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnOreInfo
	{
		public int cellIdx;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
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

		public byte diseaseIdx;

		private byte pad0;

		public float mass;

		public float temperature;

		public int diseaseCount;

		public enum FallingInfo
		{
			StartedFalling,
			StoppedFalling
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct NewGameFrame
	{
		public float elapsedSeconds;

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
	public struct PipeTemperatureChange
	{
		public int cellIdx;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MassConsumedCallback
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MassEmittedCallback
	{
		public int callbackIdx;

		public byte suceeded;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseConsumptionCallback
	{
		public int callbackIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int diseaseCount;
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

		public byte isDebugEditing;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct EmittedMassInfo
	{
		public byte elemIdx;

		public byte diseaseIdx;

		public byte pad0;

		public byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ConsumedMassInfo
	{
		public int simHandle;

		public byte removedElemIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ConsumedDiseaseInfo
	{
		public int simHandle;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ElementChunkInfo
	{
		public float temperature;

		public float deltaKJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MeltedInfo
	{
		public int handle;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BuildingTemperatureInfo
	{
		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BuildingConductivityData
	{
		public float temperature;

		public float heatCapacity;

		public float thermalConductivity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseEmittedInfo
	{
		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int count;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseConsumedInfo
	{
		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int count;
	}
}
