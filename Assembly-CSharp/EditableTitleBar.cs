using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EditableTitleBar : TitleBar
{
	public event Action<string> OnNameChanged;

	public event global::System.Action OnStartedEditing;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.randomNameButton != null)
		{
			this.randomNameButton.onClickDown += this.GenerateRandomName;
		}
		if (this.editNameButton != null)
		{
			this.editNameButton.onClickDown += this.ToggleNameEditing;
		}
		if (this.inputField != null)
		{
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
		}
	}

	private void OnEndEdit(string finalStr)
	{
		this.SetEditingState(false);
		if (string.IsNullOrEmpty(finalStr))
		{
			return;
		}
		if (this.OnNameChanged != null)
		{
			this.OnNameChanged(finalStr);
		}
		this.titleText.text = finalStr;
		if (this.postEndEdit != null)
		{
			base.StopCoroutine(this.postEndEdit);
		}
		if (base.gameObject.activeSelf && base.enabled)
		{
			this.postEndEdit = base.StartCoroutine(this.PostOnEndEdit());
		}
	}

	private IEnumerator PostOnEndEdit()
	{
		int i = 0;
		while (i < 10)
		{
			i++;
			yield return new WaitForEndOfFrame();
		}
		this.editNameButton.onClickDown += this.ToggleNameEditing;
		if (this.randomNameButton != null)
		{
			this.randomNameButton.gameObject.SetActive(false);
		}
		yield break;
	}

	private void GenerateRandomName()
	{
		if (this.postEndEdit != null)
		{
			base.StopCoroutine(this.postEndEdit);
		}
		string text = GameUtil.GenerateRandomDuplicantName(this.isMaleDuplicant);
		if (this.OnNameChanged != null)
		{
			this.OnNameChanged(text);
		}
		this.titleText.text = text;
		this.SetEditingState(true);
	}

	private void ToggleNameEditing()
	{
		this.editNameButton.ClearOnClickDown();
		bool flag = !this.inputField.gameObject.activeInHierarchy;
		if (this.randomNameButton != null)
		{
			this.randomNameButton.gameObject.SetActive(flag);
		}
		this.SetEditingState(flag);
	}

	private void SetEditingState(bool state)
	{
		this.titleText.gameObject.SetActive(!state);
		if (this.setCameraControllerState)
		{
			CameraController.Instance.DisableUserCameraControl = state;
		}
		if (this.inputField == null)
		{
			return;
		}
		this.inputField.gameObject.SetActive(state);
		if (state)
		{
			this.inputField.text = this.titleText.text;
			this.inputField.Select();
			this.inputField.ActivateInputField();
			if (this.OnStartedEditing != null)
			{
				this.OnStartedEditing();
			}
		}
		else
		{
			this.inputField.DeactivateInputField();
		}
	}

	public void SetUserEditable(bool editable)
	{
		this.userEditable = editable;
		this.editNameButton.gameObject.SetActive(editable);
	}

	public void SetTitle(string Name, bool isMale)
	{
		this.isMaleDuplicant = isMale;
		base.SetTitle(Name);
	}

	public KButton editNameButton;

	public KButton randomNameButton;

	public TMP_InputField inputField;

	private bool isMaleDuplicant;

	private Coroutine postEndEdit;
}
