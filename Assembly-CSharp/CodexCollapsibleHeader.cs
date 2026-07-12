using System;
using System.Collections.Generic;
using UnityEngine;

public class CodexCollapsibleHeader : CodexWidget<CodexCollapsibleHeader>
{
	public CodexCollapsibleHeader(string label, ContentContainer contents)
	{
		this.label = label;
		this.contents = contents;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		LocText reference = component.GetReference<LocText>("Label");
		reference.text = this.label;
		reference.textStyleSetting = textStyles[CodexTextStyle.Subtitle];
		reference.ApplySettings();
		MultiToggle reference2 = component.GetReference<MultiToggle>("ExpandToggle");
		reference2.ChangeState(1);
		reference2.onClick = delegate
		{
			this.ToggleCategoryOpen(contentGameObject, !this.contents.go.activeSelf);
		};
	}

	private void ToggleCategoryOpen(GameObject header, bool open)
	{
		header.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle").ChangeState(open ? 1 : 0);
		this.contents.go.SetActive(open);
	}

	private ContentContainer contents;

	private string label;
}
