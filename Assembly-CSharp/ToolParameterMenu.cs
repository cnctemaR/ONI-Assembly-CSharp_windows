using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ToolParameterMenu")]
public class ToolParameterMenu : KMonoBehaviour
{
	public event global::System.Action onParametersChanged;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ClearMenu();
	}

	public void PopulateMenu(Dictionary<string, ToolParameterMenu.ToggleState> parameters)
	{
		this.ClearMenu();
		this.currentParameters = parameters;
		foreach (KeyValuePair<string, ToolParameterMenu.ToggleState> keyValuePair in parameters)
		{
			GameObject gameObject = Util.KInstantiateUI(this.widgetPrefab, this.widgetContainer, true);
			gameObject.GetComponentInChildren<LocText>().text = Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + keyValuePair.Key);
			this.widgets.Add(keyValuePair.Key, gameObject);
			MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
			ToolParameterMenu.ToggleState value = keyValuePair.Value;
			if (value == ToolParameterMenu.ToggleState.Disabled)
			{
				toggle.ChangeState(2);
			}
			else if (value == ToolParameterMenu.ToggleState.On)
			{
				toggle.ChangeState(1);
				this.lastEnabledFilter = keyValuePair.Key;
			}
			else
			{
				toggle.ChangeState(0);
			}
			MultiToggle toggle2 = toggle;
			toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
			{
				foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.widgets)
				{
					if (keyValuePair2.Value == toggle.transform.parent.gameObject)
					{
						if (this.currentParameters[keyValuePair2.Key] == ToolParameterMenu.ToggleState.Disabled)
						{
							break;
						}
						this.ChangeToSetting(keyValuePair2.Key);
						this.OnChange();
						break;
					}
				}
			}));
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

	private void ChangeToSetting(string key)
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.widgets)
		{
			if (this.currentParameters[keyValuePair.Key] != ToolParameterMenu.ToggleState.Disabled)
			{
				this.currentParameters[keyValuePair.Key] = ToolParameterMenu.ToggleState.Off;
			}
		}
		this.currentParameters[key] = ToolParameterMenu.ToggleState.On;
	}

	private void OnChange()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.widgets)
		{
			switch (this.currentParameters[keyValuePair.Key])
			{
			case ToolParameterMenu.ToggleState.On:
				keyValuePair.Value.GetComponentInChildren<MultiToggle>().ChangeState(1);
				this.lastEnabledFilter = keyValuePair.Key;
				break;
			case ToolParameterMenu.ToggleState.Off:
				keyValuePair.Value.GetComponentInChildren<MultiToggle>().ChangeState(0);
				break;
			case ToolParameterMenu.ToggleState.Disabled:
				keyValuePair.Value.GetComponentInChildren<MultiToggle>().ChangeState(2);
				break;
			}
		}
		if (this.onParametersChanged != null)
		{
			this.onParametersChanged();
		}
	}

	public string GetLastEnabledFilter()
	{
		return this.lastEnabledFilter;
	}

	public GameObject content;

	public GameObject widgetContainer;

	public GameObject widgetPrefab;

	private Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

	private Dictionary<string, ToolParameterMenu.ToggleState> currentParameters;

	private string lastEnabledFilter;

	public class FILTERLAYERS
	{
		public static string BUILDINGS = "BUILDINGS";

		public static string TILES = "TILES";

		public static string WIRES = "WIRES";

		public static string LIQUIDCONDUIT = "LIQUIDPIPES";

		public static string GASCONDUIT = "GASPIPES";

		public static string SOLIDCONDUIT = "SOLIDCONDUITS";

		public static string CLEANANDCLEAR = "CLEANANDCLEAR";

		public static string DIGPLACER = "DIGPLACER";

		public static string LOGIC = "LOGIC";

		public static string BACKWALL = "BACKWALL";

		public static string CONSTRUCTION = "CONSTRUCTION";

		public static string DIG = "DIG";

		public static string CLEAN = "CLEAN";

		public static string OPERATE = "OPERATE";

		public static string METAL = "METAL";

		public static string BUILDABLE = "BUILDABLE";

		public static string FILTER = "FILTER";

		public static string LIQUIFIABLE = "LIQUIFIABLE";

		public static string LIQUID = "LIQUID";

		public static string CONSUMABLEORE = "CONSUMABLEORE";

		public static string ORGANICS = "ORGANICS";

		public static string FARMABLE = "FARMABLE";

		public static string GAS = "GAS";

		public static string HEATFLOW = "HEATFLOW";

		public static string ABSOLUTETEMPERATURE = "ABSOLUTETEMPERATURE";

		public static string ADAPTIVETEMPERATURE = "ADAPTIVETEMPERATURE";

		public static string STATECHANGE = "STATECHANGE";

		public static string ALL = "ALL";
	}

	public enum ToggleState
	{
		On,
		Off,
		Disabled
	}
}
