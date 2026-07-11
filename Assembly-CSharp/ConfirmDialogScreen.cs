using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialogScreen : KModalScreen
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
		if (e.TryConsume(global::Action.Escape))
		{
			this.OnSelect_CANCEL();
			return;
		}
		base.OnKeyDown(e);
	}

	public void PopupConfirmDialog(string text, global::System.Action on_confirm, global::System.Action on_cancel, string configurable_text = null, global::System.Action on_configurable_clicked = null, string title_text = null, string confirm_text = null, string cancel_text = null, Sprite image_sprite = null)
	{
		while (base.transform.parent.GetComponent<Canvas>() == null && base.transform.parent.parent != null)
		{
			base.transform.SetParent(base.transform.parent.parent);
		}
		base.transform.SetAsLastSibling();
		this.confirmAction = on_confirm;
		this.cancelAction = on_cancel;
		this.configurableAction = on_configurable_clicked;
		int num = 0;
		if (this.confirmAction != null)
		{
			num++;
		}
		if (this.cancelAction != null)
		{
			num++;
		}
		if (this.configurableAction != null)
		{
			num++;
		}
		this.confirmButton.GetComponentInChildren<LocText>().text = ((confirm_text == null) ? UI.CONFIRMDIALOG.OK.text : confirm_text);
		this.cancelButton.GetComponentInChildren<LocText>().text = ((cancel_text == null) ? UI.CONFIRMDIALOG.CANCEL.text : cancel_text);
		this.confirmButton.GetComponent<KButton>().onClick += this.OnSelect_OK;
		this.cancelButton.GetComponent<KButton>().onClick += this.OnSelect_CANCEL;
		this.configurableButton.GetComponent<KButton>().onClick += this.OnSelect_third;
		this.cancelButton.SetActive(on_cancel != null);
		if (this.configurableButton != null)
		{
			this.configurableButton.SetActive(this.configurableAction != null);
			if (configurable_text != null)
			{
				this.configurableButton.GetComponentInChildren<LocText>().text = configurable_text;
			}
		}
		if (image_sprite != null)
		{
			this.image.sprite = image_sprite;
			this.image.gameObject.SetActive(true);
		}
		if (title_text != null)
		{
			this.titleText.text = title_text;
		}
		this.popupMessage.text = text;
	}

	public void OnSelect_OK()
	{
		this.Deactivate();
		if (this.confirmAction != null)
		{
			this.confirmAction();
		}
	}

	public void OnSelect_CANCEL()
	{
		this.Deactivate();
		if (this.cancelAction != null)
		{
			this.cancelAction();
		}
	}

	public void OnSelect_third()
	{
		this.Deactivate();
		if (this.configurableAction != null)
		{
			this.configurableAction();
		}
	}

	protected override void OnDeactivate()
	{
		if (this.onDeactivateCB != null)
		{
			this.onDeactivateCB();
		}
		base.OnDeactivate();
	}

	private global::System.Action confirmAction;

	private global::System.Action cancelAction;

	private global::System.Action configurableAction;

	public global::System.Action onDeactivateCB;

	[SerializeField]
	private GameObject confirmButton;

	[SerializeField]
	private GameObject cancelButton;

	[SerializeField]
	private GameObject configurableButton;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText popupMessage;

	[SerializeField]
	private Image image;
}
