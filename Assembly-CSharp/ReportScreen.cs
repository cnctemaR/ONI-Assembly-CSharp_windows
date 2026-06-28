using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ReportScreen : KScreen
{
	public static ReportScreen Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ReportScreen.Instance = this;
		this.prevButton.onClick += delegate
		{
			this.ShowReport(this.currentReport.day - 1);
		};
		this.nextButton.onClick += delegate
		{
			this.ShowReport(this.currentReport.day + 1);
		};
		this.ConsumeMouseScroll = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnShow(bool bShow)
	{
		base.OnShow(bShow);
		if (ReportManager.Instance != null)
		{
			this.currentReport = ReportManager.Instance.TodaysReport;
		}
	}

	public void SetTitle(string title)
	{
		this.title.text = title;
	}

	public override void ScreenUpdate(bool b)
	{
		base.ScreenUpdate(b);
		this.Refresh();
	}

	private void Refresh()
	{
		Debug.Assert(this.currentReport != null);
		this.ClearLineItems();
		if (this.currentReport.day == ReportManager.Instance.TodaysReport.day)
		{
			this.SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE_TODAY, this.currentReport.day));
		}
		else if (this.currentReport.day == ReportManager.Instance.TodaysReport.day - 1)
		{
			this.SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE_YESTERDAY, this.currentReport.day));
		}
		else
		{
			this.SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, this.currentReport.day));
		}
		bool flag = this.currentReport.day < ReportManager.Instance.TodaysReport.day;
		this.nextButton.interactable = flag;
		if (flag)
		{
			this.nextButton.GetComponent<ToolTip>().toolTip = string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, this.currentReport.day + 1);
			this.nextButton.GetComponent<ToolTip>().enabled = true;
		}
		else
		{
			this.nextButton.GetComponent<ToolTip>().enabled = false;
		}
		flag = this.currentReport.day > 1;
		this.prevButton.interactable = flag;
		if (flag)
		{
			this.prevButton.GetComponent<ToolTip>().toolTip = string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, this.currentReport.day - 1);
			this.prevButton.GetComponent<ToolTip>().enabled = true;
		}
		else
		{
			this.prevButton.GetComponent<ToolTip>().enabled = false;
		}
		this.AddSpacer(0);
		int num = 1;
		foreach (ReportManager.ReportEntry reportEntry in this.currentReport.reportEntries)
		{
			if (ReportManager.Instance.ReportGroups.ContainsKey((ReportManager.ReportType)reportEntry.gameHash))
			{
				ReportManager.ReportGroup reportGroup = ReportManager.Instance.ReportGroups[(ReportManager.ReportType)reportEntry.gameHash];
				if (num != reportGroup.group)
				{
					num = reportGroup.group;
					this.AddSpacer(num);
				}
				if (reportGroup.reportIfZero || reportEntry.accumulate != 0f)
				{
					this.AddLine(reportEntry, reportGroup);
				}
			}
		}
	}

	public void ShowReport(int day)
	{
		this.currentReport = ReportManager.Instance.FindReport(day);
		Debug.Assert(this.currentReport != null, "Can't find report for day: " + day.ToString());
		this.Refresh();
	}

	private GameObject AddSpacer(int group)
	{
		GameObject gameObject;
		if (this.lineItems.ContainsKey(group.ToString()))
		{
			gameObject = this.lineItems[group.ToString()];
		}
		else
		{
			gameObject = Util.KInstantiateUI(this.lineItemSpacer, this.contentFolder, false);
			gameObject.name = "Spacer" + group.ToString();
			this.lineItems[group.ToString()] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private GameObject AddLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		float num = Mathf.Abs(entry.Negative);
		GameObject gameObject = null;
		if (this.lineItems.ContainsKey(reportGroup.stringKey))
		{
			gameObject = this.lineItems[reportGroup.stringKey];
		}
		else
		{
			gameObject = Util.KInstantiateUI(this.lineItem, this.contentFolder, true);
			gameObject.name = "LineItem" + this.lineItems.Count;
			this.lineItems[reportGroup.stringKey] = gameObject;
		}
		gameObject.SetActive(true);
		LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>();
		componentsInChildren[0].text = reportGroup.stringKey;
		string text = string.Empty;
		foreach (KeyValuePair<string, float> keyValuePair in entry.posNotes)
		{
			text = string.Format("{0}\n{1} : {2}", text, keyValuePair.Key, keyValuePair.Value);
		}
		componentsInChildren[1].text = reportGroup.formatfn(entry.Positive);
		string text2 = string.Format(reportGroup.positiveTooltip + "\n" + text, reportGroup.formatfn(entry.Positive));
		componentsInChildren[1].GetComponent<ToolTip>().toolTip = text2;
		string text3 = string.Empty;
		foreach (KeyValuePair<string, float> keyValuePair2 in entry.negNotes)
		{
			text3 = string.Format("{0}\n{1} : {2}", text3, keyValuePair2.Key, keyValuePair2.Value);
		}
		componentsInChildren[2].text = reportGroup.formatfn(num);
		string text4 = string.Format(reportGroup.negativeTooltip + "\n" + text3, reportGroup.formatfn(num));
		componentsInChildren[2].GetComponent<ToolTip>().toolTip = text4;
		string text5 = ((entry.Positive < num) ? text4 : text2);
		componentsInChildren[3].text = ((reportGroup.formatfn != null) ? reportGroup.formatfn(entry.Net) : entry.Net.ToString());
		componentsInChildren[3].GetComponent<ToolTip>().toolTip = text5;
		componentsInChildren[0].GetComponent<ToolTip>().toolTip = text5;
		return gameObject;
	}

	private void ClearLineItems()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.lineItems)
		{
			keyValuePair.Value.SetActive(false);
		}
	}

	private void OnClickClose()
	{
		base.PlaySound3D(GlobalAssets.GetSound("HUD_Click_Close", false));
		base.Show(false);
	}

	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton prevButton;

	[SerializeField]
	private KButton nextButton;

	[SerializeField]
	private GameObject lineItem;

	[SerializeField]
	private GameObject lineItemSpacer;

	[SerializeField]
	private GameObject contentFolder;

	private Dictionary<string, GameObject> lineItems = new Dictionary<string, GameObject>();

	private ReportManager.DailyReport currentReport;
}
