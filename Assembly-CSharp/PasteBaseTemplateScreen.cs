using System;
using System.Collections.Generic;
using UnityEngine;

public class PasteBaseTemplateScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		PasteBaseTemplateScreen.Instance = this;
		TemplateCache.Init();
		this.ConsumeMouseScroll = true;
		this.RefreshStampButtons();
	}

	public void RefreshStampButtons()
	{
		foreach (GameObject gameObject in this.template_buttons)
		{
			global::UnityEngine.Object.Destroy(gameObject);
		}
		this.template_buttons.Clear();
		this.base_template_assets = TemplateCache.CollectBaseTemplateNames("bases/");
		this.base_template_assets.AddRange(TemplateCache.CollectBaseTemplateNames("poi/"));
		this.base_template_assets.AddRange(TemplateCache.CollectBaseTemplateNames(""));
		foreach (string text in this.base_template_assets)
		{
			GameObject gameObject2 = Util.KInstantiateUI(this.prefab_paste_button, this.button_list_container, true);
			KButton component = gameObject2.GetComponent<KButton>();
			string template_name = text;
			component.onClick += delegate
			{
				this.OnClickPasteButton(template_name);
			};
			LocText componentInChildren = gameObject2.GetComponentInChildren<LocText>();
			componentInChildren.text = template_name;
			this.template_buttons.Add(gameObject2);
		}
	}

	private void OnClickPasteButton(string template_name)
	{
		if (template_name != null)
		{
			DebugTool.Instance.DeactivateTool(null);
			DebugBaseTemplateButton.Instance.ClearSelection();
			DebugBaseTemplateButton.Instance.nameField.text = template_name;
			TemplateContainer template = TemplateCache.GetTemplate(template_name);
			StampTool.Instance.Activate(template, true, false);
		}
	}

	public static PasteBaseTemplateScreen Instance;

	public GameObject button_list_container;

	public GameObject prefab_paste_button;

	private List<string> base_template_assets;

	private List<GameObject> template_buttons = new List<GameObject>();
}
