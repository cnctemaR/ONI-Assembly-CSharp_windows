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
		string text = ((!Application.isEditor) ? ("AU-" + 221865U.ToString()) : "<EDITOR>");
		this.textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.WATERMARK, text));
	}

	private void Update()
	{
		if (this.transform.GetSiblingIndex() != this.transform.parent.childCount - 1)
		{
			this.transform.SetAsLastSibling();
		}
	}

	public LocText textDisplay;

	public static BuildWatermark Instance;
}
