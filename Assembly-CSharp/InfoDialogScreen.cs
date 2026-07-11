using System;
using System.Collections.Generic;
using UnityEngine;

public class InfoDialogScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(false);
		this.confirmButton.GetComponent<KButton>().onClick += this.OnSelect_OK;
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			this.OnSelect_OK();
			return;
		}
		if (PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.OnSelect_OK();
			return;
		}
		base.OnKeyDown(e);
	}

	public void AddOption(string text, Action<InfoDialogScreen> action)
	{
		GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, this.buttonPanel, true);
		gameObject.gameObject.GetComponentInChildren<LocText>().text = text;
		gameObject.gameObject.GetComponent<KButton>().onClick += delegate
		{
			action(this);
		};
	}

	public InfoDialogScreen SetHeader(string header)
	{
		this.header.text = header;
		return this;
	}

	public InfoDialogScreen AddPlainText(string text)
	{
		Util.KInstantiateUI(this.plainTextTemplate.gameObject, this.contentContainer, false).GetComponent<InfoScreenPlainText>().SetText(text);
		return this;
	}

	public InfoDialogScreen AddLineItem(string text, string tooltip)
	{
		InfoScreenLineItem component = Util.KInstantiateUI(this.lineItemTemplate.gameObject, this.contentContainer, false).GetComponent<InfoScreenLineItem>();
		component.SetText(text);
		component.SetTooltip(tooltip);
		return this;
	}

	public InfoDialogScreen AddSubHeader(string text)
	{
		Util.KInstantiateUI(this.subHeaderTemplate.gameObject, this.contentContainer, false).GetComponent<InfoScreenPlainText>().SetText(text);
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

	public void OnSelect_OK()
	{
		this.Deactivate();
	}

	[SerializeField]
	private InfoScreenPlainText subHeaderTemplate;

	[SerializeField]
	private InfoScreenPlainText plainTextTemplate;

	[SerializeField]
	private InfoScreenLineItem lineItemTemplate;

	[Space(10f)]
	[SerializeField]
	private LocText header;

	[SerializeField]
	private GameObject contentContainer;

	[SerializeField]
	private GameObject confirmButton;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject buttonPanel;
}
