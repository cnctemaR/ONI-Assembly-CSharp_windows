using System;
using UnityEngine;

public class HealthBar : ProgressBar
{
	private bool ShouldShow
	{
		get
		{
			return this.showTimer > 0f || base.PercentFull < this.alwaysShowThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
		base.gameObject.SetActive(this.ShouldShow);
	}

	public void OnChange()
	{
		base.enabled = true;
		this.showTimer = this.maxShowTime;
	}

	public override void Update()
	{
		base.Update();
		if (Time.timeScale > 0f)
		{
			this.showTimer = Mathf.Max(0f, this.showTimer - Time.unscaledDeltaTime);
		}
		if (!this.ShouldShow)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	public override void OnOverlayChanged(object data = null)
	{
		if (!this.autoHide)
		{
			return;
		}
		if ((HashedString)data == OverlayModes.None.ID)
		{
			if (!base.gameObject.activeSelf && this.ShouldShow)
			{
				base.enabled = true;
				base.gameObject.SetActive(true);
				return;
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.enabled = false;
			base.gameObject.SetActive(false);
		}
	}

	private float showTimer;

	private float maxShowTime = 10f;

	private float alwaysShowThreshold = 0.8f;
}
