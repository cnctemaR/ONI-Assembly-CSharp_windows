using System;
using UnityEngine;
using UnityEngine.UI;

public class NotificationAnimator : MonoBehaviour
{
	public void Begin(bool startOffset = true)
	{
		this.Reset();
		this.animating = true;
		if (startOffset)
		{
			this.layoutElement.minWidth = 100f;
			return;
		}
		this.layoutElement.minWidth = 1f;
		this.speed = -10f;
	}

	private void Reset()
	{
		this.bounceCount = 2;
		this.layoutElement = base.GetComponent<LayoutElement>();
		this.layoutElement.minWidth = 0f;
		this.speed = 1f;
	}

	public void Stop()
	{
		this.Reset();
		this.animating = false;
	}

	private void LateUpdate()
	{
		if (!this.animating)
		{
			return;
		}
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
			this.Stop();
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

	[SerializeField]
	private bool animating = true;
}
