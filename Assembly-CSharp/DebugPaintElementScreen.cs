using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DebugPaintElementScreen : KScreen
{
	public static DebugPaintElementScreen Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DebugPaintElementScreen.Instance = this;
		this.SetupLocText();
		this.massPressureInput.onValueChanged.AddListener(delegate
		{
			this.OnChangeMassPressure();
			this.blockInput = true;
		});
		this.temperatureInput.onValueChanged.AddListener(delegate
		{
			this.OnChangeTemperature();
			this.blockInput = true;
		});
		this.diseaseCountInput.onValueChanged.AddListener(delegate
		{
			this.OnDiseaseCountChange();
			this.blockInput = true;
		});
		this.temperatureInput.onEndEdit.AddListener(delegate
		{
			this.blockInput = false;
		});
		this.massPressureInput.onEndEdit.AddListener(delegate
		{
			this.blockInput = false;
		});
		this.diseaseCountInput.onEndEdit.AddListener(delegate
		{
			this.blockInput = false;
		});
		base.gameObject.SetActive(false);
		this.activateOnSpawn = true;
		this.ConsumeMouseScroll = true;
	}

	private void SetupLocText()
	{
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("Title").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.TITLE;
		component.GetReference<LocText>("ElementLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT;
		component.GetReference<LocText>("MassLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.MASS_KG;
		component.GetReference<LocText>("TemperatureLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.TEMPERATURE_KELVIN;
		component.GetReference<LocText>("DiseaseLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE;
		component.GetReference<LocText>("DiseaseCountLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE_COUNT;
		component.GetReference<LocText>("AddFoWMaskLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ADD_FOW_MASK;
		component.GetReference<LocText>("RemoveFoWMaskLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.REMOVE_FOW_MASK;
		this.elementButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT;
		this.diseaseButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE;
		this.paintButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.PAINT;
		this.fillButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.FILL;
		this.affectBuildings.transform.parent.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.BUILDINGS;
		this.affectCells.transform.parent.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.CELLS;
	}

	public override float GetSortKey()
	{
		return 10f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.element = SimHashes.Ice;
		List<DebugPaintElementScreen.ElemDisplayInfo> list = new List<DebugPaintElementScreen.ElemDisplayInfo>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.name != "Element Not Loaded" && element.substance != null && element.substance.showInEditor)
			{
				list.Add(new DebugPaintElementScreen.ElemDisplayInfo
				{
					id = element.id,
					displayStr = element.name + " (" + element.GetStateString() + ")"
				});
			}
		}
		list.Sort((DebugPaintElementScreen.ElemDisplayInfo a, DebugPaintElementScreen.ElemDisplayInfo b) => a.displayStr.CompareTo(b.displayStr));
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.SlimeMold,
			SimHashes.Vacuum,
			SimHashes.Dirt,
			SimHashes.CarbonDioxide,
			SimHashes.Water,
			SimHashes.Oxygen
		};
		foreach (SimHashes simHashes in array)
		{
			Element element2 = ElementLoader.FindElementByHash(simHashes);
			list.Insert(0, new DebugPaintElementScreen.ElemDisplayInfo
			{
				id = element2.id,
				displayStr = element2.name + " (" + element2.GetStateString() + ")"
			});
		}
		this.options_list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (DebugPaintElementScreen.ElemDisplayInfo elemDisplayInfo in list)
		{
			list2.Add(elemDisplayInfo.displayStr);
			this.options_list.Add(elemDisplayInfo.id.ToString());
		}
		this.elementPopup.SetOptions(list2);
		KPopupMenu kpopupMenu = this.elementPopup;
		kpopupMenu.OnSelect = (Action<string, int>)Delegate.Combine(kpopupMenu.OnSelect, new Action<string, int>(this.OnSelectElement));
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].id == this.element)
			{
				this.elementPopup.SelectOption(list2[j], j);
			}
		}
		this.diseaseIdx = byte.MaxValue;
		List<string> list3 = new List<string>();
		list3.Insert(0, "None");
		foreach (Disease disease in Db.Get().Diseases)
		{
			list3.Add(disease.Name);
		}
		this.diseasePopup.SetOptions(list3.ToArray());
		KPopupMenu kpopupMenu2 = this.diseasePopup;
		kpopupMenu2.OnSelect = (Action<string, int>)Delegate.Combine(kpopupMenu2.OnSelect, new Action<string, int>(this.OnSelectDisease));
		this.SelectDiseaseOption((int)this.diseaseIdx);
		this.paintButton.onClick += this.OnClickPaint;
		this.fillButton.onClick += this.OnClickFill;
		this.spawnButton.enabled = false;
		this.elementButton.onClick += this.elementPopup.OnClick;
		this.diseaseButton.onClick += this.diseasePopup.OnClick;
		this.blockInput = false;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!this.blockInput || e.TryConsume(global::Action.Plan1) || e.TryConsume(global::Action.Plan2) || e.TryConsume(global::Action.Plan3) || e.TryConsume(global::Action.Plan4) || e.TryConsume(global::Action.Plan5) || e.TryConsume(global::Action.Plan6) || e.TryConsume(global::Action.Plan7) || e.TryConsume(global::Action.Plan8) || e.TryConsume(global::Action.Plan9) || e.TryConsume(global::Action.Plan10) || e.TryConsume(global::Action.DebugToggle))
		{
		}
		base.OnKeyDown(e);
	}

	private void OnClickSpawn()
	{
		WorldGenSpawner.Instance.SpawnEverything();
		this.spawnButton.enabled = false;
	}

	private void OnClickPaint()
	{
		this.OnChangeMassPressure();
		this.OnChangeTemperature();
		this.OnDiseaseCountChange();
		this.OnChangeFOWReveal();
		DebugTool.Instance.Activate(DebugTool.Type.ReplaceSubstance);
	}

	private void OnClickFill()
	{
		this.OnChangeMassPressure();
		this.OnChangeTemperature();
		this.OnDiseaseCountChange();
		DebugTool.Instance.Activate(DebugTool.Type.FillReplaceSubstance);
	}

	private void OnSelectElement(string str, int index)
	{
		this.element = (SimHashes)((int)Enum.Parse(typeof(SimHashes), this.options_list[index]));
		this.elementButton.GetComponentInChildren<LocText>().text = str;
	}

	private void OnSelectDisease(string str, int index)
	{
		this.diseaseIdx = byte.MaxValue;
		for (int i = 0; i < Db.Get().Diseases.Count; i++)
		{
			if (Db.Get().Diseases[i].Name == str)
			{
				this.diseaseIdx = (byte)i;
			}
		}
		this.SelectDiseaseOption((int)this.diseaseIdx);
	}

	private void SelectDiseaseOption(int diseaseIdx)
	{
		if (diseaseIdx == 255)
		{
			this.diseaseButton.GetComponentInChildren<LocText>().text = "None";
		}
		else
		{
			string name = Db.Get().Diseases[diseaseIdx].Name;
			this.diseaseButton.GetComponentInChildren<LocText>().text = name;
		}
	}

	private void OnChangeFOWReveal()
	{
		if (this.paintPreventFOWReveal.isOn)
		{
			this.paintAllowFOWReveal.isOn = false;
		}
		if (this.paintAllowFOWReveal.isOn)
		{
			this.paintPreventFOWReveal.isOn = false;
		}
		this.set_prevent_fow_reveal = this.paintPreventFOWReveal.isOn;
		this.set_allow_fow_reveal = this.paintAllowFOWReveal.isOn;
	}

	public void OnChangeMassPressure()
	{
		float num;
		try
		{
			num = Convert.ToSingle(this.massPressureInput.text);
		}
		catch
		{
			num = -1f;
		}
		this.mass = num;
	}

	public void OnChangeTemperature()
	{
		float num;
		try
		{
			num = Convert.ToSingle(this.temperatureInput.text);
		}
		catch
		{
			num = -1f;
		}
		this.temperature = num;
	}

	public void OnDiseaseCountChange()
	{
		int num;
		try
		{
			num = Convert.ToInt32(this.diseaseCountInput.text);
		}
		catch
		{
			num = 0;
		}
		this.diseaseCount = num;
	}

	[Header("Current State")]
	public SimHashes element;

	[NonSerialized]
	public float mass = 1000f;

	[NonSerialized]
	public float temperature = -1f;

	[NonSerialized]
	public bool set_prevent_fow_reveal;

	[NonSerialized]
	public bool set_allow_fow_reveal;

	public byte diseaseIdx;

	[NonSerialized]
	public int diseaseCount;

	private bool blockInput;

	[Header("Popup Buttons")]
	[SerializeField]
	private KButton elementButton;

	[SerializeField]
	private KButton diseaseButton;

	[Header("Popup Menus")]
	[SerializeField]
	private KPopupMenu elementPopup;

	[SerializeField]
	private KPopupMenu diseasePopup;

	[Header("Value Inputs")]
	[SerializeField]
	private InputField massPressureInput;

	[SerializeField]
	private InputField temperatureInput;

	[SerializeField]
	private InputField diseaseCountInput;

	[Header("Tool Buttons")]
	[SerializeField]
	private KButton paintButton;

	[SerializeField]
	private KButton fillButton;

	[SerializeField]
	private KButton spawnButton;

	[Header("Parameter Toggles")]
	public Toggle paintElement;

	public Toggle paintMass;

	public Toggle paintTemperature;

	public Toggle paintDisease;

	public Toggle paintDiseaseCount;

	public Toggle affectBuildings;

	public Toggle affectCells;

	public Toggle paintPreventFOWReveal;

	public Toggle paintAllowFOWReveal;

	private List<string> options_list = new List<string>();

	private struct ElemDisplayInfo
	{
		public SimHashes id;

		public string displayStr;
	}
}
