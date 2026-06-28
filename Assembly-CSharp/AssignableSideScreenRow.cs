using System;
using STRINGS;
using UnityEngine;

public class AssignableSideScreenRow : KMonoBehaviour
{
	public void SetAssignmentText(string assignmentStr)
	{
		this.assignmentText.text = (string.IsNullOrEmpty(assignmentStr) ? "-" : assignmentStr);
	}

	public bool Selected
	{
		get
		{
			return this.sideScreen.targetAssignable.assignee == this.targetIdentity;
		}
	}

	public void Refresh(object data = null)
	{
		string text = string.Empty;
		if (this.sideScreen.targetAssignable.slot != null)
		{
			Assignables assignables = null;
			if (this.targetIdentity is MinionIdentity)
			{
				if (this.sideScreen.targetAssignable is Ownable)
				{
					assignables = (this.targetIdentity as MinionIdentity).GetComponent<Ownables>();
				}
				else if (this.sideScreen.targetAssignable is Equippable)
				{
					assignables = (this.targetIdentity as MinionIdentity).GetComponent<Equipment>();
				}
			}
			if (assignables != null && assignables.GetSlot(this.sideScreen.targetAssignable.slot) != null && assignables.GetSlot(this.sideScreen.targetAssignable.slot).IsAssigned())
			{
				if (assignables.GetSlot(this.sideScreen.targetAssignable.slot).assignable == this.sideScreen.targetAssignable)
				{
					text = UI.DETAILTABS.POSSESSIONS.NAME;
				}
				else
				{
					text = assignables.GetSlot(this.sideScreen.targetAssignable.slot).assignable.GetProperName();
				}
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			this.assignmentText.text = "-";
		}
		else
		{
			this.assignmentText.text = text;
		}
		this.toggle.ChangeState((!this.Selected) ? 0 : 1);
	}

	protected override void OnCleanUp()
	{
		if (this.refreshHandle == -1)
		{
			Game.Instance.Unsubscribe(this.refreshHandle);
		}
		base.OnCleanUp();
	}

	public void SetContent(IAssignableIdentity identity_object, Action<IAssignableIdentity> selectionCallback, AssignableSideScreen assignableSideScreen)
	{
		if (this.refreshHandle == -1)
		{
			Game.Instance.Unsubscribe(this.refreshHandle);
		}
		this.refreshHandle = Game.Instance.Subscribe(-2146166042, delegate(object o)
		{
			if (this != null && this.gameObject != null && this.gameObject.activeInHierarchy)
			{
				this.Refresh(null);
			}
		});
		this.toggle = base.GetComponent<MultiToggle>();
		this.sideScreen = assignableSideScreen;
		this.targetIdentity = identity_object;
		if (this.portraitInstance == null)
		{
			this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(this.crewPortraitPrefab.gameObject, base.gameObject, false);
			this.portraitInstance.transform.SetSiblingIndex(1);
			this.portraitInstance.SetAlpha(1f);
		}
		this.toggle.onClick = delegate
		{
			selectionCallback(this.targetIdentity);
		};
		this.portraitInstance.SetIdentityObject(identity_object, false);
		base.GetComponent<ToolTip>().OnToolTip = new Func<string>(this.GetTooltip);
		this.Refresh(null);
	}

	private string GetTooltip()
	{
		ToolTip component = base.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		if (this.targetIdentity != null)
		{
			if (!this.Selected)
			{
				component.AddMultiStringTooltip(string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGN_TO_TOOLTIP, this.targetIdentity.GetProperName()), null);
			}
			else
			{
				component.AddMultiStringTooltip(string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGN_TOOLTIP, this.targetIdentity.GetProperName()), null);
			}
		}
		return string.Empty;
	}

	[SerializeField]
	private CrewPortrait crewPortraitPrefab;

	[SerializeField]
	private LocText assignmentText;

	public AssignableSideScreen sideScreen;

	private CrewPortrait portraitInstance;

	[MyCmpReq]
	private MultiToggle toggle;

	public IAssignableIdentity targetIdentity;

	private int refreshHandle = -1;
}
