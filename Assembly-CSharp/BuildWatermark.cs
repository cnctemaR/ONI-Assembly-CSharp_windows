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
		string text = ((!Application.isEditor) ? ("TB-" + 246879U.ToString()) : "<EDITOR>");
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
