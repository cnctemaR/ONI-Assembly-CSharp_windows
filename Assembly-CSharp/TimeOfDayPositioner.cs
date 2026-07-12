using System;
using UnityEngine;

public class TimeOfDayPositioner : KMonoBehaviour
{
	public void SetTargetTimetable(GameObject TimetableRow)
	{
		if (TimetableRow == null)
		{
			this.targetRect = null;
			base.transform.SetParent(null);
			return;
		}
		RectTransform rectTransform = TimetableRow.GetComponent<HierarchyReferences>().GetReference<RectTransform>("BlockContainer").rectTransform();
		this.targetRect = rectTransform;
		base.transform.SetParent(this.targetRect.transform);
	}

	private void Update()
	{
		if (this.targetRect == null)
		{
			return;
		}
		if (base.transform.parent != this.targetRect.transform)
		{
			base.transform.parent = this.targetRect.transform;
		}
		float num = GameClock.Instance.GetCurrentCycleAsPercentage() * this.targetRect.rect.width;
		(base.transform as RectTransform).anchoredPosition = new Vector2(Mathf.Round(num), 0f);
	}

	private RectTransform targetRect;
}
