using System;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SearchBar : KMonoBehaviour
{
	public string CurrentSearchValue
	{
		get
		{
			if (!string.IsNullOrEmpty(this.inputField.text))
			{
				return this.inputField.text;
			}
			return "";
		}
	}

	public bool IsInputFieldEmpty
	{
		get
		{
			return this.inputField.text == "";
		}
	}

	public bool isEditing { get; protected set; }

	public virtual void SetPlaceholder(string text)
	{
		this.inputField.placeholder.GetComponent<TextMeshProUGUI>().text = text;
	}

	protected override void OnSpawn()
	{
		this.inputField.ActivateInputField();
		KInputTextField kinputTextField = this.inputField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(this.OnFocus));
		this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
		this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnValueChanged));
		this.clearButton.onClick += this.ClearSearch;
		this.SetPlaceholder(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.SEARCH_PLACEHOLDER);
	}

	protected void SetEditingState(bool editing)
	{
		this.isEditing = editing;
		Action<bool> editingStateChanged = this.EditingStateChanged;
		if (editingStateChanged != null)
		{
			editingStateChanged(this.isEditing);
		}
		KScreenManager.Instance.RefreshStack();
	}

	protected virtual void OnValueChanged(string value)
	{
		Action<string> valueChanged = this.ValueChanged;
		if (valueChanged == null)
		{
			return;
		}
		valueChanged(value);
	}

	protected virtual void OnEndEdit(string value)
	{
		this.SetEditingState(false);
	}

	protected virtual void OnFocus()
	{
		this.SetEditingState(true);
		UISounds.PlaySound(UISounds.Sound.ClickHUD);
		global::System.Action focused = this.Focused;
		if (focused == null)
		{
			return;
		}
		focused();
	}

	public virtual void ClearSearch()
	{
		this.SetValue("");
	}

	public void SetValue(string value)
	{
		this.inputField.text = value;
		Action<string> valueChanged = this.ValueChanged;
		if (valueChanged == null)
		{
			return;
		}
		valueChanged(value);
	}

	[SerializeField]
	protected KInputTextField inputField;

	[SerializeField]
	protected KButton clearButton;

	public Action<string> ValueChanged;

	public Action<bool> EditingStateChanged;

	public global::System.Action Focused;
}
