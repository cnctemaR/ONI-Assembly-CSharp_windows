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
		this.nextButton.isInteractable = flag;
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
		this.prevButton.isInteractable = flag;
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
		foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> keyValuePair in ReportManager.Instance.ReportGroups)
		{
			ReportManager.ReportEntry entry = this.currentReport.GetEntry(keyValuePair.Key);
			if (keyValuePair.Value.reportIfZero || entry.accumulate != 0f)
			{
				if (num != keyValuePair.Value.group)
				{
					num = keyValuePair.Value.group;
					this.AddSpacer(num);
				}
				this.AddLine(entry, keyValuePair.Value);
			}
		}
	}

	public void ShowReport(int day)
	{
		this.currentReport = ReportManager.Instance.FindReport(day);
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
		GameObject gameObject;
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
		ReportScreenEntry component = gameObject.GetComponent<ReportScreenEntry>();
		component.SetMainEntry(entry, reportGroup);
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
