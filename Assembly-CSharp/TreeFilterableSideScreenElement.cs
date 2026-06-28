using System;
using System.Diagnostics;
using UnityEngine;

public class TreeFilterableSideScreenElement : KMonoBehaviour
{
	public Tag GetElementTag()
	{
		return this.elementTag;
	}

	public bool IsSelected
	{
		get
		{
			return this.checkBox.isOn;
		}
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Tag, bool> OnSelectionChanged;

	public KToggle GetCheckboxToggle()
	{
		return this.checkBox;
	}

	public TreeFilterableSideScreen Parent
	{
		get
		{
			return this.parent;
		}
		set
		{
			this.parent = value;
		}
	}

	private void Initialize()
	{
		if (!this.initialized)
		{
			this.checkBoxImg = this.checkBox.gameObject.GetComponentInChildrenOnly<KImage>();
			this.checkBox.onClick += this.CheckBoxClicked;
			this.initialized = true;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Initialize();
	}

	public Sprite GetStorageObjectSprite(Tag t)
	{
		Sprite sprite = null;
		GameObject prefab = Assets.GetPrefab(t);
		if (prefab != null)
		{
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui");
			}
		}
		return sprite;
	}

	public void SetSprite(Tag t)
	{
		Element element = ElementLoader.GetElement(t);
		Sprite sprite = ((element == null) ? this.GetStorageObjectSprite(t) : Def.GetUISpriteFromMultiObjectAnim(element.substance.anim, "ui"));
		this.elementImg.sprite = sprite;
		this.elementImg.enabled = sprite != null;
	}

	public void SetTag(Tag newTag)
	{
		this.Initialize();
		this.elementTag = newTag;
		string text = this.elementTag.ProperName();
		if (this.parent.IsStorage)
		{
			float amountInStorage = this.parent.GetAmountInStorage(this.elementTag);
			text = text + ": " + GameUtil.GetFormattedMass(amountInStorage, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		}
		this.elementName.text = text;
	}

	private void CheckBoxClicked()
	{
		this.SetCheckBox(!this.parent.IsTagAllowed(this.GetElementTag()));
	}

	public void SetCheckBox(bool checkBoxState)
	{
		this.checkBox.isOn = checkBoxState;
		this.checkBoxImg.enabled = checkBoxState;
		if (this.OnSelectionChanged != null)
		{
			this.OnSelectionChanged(this.GetElementTag(), checkBoxState);
		}
	}

	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private KToggle checkBox;

	[SerializeField]
	private KImage elementImg;

	private KImage checkBoxImg;

	private Tag elementTag;

	private TreeFilterableSideScreen parent;

	private bool initialized = false;
}
