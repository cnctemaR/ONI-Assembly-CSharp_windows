using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

public class SymbolInstanceGpuData
{
	private SymbolInstanceGpuData.SymbolInstance[] symbolInstances
	{
		get
		{
			return this.symbolInstancesConverter.symbolInstances;
		}
	}

	public int version { get; private set; }

	public SymbolInstanceGpuData(int symbol_count)
	{
		this.symbolCount = symbol_count;
		this.symbolInstancesConverter = new SymbolInstanceGpuData.SymbolInstanceToByteConverter
		{
			bytes = new byte[8 * symbol_count * 4]
		};
		for (int i = 0; i < symbol_count; i++)
		{
			SymbolInstanceGpuData.SymbolInstance[] symbolInstances = this.symbolInstances;
			int num = i;
			symbolInstances[num].isVisible = 1f;
			symbolInstances[num].scale = 1f;
			symbolInstances[num].unused = 1f;
			symbolInstances[num].color = Color.white;
		}
		this.MarkDirty();
	}

	private void MarkDirty()
	{
		int num = this.version + 1;
		this.version = num;
	}

	public void SetVisible(int symbol_idx, bool is_visible)
	{
		DebugUtil.Assert(symbol_idx < this.symbolCount);
		float num = 0f;
		if (is_visible)
		{
			num = 1f;
		}
		if (this.symbolInstances[symbol_idx].isVisible != num)
		{
			this.symbolInstances[symbol_idx].isVisible = num;
			this.MarkDirty();
		}
	}

	public bool IsVisible(int symbol_idx)
	{
		DebugUtil.Assert(symbol_idx < this.symbolCount);
		return this.symbolInstances[symbol_idx].isVisible > 0.5f;
	}

	public void SetSymbolScale(int symbol_index, float scale)
	{
		DebugUtil.Assert(symbol_index < this.symbolCount);
		if (this.symbolInstances[symbol_index].scale != scale)
		{
			this.symbolInstances[symbol_index].scale = scale;
			this.MarkDirty();
		}
	}

	public void SetSymbolTint(int symbol_index, Color color)
	{
		DebugUtil.Assert(symbol_index < this.symbolCount);
		if (this.symbolInstances[symbol_index].color != color)
		{
			this.symbolInstances[symbol_index].color = color;
			this.MarkDirty();
		}
	}

	public void WriteToTexture(NativeArray<byte> data, int data_idx, int instance_idx)
	{
		NativeArray<byte>.Copy(this.symbolInstancesConverter.bytes, 0, data, data_idx, this.symbolCount * 8 * 4);
	}

	public const int FLOATS_PER_SYMBOL_INSTANCE = 8;

	private SymbolInstanceGpuData.SymbolInstanceToByteConverter symbolInstancesConverter;

	private int symbolCount;

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolInstance
	{
		[FieldOffset(0)]
		public float isVisible;

		[FieldOffset(4)]
		public float scale;

		[FieldOffset(8)]
		public float unused;

		[FieldOffset(12)]
		public float unused2;

		[FieldOffset(16)]
		public Color color;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolInstanceToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public SymbolInstanceGpuData.SymbolInstance[] symbolInstances;
	}
}
