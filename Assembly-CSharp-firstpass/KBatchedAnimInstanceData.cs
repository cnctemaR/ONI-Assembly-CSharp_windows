using System;
using UnityEngine;

public class KBatchedAnimInstanceData
{
	public KBatchedAnimInstanceData(KAnimConverter.IAnimConverter target)
	{
		this.target = target;
	}

	public void ResetHidden()
	{
		for (int i = 0; i < 4; i++)
		{
			this.hidden[i] = 0U;
		}
	}

	public void HideAll()
	{
		for (int i = 0; i < 4; i++)
		{
			this.hidden[i] = uint.MaxValue;
		}
	}

	public void SetHiddenBit(int bit)
	{
		int num = bit / 30;
		if (num < 4)
		{
			this.hidden[num] |= 1U << 2 + bit % 30;
		}
	}

	public void UnsetHiddenBit(int bit)
	{
		int num = bit / 30;
		if (num < 4)
		{
			this.hidden[num] = this.hidden[num] & ~(1U << 2 + bit % 30);
		}
	}

	public void SetClipRadius(float x, float y, float dist_sq, bool doClip)
	{
		this.clipRadius.x = x;
		this.clipRadius.y = y;
		this.clipRadius.z = dist_sq;
		this.clipRadius.w = (float)((!doClip) ? 0 : 1);
	}

	public void SetClipParams(float clipDepth, bool doClip)
	{
		this.clipParams.x = (float)((!doClip) ? 0 : 1);
		this.clipParams.y = clipDepth;
	}

	public void SetBlend(float amt)
	{
		this.extra.x = amt;
	}

	public void WriteToTexture(float[] data, int startIndex, int thisIndex)
	{
		this.currentAnimNumFrames = this.target.GetCurrentNumFrames();
		this.currentAnimFirstFrameIdx = this.target.GetFirstFrameIndex();
		this.curAnimFrameIndex = this.target.GetCurrentFrameIndex();
		this.highlightColour = this.target.GetHighlightColour();
		this.firstTintColour = this.target.GetFirstTintColour();
		this.secondTintColour = this.target.GetSecondTintColour();
		this.temperatureColour = this.target.GetTemperatureColour();
		this.firstTintIndex = this.target.GetFirstTintIndex();
		this.secondTintIndex = this.target.GetSecondTintIndex();
		this.transformationMatrix = this.target.GetTransformMatrix();
		data[startIndex++] = (float)this.curAnimFrameIndex;
		data[startIndex++] = (float)thisIndex;
		data[startIndex++] = (float)((!this.target.IsVisible()) ? 0 : this.currentAnimNumFrames);
		data[startIndex++] = (float)this.currentAnimFirstFrameIdx;
		for (int i = 0; i < 4; i++)
		{
			data[startIndex++] = this.hidden[i];
		}
		for (int j = 0; j < 4; j++)
		{
			Vector4 column = this.transformationMatrix.GetColumn(j);
			data[startIndex++] = column[0];
			data[startIndex++] = column[1];
			data[startIndex++] = column[2];
			data[startIndex++] = column[3];
		}
		data[startIndex++] = (float)this.highlightColour.r / 255f;
		data[startIndex++] = (float)this.highlightColour.g / 255f;
		data[startIndex++] = (float)this.highlightColour.b / 255f;
		data[startIndex++] = (float)this.highlightColour.a / 255f;
		data[startIndex++] = (float)this.firstTintColour.r / 255f;
		data[startIndex++] = (float)this.firstTintColour.g / 255f;
		data[startIndex++] = (float)this.firstTintColour.b / 255f;
		data[startIndex++] = (float)this.firstTintColour.a / 255f;
		data[startIndex++] = (float)this.secondTintColour.r / 255f;
		data[startIndex++] = (float)this.secondTintColour.g / 255f;
		data[startIndex++] = (float)this.secondTintColour.b / 255f;
		data[startIndex++] = (float)this.secondTintColour.a / 255f;
		data[startIndex++] = (float)this.temperatureColour.r / 255f;
		data[startIndex++] = (float)this.temperatureColour.g / 255f;
		data[startIndex++] = (float)this.temperatureColour.b / 255f;
		data[startIndex++] = (float)this.temperatureColour.a / 255f;
		data[startIndex++] = this.clipRadius[0];
		data[startIndex++] = this.clipRadius[1];
		data[startIndex++] = this.clipRadius[2];
		data[startIndex++] = this.clipRadius[3];
		data[startIndex++] = this.clipParams[0];
		data[startIndex++] = this.clipParams[1];
		data[startIndex++] = (float)this.firstTintIndex;
		data[startIndex++] = (float)this.secondTintIndex;
		data[startIndex++] = this.extra.x;
		data[startIndex++] = this.extra.y;
		data[startIndex++] = this.extra.z;
		data[startIndex++] = this.extra.w;
	}

	public const int bitsPerSlot = 30;

	public const int bitOffest = 2;

	private uint[] hidden = new uint[4];

	private Vector4 clipRadius = Vector4.zero;

	private Vector2 clipParams = Vector2.zero;

	private int currentAnimNumFrames = -1;

	private int currentAnimFirstFrameIdx = -1;

	private int curAnimFrameIndex = -1;

	private int firstTintIndex = -1;

	private int secondTintIndex = -1;

	private Color32 highlightColour = Color.white;

	private Color32 firstTintColour = Color.white;

	private Color32 secondTintColour = Color.white;

	private Color32 temperatureColour = Color.white;

	private Matrix4x4 transformationMatrix = Matrix4x4.identity;

	private KAnimConverter.IAnimConverter target;

	private Vector4 extra = Vector4.zero;
}
