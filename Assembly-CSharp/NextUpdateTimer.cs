using System;
using STRINGS;

public class NextUpdateTimer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.nextReleaseDate = new global::System.DateTime(2017, 12, 14, 17, 0, 0, DateTimeKind.Utc);
		this.currentReleaseDate = new global::System.DateTime(2017, 11, 16, 17, 0, 0, DateTimeKind.Utc);
		this.initialAnimScale = this.UpdateAnimController.animScale;
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.RefreshScale));
	}

	protected override void OnCleanUp()
	{
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Remove(instance.OnResize, new global::System.Action(this.RefreshScale));
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TimeSpan timeSpan = this.nextReleaseDate - this.currentReleaseDate;
		TimeSpan timeSpan2 = this.nextReleaseDate - global::System.DateTime.UtcNow;
		TimeSpan timeSpan3 = global::System.DateTime.UtcNow - this.currentReleaseDate;
		string text = string.Empty;
		string text2;
		if (timeSpan2.TotalHours < 8.0)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.NOW;
			text2 = "4";
		}
		else if (timeSpan2.TotalDays < 1.0)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
			text2 = "3";
		}
		else
		{
			int num = timeSpan2.Days % 7;
			int num2 = (timeSpan2.Days - num) / 7;
			if (num2 <= 0)
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, num);
				text2 = "2";
			}
			else
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, num, num2);
				text2 = "1";
			}
		}
		this.TimerText.text = text;
		this.UpdateAnimController.Play(text2, KAnim.PlayMode.Loop, 1f, 0f);
		double num3 = timeSpan3.TotalSeconds / timeSpan.TotalSeconds;
		this.UpdateAnimMeterController.SetPositionPercent((float)num3);
	}

	private void RefreshScale()
	{
		float canvasScale = base.GetComponentInParent<KCanvasScaler>().GetCanvasScale();
		if (this.UpdateAnimController != null)
		{
			this.UpdateAnimController.animScale = this.initialAnimScale * (1f / canvasScale);
		}
		if (this.UpdateAnimMeterController != null)
		{
			this.UpdateAnimMeterController.animScale = this.initialAnimScale * (1f / canvasScale);
		}
	}

	public LocText TimerText;

	public KBatchedAnimController UpdateAnimController;

	public KBatchedAnimController UpdateAnimMeterController;

	public float initialAnimScale;

	public global::System.DateTime nextReleaseDate;

	public global::System.DateTime currentReleaseDate;
}
