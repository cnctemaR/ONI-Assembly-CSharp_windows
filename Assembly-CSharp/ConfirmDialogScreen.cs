using System;
using UnityEngine;

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
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public void PopupConfirmDialog(string text, global::System.Action onConfirm, global::System.Action onCancel, string third_text = null, global::System.Action onThird = null)
	{
		this.confirmAction = onConfirm;
		this.cancelAction = onCancel;
		this.thirdAction = onThird;
		int num = 0;
		if (this.confirmAction != null)
		{
			num++;
		}
		if (this.cancelAction != null)
		{
			num++;
		}
		if (this.thirdAction != null)
		{
			num++;
		}
		this.confirmButton.GetComponent<KButton>().onClick += this.OnSelect_OK;
		this.cancelButton.GetComponent<KButton>().onClick += this.OnSelect_CANCEL;
		this.thirdButton.GetComponent<KButton>().onClick += this.OnSelect_third;
		this.cancelButton.SetActive(onCancel != null);
		if (this.thirdButton != null)
		{
			this.thirdButton.SetActive(this.thirdAction != null);
			if (third_text != null)
			{
				LocText componentInChildren = this.thirdButton.GetComponentInChildren<LocText>();
				componentInChildren.text = third_text;
			}
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
		if (this.thirdAction != null)
		{
			this.thirdAction();
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

	private global::System.Action thirdAction;

	public LocText popupMessage;

	public global::System.Action onDeactivateCB;

	[SerializeField]
	private GameObject confirmButton;

	[SerializeField]
	private GameObject cancelButton;

	[SerializeField]
	private GameObject thirdButton;
}
