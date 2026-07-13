using System;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexTemperatureTransitionPanel : CodexWidget<CodexTemperatureTransitionPanel>
{
	public CodexTemperatureTransitionPanel(Element source, CodexTemperatureTransitionPanel.TransitionType type)
	{
		this.sourceElement = source;
		this.transitionType = type;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		this.materialPrefab = component.GetReference<RectTransform>("MaterialPrefab").gameObject;
		this.sourceContainer = component.GetReference<RectTransform>("SourceContainer").gameObject;
		this.temperaturePanel = component.GetReference<RectTransform>("TemperaturePanel").gameObject;
		this.resultsContainer = component.GetReference<RectTransform>("ResultsContainer").gameObject;
		this.headerLabel = component.GetReference<LocText>("HeaderLabel");
		this.ClearPanel();
		this.ConfigureSource(contentGameObject, displayPane, textStyles);
		this.ConfigureTemperature(contentGameObject, displayPane, textStyles);
		this.ConfigureResults(contentGameObject, displayPane, textStyles);
	}

	private void ConfigureSource(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = Util.KInstantiateUI(this.materialPrefab, this.sourceContainer, true).GetComponent<HierarchyReferences>();
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(this.sourceElement, "ui", false);
		component.GetReference<Image>("Icon").sprite = uisprite.first;
		component.GetReference<Image>("Icon").color = uisprite.second;
		component.GetReference<LocText>("Title").text = string.Format("{0}", GameUtil.GetFormattedMass(1f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		component.GetReference<LocText>("Title").color = Color.black;
		component.GetReference<ToolTip>("ToolTip").toolTip = this.sourceElement.name;
		component.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(this.sourceElement.tag.ProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
		};
	}

	private void ConfigureTemperature(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		float num = ((this.transitionType == CodexTemperatureTransitionPanel.TransitionType.COOL) ? this.sourceElement.lowTemp : this.sourceElement.highTemp);
		HierarchyReferences component = this.temperaturePanel.GetComponent<HierarchyReferences>();
		Sprite sprite = null;
		Color color = default(Color);
		string text = GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
		string text2 = "";
		switch (this.transitionType)
		{
		case CodexTemperatureTransitionPanel.TransitionType.HEAT:
			sprite = Assets.GetSprite("crew_state_temp_up");
			color = Color.red;
			text2 = GameUtil.SafeStringFormat(CODEX.FORMAT_STRINGS.TEMPERATURE_OVER, new object[] { GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false) });
			break;
		case CodexTemperatureTransitionPanel.TransitionType.COOL:
			sprite = Assets.GetSprite("crew_state_temp_down");
			color = Color.blue;
			text2 = GameUtil.SafeStringFormat(CODEX.FORMAT_STRINGS.TEMPERATURE_UNDER, new object[] { GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false) });
			break;
		case CodexTemperatureTransitionPanel.TransitionType.SUBLIMATE:
			sprite = Assets.GetSprite("codex_sublimation");
			color = CodexTemperatureTransitionPanel.SUBLIMATE_TEXT_COLOR;
			text = CODEX.FORMAT_STRINGS.SUBLIMATION_NAME;
			text2 = GameUtil.SafeStringFormat(CODEX.FORMAT_STRINGS.SUBLIMATION_TRESHOLD, new object[] { GameUtil.GetFormattedMass(1.8f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") });
			break;
		case CodexTemperatureTransitionPanel.TransitionType.OFFGASS:
			sprite = Assets.GetSprite("codex_offgas");
			color = CodexTemperatureTransitionPanel.OFFGASS_TEXT_COLOR;
			text = CODEX.FORMAT_STRINGS.OFFGASS_NAME;
			text2 = GameUtil.SafeStringFormat(CODEX.FORMAT_STRINGS.OFFGASS_TRESHOLD, new object[] { GameUtil.GetFormattedMass(1.8f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") });
			break;
		}
		component.GetReference<Image>("Icon").sprite = sprite;
		LocText reference = component.GetReference<LocText>("Label");
		reference.text = text;
		reference.textWrappingMode = TextWrappingModes.NoWrap;
		reference.gameObject.SetActive(text != null);
		component.GetReference<LocText>("Label").color = color;
		component.GetReference<ToolTip>("ToolTip").toolTip = text2;
	}

	private void ConfigureResults(GameObject contentGameObject, Transform displayPanel, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		Element primaryElement = null;
		Element secondaryElement = null;
		float num = 1f;
		float num2 = 0f;
		switch (this.transitionType)
		{
		case CodexTemperatureTransitionPanel.TransitionType.HEAT:
			primaryElement = this.sourceElement.highTempTransition;
			secondaryElement = ElementLoader.FindElementByHash(this.sourceElement.highTempTransitionOreID);
			num2 = this.sourceElement.highTempTransitionOreMassConversion;
			break;
		case CodexTemperatureTransitionPanel.TransitionType.COOL:
			primaryElement = this.sourceElement.lowTempTransition;
			secondaryElement = ElementLoader.FindElementByHash(this.sourceElement.lowTempTransitionOreID);
			num2 = this.sourceElement.lowTempTransitionOreMassConversion;
			break;
		case CodexTemperatureTransitionPanel.TransitionType.SUBLIMATE:
			primaryElement = ElementLoader.FindElementByHash(this.sourceElement.sublimateId);
			secondaryElement = null;
			num2 = this.sourceElement.sublimateRate;
			num = this.sourceElement.sublimateEfficiency;
			if (primaryElement == null)
			{
				GameObject prefab = Assets.GetPrefab(this.sourceElement.id.CreateTag());
				if (prefab != null)
				{
					Sublimates component = prefab.GetComponent<Sublimates>();
					if (component != null)
					{
						primaryElement = ElementLoader.FindElementByHash(component.info.sublimatedElement);
						num2 = component.info.sublimationRate;
						num = component.info.massPower;
					}
				}
			}
			break;
		case CodexTemperatureTransitionPanel.TransitionType.OFFGASS:
			primaryElement = ElementLoader.FindElementByHash(this.sourceElement.sublimateId);
			secondaryElement = null;
			num2 = this.sourceElement.offGasPercentage;
			num = this.sourceElement.offGasPercentage;
			break;
		}
		HierarchyReferences component2 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true).GetComponent<HierarchyReferences>();
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(primaryElement, "ui", false);
		component2.GetReference<Image>("Icon").sprite = uisprite.first;
		component2.GetReference<Image>("Icon").color = uisprite.second;
		string text = string.Format("{0}", GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		if (secondaryElement != null)
		{
			text = string.Format("{0}", GameUtil.GetFormattedMass(num - num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		}
		component2.GetReference<LocText>("Title").text = text;
		component2.GetReference<LocText>("Title").color = Color.black;
		component2.GetReference<ToolTip>("ToolTip").toolTip = primaryElement.name;
		component2.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(primaryElement.tag.ProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
		};
		if (secondaryElement != null)
		{
			HierarchyReferences component3 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true).GetComponent<HierarchyReferences>();
			global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(secondaryElement, "ui", false);
			component3.GetReference<Image>("Icon").sprite = uisprite2.first;
			component3.GetReference<Image>("Icon").color = uisprite2.second;
			component3.GetReference<LocText>("Title").text = string.Format("{0} {1}", GameUtil.GetFormattedMass(num2 * num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), secondaryElement.name);
			component3.GetReference<LocText>("Title").color = Color.black;
			component3.GetReference<ToolTip>("ToolTip").toolTip = secondaryElement.name;
			component3.GetReference<KButton>("Button").onClick += delegate
			{
				ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(secondaryElement.tag.ProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			};
		}
		this.headerLabel.SetText((secondaryElement == null) ? string.Format(CODEX.FORMAT_STRINGS.TRANSITION_LABEL_TO_ONE_ELEMENT, this.sourceElement.name, primaryElement.name) : string.Format(CODEX.FORMAT_STRINGS.TRANSITION_LABEL_TO_TWO_ELEMENTS, this.sourceElement.name, primaryElement.name, secondaryElement.name));
	}

	private void ClearPanel()
	{
		foreach (object obj in this.sourceContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (object obj2 in this.resultsContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
	}

	private Element sourceElement;

	private CodexTemperatureTransitionPanel.TransitionType transitionType;

	private static readonly Color SUBLIMATE_TEXT_COLOR = new Color(0.23137255f, 0.56078434f, 0.6666667f, 1f);

	private static readonly Color OFFGASS_TEXT_COLOR = new Color(0f, 0.2901961f, 0.38431373f, 1f);

	private GameObject materialPrefab;

	private GameObject sourceContainer;

	private GameObject temperaturePanel;

	private GameObject resultsContainer;

	private LocText headerLabel;

	public enum TransitionType
	{
		HEAT,
		COOL,
		SUBLIMATE,
		OFFGASS
	}
}
