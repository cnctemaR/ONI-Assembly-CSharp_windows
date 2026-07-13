using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowOptimizedKScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.graphicRaycaster = base.GetComponent<GraphicRaycaster>();
	}

	public override void Show(bool show = true)
	{
		this.mouseOver = false;
		this.canvasGroup.alpha = (float)(show ? 1 : 0);
		this.canvasGroup.interactable = show;
		this.canvasGroup.blocksRaycasts = show;
		this.graphicRaycaster.enabled = show;
		this.OnShow(show);
		if (this.enableLayoutOnShow != null)
		{
			this.enableLayoutOnShow.enabled = show;
		}
	}

	public override bool IsScreenActive()
	{
		return this.canvasGroup.alpha > 0f;
	}

	[SerializeField]
	private LayoutGroup enableLayoutOnShow;

	private CanvasGroup canvasGroup;

	private GraphicRaycaster graphicRaycaster;
}
