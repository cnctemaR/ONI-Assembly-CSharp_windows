using System;
using UnityEngine;

public class KAnimBatchInstanceData
{
	public KAnimBatchInstanceData(KAnimConverter.IAnimConverter target)
	{
		this.target = target;
	}

	public void ResetHidden()
	{
		this.hidden[0] = 0U;
		this.hidden[1] = 0U;
		this.hidden[2] = 0U;
		this.hidden[3] = 0U;
	}

	public void HideAll()
	{
		this.hidden[0] = uint.MaxValue;
		this.hidden[1] = uint.MaxValue;
		this.hidden[2] = uint.MaxValue;
		this.hidden[3] = uint.MaxValue;
	}

	public void SetHiddenBit(int bit)
	{
		int num = bit / 32;
		this.hidden[num] |= 1U << bit % 32;
	}

	public void UnsetHiddenBit(int bit)
	{
		int num = bit / 32;
		this.hidden[num] = this.hidden[num] & ~(1U << bit % 32);
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
		KAnimBatchInstanceData.hiddenBlock[0] = this.hidden[0];
		KAnimBatchInstanceData.hiddenBlock[1] = this.hidden[1];
		KAnimBatchInstanceData.hiddenBlock[2] = this.hidden[2];
		KAnimBatchInstanceData.hiddenBlock[3] = this.hidden[3];
		Buffer.BlockCopy(KAnimBatchInstanceData.hiddenBlock, 0, data, startIndex * 4, 16);
		startIndex += 4;
		for (int i = 0; i < 4; i++)
		{
			Vector4 column = this.transformationMatrix.GetColumn(i);
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
	}

	private static uint[] hiddenBlock = new uint[4];

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
}
