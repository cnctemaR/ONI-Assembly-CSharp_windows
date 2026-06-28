using System;
using UnityEngine;

public class HealthBar : ProgressBar
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
	}

	public void OnChange()
	{
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

	public override void OnOverlayChanged(object data = null)
	{
		if ((SimViewMode)data == SimViewMode.None)
		{
			if (!base.gameObject.activeSelf && this.showTimer != 0f)
			{
				base.gameObject.SetActive(true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	private float showTimer = 0f;

	private float maxShowTime = 3f;
}
