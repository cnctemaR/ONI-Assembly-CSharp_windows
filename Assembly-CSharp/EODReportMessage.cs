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
		return "";
	}

	public override string GetTooltip()
	{
		return this.tooltip;
	}

	public override string GetTitle()
	{
		return this.title;
	}

	public void OpenReport()
	{
		ManagementMenu.Instance.OpenReports(this.day);
	}

	public override bool ShowDialog()
	{
		return false;
	}

	public override void OnClick()
	{
		this.OpenReport();
	}

	[Serialize]
	private int day;

	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;
}
