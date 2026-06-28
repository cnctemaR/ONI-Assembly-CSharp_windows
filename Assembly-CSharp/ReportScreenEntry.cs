using System;
using System.Collections.Generic;
using UnityEngine;

public class ReportScreenEntry : KMonoBehaviour
{
	public void SetMainEntry(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		if (this.mainRow == null)
		{
			this.mainRow = Util.KInstantiateUI(this.rowTemplate.gameObject, base.gameObject, true).GetComponent<ReportScreenEntryRow>();
			MultiToggle toggle = this.mainRow.toggle;
			toggle.onClick = (global::System.Action)Delegate.Combine(toggle.onClick, new global::System.Action(this.ToggleContext));
		}
		this.mainRow.SetLine(entry, reportGroup);
		List<ReportManager.ReportEntry> contextEntries = entry.GetContextEntries();
		this.currentContextCount = contextEntries.Count;
		for (int i = 0; i < contextEntries.Count; i++)
		{
			if (i >= this.contextRows.Count)
			{
				ReportScreenEntryRow component = Util.KInstantiateUI(this.rowTemplate.gameObject, base.gameObject, false).GetComponent<ReportScreenEntryRow>();
				this.contextRows.Add(component);
			}
			this.contextRows[i].SetLine(contextEntries[i], reportGroup);
		}
		this.UpdateVisibility();
	}

	private void ToggleContext()
	{
		this.mainRow.toggle.NextState();
		this.UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		int i;
		for (i = 0; i < this.currentContextCount; i++)
		{
			this.contextRows[i].gameObject.SetActive(this.mainRow.toggle.CurrentState == 1);
		}
		while (i < this.contextRows.Count)
		{
			this.contextRows[i].gameObject.SetActive(false);
			i++;
		}
	}

	[SerializeField]
	private ReportScreenEntryRow rowTemplate;

	private ReportScreenEntryRow mainRow;

	private List<ReportScreenEntryRow> contextRows = new List<ReportScreenEntryRow>();

	private int currentContextCount;
}
