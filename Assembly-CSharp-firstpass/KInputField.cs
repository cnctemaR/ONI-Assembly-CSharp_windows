using System;
using System.Collections;
using UnityEngine;

public class KInputField : KScreen
{
	public KInputTextField field
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
		KInputTextField kinputTextField = this.inputField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(this.OnEditStart));
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
	private KInputTextField inputField;
}
