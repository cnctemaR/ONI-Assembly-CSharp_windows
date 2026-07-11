using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NextUpdateTimer")]
public class NextUpdateTimer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.initialAnimScale = this.UpdateAnimController.animScale;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshReleaseTimes();
	}

	public void UpdateReleaseTimes(string lastUpdateTime, string nextUpdateTime, string textOverride)
	{
		if (!global::System.DateTime.TryParse(lastUpdateTime, out this.currentReleaseDate))
		{
			global::Debug.LogWarning("Failed to parse last_update_time: " + lastUpdateTime);
		}
		if (!global::System.DateTime.TryParse(nextUpdateTime, out this.nextReleaseDate))
		{
			global::Debug.LogWarning("Failed to parse next_update_time: " + nextUpdateTime);
		}
		this.m_releaseTextOverride = textOverride;
		this.RefreshReleaseTimes();
	}

	private void RefreshReleaseTimes()
	{
		TimeSpan timeSpan = this.nextReleaseDate - this.currentReleaseDate;
		TimeSpan timeSpan2 = this.nextReleaseDate - global::System.DateTime.UtcNow;
		TimeSpan timeSpan3 = global::System.DateTime.UtcNow - this.currentReleaseDate;
		string text = "4";
		string text2;
		if (!string.IsNullOrEmpty(this.m_releaseTextOverride))
		{
			text2 = this.m_releaseTextOverride;
		}
		else if (timeSpan2.TotalHours < 8.0)
		{
			text2 = UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
			text = "4";
		}
		else if (timeSpan2.TotalDays < 1.0)
		{
			text2 = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, 1);
			text = "3";
		}
		else
		{
			int num = timeSpan2.Days % 7;
			int num2 = (timeSpan2.Days - num) / 7;
			if (num2 <= 0)
			{
				text2 = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, num);
				text = "2";
			}
			else
			{
				text2 = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, num, num2);
				text = "1";
			}
		}
		this.TimerText.text = text2;
		this.UpdateAnimController.Play(text, KAnim.PlayMode.Loop, 1f, 0f);
		float num3 = Mathf.Clamp01((float)(timeSpan3.TotalSeconds / timeSpan.TotalSeconds));
		this.UpdateAnimMeterController.SetPositionPercent(num3);
	}

	public LocText TimerText;

	public KBatchedAnimController UpdateAnimController;

	public KBatchedAnimController UpdateAnimMeterController;

	public float initialAnimScale;

	public global::System.DateTime nextReleaseDate;

	public global::System.DateTime currentReleaseDate;

	private string m_releaseTextOverride;
}
