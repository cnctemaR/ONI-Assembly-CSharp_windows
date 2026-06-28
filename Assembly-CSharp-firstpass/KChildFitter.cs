using System;
using UnityEngine;
using UnityEngine.UI;

public class KChildFitter : MonoBehaviour
{
	private void Awake()
	{
		this.rect_transform = base.GetComponent<RectTransform>();
		this.VLG = base.GetComponent<VerticalLayoutGroup>();
		this.HLG = base.GetComponent<HorizontalLayoutGroup>();
		this.GLG = base.GetComponent<GridLayoutGroup>();
		if (this.overrideLayoutElement == null)
		{
			this.overrideLayoutElement = base.GetComponent<LayoutElement>();
		}
	}

	private void LateUpdate()
	{
		this.FitSize();
	}

	public Vector2 GetPositionRelativeToTopLeftPivot(RectTransform element)
	{
		Vector2 zero = Vector2.zero;
		zero.x = element.anchoredPosition.x - element.sizeDelta.x * element.pivot.x;
		zero.y = element.anchoredPosition.y + element.sizeDelta.y * (1f - element.pivot.y);
		return zero;
	}

	public void FitSize()
	{
		if (this.fitWidth || this.fitHeight)
		{
			Vector2 sizeDelta = this.rect_transform.sizeDelta;
			if (this.fitWidth)
			{
				sizeDelta.x = 0f;
			}
			if (this.fitHeight)
			{
				sizeDelta.y = 0f;
			}
			float num = float.NegativeInfinity;
			float num2 = float.PositiveInfinity;
			float num3 = float.PositiveInfinity;
			float num4 = float.NegativeInfinity;
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				LayoutElement component = child.gameObject.GetComponent<LayoutElement>();
				if (component == null || !component.ignoreLayout)
				{
					if (child.gameObject.activeSelf)
					{
						RectTransform rectTransform = child as RectTransform;
						if (this.fitWidth)
						{
							if (this.findTotalBounds)
							{
								float num5 = this.GetPositionRelativeToTopLeftPivot(rectTransform).x + rectTransform.sizeDelta.x;
								if (num5 > num4)
								{
									num4 = num5;
								}
								float x = this.GetPositionRelativeToTopLeftPivot(rectTransform).x;
								if (x < num3)
								{
									num3 = x;
								}
								sizeDelta.x = Mathf.Abs(num4 - num3);
								if (this.includeLayoutGroupPadding)
								{
									sizeDelta.x += (float)((!(this.VLG != null)) ? 0 : (this.VLG.padding.left + this.VLG.padding.right));
									sizeDelta.x += (float)((!(this.HLG != null)) ? 0 : (this.HLG.padding.left + this.HLG.padding.right));
									sizeDelta.x += (float)((!(this.GLG != null)) ? 0 : (this.GLG.padding.left + this.GLG.padding.right));
								}
							}
							else
							{
								sizeDelta.x += rectTransform.sizeDelta.x;
								if (this.HLG)
								{
									sizeDelta.x += this.HLG.spacing;
								}
							}
						}
						if (this.fitHeight)
						{
							if (this.findTotalBounds)
							{
								if (this.GetPositionRelativeToTopLeftPivot(rectTransform).y > num)
								{
									num = this.GetPositionRelativeToTopLeftPivot(rectTransform).y;
								}
								if (this.GetPositionRelativeToTopLeftPivot(rectTransform).y - rectTransform.sizeDelta.y < num2)
								{
									num2 = this.GetPositionRelativeToTopLeftPivot(rectTransform).y - rectTransform.sizeDelta.y;
								}
								sizeDelta.y = Mathf.Abs(num - num2);
								if (this.includeLayoutGroupPadding)
								{
									sizeDelta.y += (float)((!(this.VLG != null)) ? 0 : (this.VLG.padding.bottom + this.VLG.padding.top));
									sizeDelta.y += (float)((!(this.HLG != null)) ? 0 : (this.HLG.padding.bottom + this.HLG.padding.top));
									sizeDelta.y += (float)((!(this.GLG != null)) ? 0 : (this.GLG.padding.bottom + this.GLG.padding.top));
								}
							}
							else
							{
								sizeDelta.y += rectTransform.sizeDelta.y;
								if (this.VLG)
								{
									sizeDelta.y += this.VLG.spacing;
								}
							}
						}
					}
				}
			}
			Vector2 vector = new Vector2(this.WidthPadding, this.HeightPadding);
			if (!this.fitWidth)
			{
				this.WidthPadding = 0f;
			}
			if (!this.fitHeight)
			{
				this.HeightPadding = 0f;
			}
			if (this.overrideLayoutElement != null)
			{
				if (this.fitWidth)
				{
					if (this.overrideLayoutElement.minWidth != (sizeDelta.x + vector.x) * this.WidthScale)
					{
						this.overrideLayoutElement.minWidth = (sizeDelta.x + vector.x) * this.WidthScale;
					}
				}
				if (this.fitHeight)
				{
					if (this.overrideLayoutElement.minHeight != (sizeDelta.y + vector.y) * this.HeightScale)
					{
						this.overrideLayoutElement.minHeight = (sizeDelta.y + vector.y) * this.HeightScale;
					}
				}
			}
			Vector2 vector2 = new Vector2(this.WidthScale * (sizeDelta.x + vector.x), this.HeightScale * (sizeDelta.y + vector.y));
			if (this.rect_transform.sizeDelta != vector2)
			{
				this.rect_transform.sizeDelta = vector2;
				if (base.transform.parent != null)
				{
					KChildFitter component2 = base.transform.parent.GetComponent<KChildFitter>();
					if (component2 != null)
					{
						component2.FitSize();
					}
				}
			}
		}
	}

	public bool fitWidth;

	public bool fitHeight;

	public float HeightPadding = 0f;

	public float WidthPadding = 0f;

	public float WidthScale = 1f;

	public float HeightScale = 1f;

	public LayoutElement overrideLayoutElement;

	private RectTransform rect_transform;

	private VerticalLayoutGroup VLG;

	private HorizontalLayoutGroup HLG;

	private GridLayoutGroup GLG;

	public bool findTotalBounds = true;

	public bool includeLayoutGroupPadding = true;
}
