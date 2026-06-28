using System;
using UnityEngine;

public class WidgetTransition : MonoBehaviour
{
	private CanvasGroup CanvasGroup
	{
		get
		{
			return (!(this.canvasGroup == null)) ? this.canvasGroup : (this.canvasGroup = base.gameObject.FindOrAddUnityComponent<CanvasGroup>());
		}
	}

	public void SetTransitionType(WidgetTransition.TransitionType transitionType)
	{
		switch (transitionType)
		{
		case WidgetTransition.TransitionType.SlideFromRight:
			this.beginningOffset = new Vector2(50f, 0f);
			return;
		case WidgetTransition.TransitionType.SlideFromLeft:
			this.beginningOffset = new Vector2(-50f, 0f);
			return;
		case WidgetTransition.TransitionType.SlideFromTop:
			this.beginningOffset = new Vector2(0f, 50f);
			return;
		}
		this.beginningOffset = new Vector2(0f, 0f);
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
		Vector2 vector = base.gameObject.rectTransform().anchoredPosition;
		this.targetPos = new Vector2(vector.x, vector.y);
		vector += this.beginningOffset;
		base.gameObject.rectTransform().anchoredPosition = vector;
	}

	public void StopTransition()
	{
		if (this.fadingIn)
		{
			this.fadingIn = false;
			base.enabled = false;
			base.gameObject.rectTransform().anchoredPosition = this.targetPos;
		}
	}

	private void Update()
	{
		if (this.fadingIn)
		{
			Vector2 anchoredPosition = base.gameObject.rectTransform().anchoredPosition;
			Vector2 vector = Vector2.Lerp(anchoredPosition, this.targetPos, 7f * Time.unscaledDeltaTime) - anchoredPosition;
			base.gameObject.rectTransform().anchoredPosition += vector;
			float num = this.CanvasGroup.alpha;
			num += 0.1f;
			if (num >= 1f)
			{
				num = 1f;
			}
			if (num == 1f && vector.magnitude < 0.001f)
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

	private const float FADEIN_SPEED = 0.1f;

	private Vector2 beginningOffset = new Vector2(50f, 0f);

	private Vector2 targetPos = default(Vector2);

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
