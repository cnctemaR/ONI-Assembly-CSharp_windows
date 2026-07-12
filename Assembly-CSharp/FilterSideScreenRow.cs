using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FilterSideScreenRow")]
public class FilterSideScreenRow : SingleItemSelectionRow
{
	public override string InvalidTagTitle
	{
		get
		{
			return UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION;
		}
	}

	protected override void SetIcon(Sprite sprite, Color color)
	{
		if (this.icon != null)
		{
			this.icon.gameObject.SetActive(false);
		}
	}
}
