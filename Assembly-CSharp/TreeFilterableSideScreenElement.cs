using System;
using UnityEngine;

public class TreeFilterableSideScreenElement : KMonoBehaviour
{
	public event Action<Tag, bool> OnSelectionChanged;

	public bool IsSelected
	{
		get
		{
			return this.checkBox.isOn;
		}
	}

	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.checkBoxImg = this.checkBox.gameObject.GetComponentInChildrenOnly<KImage>();
		this.checkBox.onClick += this.CheckBoxClicked;
		this.initialized = true;
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
		if (TreeFilterableSideScreen.Instance.IsStorage)
		{
			float amountInStorage = TreeFilterableSideScreen.Instance.GetAmountInStorage(this.elementTag);
			text = text + ": " + GameUtil.GetFormattedMass(amountInStorage, GameUtil.TimeSlice.None, true, "F1");
		}
		this.elementName.text = text;
	}

	private void CheckBoxClicked()
	{
		this.checkBox.isOn = !this.checkBox.isOn;
		this.checkBoxImg.enabled = this.checkBox.isOn;
		if (this.OnSelectionChanged != null)
		{
			this.OnSelectionChanged(this.elementTag, this.checkBox.isOn);
		}
	}

	public void SetCheckBox(bool checkBoxState)
	{
		this.checkBox.isOn = checkBoxState;
		this.checkBoxImg.enabled = checkBoxState;
	}

	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private KToggle checkBox;

	private KImage checkBoxImg;

	[SerializeField]
	private KImage elementImg;

	private TreeFilterableSideScreenRow parentRow;

	private Tag elementTag;

	private bool initialized;
}
