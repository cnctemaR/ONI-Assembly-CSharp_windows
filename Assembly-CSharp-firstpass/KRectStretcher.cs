using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("KMonoBehaviour/Plugins/KRectStretcher")]
public class KRectStretcher : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.rectTracker = default(DrivenRectTransformTracker);
		this.UpdateStretching();
	}

	private void Update()
	{
		if (base.transform.parent.hasChanged || (this.OverrideLayoutElement != null && this.OverrideLayoutElement.transform.hasChanged))
		{
			this.UpdateStretching();
		}
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
		if (base.transform.parent == null && this.OverrideLayoutElement == null)
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
			zero = new Vector2(this.StretchX ? vector.x : this.rect.sizeDelta.x, this.StretchY ? vector.y : this.rect.sizeDelta.y);
		}
		else
		{
			switch (this.AspectFitOption)
			{
			case KRectStretcher.aspectFitOption.WidthDictatesHeight:
				zero = new Vector2(this.StretchX ? vector.x : this.rect.sizeDelta.x, this.StretchY ? (vector.x / this.aspectRatioToPreserve) : this.rect.sizeDelta.y);
				break;
			case KRectStretcher.aspectFitOption.HeightDictatesWidth:
				zero = new Vector2(this.StretchX ? (vector.y * this.aspectRatioToPreserve) : this.rect.sizeDelta.x, this.StretchY ? vector.y : this.rect.sizeDelta.y);
				break;
			case KRectStretcher.aspectFitOption.EnvelopeParent:
				if (rectTransform.sizeDelta.x / rectTransform.sizeDelta.y > this.aspectRatioToPreserve)
				{
					zero = new Vector2(this.StretchX ? vector.x : this.rect.sizeDelta.x, this.StretchY ? (vector.x / this.aspectRatioToPreserve) : this.rect.sizeDelta.y);
				}
				else
				{
					zero = new Vector2(this.StretchX ? (vector.y * this.aspectRatioToPreserve) : this.rect.sizeDelta.x, this.StretchY ? vector.y : this.rect.sizeDelta.y);
				}
				break;
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
		this.rectTracker.Clear();
		if (this.StretchX)
		{
			this.rectTracker.Add(this, this.rect, DrivenTransformProperties.SizeDeltaX);
		}
		if (this.StretchY)
		{
			this.rectTracker.Add(this, this.rect, DrivenTransformProperties.SizeDeltaY);
		}
	}

	private RectTransform rect;

	private DrivenRectTransformTracker rectTracker;

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
