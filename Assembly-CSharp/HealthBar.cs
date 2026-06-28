using System;
using UnityEngine;

public class HealthBar : ProgressBar
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
		base.gameObject.SetActive(this.showTimer > 0f);
	}

	public void OnChange()
	{
		base.enabled = true;
		base.gameObject.SetActive(true);
		this.showTimer = this.maxShowTime;
	}

	public override void Update()
	{
		base.Update();
		if (Time.timeScale > 0f)
		{
			this.showTimer = Mathf.Max(0f, this.showTimer - Time.unscaledDeltaTime);
		}
		if (this.showTimer == 0f)
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
		if ((SimViewMode)data == SimViewMode.None)
		{
			if (!base.gameObject.activeSelf && this.showTimer != 0f)
			{
				base.enabled = true;
				base.gameObject.SetActive(true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.enabled = false;
			base.gameObject.SetActive(false);
		}
	}

	private float showTimer;

	private float maxShowTime = 3f;
}
