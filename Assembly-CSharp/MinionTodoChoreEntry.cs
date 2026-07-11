using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MinionTodoChoreEntry : KMonoBehaviour
{
	public void SetMoreAmount(int amount)
	{
		if (amount == 0)
		{
			this.moreLabel.gameObject.SetActive(false);
		}
		else
		{
			this.moreLabel.text = string.Format(UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TRUNCATED_CHORES, amount);
		}
	}

	public void Apply(Chore.Precondition.Context context)
	{
		ChoreConsumer consumer = context.consumerState.consumer;
		if (this.targetChore == context.chore && object.ReferenceEquals(context.chore.target, this.lastChoreTarget) && context.chore.masterPriority == this.lastPrioritySetting)
		{
			return;
		}
		this.targetChore = context.chore;
		this.lastChoreTarget = context.chore.target;
		this.lastPrioritySetting = context.chore.masterPriority;
		string choreName = GameUtil.GetChoreName(context.chore, context.data);
		string text = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string text2 = ((text == null) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET_AND_GROUP);
		text2 = text2.Replace("{Target}", (!(context.chore.target.gameObject == consumer.gameObject)) ? context.chore.target.gameObject.GetProperName() : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.SELF_LABEL.text);
		if (text != null)
		{
			text2 = text2.Replace("{Groups}", text);
		}
		string text3 = ((context.chore.masterPriority.priority_class != PriorityScreen.PriorityClass.basic) ? string.Empty : context.chore.masterPriority.priority_value.ToString());
		Sprite sprite = ((context.chore.masterPriority.priority_class != PriorityScreen.PriorityClass.basic) ? null : this.prioritySprites[context.chore.masterPriority.priority_value - 1]);
		this.label.SetText(choreName);
		this.subLabel.SetText(text2);
		this.priorityLabel.SetText(text3);
		this.icon.sprite = sprite;
		this.moreLabel.text = string.Empty;
		base.GetComponent<ToolTip>().SetSimpleTooltip(MinionTodoChoreEntry.TooltipForChore(context, consumer));
		KButton componentInChildren = base.GetComponentInChildren<KButton>();
		componentInChildren.ClearOnClick();
		if (componentInChildren.bgImage != null)
		{
			componentInChildren.bgImage.colorStyleSetting = ((!(context.chore.driver == consumer.choreDriver)) ? this.buttonColorSettingStandard : this.buttonColorSettingCurrent);
			componentInChildren.bgImage.ApplyColorStyleSetting();
		}
		GameObject gameObject = context.chore.target.gameObject;
		componentInChildren.ClearOnPointerEvents();
		componentInChildren.GetComponentInChildren<KButton>().onClick += delegate
		{
			if (context.chore != null && !context.chore.target.isNull)
			{
				Vector3 vector = new Vector3(context.chore.target.gameObject.transform.position.x, context.chore.target.gameObject.transform.position.y + 1f, CameraController.Instance.transform.position.z);
				CameraController.Instance.SetTargetPos(vector, 10f, true);
			}
		};
	}

	private static string TooltipForChore(Chore.Precondition.Context context, ChoreConsumer choreConsumer)
	{
		bool flag = context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic || context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.high;
		PriorityScreen.PriorityClass priority_class = context.chore.masterPriority.priority_class;
		string text;
		switch (priority_class + 1)
		{
		case PriorityScreen.PriorityClass.basic:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLE;
			goto IL_00B5;
		case PriorityScreen.PriorityClass.topPriority:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_PERSONAL;
			goto IL_00B5;
		case PriorityScreen.PriorityClass.compulsory:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_EMERGENCY;
			goto IL_00B5;
		case (PriorityScreen.PriorityClass)5:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_COMPULSORY;
			goto IL_00B5;
		}
		text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NORMAL;
		IL_00B5:
		float num = 0f;
		int num2 = (int)(context.chore.masterPriority.priority_class * (PriorityScreen.PriorityClass)100);
		num += (float)num2;
		int num3 = ((!flag) ? 0 : choreConsumer.GetPersonalPriority(context.chore.choreType));
		num += (float)(num3 * 10);
		int num4 = ((!flag) ? 0 : context.chore.masterPriority.priority_value);
		num += (float)num4;
		float num5 = (float)context.priority / 10000f;
		num += num5;
		text = text.Replace("{Description}", (!(context.chore.driver == choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_ACTIVE);
		text = text.Replace("{IdleDescription}", (!(context.chore.driver == choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_ACTIVE);
		string text2 = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string text3 = context.chore.choreType.Name;
		if (context.chore.choreType.groups.Length > 0)
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
		text = text.Replace("{Groups}", text2);
		text = text.Replace("{BestGroup}", text3);
		text = text.Replace("{ClassPriority}", num2.ToString());
		text = text.Replace("{PersonalPriority}", JobsTableScreen.priorityInfo[num3].name.text);
		text = text.Replace("{PersonalPriorityValue}", (num3 * 10).ToString());
		text = text.Replace("{Building}", context.chore.gameObject.GetProperName());
		text = text.Replace("{BuildingPriority}", num4.ToString());
		text = text.Replace("{TypePriority}", num5.ToString());
		return text.Replace("{TotalPriority}", num.ToString());
	}

	public Image icon;

	public LocText priorityLabel;

	public LocText label;

	public LocText subLabel;

	public LocText moreLabel;

	public List<Sprite> prioritySprites;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingCurrent;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingStandard;

	private Chore targetChore;

	private IStateMachineTarget lastChoreTarget;

	private PrioritySetting lastPrioritySetting;
}
