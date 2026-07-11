using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TimeOfDayPositioner")]
public class TimeOfDayPositioner : KMonoBehaviour
{
	private void Update()
	{
		float num = GameClock.Instance.GetCurrentCycleAsPercentage() * this.targetRect.rect.width;
		(base.transform as RectTransform).anchoredPosition = this.targetRect.anchoredPosition + new Vector2(Mathf.Round(num), 0f);
	}

	[SerializeField]
	private RectTransform targetRect;
}
