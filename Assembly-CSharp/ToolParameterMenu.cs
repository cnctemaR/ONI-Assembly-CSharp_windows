using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ToolParameterMenu : KMonoBehaviour
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
			ToolParameterMenu.ToggleState toggleState = this.currentParameters[keyValuePair.Key];
			if (toggleState != ToolParameterMenu.ToggleState.Disabled)
			{
				if (toggleState != ToolParameterMenu.ToggleState.Off)
				{
					if (toggleState == ToolParameterMenu.ToggleState.On)
					{
						keyValuePair.Value.GetComponentInChildren<MultiToggle>().ChangeState(1);
					}
				}
				else
				{
					keyValuePair.Value.GetComponentInChildren<MultiToggle>().ChangeState(0);
				}
			}
			else
			{
				keyValuePair.Value.GetComponentInChildren<MultiToggle>().ChangeState(2);
			}
		}
		if (this.onParametersChanged != null)
		{
			this.onParametersChanged();
		}
	}

	public GameObject content;

	public GameObject widgetContainer;

	public GameObject widgetPrefab;

	private Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

	private Dictionary<string, ToolParameterMenu.ToggleState> currentParameters;

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

		public static string ALL = "ALL";
	}

	public enum ToggleState
	{
		On,
		Off,
		Disabled
	}
}
