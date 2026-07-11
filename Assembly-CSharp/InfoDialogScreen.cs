using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class InfoDialogScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(false);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!this.escapeCloses)
		{
			e.TryConsume(global::Action.Escape);
			return;
		}
		if (e.TryConsume(global::Action.Escape))
		{
			this.Deactivate();
			return;
		}
		if (PlayerController.Instance != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.Deactivate();
			return;
		}
		base.OnKeyDown(e);
	}

	public InfoDialogScreen AddDefaultOK(bool escapeCloses = false)
	{
		this.AddOption(UI.CONFIRMDIALOG.OK, delegate(InfoDialogScreen d)
		{
			d.Deactivate();
		}, true);
		this.escapeCloses = escapeCloses;
		return this;
	}

	public InfoDialogScreen AddDefaultCancel()
	{
		this.AddOption(UI.CONFIRMDIALOG.CANCEL, delegate(InfoDialogScreen d)
		{
			d.Deactivate();
		}, false);
		this.escapeCloses = true;
		return this;
	}

	public InfoDialogScreen AddOption(string text, Action<InfoDialogScreen> action, bool rightSide = false)
	{
		GameObject gameObject = Util.KInstantiateUI(rightSide ? this.rightButtonPrefab : this.leftButtonPrefab, rightSide ? this.rightButtonPanel : this.leftButtonPanel, true);
		gameObject.gameObject.GetComponentInChildren<LocText>().text = text;
		gameObject.gameObject.GetComponent<KButton>().onClick += delegate
		{
			action(this);
		};
		return this;
	}

	public InfoDialogScreen SetHeader(string header)
	{
		this.header.text = header;
		return this;
	}

	public InfoDialogScreen AddSprite(Sprite sprite)
	{
		Util.KInstantiateUI<InfoScreenSpriteItem>(this.spriteItemTemplate.gameObject, this.contentContainer, false).SetSprite(sprite);
		return this;
	}

	public InfoDialogScreen AddPlainText(string text)
	{
		Util.KInstantiateUI<InfoScreenPlainText>(this.plainTextTemplate.gameObject, this.contentContainer, false).SetText(text);
		return this;
	}

	public InfoDialogScreen AddLineItem(string text, string tooltip)
	{
		InfoScreenLineItem infoScreenLineItem = Util.KInstantiateUI<InfoScreenLineItem>(this.lineItemTemplate.gameObject, this.contentContainer, false);
		infoScreenLineItem.SetText(text);
		infoScreenLineItem.SetTooltip(tooltip);
		return this;
	}

	public InfoDialogScreen AddSubHeader(string text)
	{
		Util.KInstantiateUI<InfoScreenPlainText>(this.subHeaderTemplate.gameObject, this.contentContainer, false).SetText(text);
		return this;
	}

	public InfoDialogScreen AddDescriptors(List<Descriptor> descriptors)
	{
		for (int i = 0; i < descriptors.Count; i++)
		{
			this.AddLineItem(descriptors[i].IndentedText(), descriptors[i].tooltipText);
		}
		return this;
	}

	[SerializeField]
	private InfoScreenPlainText subHeaderTemplate;

	[SerializeField]
	private InfoScreenPlainText plainTextTemplate;

	[SerializeField]
	private InfoScreenLineItem lineItemTemplate;

	[SerializeField]
	private InfoScreenSpriteItem spriteItemTemplate;

	[Space(10f)]
	[SerializeField]
	private LocText header;

	[SerializeField]
	private GameObject contentContainer;

	[SerializeField]
	private GameObject leftButtonPrefab;

	[SerializeField]
	private GameObject rightButtonPrefab;

	[SerializeField]
	private GameObject leftButtonPanel;

	[SerializeField]
	private GameObject rightButtonPanel;

	private bool escapeCloses;
}
