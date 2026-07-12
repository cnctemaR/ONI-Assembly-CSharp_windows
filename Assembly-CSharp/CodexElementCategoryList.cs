using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexElementCategoryList : CodexCollapsibleHeader
{
	public Tag categoryTag { get; set; }

	public CodexElementCategoryList()
		: base(UI.CODEX.CATEGORYNAMES.ELEMENTS, null)
	{
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		base.ContentsGameObject = component.GetReference<RectTransform>("ContentContainer").gameObject;
		base.Configure(contentGameObject, displayPane, textStyles);
		Component reference = component.GetReference<RectTransform>("HeaderLabel");
		RectTransform reference2 = component.GetReference<RectTransform>("PrefabLabelWithIcon");
		this.ClearPanel(reference2.transform.parent, reference2);
		reference.GetComponent<LocText>().SetText(UI.CODEX.CATEGORYNAMES.ELEMENTS);
		foreach (Element element in ElementLoader.elements)
		{
			if (element.HasTag(this.categoryTag) && !element.disabled)
			{
				GameObject gameObject = Util.KInstantiateUI(reference2.gameObject, reference2.parent.gameObject, true);
				Image componentInChildren = gameObject.GetComponentInChildren<Image>();
				global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(element, "ui", false);
				componentInChildren.sprite = uisprite.first;
				componentInChildren.color = uisprite.second;
				gameObject.GetComponentInChildren<LocText>().SetText(element.tag.ProperName());
				this.rows.Add(gameObject);
			}
		}
	}

	private void ClearPanel(Transform containerToClear, Transform skipDestroyingPrefab)
	{
		skipDestroyingPrefab.SetAsFirstSibling();
		for (int i = containerToClear.childCount - 1; i >= 1; i--)
		{
			global::UnityEngine.Object.Destroy(containerToClear.GetChild(i).gameObject);
		}
		for (int j = this.rows.Count - 1; j >= 0; j--)
		{
			global::UnityEngine.Object.Destroy(this.rows[j].gameObject);
		}
		this.rows.Clear();
	}

	private List<GameObject> rows = new List<GameObject>();
}
