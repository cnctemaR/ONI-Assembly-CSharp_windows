using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ReportScreenEntryRow : KMonoBehaviour
{
	public void SetLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		if (entry.context == null)
		{
			if (entry.HasContextEntries())
			{
				this.toggle.gameObject.SetActive(true);
				this.spacer.minWidth = this.groupSpacerWidth;
			}
			else
			{
				this.toggle.gameObject.SetActive(false);
				this.spacer.minWidth = this.groupSpacerWidth + this.toggle.GetComponent<LayoutElement>().minWidth;
			}
			this.name.text = reportGroup.stringKey;
		}
		else
		{
			this.toggle.gameObject.SetActive(false);
			this.spacer.minWidth = this.contextSpacerWidth;
			this.name.text = entry.context;
		}
		string text = "";
		foreach (KeyValuePair<string, float> keyValuePair in entry.posNotes)
		{
			text = string.Format(UI.ENDOFDAYREPORT.NOTES.NOTE_ENTRY_LINE_ITEM, text, keyValuePair.Key, reportGroup.formatfn(keyValuePair.Value));
		}
		this.added.text = reportGroup.formatfn(entry.Positive);
		string text2 = string.Format(reportGroup.positiveTooltip + "\n" + text, reportGroup.formatfn(entry.Positive));
		this.added.GetComponent<ToolTip>().toolTip = text2;
		string text3 = "";
		foreach (KeyValuePair<string, float> keyValuePair2 in entry.negNotes)
		{
			text3 = string.Format(UI.ENDOFDAYREPORT.NOTES.NOTE_ENTRY_LINE_ITEM, text3, keyValuePair2.Key, reportGroup.formatfn(keyValuePair2.Value));
		}
		this.removed.text = reportGroup.formatfn(entry.Negative);
		string text4 = string.Format(reportGroup.negativeTooltip + "\n" + text3, reportGroup.formatfn(entry.Negative));
		this.removed.GetComponent<ToolTip>().toolTip = text4;
		string text5 = ((entry.Positive < entry.Negative) ? text4 : text2);
		this.net.text = ((reportGroup.formatfn != null) ? reportGroup.formatfn(entry.Net) : entry.Net.ToString());
		this.net.GetComponent<ToolTip>().toolTip = text5;
		this.name.GetComponent<ToolTip>().toolTip = text5;
	}

	[SerializeField]
	private new LocText name;

	[SerializeField]
	private LocText added;

	[SerializeField]
	private LocText removed;

	[SerializeField]
	private LocText net;

	[SerializeField]
	public MultiToggle toggle;

	[SerializeField]
	private LayoutElement spacer;

	public float groupSpacerWidth;

	public float contextSpacerWidth;
}
