using System;
using UnityEngine;

public class WidgetTransition : MonoBehaviour
{
	private CanvasGroup CanvasGroup
	{
		get
		{
			if (!(this.canvasGroup == null))
			{
				return this.canvasGroup;
			}
			return this.canvasGroup = base.gameObject.FindOrAddUnityComponent<CanvasGroup>();
		}
	}

	public void SetTransitionType(WidgetTransition.TransitionType transitionType)
	{
	}

	public void StartTransition()
	{
		if (this.fadingIn)
		{
			return;
		}
		this.CanvasGroup.alpha = 0f;
		this.fadingIn = true;
		base.enabled = true;
	}

	public void StopTransition()
	{
		if (this.fadingIn)
		{
			this.fadingIn = false;
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (this.fadingIn)
		{
			float num = this.CanvasGroup.alpha;
			num += 6f * Time.unscaledDeltaTime;
			if (num >= 1f)
			{
				num = 1f;
			}
			if (num == 1f)
			{
				this.fadingIn = false;
				base.enabled = false;
			}
			this.CanvasGroup.alpha = num;
		}
	}

	private void OnDisable()
	{
		this.StopTransition();
	}

	private const float OFFSETX = 50f;

	private const float SLIDE_SPEED = 7f;

	private const float FADEIN_SPEED = 6f;

	private bool fadingIn;

	private CanvasGroup canvasGroup;

	public enum TransitionType
	{
		SlideFromRight,
		SlideFromLeft,
		FadeOnly,
		SlideFromTop
	}
}
