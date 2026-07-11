using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SandboxToolParameterMenu : KScreen
{
	public static void DestroyInstance()
	{
		SandboxToolParameterMenu.instance = null;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.settings = new SandboxSettings();
		SandboxSettings sandboxSettings = this.settings;
		sandboxSettings.OnChangeElement = (global::System.Action)Delegate.Combine(sandboxSettings.OnChangeElement, new global::System.Action(delegate
		{
			this.elementSelector.button.GetComponentInChildren<LocText>().text = SandboxToolParameterMenu.instance.settings.Element.name + " (" + SandboxToolParameterMenu.instance.settings.Element.GetStateString() + ")";
			Tuple<Sprite, Color> uisprite = Def.GetUISprite(this.settings.Element, "ui", false);
			this.elementSelector.button.GetComponentsInChildren<Image>()[1].sprite = uisprite.first;
			this.elementSelector.button.GetComponentsInChildren<Image>()[1].color = uisprite.second;
			this.temperatureSlider.SetRange(Mathf.Max(SandboxToolParameterMenu.instance.settings.Element.lowTemp - 10f, 1f), Mathf.Min(9999f, SandboxToolParameterMenu.instance.settings.Element.highTemp + 10f));
			this.temperatureSlider.SetValue(SandboxToolParameterMenu.instance.settings.Element.defaultValues.temperature);
			this.massSlider.SetRange(0.1f, SandboxToolParameterMenu.instance.settings.Element.defaultValues.mass * 2f);
		}));
		SandboxSettings sandboxSettings2 = this.settings;
		sandboxSettings2.OnChangeDisease = (global::System.Action)Delegate.Combine(sandboxSettings2.OnChangeDisease, new global::System.Action(delegate
		{
			this.diseaseSelector.button.GetComponentInChildren<LocText>().text = SandboxToolParameterMenu.instance.settings.Disease.Name;
			this.diseaseSelector.button.GetComponentsInChildren<Image>()[1].sprite = Assets.GetSprite("germ");
			this.diseaseCountSlider.SetRange(0f, 1000000f);
		}));
		SandboxSettings sandboxSettings3 = this.settings;
		sandboxSettings3.OnChangeEntity = (global::System.Action)Delegate.Combine(sandboxSettings3.OnChangeEntity, new global::System.Action(delegate
		{
			this.entitySelector.button.GetComponentInChildren<LocText>().text = SandboxToolParameterMenu.instance.settings.Entity.GetProperName();
			Tuple<Sprite, Color> tuple;
			if (this.settings.Entity.PrefabTag == MinionConfig.ID)
			{
				tuple = new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
			}
			else
			{
				tuple = Def.GetUISprite(this.settings.Entity.PrefabTag, "ui", false);
			}
			if (tuple != null)
			{
				this.entitySelector.button.GetComponentsInChildren<Image>()[1].sprite = tuple.first;
				this.entitySelector.button.GetComponentsInChildren<Image>()[1].color = tuple.second;
			}
		}));
		SandboxSettings sandboxSettings4 = this.settings;
		sandboxSettings4.OnChangeBrushSize = (global::System.Action)Delegate.Combine(sandboxSettings4.OnChangeBrushSize, new global::System.Action(delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(this.settings.BrushSize);
			}
		}));
		SandboxSettings sandboxSettings5 = this.settings;
		sandboxSettings5.OnChangeNoiseScale = (global::System.Action)Delegate.Combine(sandboxSettings5.OnChangeNoiseScale, new global::System.Action(delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(this.settings.BrushSize);
			}
		}));
		SandboxSettings sandboxSettings6 = this.settings;
		sandboxSettings6.OnChangeNoiseDensity = (global::System.Action)Delegate.Combine(sandboxSettings6.OnChangeNoiseDensity, new global::System.Action(delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(this.settings.BrushSize);
			}
		}));
		this.settings.InstantBuild = true;
		this.activateOnSpawn = true;
		this.ConsumeMouseScroll = true;
	}

	public void DisableParameters()
	{
		this.elementSelector.row.SetActive(false);
		this.entitySelector.row.SetActive(false);
		this.brushRadiusSlider.row.SetActive(false);
		this.noiseScaleSlider.row.SetActive(false);
		this.noiseDensitySlider.row.SetActive(false);
		this.massSlider.row.SetActive(false);
		this.temperatureAdditiveSlider.row.SetActive(false);
		this.temperatureSlider.row.SetActive(false);
		this.diseaseCountSlider.row.SetActive(false);
		this.diseaseSelector.row.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ConfigureElementSelector();
		this.ConfigureDiseaseSelector();
		this.ConfigureEntitySelector();
		this.SpawnSelector(this.entitySelector);
		this.SpawnSelector(this.elementSelector);
		this.SpawnSlider(this.brushRadiusSlider);
		this.SpawnSlider(this.noiseScaleSlider);
		this.SpawnSlider(this.noiseDensitySlider);
		this.SpawnSlider(this.massSlider);
		this.SpawnSlider(this.temperatureSlider);
		this.SpawnSlider(this.temperatureAdditiveSlider);
		this.SpawnSelector(this.diseaseSelector);
		this.SpawnSlider(this.diseaseCountSlider);
		if (SandboxToolParameterMenu.instance == null)
		{
			SandboxToolParameterMenu.instance = this;
			base.gameObject.SetActive(false);
			this.settings.SelectElement(ElementLoader.FindElementByHash(SimHashes.Water));
			this.brushRadiusSlider.SetRange(1f, 10f);
			this.brushRadiusSlider.slider.wholeNumbers = true;
			this.noiseScaleSlider.SetRange(0f, 1f);
			this.noiseDensitySlider.SetRange(0f, 20f);
			this.temperatureSlider.SetRange(Mathf.Max(SandboxToolParameterMenu.instance.settings.Element.lowTemp - 10f, 1f), SandboxToolParameterMenu.instance.settings.Element.highTemp + 10f);
			this.massSlider.SetRange(0.1f, SandboxToolParameterMenu.instance.settings.Element.defaultValues.mass * 2f);
			this.massSlider.SetValue(this.settings.Mass);
			this.settings.SelectDisease(Db.Get().Diseases.FoodPoisoning);
			this.settings.SelectEntity(Assets.GetPrefab("MushBar".ToTag()).GetComponent<KPrefabID>());
		}
	}

	private void ConfigureElementSelector()
	{
		Func<object, bool> func = (object element) => (element as Element).IsSolid;
		Func<object, bool> func2 = (object element) => (element as Element).IsLiquid;
		Func<object, bool> func3 = (object element) => (element as Element).IsGas;
		List<Element> commonElements = new List<Element>();
		Func<object, bool> func4 = (object element) => commonElements.Contains(element as Element);
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Oxygen));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Water));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Vacuum));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Dirt));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.SandStone));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Cuprite));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Algae));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.CarbonDioxide));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Sand));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.SlimeMold));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Granite));
		List<Element> list = new List<Element>();
		foreach (Element element2 in ElementLoader.elements)
		{
			if (!element2.disabled)
			{
				list.Add(element2);
			}
		}
		list.Sort((Element a, Element b) => a.name.CompareTo(b.name));
		object[] array = list.ToArray();
		Action<object> action = delegate(object element)
		{
			this.settings.SelectElement(element as Element);
		};
		Func<object, string> func5 = (object element) => (element as Element).name + " (" + (element as Element).GetStateString() + ")";
		Func<string, object, bool> func6 = (string filterString, object option) => ((option as Element).name.ToUpper() + (option as Element).GetStateString().ToUpper()).Contains(filterString.ToUpper());
		Func<object, Tuple<Sprite, Color>> func7 = (object element) => Def.GetUISprite(element as Element, "ui", false);
		SandboxToolParameterMenu.SelectorValue.SearchFilter[] array2 = new SandboxToolParameterMenu.SelectorValue.SearchFilter[4];
		array2[0] = new SandboxToolParameterMenu.SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.COMMON, func4, null, null);
		int num = 1;
		string text = UI.SANDBOXTOOLS.FILTERS.SOLID;
		Func<object, bool> func8 = func;
		Tuple<Sprite, Color> tuple = Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.SandStone), "ui", false);
		array2[num] = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func8, null, tuple);
		int num2 = 2;
		text = UI.SANDBOXTOOLS.FILTERS.LIQUID;
		func8 = func2;
		tuple = Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.Water), "ui", false);
		array2[num2] = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func8, null, tuple);
		int num3 = 3;
		text = UI.SANDBOXTOOLS.FILTERS.GAS;
		func8 = func3;
		tuple = Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.Oxygen), "ui", false);
		array2[num3] = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func8, null, tuple);
		this.elementSelector = new SandboxToolParameterMenu.SelectorValue(array, action, func5, func6, func7, array2);
	}

	private void ConfigureEntitySelector()
	{
		List<SandboxToolParameterMenu.SelectorValue.SearchFilter> list = new List<SandboxToolParameterMenu.SelectorValue.SearchFilter>();
		string text = UI.SANDBOXTOOLS.FILTERS.ENTITIES.FOOD;
		Func<object, bool> func = delegate(object entity)
		{
			string idString = (entity as KPrefabID).PrefabID().ToString();
			return !(entity as KPrefabID).HasPrefabTag(GameTags.Egg) && FOOD.FOOD_TYPES_LIST.Find((EdiblesManager.FoodInfo match) => match.Id == idString) != null;
		};
		Tuple<Sprite, Color> tuple = Def.GetUISprite(Assets.GetPrefab("MushBar"), "ui", false);
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func, null, tuple);
		list.Add(searchFilter);
		text = UI.SANDBOXTOOLS.FILTERS.ENTITIES.SPECIAL;
		func = (object entity) => (entity as KPrefabID).PrefabID().Name == MinionConfig.ID || (entity as KPrefabID).PrefabID().Name == DustCometConfig.ID || (entity as KPrefabID).PrefabID().Name == RockCometConfig.ID || (entity as KPrefabID).PrefabID().Name == IronCometConfig.ID;
		tuple = new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter2 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func, null, tuple);
		list.Add(searchFilter2);
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter3 = null;
		text = UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE;
		func = (object entity) => false;
		tuple = Def.GetUISprite(Assets.GetPrefab("Hatch"), "ui", false);
		searchFilter3 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func, null, tuple);
		list.Add(searchFilter3);
		List<Tag> list2 = new List<Tag>();
		foreach (GameObject gameObject in Assets.GetPrefabsWithTag("CreatureBrain".ToTag()))
		{
			CreatureBrain brain = gameObject.GetComponent<CreatureBrain>();
			if (!list2.Contains(brain.species))
			{
				Tuple<Sprite, Color> tuple2 = new Tuple<Sprite, Color>(CodexCache.entries[brain.species.ToString().ToUpper()].icon, CodexCache.entries[brain.species.ToString().ToUpper()].iconColor);
				list2.Add(brain.species);
				string text2 = "STRINGS.CREATURES.FAMILY_PLURAL." + brain.species.ToString().ToUpper();
				SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter4 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(Strings.Get(text2), delegate(object entity)
				{
					CreatureBrain component = Assets.GetPrefab((entity as KPrefabID).PrefabID()).GetComponent<CreatureBrain>();
					return (entity as KPrefabID).HasTag("CreatureBrain".ToString()) && component.species == brain.species;
				}, searchFilter3, tuple2);
				list.Add(searchFilter4);
			}
		}
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter5 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE_EGG, (object entity) => (entity as KPrefabID).HasPrefabTag(GameTags.Egg), searchFilter3, Def.GetUISprite(Assets.GetPrefab("HatchEgg"), "ui", false));
		list.Add(searchFilter5);
		text = UI.SANDBOXTOOLS.FILTERS.ENTITIES.EQUIPMENT;
		func = delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject2 = (entity as KPrefabID).gameObject;
			return gameObject2 != null && gameObject2.GetComponent<Equippable>() != null;
		};
		tuple = Def.GetUISprite(Assets.GetPrefab("Funky_Vest"), "ui", false);
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter6 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func, null, tuple);
		list.Add(searchFilter6);
		text = UI.SANDBOXTOOLS.FILTERS.ENTITIES.PLANTS;
		func = delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject3 = (entity as KPrefabID).gameObject;
			return gameObject3 != null && (gameObject3.GetComponent<Harvestable>() != null || gameObject3.GetComponent<WiltCondition>() != null);
		};
		tuple = Def.GetUISprite(Assets.GetPrefab("PrickleFlower"), "ui", false);
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter7 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(text, func, null, tuple);
		list.Add(searchFilter7);
		SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter8 = new SandboxToolParameterMenu.SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.SEEDS, delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject4 = (entity as KPrefabID).gameObject;
			return gameObject4 != null && gameObject4.GetComponent<PlantableSeed>() != null;
		}, searchFilter7, Def.GetUISprite(Assets.GetPrefab("PrickleFlowerSeed"), "ui", false));
		list.Add(searchFilter8);
		List<KPrefabID> list3 = new List<KPrefabID>();
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			foreach (SandboxToolParameterMenu.SelectorValue.SearchFilter searchFilter9 in list)
			{
				if (searchFilter9.condition(kprefabID))
				{
					list3.Add(kprefabID);
					break;
				}
			}
		}
		this.entitySelector = new SandboxToolParameterMenu.SelectorValue(list3.ToArray(), delegate(object entity)
		{
			this.settings.SelectEntity(entity as KPrefabID);
		}, (object entity) => (entity as KPrefabID).GetProperName(), (string filterString, object option) => (option as KPrefabID).GetProperName().ToUpper().Contains(filterString.ToUpper()), delegate(object entity)
		{
			GameObject prefab = Assets.GetPrefab((entity as KPrefabID).PrefabTag);
			if (prefab != null)
			{
				if (prefab.PrefabID() == MinionConfig.ID)
				{
					return new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
				}
				KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
				if (component2 != null && component2.AnimFiles.Length > 0)
				{
					KAnimFile kanimFile = component2.AnimFiles[0];
					if (kanimFile != null)
					{
						return Def.GetUISprite(prefab, "ui", false);
					}
				}
			}
			return null;
		}, list.ToArray());
	}

	private void ConfigureDiseaseSelector()
	{
		this.diseaseSelector = new SandboxToolParameterMenu.SelectorValue(Db.Get().Diseases.resources.ToArray(), delegate(object disease)
		{
			this.settings.SelectDisease(disease as Disease);
		}, (object disease) => (disease as Disease).Name, (string filterText, object option) => (option as Disease).Name.ToUpper().Contains(filterText.ToUpper()), (object disease) => new Tuple<Sprite, Color>(Assets.GetSprite("germ"), (disease as Disease).overlayColour), null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (PlayerController.Instance.ActiveTool != null && SandboxToolParameterMenu.instance != null)
		{
			this.RefreshDisplay();
		}
	}

	public void RefreshDisplay()
	{
		this.brushRadiusSlider.row.SetActive(PlayerController.Instance.ActiveTool is BrushTool);
		if (PlayerController.Instance.ActiveTool is BrushTool)
		{
			this.brushRadiusSlider.SetValue((float)this.settings.BrushSize);
		}
		this.massSlider.SetValue(this.settings.Mass);
		this.temperatureSlider.SetValue(this.settings.temperature);
		this.temperatureAdditiveSlider.SetValue(this.settings.temperatureAdditive);
		this.diseaseCountSlider.SetValue((float)this.settings.diseaseCount);
	}

	private GameObject SpawnSelector(SandboxToolParameterMenu.SelectorValue selector)
	{
		GameObject gameObject = Util.KInstantiateUI(this.selectorPropertyPrefab, base.gameObject, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		GameObject panel = component.GetReference("ScrollPanel").gameObject;
		GameObject gameObject2 = component.GetReference("Content").gameObject;
		InputField reference = component.GetReference<InputField>("Filter");
		KButton reference2 = component.GetReference<KButton>("Button");
		reference2.onClick += delegate
		{
			panel.SetActive(!panel.activeSelf);
			if (panel.activeSelf)
			{
				panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
			}
		};
		GameObject gameObject3 = component.GetReference("optionPrefab").gameObject;
		selector.row = gameObject;
		selector.optionButtons = new List<KeyValuePair<object, GameObject>>();
		if (selector.filters != null)
		{
			GameObject clearFilterButton = Util.KInstantiateUI(gameObject3, gameObject2, false);
			clearFilterButton.GetComponentInChildren<LocText>().text = UI.SANDBOXTOOLS.FILTERS.BACK;
			clearFilterButton.GetComponentsInChildren<Image>()[1].enabled = false;
			clearFilterButton.GetComponent<KButton>().onClick += delegate
			{
				selector.currentFilter = null;
				selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
				{
					if (test.Key is SandboxToolParameterMenu.SelectorValue.SearchFilter)
					{
						test.Value.SetActive((test.Key as SandboxToolParameterMenu.SelectorValue.SearchFilter).parentFilter == null);
					}
					else
					{
						test.Value.SetActive(false);
					}
				});
				clearFilterButton.SetActive(false);
				panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
			};
			SandboxToolParameterMenu.SelectorValue.SearchFilter[] filters = selector.filters;
			for (int i = 0; i < filters.Length; i++)
			{
				SandboxToolParameterMenu.SelectorValue.SearchFilter filter = filters[i];
				GameObject gameObject4 = Util.KInstantiateUI(gameObject3, gameObject2, false);
				gameObject4.SetActive(filter.parentFilter == null);
				gameObject4.GetComponentInChildren<LocText>().text = filter.Name;
				if (filter.icon != null)
				{
					gameObject4.GetComponentsInChildren<Image>()[1].sprite = filter.icon.first;
					gameObject4.GetComponentsInChildren<Image>()[1].color = filter.icon.second;
				}
				gameObject4.GetComponent<KButton>().onClick += delegate
				{
					selector.currentFilter = filter;
					clearFilterButton.SetActive(true);
					selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
					{
						if (!(test.Key is SandboxToolParameterMenu.SelectorValue.SearchFilter))
						{
							test.Value.SetActive(selector.runCurrentFilter(test.Key));
						}
						else if ((test.Key as SandboxToolParameterMenu.SelectorValue.SearchFilter).parentFilter == null)
						{
							test.Value.SetActive(false);
						}
						else
						{
							test.Value.SetActive((test.Key as SandboxToolParameterMenu.SelectorValue.SearchFilter).parentFilter == filter);
						}
					});
					panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
				};
				selector.optionButtons.Add(new KeyValuePair<object, GameObject>(filter, gameObject4));
			}
		}
		object[] options = selector.options;
		for (int j = 0; j < options.Length; j++)
		{
			object option = options[j];
			GameObject gameObject5 = Util.KInstantiateUI(gameObject3, gameObject2, true);
			gameObject5.GetComponentInChildren<LocText>().text = selector.getOptionName(option);
			gameObject5.GetComponent<KButton>().onClick += delegate
			{
				selector.onValueChanged(option);
				panel.SetActive(false);
			};
			Tuple<Sprite, Color> tuple = selector.getOptionSprite(option);
			gameObject5.GetComponentsInChildren<Image>()[1].sprite = tuple.first;
			gameObject5.GetComponentsInChildren<Image>()[1].color = tuple.second;
			selector.optionButtons.Add(new KeyValuePair<object, GameObject>(option, gameObject5));
			if (option is SandboxToolParameterMenu.SelectorValue.SearchFilter)
			{
				gameObject5.SetActive((option as SandboxToolParameterMenu.SelectorValue.SearchFilter).parentFilter == null);
			}
			else
			{
				gameObject5.SetActive(false);
			}
		}
		selector.button = reference2;
		reference.onValueChanged.AddListener(delegate(string filterString)
		{
			List<KeyValuePair<object, GameObject>> list = new List<KeyValuePair<object, GameObject>>();
			selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
			{
				if (test.Key is SandboxToolParameterMenu.SelectorValue.SearchFilter)
				{
					test.Value.SetActive((test.Key as SandboxToolParameterMenu.SelectorValue.SearchFilter).Name.ToUpper().Contains(filterString.ToUpper()));
				}
			});
			object[] options2 = selector.options;
			for (int k = 0; k < options2.Length; k++)
			{
				object option = options2[k];
				list = selector.optionButtons.FindAll((KeyValuePair<object, GameObject> match) => match.Key == option);
				foreach (KeyValuePair<object, GameObject> keyValuePair in list)
				{
					if (filterString == string.Empty)
					{
						keyValuePair.Value.SetActive(false);
					}
					else
					{
						keyValuePair.Value.SetActive(selector.filterOptionFunction(filterString, option));
					}
				}
			}
			panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
		});
		this.inputFields.Add(reference.gameObject);
		panel.SetActive(false);
		return gameObject;
	}

	private GameObject SpawnSlider(SandboxToolParameterMenu.SliderValue value)
	{
		GameObject gameObject = Util.KInstantiateUI(this.sliderPropertyPrefab, base.gameObject, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("BottomIcon").sprite = Assets.GetSprite(value.bottomSprite);
		component.GetReference<Image>("TopIcon").sprite = Assets.GetSprite(value.topSprite);
		KSlider slider = component.GetReference<KSlider>("Slider");
		KNumberInputField inputField = component.GetReference<KNumberInputField>("InputField");
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(value.tooltip);
		slider.minValue = value.minValue;
		slider.maxValue = value.maxValue;
		inputField.minValue = 0f;
		inputField.maxValue = 99999f;
		this.inputFields.Add(inputField.gameObject);
		value.slider = slider;
		value.inputField = inputField;
		value.row = gameObject;
		slider.onReleaseHandle += delegate
		{
			slider.value = Mathf.Round(slider.value * 10f) / 10f;
			inputField.currentValue = slider.value;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		slider.onDrag += delegate
		{
			slider.value = Mathf.Round(slider.value * 10f) / 10f;
			inputField.currentValue = slider.value;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		slider.onMove += delegate
		{
			slider.value = Mathf.Round(slider.value * 10f) / 10f;
			inputField.currentValue = slider.value;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		inputField.onEndEdit += delegate
		{
			float num = Mathf.Clamp(Mathf.Round(inputField.currentValue), inputField.minValue, inputField.maxValue);
			inputField.SetDisplayValue(num.ToString());
			slider.value = Mathf.Round(num);
			if (value.onValueChanged != null)
			{
				value.onValueChanged(num);
			}
		};
		component.GetReference<LocText>("UnitLabel").text = value.unitString;
		return gameObject;
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

	private bool CheckBlockedInput()
	{
		bool flag = false;
		if (global::UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = global::UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (GameObject gameObject in this.inputFields)
				{
					if (currentSelectedGameObject == gameObject.gameObject)
					{
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	public static SandboxToolParameterMenu instance;

	public SandboxSettings settings;

	[SerializeField]
	private GameObject sliderPropertyPrefab;

	[SerializeField]
	private GameObject selectorPropertyPrefab;

	private List<GameObject> inputFields = new List<GameObject>();

	public SandboxToolParameterMenu.SelectorValue elementSelector;

	public SandboxToolParameterMenu.SliderValue brushRadiusSlider = new SandboxToolParameterMenu.SliderValue(1f, 10f, "dash", "circle_hard", string.Empty, UI.SANDBOXTOOLS.SETTINGS.BRUSH_SIZE.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.BrushSize = Mathf.RoundToInt(value);
	});

	public SandboxToolParameterMenu.SliderValue noiseScaleSlider = new SandboxToolParameterMenu.SliderValue(0f, 1f, "little", "lots", string.Empty, UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.NoiseScale = value;
	});

	public SandboxToolParameterMenu.SliderValue noiseDensitySlider = new SandboxToolParameterMenu.SliderValue(1f, 20f, "little", "lots", string.Empty, UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.NoiseDensity = value;
	});

	public SandboxToolParameterMenu.SliderValue massSlider = new SandboxToolParameterMenu.SliderValue(0.1f, 1000f, "action_pacify", "status_item_plant_solid", UI.UNITSUFFIXES.MASS.KILOGRAM, UI.SANDBOXTOOLS.SETTINGS.MASS.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.Mass = (float)Mathf.RoundToInt(value * 10000f) / 10000f;
	});

	public SandboxToolParameterMenu.SliderValue temperatureSlider = new SandboxToolParameterMenu.SliderValue(150f, 500f, "cold", "hot", UI.UNITSUFFIXES.TEMPERATURE.KELVIN, UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.temperature = Mathf.Clamp((float)Mathf.RoundToInt(value * 100f) / 100f, 1f, 9999f);
	});

	public SandboxToolParameterMenu.SliderValue temperatureAdditiveSlider = new SandboxToolParameterMenu.SliderValue(-15f, 15f, "cold", "hot", UI.UNITSUFFIXES.TEMPERATURE.KELVIN, UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE_ADDITIVE.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.temperatureAdditive = (float)Mathf.RoundToInt(value * 100f) / 100f;
	});

	public SandboxToolParameterMenu.SelectorValue diseaseSelector;

	public SandboxToolParameterMenu.SliderValue diseaseCountSlider = new SandboxToolParameterMenu.SliderValue(0f, 10000f, "status_item_barren", "germ", UI.UNITSUFFIXES.DISEASE.UNITS, UI.SANDBOXTOOLS.SETTINGS.DISEASE_COUNT.TOOLTIP, delegate(float value)
	{
		SandboxToolParameterMenu.instance.settings.diseaseCount = Mathf.RoundToInt(value);
	});

	public SandboxToolParameterMenu.SelectorValue entitySelector;

	public class SelectorValue
	{
		public SelectorValue(object[] options, Action<object> onValueChanged, Func<object, string> getOptionName, Func<string, object, bool> filterOptionFunction, Func<object, Tuple<Sprite, Color>> getOptionSprite, SandboxToolParameterMenu.SelectorValue.SearchFilter[] filters = null)
		{
			this.options = options;
			this.onValueChanged = onValueChanged;
			this.getOptionName = getOptionName;
			this.filterOptionFunction = filterOptionFunction;
			this.getOptionSprite = getOptionSprite;
			this.filters = filters;
		}

		public bool runCurrentFilter(object obj)
		{
			return this.currentFilter == null || this.currentFilter.condition(obj);
		}

		public GameObject row;

		public List<KeyValuePair<object, GameObject>> optionButtons;

		public KButton button;

		public object[] options;

		public Action<object> onValueChanged;

		public Func<object, string> getOptionName;

		public Func<string, object, bool> filterOptionFunction;

		public Func<object, Tuple<Sprite, Color>> getOptionSprite;

		public SandboxToolParameterMenu.SelectorValue.SearchFilter[] filters;

		public List<SandboxToolParameterMenu.SelectorValue.SearchFilter> activeFilters = new List<SandboxToolParameterMenu.SelectorValue.SearchFilter>();

		public SandboxToolParameterMenu.SelectorValue.SearchFilter currentFilter;

		public class SearchFilter
		{
			public SearchFilter(string Name, Func<object, bool> condition, SandboxToolParameterMenu.SelectorValue.SearchFilter parentFilter = null, Tuple<Sprite, Color> icon = null)
			{
				this.Name = Name;
				this.condition = condition;
				this.parentFilter = parentFilter;
				this.icon = icon;
			}

			public string Name;

			public Func<object, bool> condition;

			public SandboxToolParameterMenu.SelectorValue.SearchFilter parentFilter;

			public Tuple<Sprite, Color> icon;
		}
	}

	public class SliderValue
	{
		public SliderValue(float minValue, float maxValue, string bottomSprite, string topSprite, string unitString, string tooltip, Action<float> onValueChanged)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.bottomSprite = bottomSprite;
			this.topSprite = topSprite;
			this.unitString = unitString;
			this.onValueChanged = onValueChanged;
			this.tooltip = tooltip;
		}

		public void SetRange(float min, float max)
		{
			this.minValue = min;
			this.maxValue = max;
			this.slider.minValue = this.minValue;
			this.slider.maxValue = this.maxValue;
			this.inputField.currentValue = this.minValue + (this.maxValue - this.minValue) / 2f;
			this.inputField.SetDisplayValue(this.inputField.currentValue.ToString());
			this.slider.value = this.minValue + (this.maxValue - this.minValue) / 2f;
			this.onValueChanged(this.minValue + (this.maxValue - this.minValue) / 2f);
		}

		public void SetValue(float value)
		{
			this.slider.value = value;
			this.inputField.currentValue = value;
			this.onValueChanged(value);
			this.RefreshDisplay();
		}

		public void RefreshDisplay()
		{
			this.inputField.SetDisplayValue(this.inputField.currentValue.ToString());
		}

		public GameObject row;

		public string bottomSprite;

		public string topSprite;

		public float minValue;

		public float maxValue;

		public string unitString;

		public Action<float> onValueChanged;

		public string tooltip;

		public KSlider slider;

		public KNumberInputField inputField;
	}
}
