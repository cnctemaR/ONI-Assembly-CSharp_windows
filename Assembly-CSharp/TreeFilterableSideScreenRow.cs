using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TreeFilterableSideScreenRow : KMonoBehaviour
{
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

	public bool IsNotOff
	{
		get
		{
			foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in this.rowElements)
			{
				if (this.parent.GetElementTagAcceptedState(treeFilterableSideScreenElement.GetElementTag()))
				{
					return true;
				}
			}
			return this.checkBoxToggle.isOn;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.checkBoxToggle.onClick += this.OnCheckBoxToggled;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.SetArrowToggleState(false);
	}

	protected override void OnCmpDisable()
	{
		this.SetArrowToggleState(false);
		this.rowElements.ForEach(delegate(TreeFilterableSideScreenElement row)
		{
			row.OnSelectionChanged -= this.OnElementSelectionChanged;
		});
		base.OnCmpDisable();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.arrowToggle.onClick -= this.ArrowToggleClicked;
	}

	private void SetCheckBoxVisualState(bool state)
	{
		this.checkBoxMarkImg.enabled = state;
		this.mixedStateImg.enabled = false;
		if (!state)
		{
			this.CheckForMixedState();
		}
	}

	private void CheckForMixedState()
	{
		int count = this.rowElements.FindAll((TreeFilterableSideScreenElement elem) => elem.IsSelected).Count;
		if (count != 0)
		{
			this.checkBoxMarkImg.enabled = false;
			this.mixedStateImg.enabled = true;
		}
		else
		{
			this.mixedStateImg.enabled = false;
		}
	}

	public void SetCheckBoxState(bool state, bool children)
	{
		this.checkBoxToggle.isOn = state;
		this.UpdateCheckBoxState(children);
	}

	private void UpdateCheckBoxState(bool children)
	{
		if (children)
		{
			this.ChangeFilterByState(this.checkBoxToggle.isOn, this.subTags);
			this.rowElements.ForEach(delegate(TreeFilterableSideScreenElement re)
			{
				re.SetCheckBox(this.checkBoxToggle.isOn);
			});
		}
		if (this.checkBoxToggle.isOn)
		{
			this.parent.AddTag(this.categoryTag);
		}
		else
		{
			this.parent.RemoveTag(this.categoryTag);
		}
		this.SetCheckBoxVisualState(this.checkBoxToggle.isOn);
	}

	private void OnCheckBoxToggled()
	{
		this.UpdateCheckBoxState(true);
	}

	private void ArrowToggleClicked()
	{
		this.UpdateArrowToggleState();
	}

	private void SetArrowToggleState(bool state)
	{
		this.arrowToggle.isOn = state;
		this.UpdateArrowToggleState();
	}

	private void UpdateArrowToggleState()
	{
		bool isOn = this.arrowToggle.isOn;
		this.arrowToggle.GetComponent<ImageToggleState>().SetActiveState(isOn);
		this.elementGroup.SetActive(isOn);
		this.bgImg.enabled = isOn;
	}

	private void ArrowToggleDisabledClick()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
	}

	private void ChangeFilterByState(bool state, List<Tag> tagList)
	{
		if (state)
		{
			this.parent.AddTags(tagList);
		}
		else
		{
			this.parent.RemoveTags(tagList);
		}
	}

	private void OnElementSelectionChanged(Tag t, bool state)
	{
		if (state)
		{
			this.parent.AddTag(t);
		}
		else
		{
			this.parent.RemoveTag(t);
			this.SetCheckBoxState(false, false);
		}
		this.CheckForMixedState();
		this.parent.ElementSelectionChanged();
	}

	public void SetElement(Tag mainElementTag, bool state, Dictionary<Tag, bool> filterMap)
	{
		this.subTags.Clear();
		this.rowElements.Clear();
		this.elementName.text = mainElementTag.ProperName();
		this.categoryTag = mainElementTag;
		this.arrowToggle.ClearOnClick();
		this.bgImg.enabled = false;
		string text = string.Format(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.CATEGORYBUTTONTOOLTIP, mainElementTag.ProperName());
		this.checkBoxToggle.GetComponent<ToolTip>().SetSimpleTooltip(text);
		if (filterMap.Count == 0)
		{
			if (this.elementGroup.activeInHierarchy)
			{
				this.elementGroup.SetActive(false);
			}
			this.arrowToggle.interactable = false;
			this.arrowToggle.onClick += this.ArrowToggleDisabledClick;
			this.arrowToggle.GetComponent<ImageToggleState>().SetDisabled();
		}
		else
		{
			this.arrowToggle.interactable = true;
			this.arrowToggle.onClick += this.ArrowToggleClicked;
			this.arrowToggle.GetComponent<ImageToggleState>().SetActiveState(false);
			foreach (KeyValuePair<Tag, bool> keyValuePair in filterMap)
			{
				TreeFilterableSideScreenElement freeElement = this.parent.elementPool.GetFreeElement(this.elementGroup, true);
				freeElement.Parent = this.parent;
				freeElement.SetTag(keyValuePair.Key);
				freeElement.SetCheckBox(keyValuePair.Value);
				freeElement.OnSelectionChanged += this.OnElementSelectionChanged;
				this.rowElements.Add(freeElement);
				this.subTags.Add(keyValuePair.Key);
				if (keyValuePair.Value)
				{
					this.SetCheckBoxVisualState(true);
				}
			}
		}
		this.checkBoxToggle.isOn = state;
		this.SetCheckBoxVisualState(state);
	}

	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private GameObject elementGroup;

	[SerializeField]
	private KToggle checkBoxToggle;

	[SerializeField]
	private KImage checkBoxMarkImg;

	[SerializeField]
	private KImage mixedStateImg;

	[SerializeField]
	private KToggle arrowToggle;

	[SerializeField]
	private KImage bgImg;

	private List<Tag> subTags = new List<Tag>();

	private List<TreeFilterableSideScreenElement> rowElements = new List<TreeFilterableSideScreenElement>();

	private Tag categoryTag;

	private TreeFilterableSideScreen parent;
}
