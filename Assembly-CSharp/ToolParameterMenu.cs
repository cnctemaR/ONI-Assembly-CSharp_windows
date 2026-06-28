using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolParameterMenu : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ClearMenu();
	}

	public void PopulateMenu(Dictionary<string, ToolParameterMenu.ToggleState> parameters)
	{
		this.ClearMenu();
		this.currentParameters = parameters;
		this.radioGroup = this.widgetContainer.GetComponent<ToggleGroup>();
		foreach (KeyValuePair<string, ToolParameterMenu.ToggleState> keyValuePair in parameters)
		{
			GameObject gameObject = Util.KInstantiateUI(this.widgetPrefab, this.widgetContainer, true);
			gameObject.GetComponentInChildren<LocText>().text = Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + keyValuePair.Key);
			this.widgets.Add(keyValuePair.Key, gameObject);
			Toggle componentInChildren = gameObject.GetComponentInChildren<Toggle>();
			ToolParameterMenu.ToggleState value2 = keyValuePair.Value;
			if (value2 == ToolParameterMenu.ToggleState.Disabled)
			{
				componentInChildren.interactable = false;
				componentInChildren.isOn = false;
			}
			else
			{
				componentInChildren.interactable = true;
				componentInChildren.isOn = value2 == ToolParameterMenu.ToggleState.On;
			}
			componentInChildren.group = this.radioGroup;
			componentInChildren.onValueChanged.AddListener(delegate(bool value)
			{
				this.OnChange();
			});
		}
		this.content.SetActive(true);
	}

	public void ClearMenu()
	{
		this.content.SetActive(false);
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.widgets)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.widgets.Clear();
	}

	private void OnChange()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.widgets)
		{
			Toggle componentInChildren = keyValuePair.Value.GetComponentInChildren<Toggle>();
			if (componentInChildren.interactable)
			{
				this.currentParameters[keyValuePair.Key] = ((!componentInChildren.isOn) ? ToolParameterMenu.ToggleState.Off : ToolParameterMenu.ToggleState.On);
			}
		}
	}

	public GameObject content;

	public GameObject widgetContainer;

	public GameObject widgetPrefab;

	private ToggleGroup radioGroup;

	private Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

	private Dictionary<string, ToolParameterMenu.ToggleState> currentParameters;

	public enum ToggleState
	{
		On,
		Off,
		Disabled
	}
}
