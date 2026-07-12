using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationHighlightController : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.highlightBox = Util.KInstantiateUI<RectTransform>(this.highlightBoxPrefab.gameObject, base.gameObject, false);
		this.HideBox();
	}

	[ContextMenu("Force Update")]
	protected void LateUpdate()
	{
		bool flag = false;
		if (this.activeTargetNotification != null)
		{
			foreach (NotificationHighlightTarget notificationHighlightTarget in this.targets)
			{
				if (notificationHighlightTarget.targetKey == this.activeTargetNotification.highlightTarget)
				{
					this.SnapBoxToTarget(notificationHighlightTarget);
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			this.HideBox();
		}
	}

	public void AddTarget(NotificationHighlightTarget target)
	{
		this.targets.Add(target);
	}

	public void RemoveTarget(NotificationHighlightTarget target)
	{
		this.targets.Remove(target);
	}

	public void SetActiveTarget(ManagementMenuNotification notification)
	{
		this.activeTargetNotification = notification;
	}

	public void ClearActiveTarget(ManagementMenuNotification checkNotification)
	{
		if (checkNotification == this.activeTargetNotification)
		{
			this.activeTargetNotification = null;
		}
	}

	public void ClearActiveTarget()
	{
		this.activeTargetNotification = null;
	}

	public void TargetViewed(NotificationHighlightTarget target)
	{
		if (this.activeTargetNotification != null && this.activeTargetNotification.highlightTarget == target.targetKey)
		{
			this.activeTargetNotification.View();
		}
	}

	private void SnapBoxToTarget(NotificationHighlightTarget target)
	{
		RectTransform rectTransform = target.rectTransform();
		Vector3 position = rectTransform.GetPosition();
		this.highlightBox.sizeDelta = rectTransform.rect.size;
		this.highlightBox.SetPosition(position + new Vector3(rectTransform.rect.position.x, rectTransform.rect.position.y, 0f));
		RectMask2D componentInParent = rectTransform.GetComponentInParent<RectMask2D>();
		if (componentInParent != null)
		{
			RectTransform rectTransform2 = componentInParent.rectTransform();
			Vector3 vector = rectTransform2.TransformPoint(rectTransform2.rect.min);
			Vector3 vector2 = rectTransform2.TransformPoint(rectTransform2.rect.max);
			Vector3 vector3 = this.highlightBox.TransformPoint(this.highlightBox.rect.min);
			Vector3 vector4 = this.highlightBox.TransformPoint(this.highlightBox.rect.max);
			Vector3 vector5 = vector - vector3;
			Vector3 vector6 = vector2 - vector4;
			if (vector5.x > 0f)
			{
				this.highlightBox.anchoredPosition = this.highlightBox.anchoredPosition + new Vector2(vector5.x, 0f);
				this.highlightBox.sizeDelta -= new Vector2(vector5.x, 0f);
			}
			else if (vector5.y > 0f)
			{
				this.highlightBox.anchoredPosition = this.highlightBox.anchoredPosition + new Vector2(0f, vector5.y);
				this.highlightBox.sizeDelta -= new Vector2(0f, vector5.y);
			}
			if (vector6.x < 0f)
			{
				this.highlightBox.sizeDelta += new Vector2(vector6.x, 0f);
			}
			if (vector6.y < 0f)
			{
				this.highlightBox.sizeDelta += new Vector2(0f, vector6.y);
			}
		}
		this.highlightBox.gameObject.SetActive(this.highlightBox.sizeDelta.x > 0f && this.highlightBox.sizeDelta.y > 0f);
	}

	private void HideBox()
	{
		this.highlightBox.gameObject.SetActive(false);
	}

	public RectTransform highlightBoxPrefab;

	private RectTransform highlightBox;

	private List<NotificationHighlightTarget> targets = new List<NotificationHighlightTarget>();

	private ManagementMenuNotification activeTargetNotification;
}
