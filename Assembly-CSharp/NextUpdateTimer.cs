using System;
using STRINGS;

public class NextUpdateTimer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.nextReleaseDate = new global::System.DateTime(2017, 10, 5, 17, 0, 0, DateTimeKind.Utc);
	}

	private void Update()
	{
		TimeSpan timeSpan = this.nextReleaseDate - global::System.DateTime.UtcNow;
		string text = string.Empty;
		if (timeSpan.TotalHours < 8.0)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.NOW;
		}
		else if (timeSpan.TotalDays < 1.0)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
		}
		else
		{
			int num = timeSpan.Days % 7;
			int num2 = (timeSpan.Days - num) / 7;
			if (num2 <= 0)
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, num);
			}
			else
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, num, num2);
			}
		}
		this.TimerText.text = text;
	}

	public LocText TimerText;

	public global::System.DateTime nextReleaseDate;
}
