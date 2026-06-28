using System;
using KSerialization;

public class EODReportMessage : Message
{
	public EODReportMessage(string title, string tooltip)
	{
		this.day = GameUtil.GetCurrentCycle();
		this.title = title;
		this.tooltip = tooltip;
	}

	public EODReportMessage()
	{
	}

	public override string GetSound()
	{
		return null;
	}

	public override string GetMessageBody()
	{
		return string.Empty;
	}

	public override string GetTooltip()
	{
		return this.tooltip;
	}

	public override string GetTitle()
	{
		return this.title;
	}

	public override Message.clickFn OnClick
	{
		get
		{
			return delegate
			{
				this.OpenReport();
			};
		}
	}

	public void OpenReport()
	{
		ManagementMenu.Instance.OpenReports(this.day);
	}

	[Serialize]
	private int day;

	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;
}
