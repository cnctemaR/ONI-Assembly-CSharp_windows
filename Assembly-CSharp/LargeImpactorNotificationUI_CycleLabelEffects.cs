using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LargeImpactorNotificationUI_CycleLabelEffects : MonoBehaviour
{
	public void InitializeCycleLabelFocusMonitor()
	{
		this.AbortCycleLabelFocusMonitor();
		this.cycleLabelFocusCoroutine = base.StartCoroutine(this.CycleLabelFocusMonitor());
	}

	public void AbortCycleLabelFocusMonitor()
	{
		if (this.cycleLabelFocusCoroutine != null)
		{
			base.StopCoroutine(this.cycleLabelFocusCoroutine);
			this.cycleLabelFocusCoroutine = null;
		}
	}

	private IEnumerator CycleLabelFocusMonitor()
	{
		float previousVisibleValue = -1f;
		float visibleValue = 0f;
		for (;;)
		{
			visibleValue = Mathf.Clamp(visibleValue + Time.unscaledDeltaTime / (this.notificationTooltipComponent.isHovering ? this.cycleFocusSpeed : this.cycleUnfocusSpeed) * (float)(this.notificationTooltipComponent.isHovering ? 1 : (-1)), 0f, 1f);
			if (visibleValue != previousVisibleValue)
			{
				previousVisibleValue = visibleValue;
				this.cyclesLabelBackground.Opacity(visibleValue);
				this.numberOfCyclesLabel.Opacity(visibleValue);
			}
			yield return null;
		}
		yield break;
	}

	public ToolTip notificationTooltipComponent;

	public Image cyclesLabelBackground;

	public LocText numberOfCyclesLabel;

	private Coroutine cycleLabelFocusCoroutine;

	private float cycleFocusSpeed = 0.2f;

	private float cycleUnfocusSpeed = 0.4f;
}
