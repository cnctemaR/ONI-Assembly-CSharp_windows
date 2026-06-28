using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class KBatchedAnimInstanceData
{
	public KBatchedAnimInstanceData(KAnimConverter.IAnimConverter target)
	{
		this.target = target;
		this.bytes = new byte[112];
		this.converter = new KBatchedAnimInstanceData.AnimInstanceDataToByteConverter
		{
			bytes = this.bytes
		};
		KBatchedAnimInstanceData.AnimInstanceData animInstanceData = this.converter.animInstanceData[0];
		animInstanceData.tintColour = Color.white;
		animInstanceData.highlightColour = Color.black;
		animInstanceData.overlayColour = Color.white;
		this.converter.animInstanceData[0] = animInstanceData;
	}

	public void SetClipRadius(float x, float y, float dist_sq, bool do_clip)
	{
		this.converter.animInstanceData[0].clipParameters = new Vector4(x, y, dist_sq, (float)((!do_clip) ? 0 : 1));
	}

	public void SetBlend(float amt)
	{
		this.converter.animInstanceData[0].blend = amt;
	}

	public Color GetOverlayColour()
	{
		return this.converter.animInstanceData[0].overlayColour;
	}

	public bool SetOverlayColour(Color color)
	{
		if (color != this.converter.animInstanceData[0].overlayColour)
		{
			this.converter.animInstanceData[0].overlayColour = color;
			return true;
		}
		return false;
	}

	public Color GetTintColour()
	{
		return this.converter.animInstanceData[0].tintColour;
	}

	public bool SetTintColour(Color color)
	{
		if (color != this.converter.animInstanceData[0].tintColour)
		{
			this.converter.animInstanceData[0].tintColour = color;
			return true;
		}
		return false;
	}

	public Color GetHighlightcolour()
	{
		return this.converter.animInstanceData[0].highlightColour;
	}

	public bool SetHighlightColour(Color color)
	{
		if (color != this.converter.animInstanceData[0].highlightColour)
		{
			this.converter.animInstanceData[0].highlightColour = color;
			return true;
		}
		return false;
	}

	public void WriteToTexture(byte[] output_bytes, int output_index, int this_index)
	{
		KBatchedAnimInstanceData.AnimInstanceData animInstanceData = this.converter.animInstanceData[0];
		animInstanceData.curAnimFrameIndex = (float)this.target.GetCurrentFrameIndex();
		animInstanceData.thisIndex = (float)this_index;
		animInstanceData.currentAnimNumFrames = (float)((!this.target.IsVisible()) ? 0 : this.target.GetCurrentNumFrames());
		animInstanceData.currentAnimFirstFrameIdx = (float)this.target.GetFirstFrameIndex();
		if (!this.isTransformOverriden)
		{
			animInstanceData.transformMatrix = this.target.GetTransformMatrix();
		}
		this.converter.animInstanceData[0] = animInstanceData;
		Buffer.BlockCopy(this.bytes, 0, output_bytes, output_index, 112);
	}

	public void SetOverrideTransformMatrix(Matrix2x3 transform_matrix)
	{
		this.isTransformOverriden = true;
		this.converter.animInstanceData[0].transformMatrix = transform_matrix;
	}

	public void ClearOverrideTransformMatrix()
	{
		this.isTransformOverriden = false;
	}

	public const int SIZE_IN_BYTES = 112;

	public const int SIZE_IN_FLOATS = 28;

	private KAnimConverter.IAnimConverter target;

	private bool isTransformOverriden;

	private byte[] bytes;

	private KBatchedAnimInstanceData.AnimInstanceDataToByteConverter converter;

	[StructLayout(LayoutKind.Explicit)]
	public struct AnimInstanceData
	{
		[FieldOffset(0)]
		public float curAnimFrameIndex;

		[FieldOffset(4)]
		public float thisIndex;

		[FieldOffset(8)]
		public float currentAnimNumFrames;

		[FieldOffset(12)]
		public float currentAnimFirstFrameIdx;

		[FieldOffset(16)]
		public Matrix2x3 transformMatrix;

		[FieldOffset(40)]
		public float blend;

		[FieldOffset(44)]
		public float unused;

		[FieldOffset(48)]
		public Color highlightColour;

		[FieldOffset(64)]
		public Color tintColour;

		[FieldOffset(80)]
		public Color overlayColour;

		[FieldOffset(96)]
		public Vector4 clipParameters;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct AnimInstanceDataToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public KBatchedAnimInstanceData.AnimInstanceData[] animInstanceData;
	}
}
