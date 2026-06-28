using System;
using UnityEngine;
using UnityEngine.UI;

public class KRectStretcher : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.UpdateStretching();
		Canvas.ForceUpdateCanvases();
	}

	private void Update()
	{
		this.UpdateStretching();
	}

	public void UpdateStretching()
	{
		if (this.rect == null)
		{
			this.rect = base.GetComponent<RectTransform>();
		}
		if (this.rect == null)
		{
			return;
		}
		if (base.transform.parent == null)
		{
			return;
		}
		RectTransform rectTransform = base.transform.parent.rectTransform();
		Vector3 vector = Vector3.zero;
		if (this.SizeReferenceMethod == KRectStretcher.ParentSizeReferenceValue.SizeDelta)
		{
			vector = rectTransform.sizeDelta;
		}
		else if (this.SizeReferenceMethod == KRectStretcher.ParentSizeReferenceValue.RectDimensions)
		{
			vector = rectTransform.rect.size;
		}
		Vector2 zero = Vector2.zero;
		if (!this.PreserveAspectRatio)
		{
			zero = new Vector2((!this.StretchX) ? this.rect.sizeDelta.x : vector.x, (!this.StretchY) ? this.rect.sizeDelta.y : vector.y);
		}
		else
		{
			KRectStretcher.aspectFitOption aspectFitOption = this.AspectFitOption;
			if (aspectFitOption != KRectStretcher.aspectFitOption.WidthDictatesHeight)
			{
				if (aspectFitOption != KRectStretcher.aspectFitOption.HeightDictatesWidth)
				{
					if (aspectFitOption == KRectStretcher.aspectFitOption.EnvelopeParent)
					{
						if (rectTransform.sizeDelta.x / rectTransform.sizeDelta.y > this.aspectRatioToPreserve)
						{
							zero = new Vector2((!this.StretchX) ? this.rect.sizeDelta.x : vector.x, (!this.StretchY) ? this.rect.sizeDelta.y : (vector.x / this.aspectRatioToPreserve));
						}
						else
						{
							zero = new Vector2((!this.StretchX) ? this.rect.sizeDelta.x : (vector.y * this.aspectRatioToPreserve), (!this.StretchY) ? this.rect.sizeDelta.y : vector.y);
						}
					}
				}
				else
				{
					zero = new Vector2((!this.StretchX) ? this.rect.sizeDelta.x : (vector.y * this.aspectRatioToPreserve), (!this.StretchY) ? this.rect.sizeDelta.y : vector.y);
				}
			}
			else
			{
				zero = new Vector2((!this.StretchX) ? this.rect.sizeDelta.x : vector.x, (!this.StretchY) ? this.rect.sizeDelta.y : (vector.x / this.aspectRatioToPreserve));
			}
		}
		if (this.StretchX)
		{
			zero.x *= this.XStretchFactor;
		}
		if (this.StretchY)
		{
			zero.y *= this.YStretchFactor;
		}
		if (this.StretchX)
		{
			zero.x += this.Padding.x;
		}
		if (this.StretchY)
		{
			zero.y += this.Padding.y;
		}
		if (this.rect.sizeDelta != zero)
		{
			if (this.lerpToSize)
			{
				if (this.OverrideLayoutElement != null)
				{
					if (this.StretchX)
					{
						this.OverrideLayoutElement.minWidth = Mathf.Lerp(this.OverrideLayoutElement.minWidth, zero.x, Time.unscaledDeltaTime * this.lerpTime);
					}
					if (this.StretchY)
					{
						this.OverrideLayoutElement.minHeight = Mathf.Lerp(this.OverrideLayoutElement.minHeight, zero.y, Time.unscaledDeltaTime * this.lerpTime);
					}
				}
				else
				{
					this.rect.sizeDelta = Vector2.Lerp(this.rect.sizeDelta, zero, this.lerpTime * Time.unscaledDeltaTime);
				}
			}
			else
			{
				if (this.OverrideLayoutElement != null)
				{
					if (this.StretchX)
					{
						this.OverrideLayoutElement.minWidth = zero.x;
					}
					if (this.StretchY)
					{
						this.OverrideLayoutElement.minHeight = zero.y;
					}
				}
				this.rect.sizeDelta = zero;
			}
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			KRectStretcher component = base.transform.GetChild(i).GetComponent<KRectStretcher>();
			if (component)
			{
				component.UpdateStretching();
			}
		}
	}

	private RectTransform rect;

	public bool StretchX;

	public bool StretchY;

	public float XStretchFactor = 1f;

	public float YStretchFactor = 1f;

	public KRectStretcher.ParentSizeReferenceValue SizeReferenceMethod;

	public Vector2 Padding;

	public bool lerpToSize;

	public float lerpTime = 1f;

	public LayoutElement OverrideLayoutElement;

	public bool PreserveAspectRatio;

	public float aspectRatioToPreserve = 1f;

	public KRectStretcher.aspectFitOption AspectFitOption;

	public enum ParentSizeReferenceValue
	{
		SizeDelta,
		RectDimensions
	}

	public enum aspectFitOption
	{
		WidthDictatesHeight,
		HeightDictatesWidth,
		EnvelopeParent
	}
}
