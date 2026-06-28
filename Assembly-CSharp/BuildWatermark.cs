using System;
using STRINGS;
using UnityEngine;

public class BuildWatermark : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		string text = ((!Application.isEditor) ? 208689U.ToString() : "<EDITOR>");
		this.textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.BUILDWATERMARK, text));
	}

	private void Update()
	{
		if (this.transform.GetSiblingIndex() != this.transform.parent.childCount - 1)
		{
			this.transform.SetAsLastSibling();
		}
	}

	public LocText textDisplay;
}
