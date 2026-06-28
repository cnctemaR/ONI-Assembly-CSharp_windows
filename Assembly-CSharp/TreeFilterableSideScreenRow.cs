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

	public TreeFilterableSideScreenRow.State GetState()
	{
		bool flag = false;
		bool flag2 = false;
		foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in this.rowElements)
		{
			if (this.parent.GetElementTagAcceptedState(treeFilterableSideScreenElement.GetElementTag()))
			{
				flag = true;
			}
			else
			{
				flag2 = true;
			}
		}
		if (flag && !flag2)
		{
			return TreeFilterableSideScreenRow.State.On;
		}
		if (!flag && flag2)
		{
			return TreeFilterableSideScreenRow.State.Off;
		}
		if (flag && flag2)
		{
			return TreeFilterableSideScreenRow.State.Mixed;
		}
		return TreeFilterableSideScreenRow.State.On;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.checkBoxToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			switch (this.GetState())
			{
			case TreeFilterableSideScreenRow.State.Off:
			case TreeFilterableSideScreenRow.State.Mixed:
				this.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
				break;
			case TreeFilterableSideScreenRow.State.On:
				this.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
				break;
			}
		}));
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

	public void UpdateCheckBoxVisualState()
	{
		this.checkBoxToggle.ChangeState((int)this.GetState());
		this.visualDirty = false;
	}

	public void ChangeCheckBoxState(TreeFilterableSideScreenRow.State newState)
	{
		switch (newState)
		{
		case TreeFilterableSideScreenRow.State.Off:
			this.rowElements.ForEach(delegate(TreeFilterableSideScreenElement re)
			{
				re.SetCheckBox(false);
			});
			break;
		case TreeFilterableSideScreenRow.State.On:
			this.rowElements.ForEach(delegate(TreeFilterableSideScreenElement re)
			{
				re.SetCheckBox(true);
			});
			break;
		}
		this.visualDirty = true;
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

	private void OnElementSelectionChanged(Tag t, bool state)
	{
		if (state)
		{
			this.parent.AddTag(t);
		}
		else
		{
			this.parent.RemoveTag(t);
		}
		this.visualDirty = true;
	}

	public void SetElement(Tag mainElementTag, bool state, Dictionary<Tag, bool> filterMap)
	{
		this.subTags.Clear();
		this.rowElements.Clear();
		this.elementName.text = mainElementTag.ProperName();
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
				freeElement.SetCheckBox(this.parent.IsTagAllowed(keyValuePair.Key));
				this.rowElements.Add(freeElement);
				this.subTags.Add(keyValuePair.Key);
			}
		}
		this.UpdateCheckBoxVisualState();
	}

	public bool visualDirty;

	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private GameObject elementGroup;

	[SerializeField]
	private MultiToggle checkBoxToggle;

	[SerializeField]
	private KToggle arrowToggle;

	[SerializeField]
	private KImage bgImg;

	private List<Tag> subTags = new List<Tag>();

	private List<TreeFilterableSideScreenElement> rowElements = new List<TreeFilterableSideScreenElement>();

	private TreeFilterableSideScreen parent;

	public enum State
	{
		Off,
		Mixed,
		On
	}
}
