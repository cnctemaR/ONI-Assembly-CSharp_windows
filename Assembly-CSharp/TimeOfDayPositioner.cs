using System;
using UnityEngine;

public class TimeOfDayPositioner : KMonoBehaviour
{
	private void Update()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		float num = currentCycleAsPercentage * this.targetRect.rect.width;
		RectTransform rectTransform = base.transform as RectTransform;
		rectTransform.anchoredPosition = this.targetRect.anchoredPosition + new Vector2(Mathf.Round(num), 0f);
	}

	[SerializeField]
	private RectTransform targetRect;
}
