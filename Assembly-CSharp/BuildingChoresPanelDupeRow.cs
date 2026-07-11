using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingChoresPanelDupeRow")]
public class BuildingChoresPanelDupeRow : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.button.onClick += this.OnClick;
	}

	public void Init(BuildingChoresPanel.DupeEntryData data)
	{
		this.choreConsumer = data.consumer;
		if (data.context.IsPotentialSuccess())
		{
			string text = ((data.context.chore.driver == data.consumer.choreDriver) ? DUPLICANTS.CHORES.PRECONDITIONS.CURRENT_ERRAND.text : string.Format(DUPLICANTS.CHORES.PRECONDITIONS.RANK_FORMAT.text, data.rank));
			this.label.text = DUPLICANTS.CHORES.PRECONDITIONS.SUCCESS_ROW.Replace("{Duplicant}", data.consumer.name).Replace("{Rank}", text);
		}
		else
		{
			string text2 = data.context.chore.GetPreconditions()[data.context.failedPreconditionId].description;
			DebugUtil.Assert(text2 != null, "Chore requires description!", data.context.chore.GetPreconditions()[data.context.failedPreconditionId].id);
			if (data.context.chore.driver != null)
			{
				text2 = text2.Replace("{Assignee}", data.context.chore.driver.GetProperName());
			}
			text2 = text2.Replace("{Selected}", data.context.chore.gameObject.GetProperName());
			this.label.text = DUPLICANTS.CHORES.PRECONDITIONS.FAILURE_ROW.Replace("{Duplicant}", data.consumer.name).Replace("{Reason}", text2);
		}
		this.icon.sprite = JobsTableScreen.priorityInfo[data.personalPriority].sprite;
		this.toolTip.toolTip = BuildingChoresPanelDupeRow.TooltipForDupe(data.context, data.consumer, data.rank);
	}

	private void OnClick()
	{
		Vector3 vector = this.choreConsumer.gameObject.transform.GetPosition() + Vector3.up;
		CameraController.Instance.SetTargetPos(vector, 10f, true);
	}

	private static string TooltipForDupe(Chore.Precondition.Context context, ChoreConsumer choreConsumer, int rank)
	{
		bool flag = context.IsPotentialSuccess();
		string text = (flag ? UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_SUCCEEDED : UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_FAILED);
		float num = 0f;
		int personalPriority = choreConsumer.GetPersonalPriority(context.chore.choreType);
		num += (float)(personalPriority * 10);
		int priority_value = context.chore.masterPriority.priority_value;
		num += (float)priority_value;
		float num2 = (float)context.priority / 10000f;
		num += num2;
		text = text.Replace("{Description}", (context.chore.driver == choreConsumer.choreDriver) ? UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_ACTIVE : UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_INACTIVE);
		string text2 = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string text3 = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text;
		if (flag && context.chore.choreType.groups.Length != 0)
		{
			ChoreGroup choreGroup = context.chore.choreType.groups[0];
			for (int i = 1; i < context.chore.choreType.groups.Length; i++)
			{
				if (choreConsumer.GetPersonalPriority(choreGroup) < choreConsumer.GetPersonalPriority(context.chore.choreType.groups[i]))
				{
					choreGroup = context.chore.choreType.groups[i];
				}
			}
			text3 = choreGroup.Name;
		}
		text = text.Replace("{Name}", choreConsumer.name);
		text = text.Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
		if (!flag)
		{
			text = text.Replace("{FailedPrecondition}", context.chore.GetPreconditions()[context.failedPreconditionId].description);
		}
		else
		{
			text = text.Replace("{Rank}", rank.ToString());
			text = text.Replace("{Groups}", text2);
			text = text.Replace("{BestGroup}", text3);
			text = text.Replace("{PersonalPriority}", JobsTableScreen.priorityInfo[personalPriority].name.text);
			text = text.Replace("{PersonalPriorityValue}", (personalPriority * 10).ToString());
			text = text.Replace("{Building}", context.chore.gameObject.GetProperName());
			text = text.Replace("{BuildingPriority}", priority_value.ToString());
			text = text.Replace("{TypePriority}", num2.ToString());
			text = text.Replace("{TotalPriority}", num.ToString());
		}
		return text;
	}

	public Image icon;

	public LocText label;

	public ToolTip toolTip;

	private ChoreConsumer choreConsumer;

	public KButton button;
}
