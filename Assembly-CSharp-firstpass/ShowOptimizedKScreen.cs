using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowOptimizedKScreen : KScreen
{
	public override void Show(bool show = true)
	{
		this.mouseOver = false;
		foreach (Canvas canvas in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas.enabled != show)
			{
				canvas.enabled = show;
			}
		}
		CanvasGroup component = base.GetComponent<CanvasGroup>();
		if (component != null)
		{
			component.interactable = show;
			component.blocksRaycasts = show;
			component.ignoreParentGroups = true;
		}
		this.isHiddenButActive = !show;
		this.OnShow(show);
		if (this.enableLayoutOnShow != null)
		{
			this.enableLayoutOnShow.enabled = show;
		}
	}

	[SerializeField]
	private LayoutGroup enableLayoutOnShow;
}
