using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SingleItemSelectionRow : KMonoBehaviour
{
	public virtual string InvalidTagTitle
	{
		get
		{
			return UI.UISIDESCREENS.SINGLEITEMSELECTIONSIDESCREEN.NO_SELECTION;
		}
	}

	public Tag InvalidTag { get; protected set; } = GameTags.Void;

	public new Tag tag { get; protected set; }

	public bool IsVisible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	public bool IsSelected { get; protected set; }

	protected override void OnPrefabInit()
	{
		this.regularColor = this.outline.color;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.button != null)
		{
			this.button.onPointerEnter += delegate
			{
				if (!this.IsSelected)
				{
					this.outline.color = this.outlineHighLightColor;
				}
			};
			this.button.onPointerExit += delegate
			{
				if (!this.IsSelected)
				{
					this.outline.color = this.regularColor;
				}
			};
			this.button.onClick += this.OnItemClicked;
		}
	}

	public virtual void SetVisibleState(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
	}

	protected virtual void OnItemClicked()
	{
		Action<SingleItemSelectionRow> clicked = this.Clicked;
		if (clicked == null)
		{
			return;
		}
		clicked(this);
	}

	public virtual void SetTag(Tag tag)
	{
		this.tag = tag;
		this.SetText((tag == this.InvalidTag) ? this.InvalidTagTitle : tag.ProperName());
		if (tag != this.InvalidTag)
		{
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(tag, "ui", false);
			this.SetIcon(uisprite.first, uisprite.second);
			return;
		}
		this.SetIcon(null, Color.white);
	}

	protected virtual void SetText(string assignmentStr)
	{
		this.labelText.text = ((!string.IsNullOrEmpty(assignmentStr)) ? assignmentStr : "-");
	}

	public virtual void SetSelected(bool selected)
	{
		this.IsSelected = selected;
		this.outline.color = (selected ? this.outlineHighLightColor : this.outlineDefaultColor);
		this.BG.color = (selected ? this.BGHighLightColor : Color.white);
	}

	protected virtual void SetIcon(Sprite sprite, Color color)
	{
		this.icon.sprite = sprite;
		this.icon.color = color;
		this.icon.gameObject.SetActive(sprite != null);
	}

	[SerializeField]
	protected Image icon;

	[SerializeField]
	protected LocText labelText;

	[SerializeField]
	protected Image BG;

	[SerializeField]
	protected Image outline;

	[SerializeField]
	protected Color outlineHighLightColor = new Color32(168, 74, 121, byte.MaxValue);

	[SerializeField]
	protected Color BGHighLightColor = new Color32(168, 74, 121, 80);

	[SerializeField]
	protected Color outlineDefaultColor = new Color32(204, 204, 204, byte.MaxValue);

	protected Color regularColor = Color.white;

	[SerializeField]
	public KButton button;

	public Action<SingleItemSelectionRow> Clicked;
}
