using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakdownList : KMonoBehaviour
{
	public BreakdownListRow AddRow()
	{
		BreakdownListRow breakdownListRow;
		if (this.unusedListRows.Count > 0)
		{
			breakdownListRow = this.unusedListRows[0];
			this.unusedListRows.RemoveAt(0);
		}
		else
		{
			breakdownListRow = global::UnityEngine.Object.Instantiate<BreakdownListRow>(this.listRowTemplate);
		}
		breakdownListRow.gameObject.transform.SetParent(base.transform);
		breakdownListRow.gameObject.transform.SetAsLastSibling();
		this.listRows.Add(breakdownListRow);
		breakdownListRow.gameObject.SetActive(true);
		return breakdownListRow;
	}

	public GameObject AddCustomRow(GameObject newRow)
	{
		newRow.transform.SetParent(base.transform);
		newRow.gameObject.transform.SetAsLastSibling();
		this.customRows.Add(newRow);
		newRow.SetActive(true);
		return newRow;
	}

	public void ClearRows()
	{
		foreach (BreakdownListRow breakdownListRow in this.listRows)
		{
			this.unusedListRows.Add(breakdownListRow);
			breakdownListRow.gameObject.SetActive(false);
			breakdownListRow.ClearTooltip();
		}
		this.listRows.Clear();
		foreach (GameObject gameObject in this.customRows)
		{
			gameObject.SetActive(false);
		}
	}

	public void SetTitle(string title)
	{
		this.headerTitle.text = title;
	}

	public void SetDescription(string description)
	{
		if (description != null && description.Length >= 0)
		{
			this.infoTextLabel.gameObject.SetActive(true);
			this.infoTextLabel.text = description;
			return;
		}
		this.infoTextLabel.gameObject.SetActive(false);
	}

	public void SetIcon(Sprite icon)
	{
		this.headerIcon.sprite = icon;
	}

	public Image headerIcon;

	public Sprite headerIconSprite;

	public Image headerBar;

	public LocText headerTitle;

	public LocText headerValue;

	public LocText infoTextLabel;

	public BreakdownListRow listRowTemplate;

	private List<BreakdownListRow> listRows = new List<BreakdownListRow>();

	private List<BreakdownListRow> unusedListRows = new List<BreakdownListRow>();

	private List<GameObject> customRows = new List<GameObject>();
}
