using System;
using UnityEngine;

public class LoreDialogScreen : KModalScreen
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
			this.OnSelect_OK();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public void PopupLoreDialog(string text, string header, global::System.Action onConfirm)
	{
		this.confirmAction = onConfirm;
		int num = 0;
		if (this.confirmAction != null)
		{
			num++;
		}
		this.confirmButton.GetComponent<KButton>().onClick += this.OnSelect_OK;
		this.header.text = header;
		this.popupMessage.text = text;
		this.wasPaused = SpeedControlScreen.Instance.IsPaused;
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.TogglePause(false);
		}
	}

	public void OnSelect_OK()
	{
		this.Deactivate();
		if (this.confirmAction != null)
		{
			this.confirmAction();
		}
	}

	protected override void OnDeactivate()
	{
		if (!this.wasPaused)
		{
			SpeedControlScreen.Instance.TogglePause(false);
		}
		if (this.onDeactivateCB != null)
		{
			this.onDeactivateCB();
		}
		base.OnDeactivate();
	}

	private global::System.Action confirmAction;

	public LocText popupMessage;

	public LocText header;

	public GameObject imageGO;

	private bool wasPaused;

	public global::System.Action onDeactivateCB;

	[SerializeField]
	private GameObject confirmButton;
}
