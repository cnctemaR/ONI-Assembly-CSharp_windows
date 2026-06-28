using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TreeFilterableSideScreenRow : KMonoBehaviour
{
	public bool IsSelected
	{
		get
		{
			return this.checkBoxToggle.isOn;
		}
	}

	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.arrowToggleImageState = this.arrowToggle.GetComponent<ImageToggleState>();
		this.checkBoxToggle.onClick += this.ToggleCheckBox;
		this.initialized = true;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.SetArrowToggleState(false);
		this.rowElements.ForEach(delegate(TreeFilterableSideScreenElement row)
		{
			row.OnSelectionChanged -= this.OnElementSelectionChanged;
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.arrowToggle.onClick -= this.ArrowToggleClicked;
	}

	private void SetCheckBoxVisualState(bool state)
	{
		this.checkBoxToggle.isOn = state;
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
		if (children)
		{
			this.ChangeFilterByState(state, this.subTags);
			this.rowElements.ForEach(delegate(TreeFilterableSideScreenElement re)
			{
				re.SetCheckBox(state);
			});
		}
		if (state)
		{
			TreeFilterableSideScreen.Instance.AddTag(this.categoryTag);
		}
		else
		{
			TreeFilterableSideScreen.Instance.RemoveTag(this.categoryTag);
		}
		this.SetCheckBoxVisualState(state);
	}

	private void ToggleCheckBox()
	{
		this.SetCheckBoxState(!this.checkBoxToggle.isOn, true);
	}

	private void ArrowToggleClicked()
	{
		this.SetArrowToggleState(!this.arrowToggle.isOn);
	}

	private void SetArrowToggleState(bool state)
	{
		this.arrowToggle.isOn = state;
		this.arrowToggleImageState.SetActiveState(state);
		this.elementGroup.SetActive(state);
		this.bgImg.enabled = state;
	}

	private void ArrowToggleDisabledClick()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
	}

	private void ChangeFilterByState(bool state, List<Tag> tagList)
	{
		if (state)
		{
			TreeFilterableSideScreen.Instance.AddTags(tagList);
		}
		else
		{
			TreeFilterableSideScreen.Instance.RemoveTags(tagList);
		}
	}

	private void OnElementSelectionChanged(Tag t, bool state)
	{
		if (state)
		{
			TreeFilterableSideScreen.Instance.AddTag(t);
		}
		else
		{
			TreeFilterableSideScreen.Instance.RemoveTag(t);
			this.SetCheckBoxState(false, false);
		}
		this.CheckForMixedState();
	}

	public void SetElement(Tag mainElementTag, bool state, Dictionary<Tag, bool> filterMap)
	{
		this.Initialize();
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
			this.arrowToggleImageState.SetDisabled();
			this.arrowToggle.onClick += this.ArrowToggleDisabledClick;
		}
		else
		{
			this.arrowToggle.interactable = true;
			this.arrowToggleImageState.SetActiveState(false);
			this.arrowToggle.onClick += this.ArrowToggleClicked;
			foreach (KeyValuePair<Tag, bool> keyValuePair in filterMap)
			{
				TreeFilterableSideScreenElement freeElement = TreeFilterableSideScreen.Instance.elementPool.GetFreeElement(this.elementGroup, true);
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

	private ImageToggleState arrowToggleImageState;

	[SerializeField]
	private KImage bgImg;

	private List<Tag> subTags = new List<Tag>();

	private List<TreeFilterableSideScreenElement> rowElements = new List<TreeFilterableSideScreenElement>();

	private Tag categoryTag;

	private bool initialized;
}
