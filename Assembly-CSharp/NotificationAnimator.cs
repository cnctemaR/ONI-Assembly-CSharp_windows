using System;
using UnityEngine;
using UnityEngine.UI;

public class NotificationAnimator : MonoBehaviour
{
	public void Init()
	{
		this.layoutElement = base.GetComponent<LayoutElement>();
		this.layoutElement.minWidth = 100f;
	}

	private void LateUpdate()
	{
		this.layoutElement.minWidth -= this.speed;
		this.speed += 0.5f;
		if (this.layoutElement.minWidth <= 0f)
		{
			if (this.bounceCount > 0)
			{
				this.bounceCount--;
				this.speed = -this.speed / Mathf.Pow(2f, (float)(2 - this.bounceCount));
				this.layoutElement.minWidth = -this.speed;
				return;
			}
			this.layoutElement.minWidth = 0f;
			base.enabled = false;
		}
	}

	private const float START_SPEED = 1f;

	private const float ACCELERATION = 0.5f;

	private const float BOUNCE_DAMPEN = 2f;

	private const int BOUNCE_COUNT = 2;

	private const float OFFSETX = 100f;

	private float speed = 1f;

	private int bounceCount = 2;

	private LayoutElement layoutElement;
}
