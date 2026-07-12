using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AssignmentGroupControllerSideScreen : KScreen
{
	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.RefreshRows();
		}
	}

	protected override void OnCmpDisable()
	{
		for (int i = 0; i < this.identityRowMap.Count; i++)
		{
			global::UnityEngine.Object.Destroy(this.identityRowMap[i]);
		}
		this.identityRowMap.Clear();
		base.OnCmpDisable();
	}

	public void SetTarget(GameObject target)
	{
		this.target = target.GetComponent<AssignmentGroupController>();
		this.RefreshRows();
	}

	private void RefreshRows()
	{
		int num = 0;
		WorldContainer worldContainer = this.target.GetMyWorld();
		ClustercraftExteriorDoor component = this.target.GetComponent<ClustercraftExteriorDoor>();
		if (component != null)
		{
			worldContainer = component.GetInteriorDoor().GetMyWorld();
		}
		List<AssignmentGroupControllerSideScreen.RowSortHelper> list = new List<AssignmentGroupControllerSideScreen.RowSortHelper>();
		for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
		{
			MinionAssignablesProxy minionAssignablesProxy = Components.MinionAssignablesProxy[i];
			GameObject targetGameObject = minionAssignablesProxy.GetTargetGameObject();
			WorldContainer myWorld = targetGameObject.GetMyWorld();
			if (!(targetGameObject == null) && !targetGameObject.HasTag(GameTags.Dead))
			{
				MinionResume component2 = minionAssignablesProxy.GetTargetGameObject().GetComponent<MinionResume>();
				bool flag = false;
				if (component2 != null && component2.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
				{
					flag = true;
				}
				bool flag2 = myWorld.ParentWorldId == worldContainer.ParentWorldId;
				list.Add(new AssignmentGroupControllerSideScreen.RowSortHelper
				{
					minion = minionAssignablesProxy,
					isPilot = flag,
					isSameWorld = flag2
				});
			}
		}
		list.Sort(delegate(AssignmentGroupControllerSideScreen.RowSortHelper a, AssignmentGroupControllerSideScreen.RowSortHelper b)
		{
			int num2 = b.isSameWorld.CompareTo(a.isSameWorld);
			if (num2 != 0)
			{
				return num2;
			}
			return b.isPilot.CompareTo(a.isPilot);
		});
		foreach (AssignmentGroupControllerSideScreen.RowSortHelper rowSortHelper in list)
		{
			MinionAssignablesProxy minion = rowSortHelper.minion;
			GameObject gameObject;
			if (num >= this.identityRowMap.Count)
			{
				gameObject = Util.KInstantiateUI(this.minionRowPrefab, this.minionRowContainer, true);
				this.identityRowMap.Add(gameObject);
			}
			else
			{
				gameObject = this.identityRowMap[num];
				gameObject.SetActive(true);
			}
			num++;
			HierarchyReferences component3 = gameObject.GetComponent<HierarchyReferences>();
			MultiToggle toggle = component3.GetReference<MultiToggle>("Toggle");
			toggle.ChangeState(this.target.CheckMinionIsMember(minion) ? 1 : 0);
			component3.GetReference<CrewPortrait>("Portrait").SetIdentityObject(minion, false);
			LocText reference = component3.GetReference<LocText>("Label");
			LocText reference2 = component3.GetReference<LocText>("Designation");
			if (rowSortHelper.isSameWorld)
			{
				if (rowSortHelper.isPilot)
				{
					reference2.text = UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.PILOT;
				}
				else
				{
					reference2.text = "";
				}
				reference.color = Color.black;
				reference2.color = Color.black;
			}
			else
			{
				reference.color = Color.grey;
				reference2.color = Color.grey;
				reference2.text = UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.OFFWORLD;
				gameObject.transform.SetAsLastSibling();
			}
			toggle.onClick = delegate
			{
				this.target.SetMember(minion, !this.target.CheckMinionIsMember(minion));
				toggle.ChangeState(this.target.CheckMinionIsMember(minion) ? 1 : 0);
				this.RefreshRows();
			};
			string text = this.UpdateToolTip(minion, !rowSortHelper.isSameWorld);
			toggle.GetComponent<ToolTip>().SetSimpleTooltip(text);
		}
		for (int j = num; j < this.identityRowMap.Count; j++)
		{
			this.identityRowMap[j].SetActive(false);
		}
		this.minionRowContainer.GetComponent<QuickLayout>().ForceUpdate();
	}

	private string UpdateToolTip(MinionAssignablesProxy minion, bool offworld)
	{
		string text = (this.target.CheckMinionIsMember(minion) ? UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.UNASSIGN : UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.ASSIGN);
		if (offworld)
		{
			text = string.Concat(new string[]
			{
				text,
				"\n\n",
				UIConstants.ColorPrefixYellow,
				UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.DIFFERENT_WORLD,
				UIConstants.ColorSuffix
			});
		}
		return text;
	}

	[SerializeField]
	private GameObject header;

	[SerializeField]
	private GameObject minionRowPrefab;

	[SerializeField]
	private GameObject footer;

	[SerializeField]
	private GameObject minionRowContainer;

	private AssignmentGroupController target;

	private List<GameObject> identityRowMap = new List<GameObject>();

	private struct RowSortHelper
	{
		public MinionAssignablesProxy minion;

		public bool isPilot;

		public bool isSameWorld;
	}
}
