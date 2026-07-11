using System;
using STRINGS;
using UnityEngine;

public class BuildWatermark : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		BuildWatermark.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshText();
	}

	public void RefreshText()
	{
		string text = "RP-";
		if (Application.isEditor)
		{
			text += "<EDITOR>";
		}
		else
		{
			text += 393231U.ToString();
			if (DebugHandler.enabled)
			{
				text += "-D";
			}
		}
		this.textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.WATERMARK, text));
	}

	private void Update()
	{
		if (base.transform.GetSiblingIndex() != base.transform.parent.childCount - 1)
		{
			base.transform.SetAsLastSibling();
		}
	}

	public LocText textDisplay;

	public static BuildWatermark Instance;
}
