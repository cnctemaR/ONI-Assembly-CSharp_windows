using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ToolParameterMenu")]
public class ToolParameterMenu : KMonoBehaviour
{
	public event global::System.Action onParametersChanged;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ClearMenu();
	}

	private int ToggleStateToMultiToggleInt(ToolParameterMenu.ToggleData data)
	{
		switch (data.state)
		{
		case ToolParameterMenu.ToggleState.On:
			if (!data.isToggleInclusive)
			{
				return 1;
			}
			return 3;
		case ToolParameterMenu.ToggleState.Off:
			return 0;
		case ToolParameterMenu.ToggleState.Disabled:
			return 2;
		default:
			return 0;
		}
	}

	public void PopulateMenu(ToolParameterMenu.ToggleData[] togglesData)
	{
		this.ClearMenu();
		this.currentTogglesData = togglesData;
		bool flag = true;
		for (int i = 0; i < togglesData.Length; i++)
		{
			if (togglesData[i].isToggleInclusive)
			{
				flag = false;
				break;
			}
		}
		this.widgetContainer.GetComponent<ToggleGroup>().enabled = flag;
		foreach (ToolParameterMenu.ToggleData toggleData in togglesData)
		{
			GameObject gameObject = this.CreateToggleGameObject(toggleData);
			this.widgets.Add(toggleData.name, new ToolParameterMenu.Widget
			{
				gameObject = gameObject,
				data = toggleData
			});
		}
		this.content.SetActive(true);
	}

	private GameObject CreateToggleGameObject(ToolParameterMenu.ToggleData data)
	{
		GameObject newWidget = Util.KInstantiateUI(this.widgetPrefab, this.widgetContainer, true);
		TMP_Text componentInChildren = newWidget.GetComponentInChildren<LocText>();
		ToolTip componentInChildren2 = newWidget.GetComponentInChildren<ToolTip>();
		MultiToggle componentInChildren3 = newWidget.GetComponentInChildren<MultiToggle>();
		ToolParameterMenu.ToggleState state = data.state;
		componentInChildren.text = Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + data.name + ".NAME");
		if (componentInChildren2 != null)
		{
			componentInChildren2.SetSimpleTooltip(Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + data.name + ".TOOLTIP"));
		}
		componentInChildren3.ChangeState(this.ToggleStateToMultiToggleInt(data));
		MultiToggle multiToggle = componentInChildren3;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			foreach (KeyValuePair<string, ToolParameterMenu.Widget> keyValuePair in this.widgets)
			{
				ToolParameterMenu.Widget value = keyValuePair.Value;
				ToolParameterMenu.ToggleData data2 = value.data;
				if (value.gameObject == newWidget)
				{
					if (data2.state == ToolParameterMenu.ToggleState.Disabled)
					{
						break;
					}
					this.ChangeToSetting(value);
					this.OnChange();
					break;
				}
			}
		}));
		return newWidget;
	}

	public void ClearMenu()
	{
		this.content.SetActive(false);
		foreach (KeyValuePair<string, ToolParameterMenu.Widget> keyValuePair in this.widgets)
		{
			Util.KDestroyGameObject(keyValuePair.Value.gameObject);
		}
		this.widgets.Clear();
	}

	private void ChangeToSetting(ToolParameterMenu.Widget clickedWidget)
	{
		ToolParameterMenu.ToggleData data = clickedWidget.data;
		if (data.isToggleInclusive)
		{
			data.state = ((data.state == ToolParameterMenu.ToggleState.Off) ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Off);
			using (Dictionary<string, ToolParameterMenu.Widget>.Enumerator enumerator = this.widgets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ToolParameterMenu.Widget> keyValuePair = enumerator.Current;
					ToolParameterMenu.ToggleData data2 = keyValuePair.Value.data;
					if (data2.state != ToolParameterMenu.ToggleState.Disabled && !data.isToggleInclusive)
					{
						data2.state = ToolParameterMenu.ToggleState.Off;
					}
				}
				return;
			}
		}
		foreach (KeyValuePair<string, ToolParameterMenu.Widget> keyValuePair2 in this.widgets)
		{
			ToolParameterMenu.ToggleData data3 = keyValuePair2.Value.data;
			if (data3.state != ToolParameterMenu.ToggleState.Disabled)
			{
				data3.state = ToolParameterMenu.ToggleState.Off;
			}
		}
		data.state = ToolParameterMenu.ToggleState.On;
	}

	private void OnChange()
	{
		foreach (KeyValuePair<string, ToolParameterMenu.Widget> keyValuePair in this.widgets)
		{
			ToolParameterMenu.Widget value = keyValuePair.Value;
			ToolParameterMenu.ToggleData data = value.data;
			GameObject gameObject = value.gameObject;
			int num = this.ToggleStateToMultiToggleInt(data);
			gameObject.GetComponentInChildren<MultiToggle>().ChangeState(num);
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

	private Dictionary<string, ToolParameterMenu.Widget> widgets = new Dictionary<string, ToolParameterMenu.Widget>();

	private ToolParameterMenu.ToggleData[] currentTogglesData;

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

		public static string NATURALBACKWALL = "NATURALBACKWALL";

		public static string UPROOTPLANTS = "UPROOTPLANTS";

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

		public static string MISC = "MISC";

		public static string HEATFLOW = "HEATFLOW";

		public static string ABSOLUTETEMPERATURE = "ABSOLUTETEMPERATURE";

		public static string RELATIVETEMPERATURE = "RELATIVETEMPERATURE";

		public static string ADAPTIVETEMPERATURE = "ADAPTIVETEMPERATURE";

		public static string STATECHANGE = "STATECHANGE";

		public static string ALL = "ALL";
	}

	public class ToggleData
	{
		public bool IsOn
		{
			get
			{
				return this.state == ToolParameterMenu.ToggleState.On;
			}
		}

		public ToggleData()
		{
		}

		public ToggleData(string name, ToolParameterMenu.ToggleState state, bool isToggleInclusive = false)
		{
			this.name = name;
			this.state = state;
			this.isToggleInclusive = isToggleInclusive;
		}

		public string name;

		public bool isToggleInclusive;

		public ToolParameterMenu.ToggleState state;
	}

	private class Widget
	{
		public GameObject gameObject;

		public ToolParameterMenu.ToggleData data;
	}

	public enum ToggleState
	{
		On,
		Off,
		Disabled
	}
}
