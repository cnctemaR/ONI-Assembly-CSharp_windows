using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SymbolOverrideInfoGpuData
{
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

	private SymbolOverrideInfoGpuData.SymbolOverrideInfo[] symbolOverrideInfos
	{
		get
		{
			return this.symbolOverrideInfoConverter.symbolOverrideInfos;
		}
	}

	public int version { get; private set; }

	private void MarkDirty()
	{
		this.version++;
	}

	public void SetSymbolOverrideInfo(int symbol_idx, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		if (symbol_idx >= this.symbolCount)
		{
			DebugUtil.Assert(false, "Assert!");
		}
		SymbolOverrideInfoGpuData.SymbolOverrideInfo symbolOverrideInfo = this.symbolOverrideInfos[symbol_idx];
		symbolOverrideInfo.atlas = (float)symbol_frame_instance.buildImageIdx;
		symbolOverrideInfo.isoverriden = 1f;
		symbolOverrideInfo.bboxMin = symbol_frame_instance.symbolFrame.bboxMin;
		symbolOverrideInfo.bboxMax = symbol_frame_instance.symbolFrame.bboxMax;
		symbolOverrideInfo.uvMin = symbol_frame_instance.symbolFrame.uvMin;
		symbolOverrideInfo.uvMax = symbol_frame_instance.symbolFrame.uvMax;
		this.symbolOverrideInfos[symbol_idx] = symbolOverrideInfo;
		this.MarkDirty();
	}

	public void WriteToTexture(byte[] data, int data_idx, int instance_idx)
	{
		DebugUtil.Assert(instance_idx * this.symbolCount * 12 * 4 == data_idx, "Assert!");
		Buffer.BlockCopy(this.symbolOverrideInfoConverter.bytes, 0, data, data_idx, this.symbolCount * 12 * 4);
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
