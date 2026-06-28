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

	public void PopulateMenu(Dictionary<string, bool> parameters)
	{
		this.ClearMenu();
		this.content.SetActive(true);
		this.currentParameters = parameters;
		this.radioGroup = this.widgetContainer.GetComponent<ToggleGroup>();
		foreach (KeyValuePair<string, bool> keyValuePair in parameters)
		{
			GameObject gameObject = Util.KInstantiateUI(this.widgetPrefab, this.widgetContainer, true);
			this.widgetMap.Add(keyValuePair.Key, gameObject);
			Toggle componentInChildren = gameObject.GetComponentInChildren<Toggle>();
			componentInChildren.isOn = keyValuePair.Value;
			componentInChildren.group = this.radioGroup;
			componentInChildren.onValueChanged.AddListener(delegate(bool value)
			{
				this.OnChange();
			});
			gameObject.GetComponentInChildren<LocText>().text = Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + keyValuePair.Key);
		}
	}

	public void ClearMenu()
	{
		this.content.SetActive(false);
		if (this.currentParameters == null)
		{
			return;
		}
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.widgetMap)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.widgetMap.Clear();
	}

	private void OnChange()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.widgetMap)
		{
			Toggle componentInChildren = keyValuePair.Value.GetComponentInChildren<Toggle>();
			this.currentParameters[keyValuePair.Key] = componentInChildren.isOn;
		}
	}

	public GameObject content;

	public GameObject widgetContainer;

	public GameObject widgetPrefab;

	private ToggleGroup radioGroup;

	private Dictionary<string, GameObject> widgetMap = new Dictionary<string, GameObject>();

	private Dictionary<string, bool> currentParameters;
}
