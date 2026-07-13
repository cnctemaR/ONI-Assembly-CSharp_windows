using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CollapsibleDetailContentPanel")]
public class CollapsibleDetailContentPanel : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.collapseButton;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.ToggleOpen));
		this.ArrowIcon.SetActive();
		this.log = new LoggerFSS("detailpanel", 35);
		this.labels = new Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabel>>();
		this.buttonLabels = new Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabelWithButton>>();
		this.collapsableButtonLabels = new Dictionary<string, CollapsibleDetailContentPanel.Label<DetailCollapsableLabel>>();
		this.Commit();
	}

	public void SetTitle(string title)
	{
		this.HeaderLabel.text = title;
	}

	public void Commit()
	{
		int num = 0;
		foreach (CollapsibleDetailContentPanel.Label<DetailLabel> label in this.labels.Values)
		{
			if (label.used)
			{
				num++;
				if (!label.obj.gameObject.activeSelf)
				{
					label.obj.gameObject.SetActive(true);
				}
			}
			else if (!label.used && label.obj.gameObject.activeSelf)
			{
				label.obj.gameObject.SetActive(false);
			}
			label.used = false;
		}
		foreach (CollapsibleDetailContentPanel.Label<DetailLabelWithButton> label2 in this.buttonLabels.Values)
		{
			if (label2.used)
			{
				num++;
				if (!label2.obj.gameObject.activeSelf)
				{
					label2.obj.gameObject.SetActive(true);
				}
			}
			else if (!label2.used && label2.obj.gameObject.activeSelf)
			{
				label2.obj.gameObject.SetActive(false);
			}
			label2.used = false;
		}
		foreach (CollapsibleDetailContentPanel.Label<DetailCollapsableLabel> label3 in this.collapsableButtonLabels.Values)
		{
			if (label3.used)
			{
				num++;
				if (!label3.obj.gameObject.activeSelf)
				{
					label3.obj.gameObject.SetActive(true);
				}
			}
			else if (!label3.used && label3.obj.gameObject.activeSelf)
			{
				label3.obj.gameObject.SetActive(false);
			}
			label3.used = false;
		}
		if (base.gameObject.activeSelf && num == 0)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!base.gameObject.activeSelf && num > 0)
		{
			base.gameObject.SetActive(true);
		}
	}

	public void SetLabel(string id, string text, string tooltip)
	{
		CollapsibleDetailContentPanel.Label<DetailLabel> label;
		if (!this.labels.TryGetValue(id, out label))
		{
			label = new CollapsibleDetailContentPanel.Label<DetailLabel>
			{
				used = true,
				obj = Util.KInstantiateUI(this.labelTemplate.gameObject, this.Content.gameObject, false).GetComponent<DetailLabel>()
			};
			label.obj.gameObject.name = id;
			this.labels[id] = label;
		}
		label.obj.label.AllowLinks = true;
		label.obj.label.text = text;
		label.obj.toolTip.toolTip = tooltip;
		label.used = true;
	}

	public DetailLabelWithButton SetLabelWithButton(string id, string text, string tooltip, global::System.Action buttonCb)
	{
		return this.SetLabelWithButton(id, text, null, null, tooltip, buttonCb);
	}

	public DetailLabelWithButton SetLabelWithButton(string id, string mainText, string secondaryText, string thirdText, string tooltip, global::System.Action buttonCb)
	{
		CollapsibleDetailContentPanel.Label<DetailLabelWithButton> label;
		if (!this.buttonLabels.TryGetValue(id, out label))
		{
			label = new CollapsibleDetailContentPanel.Label<DetailLabelWithButton>
			{
				used = true,
				obj = Util.KInstantiateUI(this.labelWithActionButtonTemplate.gameObject, this.Content.gameObject, false).GetComponent<DetailLabelWithButton>()
			};
			label.obj.gameObject.name = id;
			this.buttonLabels[id] = label;
		}
		label.obj.label.AllowLinks = false;
		label.obj.label.raycastTarget = false;
		label.obj.label.text = mainText;
		label.obj.label2.AllowLinks = false;
		label.obj.label2.raycastTarget = false;
		label.obj.label2.text = secondaryText;
		label.obj.label3.AllowLinks = false;
		label.obj.label3.raycastTarget = false;
		label.obj.label3.text = thirdText;
		label.obj.RefreshLabelsVisibility();
		label.obj.toolTip.toolTip = tooltip;
		label.obj.button.ClearOnClick();
		label.obj.button.onClick += buttonCb;
		label.used = true;
		return label.obj;
	}

	public DetailCollapsableLabel SetCollapsableLabel(string id, string text, string valueText, string tooltip, object data, Action<DetailCollapsableLabel> onExpanded, Action<DetailCollapsableLabel> onCollapsed)
	{
		CollapsibleDetailContentPanel.Label<DetailCollapsableLabel> label;
		if (!this.collapsableButtonLabels.TryGetValue(id, out label))
		{
			label = new CollapsibleDetailContentPanel.Label<DetailCollapsableLabel>
			{
				used = true,
				obj = Util.KInstantiateUI(this.labelWithCollapsableToggleTemplate.gameObject, this.Content.gameObject, false).GetComponent<DetailCollapsableLabel>()
			};
			label.obj.gameObject.name = id;
			this.collapsableButtonLabels[id] = label;
		}
		label.obj.nameLabel.AllowLinks = false;
		label.obj.nameLabel.raycastTarget = false;
		label.obj.nameLabel.SetText(text);
		label.obj.valueLabel.SetText(valueText);
		label.obj.toolTip.toolTip = tooltip;
		label.obj.ClearToggleCallbacks();
		DetailCollapsableLabel obj = label.obj;
		obj.OnCollapsed = (Action<DetailCollapsableLabel>)Delegate.Combine(obj.OnCollapsed, onCollapsed);
		DetailCollapsableLabel obj2 = label.obj;
		obj2.OnExpanded = (Action<DetailCollapsableLabel>)Delegate.Combine(obj2.OnExpanded, onExpanded);
		label.used = true;
		label.obj.SetData(data);
		if (label.obj.IsExpanded)
		{
			label.obj.ManualTriggerOnExpanded();
		}
		return label.obj;
	}

	private void ToggleOpen()
	{
		bool flag = this.scalerMask.gameObject.activeSelf;
		flag = !flag;
		this.scalerMask.gameObject.SetActive(flag);
		if (flag)
		{
			this.ArrowIcon.SetActive();
			this.ForceLocTextsMeshRebuild();
			return;
		}
		this.ArrowIcon.SetInactive();
	}

	public void ForceLocTextsMeshRebuild()
	{
		LocText[] componentsInChildren = base.GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ForceMeshUpdate(false, false);
		}
	}

	public void SetActive(bool active)
	{
		if (base.gameObject.activeSelf != active)
		{
			base.gameObject.SetActive(active);
		}
	}

	public ImageToggleState ArrowIcon;

	public LocText HeaderLabel;

	public MultiToggle collapseButton;

	public Transform Content;

	public ScalerMask scalerMask;

	[Space(10f)]
	public DetailLabel labelTemplate;

	public DetailLabelWithButton labelWithActionButtonTemplate;

	public DetailCollapsableLabel labelWithCollapsableToggleTemplate;

	private Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabel>> labels;

	private Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabelWithButton>> buttonLabels;

	private Dictionary<string, CollapsibleDetailContentPanel.Label<DetailCollapsableLabel>> collapsableButtonLabels;

	private LoggerFSS log;

	private class Label<T>
	{
		public T obj;

		public bool used;
	}
}
