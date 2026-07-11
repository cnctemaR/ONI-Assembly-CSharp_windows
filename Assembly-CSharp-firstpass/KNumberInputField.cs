using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class KNumberInputField : KScreen
{
	public TMP_InputField field
	{
		get
		{
			return this.inputField;
		}
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onStartEdit;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
		this.isEditing = true;
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
		}
		else
		{
			this.StopEditing();
		}
	}

	private IEnumerator DelayedEndEdit()
	{
		if (this.isEditing)
		{
			yield return new WaitForEndOfFrame();
			this.StopEditing();
		}
		yield break;
	}

	private void StopEditing()
	{
		this.isEditing = false;
		this.inputField.DeactivateInputField();
		if (this.onEndEdit != null)
		{
			this.onEndEdit();
		}
	}

	public void SetAmount(float newValue)
	{
		newValue = Mathf.Clamp(newValue, this.minValue, this.maxValue);
		if (this.decimalPlaces != -1)
		{
			float num = Mathf.Pow(10f, (float)this.decimalPlaces);
			newValue = Mathf.Round(newValue * num) / num;
		}
		this.currentValue = newValue;
		this.SetDisplayValue(this.currentValue.ToString());
	}

	private void ProcessInput(string input)
	{
		input = ((!(input == string.Empty)) ? input : this.minValue.ToString());
		float num = this.minValue;
		try
		{
			num = float.Parse(input);
			this.SetAmount(num);
		}
		catch
		{
		}
	}

	public void SetDisplayValue(string input)
	{
		this.inputField.text = input;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override float GetSortKey()
	{
		if (this.isEditing)
		{
			return 10f;
		}
		return base.GetSortKey();
	}

	public int decimalPlaces = -1;

	public float currentValue;

	public float minValue;

	public float maxValue;

	private bool isEditing;

	[SerializeField]
	private TMP_InputField inputField;
}
