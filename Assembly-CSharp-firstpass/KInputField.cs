using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KInputField : KScreen
{
	public TMP_InputField field
	{
		get
		{
			return this.inputField;
		}
	}

	public event global::System.Action onStartEdit;

	public event global::System.Action onEndEdit;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TMP_InputField tmp_InputField = this.inputField;
		tmp_InputField.onFocus = (global::System.Action)Delegate.Combine(tmp_InputField.onFocus, new global::System.Action(this.OnEditStart));
		this.inputField.onEndEdit.AddListener(delegate
		{
			this.OnEditEnd(this.inputField.text);
		});
	}

	private void OnEditStart()
	{
		base.isEditing = true;
		this.inputField.Select();
		this.inputField.ActivateInputField();
		KScreenManager.Instance.RefreshStack();
		if (this.onStartEdit != null)
		{
			this.onStartEdit();
		}
	}

	private void OnEditEnd(string input)
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.ProcessInput(input);
			base.StartCoroutine(this.DelayedEndEdit());
			return;
		}
		this.StopEditing();
	}

	private IEnumerator DelayedEndEdit()
	{
		if (base.isEditing)
		{
			yield return new WaitForEndOfFrame();
			this.StopEditing();
		}
		yield break;
	}

	private void StopEditing()
	{
		base.isEditing = false;
		this.inputField.DeactivateInputField();
		if (this.onEndEdit != null)
		{
			this.onEndEdit();
		}
	}

	protected virtual void ProcessInput(string input)
	{
		this.SetDisplayValue(input);
	}

	public void SetDisplayValue(string input)
	{
		this.inputField.text = input;
	}

	[SerializeField]
	private TMP_InputField inputField;
}
