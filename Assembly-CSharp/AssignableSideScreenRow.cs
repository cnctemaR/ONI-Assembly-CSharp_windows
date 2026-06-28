using System;
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
			return this.toggle.isOn;
		}
		set
		{
			this.toggle.isOn = value;
		}
	}

	public void SetContent(MinionIdentity identity, string assignmentStr, Action<MinionIdentity> selectionCallback)
	{
		if (identity == null)
		{
			global::Debug.LogError("Invalid data received.", null);
			return;
		}
		if (!string.IsNullOrEmpty(assignmentStr))
		{
			if (!this.toggle.isOn)
			{
				this.assignmentText.text = assignmentStr;
			}
		}
		else if (!this.toggle.isOn)
		{
			this.assignmentText.text = "-";
		}
		if (this.portraitInstance == null)
		{
			this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(this.crewPortraitPrefab.gameObject, base.gameObject, false);
			this.portraitInstance.transform.SetSiblingIndex(1);
			this.portraitInstance.SetAlpha(1f);
		}
		this.targetIdentity = identity;
		this.toggle.ClearOnClick();
		this.toggle.onClick += delegate
		{
			selectionCallback(this.targetIdentity);
		};
		this.portraitInstance.SetCrewMember(identity, false);
	}

	[SerializeField]
	private CrewPortrait crewPortraitPrefab;

	[SerializeField]
	private LocText assignmentText;

	[SerializeField]
	private KImage BG;

	[SerializeField]
	private KImage outline;

	private CrewPortrait portraitInstance;

	[MyCmpReq]
	private KToggle toggle;

	private MinionIdentity targetIdentity;
}
