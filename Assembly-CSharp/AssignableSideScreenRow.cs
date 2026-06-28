using System;
using UnityEngine;

public class AssignableSideScreenRow : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.regularColor = new Color32(204, 204, 204, byte.MaxValue);
	}

	public void SetAssignmentText(string assignmentStr)
	{
		this.assignmentText.text = (string.IsNullOrEmpty(assignmentStr) ? "-" : assignmentStr);
	}

	public bool IsSelected
	{
		get
		{
			return this.isSelected;
		}
	}

	public void SetSelected(bool selected)
	{
		this.isSelected = selected;
		this.outline.color = ((!selected) ? this.regularColor : this.outlineHighLightColor);
		this.BG.color = ((!selected) ? Color.white : this.BGHighLightColor);
	}

	public void SetContent(MinionIdentity identity, string assignmentStr, Action<MinionIdentity> selectionCallback)
	{
		if (identity == null)
		{
			Debug.LogError("Invalid data received.");
			return;
		}
		if (!string.IsNullOrEmpty(assignmentStr))
		{
			if (!this.isSelected)
			{
				this.assignmentText.text = assignmentStr;
			}
		}
		else if (!this.isSelected)
		{
			this.assignmentText.text = "-";
		}
		if (this.portraitInstance == null)
		{
			this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(this.crewPortraitPrefab.gameObject, base.gameObject, false);
			this.portraitInstance.transform.SetSiblingIndex(2);
			this.portraitInstance.SetAlpha(1f);
		}
		if (this.button == null)
		{
			this.button = base.GetComponent<KButton>();
			KButton kbutton = this.button;
			kbutton.onPointerEnter = (global::System.Action)Delegate.Combine(kbutton.onPointerEnter, new global::System.Action(delegate
			{
				if (!this.isSelected)
				{
					this.outline.color = this.outlineHighLightColor;
				}
			}));
			KButton kbutton2 = this.button;
			kbutton2.onPointerExit = (global::System.Action)Delegate.Combine(kbutton2.onPointerExit, new global::System.Action(delegate
			{
				if (!this.isSelected)
				{
					this.outline.color = this.regularColor;
				}
			}));
		}
		this.targetIdentity = identity;
		this.button.ClearOnClick();
		this.button.onClick += delegate
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

	[SerializeField]
	private Color outlineHighLightColor = new Color32(168, 74, 121, byte.MaxValue);

	[SerializeField]
	private Color BGHighLightColor = new Color32(168, 74, 121, 80);

	private Color regularColor;

	private CrewPortrait portraitInstance;

	private KButton button;

	private MinionIdentity targetIdentity;

	private bool isSelected;
}
