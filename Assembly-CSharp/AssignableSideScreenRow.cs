using System;
using STRINGS;
using UnityEngine;

public class AssignableSideScreenRow : KMonoBehaviour
{
	public void Refresh(object data = null)
	{
		if (!this.sideScreen.targetAssignable.CanAssignTo(this.targetIdentity))
		{
			this.currentState = AssignableSideScreenRow.AssignableState.Disabled;
			this.assignmentText.text = UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.DISABLED;
		}
		else if (this.sideScreen.targetAssignable.assignee == this.targetIdentity)
		{
			this.currentState = AssignableSideScreenRow.AssignableState.Selected;
			this.assignmentText.text = UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGNED;
		}
		else
		{
			Assignable assignable = null;
			MinionIdentity minionIdentity = this.targetIdentity as MinionIdentity;
			if (minionIdentity != null)
			{
				foreach (Assignables assignables in minionIdentity.GetComponents<Assignables>())
				{
					Assignable assignable2 = assignables.GetAssignable(this.sideScreen.targetAssignable.slot);
					if (assignable2 != null && assignable2 != this.sideScreen.targetAssignable)
					{
						assignable = assignable2;
						break;
					}
				}
			}
			if (assignable != null)
			{
				this.currentState = AssignableSideScreenRow.AssignableState.AssignedToOther;
				this.assignmentText.text = assignable.GetProperName();
			}
			else
			{
				this.currentState = AssignableSideScreenRow.AssignableState.Unassigned;
				this.assignmentText.text = UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGNED;
			}
		}
		this.toggle.ChangeState((int)this.currentState);
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
			AssignableSideScreenRow.AssignableState assignableState = this.currentState;
			if (assignableState != AssignableSideScreenRow.AssignableState.Selected)
			{
				if (assignableState != AssignableSideScreenRow.AssignableState.Disabled)
				{
					component.AddMultiStringTooltip(string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGN_TO_TOOLTIP, this.targetIdentity.GetProperName()), null);
				}
				else
				{
					component.AddMultiStringTooltip(string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.DISABLED_TOOLTIP, this.targetIdentity.GetProperName()), null);
				}
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

	public AssignableSideScreenRow.AssignableState currentState;

	private int refreshHandle = -1;

	public enum AssignableState
	{
		Selected,
		AssignedToOther,
		Unassigned,
		Disabled
	}
}
