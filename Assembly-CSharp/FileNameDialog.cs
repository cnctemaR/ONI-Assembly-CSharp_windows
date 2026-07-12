using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FileNameDialog : KModalScreen
{
	public override float GetSortKey()
	{
		return 150f;
	}

	public void SetTextAndSelect(string text)
	{
		if (this.inputField == null)
		{
			return;
		}
		this.inputField.text = text;
		this.inputField.Select();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.confirmButton.onClick += this.OnConfirm;
		this.cancelButton.onClick += this.OnCancel;
		this.closeButton.onClick += this.OnCancel;
		this.inputField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(this.inputField, false, false);
		});
		this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.inputField.Select();
		this.inputField.ActivateInputField();
		CameraController.Instance.DisableUserCameraControl = true;
	}

	protected override void OnDeactivate()
	{
		CameraController.Instance.DisableUserCameraControl = false;
		base.OnDeactivate();
	}

	public void OnConfirm()
	{
		if (this.onConfirm != null && !string.IsNullOrEmpty(this.inputField.text))
		{
			string text = this.inputField.text;
			if (!text.EndsWith(".sav"))
			{
				text += ".sav";
			}
			this.onConfirm(text);
			this.Deactivate();
		}
	}

	private void OnEndEdit(string str)
	{
		if (Localization.HasDirtyWords(str))
		{
			this.inputField.text = "";
		}
	}

	public void OnCancel()
	{
		if (this.onCancel != null)
		{
			this.onCancel();
		}
		this.Deactivate();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			this.Deactivate();
		}
		else if (e.TryConsume(global::Action.DialogSubmit))
		{
			this.OnConfirm();
		}
		e.Consumed = true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		e.Consumed = true;
	}

	public Action<string> onConfirm;

	public global::System.Action onCancel;

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private KButton confirmButton;

	[SerializeField]
	private KButton cancelButton;

	[SerializeField]
	private KButton closeButton;
}
