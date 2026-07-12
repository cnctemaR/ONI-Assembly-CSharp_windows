using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterableSideScreenElement")]
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
			return this.checkBox.CurrentState == 1;
		}
	}

	public MultiToggle GetCheckboxToggle()
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
		if (this.initialized)
		{
			return;
		}
		this.checkBoxImg = this.checkBox.gameObject.GetComponentInChildrenOnly<KImage>();
		this.checkBox.onClick = new global::System.Action(this.CheckBoxClicked);
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
				sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false, "");
			}
		}
		return sprite;
	}

	public void SetSprite(Tag t)
	{
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(t, "ui", false);
		this.elementImg.sprite = uisprite.first;
		this.elementImg.color = uisprite.second;
		this.elementImg.gameObject.SetActive(true);
	}

	public void SetTag(Tag newTag)
	{
		this.Initialize();
		this.elementTag = newTag;
		this.SetSprite(this.elementTag);
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
		this.checkBox.ChangeState(checkBoxState ? 1 : 0);
		this.checkBoxImg.enabled = checkBoxState;
		if (this.OnSelectionChanged != null)
		{
			this.OnSelectionChanged(this.GetElementTag(), checkBoxState);
		}
	}

	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private MultiToggle checkBox;

	[SerializeField]
	private KImage elementImg;

	private KImage checkBoxImg;

	private Tag elementTag;

	public Action<Tag, bool> OnSelectionChanged;

	private TreeFilterableSideScreen parent;

	private bool initialized;
}
