using System;
using UnityEngine;
using UnityEngine.UI;

public class PlanCategoryNotifications : MonoBehaviour
{
	public void ToggleAttention(bool active)
	{
		if (!this.AttentionImage)
		{
			return;
		}
		this.AttentionImage.gameObject.SetActive(active);
	}

	public Image AttentionImage;
}
