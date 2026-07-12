using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

public class SymbolOverrideInfoGpuData
{
	private SymbolOverrideInfoGpuData.SymbolOverrideInfo[] symbolOverrideInfos
	{
		get
		{
			return this.symbolOverrideInfoConverter.symbolOverrideInfos;
		}
	}

	public int version { get; private set; }

	public SymbolOverrideInfoGpuData(int symbol_count)
	{
		this.symbolCount = symbol_count;
		this.symbolOverrideInfoConverter = new SymbolOverrideInfoGpuData.SymbolOverrideInfoToByteConverter
		{
			bytes = new byte[12 * symbol_count * 4]
		};
		for (int i = 0; i < symbol_count; i++)
		{
			this.symbolOverrideInfos[i].atlas = 0f;
		}
		this.MarkDirty();
	}

	private void MarkDirty()
	{
		int num = this.version + 1;
		this.version = num;
	}

	public void SetSymbolOverrideInfo(int symbol_start_idx, int symbol_num_frames, int atlas_idx, KBatchGroupData source_data, int source_start_idx, int source_num_frames)
	{
		for (int i = 0; i < symbol_num_frames; i++)
		{
			int num = symbol_start_idx + i;
			int num2 = source_start_idx + Math.Min(source_num_frames - 1, i);
			KAnim.Build.SymbolFrameInstance symbolFrameInstance = source_data.symbolFrameInstances[num2];
			SymbolOverrideInfoGpuData.SymbolOverrideInfo[] symbolOverrideInfos = this.symbolOverrideInfos;
			int num3 = num;
			symbolOverrideInfos[num3].atlas = (float)atlas_idx;
			symbolOverrideInfos[num3].isoverriden = 1f;
			symbolOverrideInfos[num3].bboxMin = symbolFrameInstance.bboxMin;
			symbolOverrideInfos[num3].bboxMax = symbolFrameInstance.bboxMax;
			symbolOverrideInfos[num3].uvMin = symbolFrameInstance.uvMin;
			symbolOverrideInfos[num3].uvMax = symbolFrameInstance.uvMax;
		}
		this.MarkDirty();
	}

	public void SetSymbolOverrideInfo(int symbol_idx, ref KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		SymbolOverrideInfoGpuData.SymbolOverrideInfo[] symbolOverrideInfos = this.symbolOverrideInfos;
		symbolOverrideInfos[symbol_idx].atlas = (float)symbol_frame_instance.buildImageIdx;
		symbolOverrideInfos[symbol_idx].isoverriden = 1f;
		symbolOverrideInfos[symbol_idx].bboxMin = symbol_frame_instance.bboxMin;
		symbolOverrideInfos[symbol_idx].bboxMax = symbol_frame_instance.bboxMax;
		symbolOverrideInfos[symbol_idx].uvMin = symbol_frame_instance.uvMin;
		symbolOverrideInfos[symbol_idx].uvMax = symbol_frame_instance.uvMax;
		this.MarkDirty();
	}

	public void WriteToTexture(NativeArray<byte> data, int data_idx, int instance_idx)
	{
		DebugUtil.Assert(instance_idx * this.symbolCount * 12 * 4 == data_idx);
		NativeArray<byte>.Copy(this.symbolOverrideInfoConverter.bytes, 0, data, data_idx, this.symbolCount * 12 * 4);
	}

	public const int FLOATS_PER_SYMBOL_OVERRIDE_INFO = 12;

	private SymbolOverrideInfoGpuData.SymbolOverrideInfoToByteConverter symbolOverrideInfoConverter;

	private int symbolCount;

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolOverrideInfo
	{
		[FieldOffset(0)]
		public float atlas;

		[FieldOffset(4)]
		public float isoverriden;

		[FieldOffset(8)]
		public float unused1;

		[FieldOffset(12)]
		public float unused2;

		[FieldOffset(16)]
		public Vector2 bboxMin;

		[FieldOffset(24)]
		public Vector2 bboxMax;

		[FieldOffset(32)]
		public Vector2 uvMin;

		[FieldOffset(40)]
		public Vector2 uvMax;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolOverrideInfoToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public SymbolOverrideInfoGpuData.SymbolOverrideInfo[] symbolOverrideInfos;
	}
}
