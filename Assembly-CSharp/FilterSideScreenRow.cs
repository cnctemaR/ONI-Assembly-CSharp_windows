using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FilterSideScreenRow")]
public class FilterSideScreenRow : KMonoBehaviour
{
	public new Tag tag { get; private set; }

	public bool isSelected { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.regularColor = this.outline.color;
		if (this.button != null)
		{
			this.button.onPointerEnter += delegate
			{
				if (!this.isSelected)
				{
					this.outline.color = this.outlineHighLightColor;
				}
			};
			this.button.onPointerExit += delegate
			{
				if (!this.isSelected)
				{
					this.outline.color = this.regularColor;
				}
			};
		}
	}

	public void SetTag(Tag tag)
	{
		this.tag = tag;
		this.SetText((tag == GameTags.Void) ? UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION.text : tag.ProperName());
	}

	private void SetText(string assignmentStr)
	{
		this.labelText.text = ((!string.IsNullOrEmpty(assignmentStr)) ? assignmentStr : "-");
	}

	public void SetSelected(bool selected)
	{
		this.isSelected = selected;
		this.outline.color = (selected ? this.outlineHighLightColor : this.outlineDefaultColor);
		this.BG.color = (selected ? this.BGHighLightColor : Color.white);
	}

	[SerializeField]
	private LocText labelText;

	[SerializeField]
	private Image BG;

	[SerializeField]
	private Image outline;

	[SerializeField]
	private Color outlineHighLightColor = new Color32(168, 74, 121, byte.MaxValue);

	[SerializeField]
	private Color BGHighLightColor = new Color32(168, 74, 121, 80);

	[SerializeField]
	private Color outlineDefaultColor = new Color32(204, 204, 204, byte.MaxValue);

	private Color regularColor = Color.white;

	[SerializeField]
	public KButton button;
}
