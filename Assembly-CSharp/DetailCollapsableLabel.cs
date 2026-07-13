using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailCollapsableLabel : MonoBehaviour
{
	public bool IsExpanded
	{
		get
		{
			return this.toggle.isOn;
		}
	}

	public object Data
	{
		get
		{
			return this.data;
		}
	}

	private void OnDisable()
	{
		this.MarkAllRowsUnused();
		this.RefreshRowVisibilityState();
		this.toggle.SetIsOnWithoutNotify(false);
		this.RefreshArrowIcon();
	}

	public void SetData(object data)
	{
		this.data = data;
	}

	public void ClearToggleCallbacks()
	{
		this.toggle.ClearOnValueChanged();
		this.toggle.onValueChanged += this.OnToggleChanged;
		this.OnExpanded = null;
		this.OnCollapsed = null;
	}

	public void MarkAllRowsUnused()
	{
		this.lastKnownRowAvailable = 0;
		foreach (DetailCollapsableLabel.ContentRow contentRow in this.contentRows)
		{
			contentRow.inUse = false;
		}
	}

	public void RefreshRowVisibilityState()
	{
		foreach (DetailCollapsableLabel.ContentRow contentRow in this.contentRows)
		{
			if (contentRow.label.gameObject.activeInHierarchy != contentRow.inUse)
			{
				contentRow.label.gameObject.SetActive(contentRow.inUse);
			}
		}
	}

	public DetailLabelWithButton AddOrGetAvailableContentRow()
	{
		DetailCollapsableLabel.ContentRow contentRow = ((this.lastKnownRowAvailable < this.contentRows.Count && !this.contentRows[this.lastKnownRowAvailable].inUse) ? this.contentRows[this.lastKnownRowAvailable] : null);
		int siblingIndex = base.transform.GetSiblingIndex();
		if (contentRow == null)
		{
			contentRow = new DetailCollapsableLabel.ContentRow
			{
				label = Util.KInstantiateUI(this.contentRowPrefab, base.transform.parent.gameObject, false).GetComponent<DetailLabelWithButton>()
			};
			this.contentRows.Add(contentRow);
		}
		contentRow.inUse = true;
		contentRow.label.transform.SetSiblingIndex(siblingIndex + 1);
		this.lastKnownRowAvailable++;
		return contentRow.label;
	}

	private void OnToggleChanged(bool expanded)
	{
		this.RefreshArrowIcon();
		if (expanded)
		{
			if (this.OnExpanded != null)
			{
				this.OnExpanded(this);
				return;
			}
		}
		else if (this.OnCollapsed != null)
		{
			this.OnCollapsed(this);
		}
	}

	public void ManualTriggerOnExpanded()
	{
		if (this.OnExpanded != null)
		{
			this.OnExpanded(this);
		}
	}

	private void RefreshArrowIcon()
	{
		this.arrowImage.sprite = (this.toggle.isOn ? this.unfoldedIcon : this.collapsedIcon);
		this.arrowImage.enabled = false;
		this.arrowImage.enabled = true;
	}

	public Image arrowImage;

	public LocText nameLabel;

	public LocText valueLabel;

	public ToolTip toolTip;

	public KToggle toggle;

	[SerializeField]
	private GameObject contentRowPrefab;

	[Header("Icons")]
	public Sprite collapsedIcon;

	public Sprite unfoldedIcon;

	public Action<DetailCollapsableLabel> OnExpanded;

	public Action<DetailCollapsableLabel> OnCollapsed;

	private int lastKnownRowAvailable;

	public List<DetailCollapsableLabel.ContentRow> contentRows = new List<DetailCollapsableLabel.ContentRow>();

	private object data;

	public class ContentRow
	{
		public bool inUse;

		public DetailLabelWithButton label;
	}
}
