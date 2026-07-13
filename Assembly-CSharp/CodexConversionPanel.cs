using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexConversionPanel : CodexWidget<CodexConversionPanel>
{
	public CodexConversionPanel(string title, Tag ctag, float inputAmount, bool inputContinuous, Tag ptag, float outputAmount, bool outputContinuous, GameObject converter)
		: this(title, ctag, inputAmount, inputContinuous, null, ptag, outputAmount, outputContinuous, null, converter)
	{
	}

	public CodexConversionPanel(string title, Tag ctag, float inputAmount, bool inputContinuous, Func<Tag, float, bool, string> input_customFormating, Tag ptag, float outputAmount, bool outputContinuous, Func<Tag, float, bool, string> output_customFormating, GameObject converter)
	{
		this.title = title;
		this.ins = new ElementUsage[]
		{
			new ElementUsage(ctag, inputAmount, inputContinuous, input_customFormating)
		};
		this.outs = new ElementUsage[]
		{
			new ElementUsage(ptag, outputAmount, outputContinuous, output_customFormating)
		};
		this.Converter = converter;
	}

	public CodexConversionPanel(string title, ElementUsage[] ins, ElementUsage[] outs, GameObject converter)
		: this(title, ins, outs, converter, null)
	{
	}

	public CodexConversionPanel(string title, ElementUsage[] ins, ElementUsage[] outs, GameObject converter, CodexConversionPanel.IconSettings aidIcon)
	{
		this.title = title;
		this.ins = ((ins != null) ? ins : new ElementUsage[0]);
		this.outs = ((outs != null) ? outs : new ElementUsage[0]);
		this.Converter = converter;
		this.aidIcon = aidIcon;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		this.label = component.GetReference<LocText>("Title");
		this.materialPrefab = component.GetReference<RectTransform>("MaterialPrefab").gameObject;
		this.fabricatorPrefab = component.GetReference<RectTransform>("FabricatorPrefab").gameObject;
		this.ingredientsContainer = component.GetReference<RectTransform>("IngredientsContainer").gameObject;
		this.resultsContainer = component.GetReference<RectTransform>("ResultsContainer").gameObject;
		this.fabricatorContainer = component.GetReference<RectTransform>("FabricatorContainer").gameObject;
		this.arrow1 = component.GetReference<RectTransform>("Arrow1").gameObject;
		this.arrow2 = component.GetReference<RectTransform>("Arrow2").gameObject;
		this.ClearPanel();
		this.ConfigureConversion();
	}

	private global::Tuple<Sprite, Color> GetUISprite(Tag tag)
	{
		if (ElementLoader.GetElement(tag) != null)
		{
			return Def.GetUISprite(ElementLoader.GetElement(tag), "ui", false);
		}
		if (Assets.GetPrefab(tag) != null)
		{
			return Def.GetUISprite(Assets.GetPrefab(tag), "ui", false);
		}
		if (Assets.GetSprite(tag.Name) != null)
		{
			return new global::Tuple<Sprite, Color>(Assets.GetSprite(tag.Name), Color.white);
		}
		return Def.GetUISprite(tag, "ui", false);
	}

	private void ConfigureConversion()
	{
		this.label.text = this.title;
		bool flag = false;
		ElementUsage[] array = this.ins;
		for (int i = 0; i < array.Length; i++)
		{
			ElementUsage elementUsage = array[i];
			Tag tag2 = elementUsage.tag;
			if (!(tag2 == Tag.Invalid))
			{
				float amount = elementUsage.amount;
				flag = true;
				HierarchyReferences component = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true).GetComponent<HierarchyReferences>();
				global::Tuple<Sprite, Color> uisprite = this.GetUISprite(tag2);
				if (uisprite != null)
				{
					component.GetReference<Image>("Icon").sprite = uisprite.first;
					component.GetReference<Image>("Icon").color = uisprite.second;
				}
				GameUtil.TimeSlice timeSlice = (elementUsage.continuous ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None);
				component.GetReference<LocText>("Amount").text = ((elementUsage.customFormating == null) ? GameUtil.GetFormattedByTag(tag2, amount, timeSlice) : elementUsage.customFormating(tag2, amount, elementUsage.continuous));
				component.GetReference<LocText>("Amount").color = Color.black;
				string text = tag2.ProperName();
				GameObject prefab = Assets.GetPrefab(tag2);
				if (prefab && prefab.GetComponent<Edible>() != null)
				{
					text = text + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
				}
				component.GetReference<ToolTip>("Tooltip").toolTip = text;
				component.GetReference<KButton>("Button").onClick += delegate
				{
					ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(tag2.ProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
				};
			}
		}
		this.arrow1.SetActive(flag);
		string name = this.Converter.PrefabID().Name;
		HierarchyReferences component2 = Util.KInstantiateUI(this.fabricatorPrefab, this.fabricatorContainer, true).GetComponent<HierarchyReferences>();
		global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(name, "ui", false);
		component2.GetReference<Image>("Icon").sprite = uisprite2.first;
		component2.GetReference<Image>("Icon").color = uisprite2.second;
		component2.GetReference<ToolTip>("Tooltip").toolTip = this.Converter.GetProperName();
		component2.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(this.Converter.GetProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
		};
		Image reference = component2.GetReference<Image>("AidIconIcon");
		reference.gameObject.SetActive(this.aidIcon != null);
		if (this.aidIcon != null)
		{
			global::Tuple<Sprite, Color> uisprite3 = Def.GetUISprite(this.aidIcon.spriteName, "ui", false);
			reference.sprite = uisprite3.first;
			reference.color = uisprite3.second;
			component2.GetReference<ToolTip>("AidIconTooltip").toolTip = this.aidIcon.tooltip;
			component2.GetReference<KButton>("AidIconButton").onClick += this.aidIcon.onClickActions;
		}
		bool flag2 = false;
		array = this.outs;
		for (int i = 0; i < array.Length; i++)
		{
			ElementUsage elementUsage2 = array[i];
			Tag tag = elementUsage2.tag;
			if (!(tag == Tag.Invalid))
			{
				float amount2 = elementUsage2.amount;
				flag2 = true;
				HierarchyReferences component3 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true).GetComponent<HierarchyReferences>();
				global::Tuple<Sprite, Color> uisprite4 = this.GetUISprite(tag);
				if (uisprite4 != null)
				{
					component3.GetReference<Image>("Icon").sprite = uisprite4.first;
					component3.GetReference<Image>("Icon").color = uisprite4.second;
				}
				GameUtil.TimeSlice timeSlice2 = (elementUsage2.continuous ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None);
				component3.GetReference<LocText>("Amount").text = ((elementUsage2.customFormating == null) ? GameUtil.GetFormattedByTag(tag, amount2, timeSlice2) : elementUsage2.customFormating(tag, amount2, elementUsage2.continuous));
				component3.GetReference<LocText>("Amount").color = Color.black;
				string text2 = tag.ProperName();
				GameObject prefab2 = Assets.GetPrefab(tag);
				if (prefab2 && prefab2.GetComponent<Edible>() != null)
				{
					text2 = text2 + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab2.GetComponent<Edible>().GetQuality()));
				}
				component3.GetReference<ToolTip>("Tooltip").toolTip = text2;
				component3.GetReference<KButton>("Button").onClick += delegate
				{
					ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(tag.ProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
				};
			}
		}
		this.arrow2.SetActive(flag2);
	}

	private void ClearPanel()
	{
		foreach (object obj in this.ingredientsContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (object obj2 in this.resultsContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
		foreach (object obj3 in this.fabricatorContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj3).gameObject);
		}
	}

	private LocText label;

	private GameObject materialPrefab;

	private GameObject fabricatorPrefab;

	private GameObject ingredientsContainer;

	private GameObject resultsContainer;

	private GameObject fabricatorContainer;

	private GameObject arrow1;

	private GameObject arrow2;

	private string title;

	private ElementUsage[] ins;

	private ElementUsage[] outs;

	private GameObject Converter;

	public CodexConversionPanel.IconSettings aidIcon;

	public class IconSettings
	{
		public string spriteName;

		public string tooltip;

		public global::System.Action onClickActions;
	}
}
