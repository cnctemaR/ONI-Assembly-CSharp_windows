using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterableSideScreenRow")]
public class TreeFilterableSideScreenRow : KMonoBehaviour
{
	public bool ArrowExpanded { get; private set; }

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
		if (this.rowElements.Count <= 0)
		{
			return TreeFilterableSideScreenRow.State.Off;
		}
		return TreeFilterableSideScreenRow.State.On;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.checkBoxToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			if (this.parent.CurrentSearchValue == "")
			{
				TreeFilterableSideScreenRow.State state = this.GetState();
				if (state > TreeFilterableSideScreenRow.State.Mixed)
				{
					if (state == TreeFilterableSideScreenRow.State.On)
					{
						this.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
						return;
					}
				}
				else
				{
					this.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
				}
			}
		}));
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.SetArrowToggleState(this.GetState() > TreeFilterableSideScreenRow.State.Off);
	}

	protected override void OnCmpDisable()
	{
		this.SetArrowToggleState(false);
		base.OnCmpDisable();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
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
		{
			for (int i = 0; i < this.rowElements.Count; i++)
			{
				this.rowElements[i].SetCheckBox(false);
			}
			break;
		}
		case TreeFilterableSideScreenRow.State.On:
		{
			for (int j = 0; j < this.rowElements.Count; j++)
			{
				this.rowElements[j].SetCheckBox(true);
			}
			break;
		}
		}
		this.visualDirty = true;
	}

	private void ArrowToggleClicked()
	{
		this.SetArrowToggleState(!this.ArrowExpanded);
		this.RefreshArrowToggleState();
	}

	public void SetArrowToggleState(bool state)
	{
		this.ArrowExpanded = state;
		this.RefreshArrowToggleState();
	}

	private void RefreshArrowToggleState()
	{
		this.arrowToggle.ChangeState(this.ArrowExpanded ? 1 : 0);
		this.elementGroup.SetActive(this.ArrowExpanded);
		this.bgImg.enabled = this.ArrowExpanded;
	}

	private void ArrowToggleDisabledClick()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
	}

	public void ShowToggleBox(bool show)
	{
		this.checkBoxToggle.gameObject.SetActive(show);
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
		this.bgImg.enabled = false;
		string text = string.Format(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.CATEGORYBUTTONTOOLTIP, mainElementTag.ProperName());
		this.checkBoxToggle.GetComponent<ToolTip>().SetSimpleTooltip(text);
		if (filterMap.Count == 0)
		{
			if (this.elementGroup.activeInHierarchy)
			{
				this.elementGroup.SetActive(false);
			}
			this.arrowToggle.onClick = new global::System.Action(this.ArrowToggleDisabledClick);
			this.arrowToggle.ChangeState(0);
		}
		else
		{
			this.arrowToggle.onClick = new global::System.Action(this.ArrowToggleClicked);
			this.arrowToggle.ChangeState(0);
			foreach (KeyValuePair<Tag, bool> keyValuePair in filterMap)
			{
				TreeFilterableSideScreenElement freeElement = this.parent.elementPool.GetFreeElement(this.elementGroup, true);
				freeElement.Parent = this.parent;
				freeElement.SetTag(keyValuePair.Key);
				freeElement.SetCheckBox(keyValuePair.Value);
				freeElement.OnSelectionChanged = new Action<Tag, bool>(this.OnElementSelectionChanged);
				freeElement.SetCheckBox(this.parent.IsTagAllowed(keyValuePair.Key));
				this.rowElements.Add(freeElement);
				this.subTags.Add(keyValuePair.Key);
			}
		}
		this.UpdateCheckBoxVisualState();
	}

	public void RefreshRowElements()
	{
		foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in this.rowElements)
		{
			treeFilterableSideScreenElement.SetCheckBox(this.parent.IsTagAllowed(treeFilterableSideScreenElement.GetElementTag()));
		}
	}

	public void FilterAgainstSearch(Tag thisCategoryTag, string search)
	{
		bool flag = false;
		bool flag2 = thisCategoryTag.ProperNameStripLink().ToUpper().Contains(search.ToUpper());
		search = search.ToUpper();
		foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in this.rowElements)
		{
			bool flag3 = flag2 || treeFilterableSideScreenElement.GetElementTag().ProperNameStripLink().ToUpper()
				.Contains(search.ToUpper());
			treeFilterableSideScreenElement.gameObject.SetActive(flag3);
			flag = flag || flag3;
		}
		base.gameObject.SetActive(flag);
		if (search != "" && flag && this.arrowToggle.CurrentState == 0)
		{
			this.SetArrowToggleState(true);
		}
	}

	public bool visualDirty;

	public bool standardCommodity = true;

	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private GameObject elementGroup;

	[SerializeField]
	private MultiToggle checkBoxToggle;

	[SerializeField]
	private MultiToggle arrowToggle;

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
