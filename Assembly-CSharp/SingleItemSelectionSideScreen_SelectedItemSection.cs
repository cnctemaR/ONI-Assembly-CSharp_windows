using System;
using STRINGS;
using UnityEngine;

public class SingleItemSelectionSideScreen_SelectedItemSection : KMonoBehaviour
{
	public Tag Item { get; private set; }

	public void Clear()
	{
		this.SetItem(null);
	}

	public void SetItem(Tag item)
	{
		this.Item = item;
		if (this.Item != GameTags.Void)
		{
			this.SetTitleText(UI.UISIDESCREENS.SINGLEITEMSELECTIONSIDESCREEN.CURRENT_ITEM_SELECTED_SECTION.TITLE);
			this.SetContentText(this.Item.ProperName());
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(this.Item, "ui", false);
			this.SetImage(uisprite.first, uisprite.second);
			return;
		}
		this.SetTitleText(UI.UISIDESCREENS.SINGLEITEMSELECTIONSIDESCREEN.CURRENT_ITEM_SELECTED_SECTION.NO_ITEM_TITLE);
		this.SetContentText(UI.UISIDESCREENS.SINGLEITEMSELECTIONSIDESCREEN.CURRENT_ITEM_SELECTED_SECTION.NO_ITEM_MESSAGE);
		this.SetImage(null, Color.white);
	}

	private void SetTitleText(string text)
	{
		this.title.text = text;
	}

	private void SetContentText(string text)
	{
		this.contentText.text = text;
	}

	private void SetImage(Sprite sprite, Color color)
	{
		this.image.sprite = sprite;
		this.image.color = color;
		this.image.gameObject.SetActive(sprite != null);
	}

	[Header("References")]
	[SerializeField]
	private LocText title;

	[SerializeField]
	private LocText contentText;

	[SerializeField]
	private KImage image;
}
