using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class KleiPermitDioramaVisScaler : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange()
	{
		this.Layout();
	}

	public void Layout()
	{
		KleiPermitDioramaVisScaler.Layout(this.root, this.scaleTarget, this.slot);
	}

	public static void Layout(RectTransform root, RectTransform scaleTarget, RectTransform slot)
	{
		float num = 2.125f;
		AspectRatioFitter aspectRatioFitter = slot.FindOrAddComponent<AspectRatioFitter>();
		aspectRatioFitter.aspectRatio = num;
		aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
		float num2 = 128f;
		float num3 = 128f;
		float num4 = 1700f;
		float num5 = Mathf.Max(0.1f, root.rect.width - num2) / num4;
		float num6 = 800f;
		float num7 = Mathf.Max(0.1f, root.rect.height - num3) / num6;
		float num8 = Mathf.Max(num5, num7);
		scaleTarget.localScale = Vector3.one * num8;
		scaleTarget.sizeDelta = new Vector2(1700f, 800f);
		scaleTarget.anchorMin = Vector2.one * 0.5f;
		scaleTarget.anchorMax = Vector2.one * 0.5f;
		scaleTarget.pivot = Vector2.one * 0.5f;
		scaleTarget.anchoredPosition = Vector2.zero;
	}

	public const float REFERENCE_WIDTH = 1700f;

	public const float REFERENCE_HEIGHT = 800f;

	[SerializeField]
	private RectTransform root;

	[SerializeField]
	private RectTransform scaleTarget;

	[SerializeField]
	private RectTransform slot;
}
