using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugPaintElementScreen : KScreen
{
	public static DebugPaintElementScreen Instance { get; private set; }

	public static void DestroyInstance()
	{
		DebugPaintElementScreen.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DebugPaintElementScreen.Instance = this;
		this.SetupLocText();
		this.inputFields.Add(this.massPressureInput);
		this.inputFields.Add(this.temperatureInput);
		this.inputFields.Add(this.diseaseCountInput);
		this.inputFields.Add(this.filterInput);
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
		this.sampleButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.SAMPLE;
		this.storeButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.STORE;
		this.affectBuildings.transform.parent.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.BUILDINGS;
		this.affectCells.transform.parent.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.CELLS;
	}

	public override float GetSortKey()
	{
		return 100000f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.element = SimHashes.Ice;
		this.diseaseIdx = byte.MaxValue;
		this.ConfigureElements();
		List<string> list = new List<string>();
		list.Insert(0, "None");
		foreach (Disease disease in Db.Get().Diseases.resources)
		{
			list.Add(disease.Name);
		}
		this.diseasePopup.SetOptions(list.ToArray());
		KPopupMenu kpopupMenu = this.diseasePopup;
		kpopupMenu.OnSelect = (Action<string, int>)Delegate.Combine(kpopupMenu.OnSelect, new Action<string, int>(this.OnSelectDisease));
		this.SelectDiseaseOption((int)this.diseaseIdx);
		this.paintButton.onClick += this.OnClickPaint;
		this.fillButton.onClick += this.OnClickFill;
		this.sampleButton.onClick += this.OnClickSample;
		this.storeButton.onClick += this.OnClickStore;
		this.spawnButton.enabled = false;
		KPopupMenu kpopupMenu2 = this.elementPopup;
		kpopupMenu2.OnSelect = (Action<string, int>)Delegate.Combine(kpopupMenu2.OnSelect, new Action<string, int>(this.OnSelectElement));
		this.elementButton.onClick += this.elementPopup.OnClick;
		this.diseaseButton.onClick += this.diseasePopup.OnClick;
	}

	private void FilterElements(string filterValue)
	{
		if (string.IsNullOrEmpty(filterValue))
		{
			foreach (KButtonMenu.ButtonInfo buttonInfo in this.elementPopup.GetButtons())
			{
				buttonInfo.uibutton.gameObject.SetActive(true);
			}
			return;
		}
		filterValue = this.filter.ToLower();
		foreach (KButtonMenu.ButtonInfo buttonInfo2 in this.elementPopup.GetButtons())
		{
			buttonInfo2.uibutton.gameObject.SetActive(buttonInfo2.text.ToLower().Contains(filterValue));
		}
	}

	private void ConfigureElements()
	{
		if (this.filter != null)
		{
			this.filter = this.filter.ToLower();
		}
		List<DebugPaintElementScreen.ElemDisplayInfo> list = new List<DebugPaintElementScreen.ElemDisplayInfo>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.name != "Element Not Loaded" && element.substance != null && element.substance.showInEditor && (string.IsNullOrEmpty(this.filter) || element.name.ToLower().Contains(this.filter)))
			{
				list.Add(new DebugPaintElementScreen.ElemDisplayInfo
				{
					id = element.id,
					displayStr = element.name + " (" + element.GetStateString() + ")"
				});
			}
		}
		list.Sort((DebugPaintElementScreen.ElemDisplayInfo a, DebugPaintElementScreen.ElemDisplayInfo b) => a.displayStr.CompareTo(b.displayStr));
		if (string.IsNullOrEmpty(this.filter))
		{
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
		}
		this.options_list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (DebugPaintElementScreen.ElemDisplayInfo elemDisplayInfo in list)
		{
			list2.Add(elemDisplayInfo.displayStr);
			this.options_list.Add(elemDisplayInfo.id.ToString());
		}
		this.elementPopup.SetOptions(list2);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].id == this.element)
			{
				this.elementPopup.SelectOption(list2[j], j);
			}
		}
		this.elementPopup.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0f, 1f);
	}

	private void OnClickSpawn()
	{
		SaveGame.Instance.worldGenSpawner.SpawnEverything();
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

	private void OnClickStore()
	{
		this.OnChangeMassPressure();
		this.OnChangeTemperature();
		this.OnDiseaseCountChange();
		this.OnChangeFOWReveal();
		DebugTool.Instance.Activate(DebugTool.Type.StoreSubstance);
	}

	private void OnClickSample()
	{
		this.OnChangeMassPressure();
		this.OnChangeTemperature();
		this.OnDiseaseCountChange();
		this.OnChangeFOWReveal();
		DebugTool.Instance.Activate(DebugTool.Type.Sample);
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
		this.element = (SimHashes)Enum.Parse(typeof(SimHashes), this.options_list[index]);
		this.elementButton.GetComponentInChildren<LocText>().text = str;
	}

	private void OnSelectElement(SimHashes element)
	{
		this.element = element;
		this.elementButton.GetComponentInChildren<LocText>().text = ElementLoader.FindElementByHash(element).name;
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

	public void OnElementsFilterEdited(string new_filter)
	{
		this.filter = ((!string.IsNullOrEmpty(this.filterInput.text)) ? this.filterInput.text : null);
		this.FilterElements(this.filter);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		bool flag = false;
		if (global::UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = global::UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (TMP_InputField tmp_InputField in this.inputFields)
				{
					if (currentSelectedGameObject == tmp_InputField.gameObject)
					{
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	public void SampleCell(int cell)
	{
		this.massPressureInput.text = (Grid.Pressure[cell] * 0.010000001f).ToString();
		this.temperatureInput.text = Grid.Temperature[cell].ToString();
		this.OnSelectElement(ElementLoader.GetElementID(Grid.Element[cell].tag));
		this.OnChangeMassPressure();
		this.OnChangeTemperature();
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

	[NonSerialized]
	public int diseaseCount;

	public byte diseaseIdx;

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
	private TMP_InputField massPressureInput;

	[SerializeField]
	private TMP_InputField temperatureInput;

	[SerializeField]
	private TMP_InputField diseaseCountInput;

	[SerializeField]
	private TMP_InputField filterInput;

	[Header("Tool Buttons")]
	[SerializeField]
	private KButton paintButton;

	[SerializeField]
	private KButton fillButton;

	[SerializeField]
	private KButton sampleButton;

	[SerializeField]
	private KButton spawnButton;

	[SerializeField]
	private KButton storeButton;

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

	private List<TMP_InputField> inputFields = new List<TMP_InputField>();

	private List<string> options_list = new List<string>();

	private string filter;

	private struct ElemDisplayInfo
	{
		public SimHashes id;

		public string displayStr;
	}
}
