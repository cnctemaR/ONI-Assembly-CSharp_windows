using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandModuleSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ScheduleUpdate();
	}

	private void ScheduleUpdate()
	{
		this.updateHandle = UIScheduler.Instance.Schedule("RefreshCommandModuleSideScreen", 1f, delegate(object o)
		{
			this.RefreshConditions();
			this.ScheduleUpdate();
		}, null, null);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LaunchConditionManager>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.target = new_target.GetComponent<LaunchConditionManager>();
		if (this.target == null)
		{
			global::Debug.LogError("The gameObject received does not contain a LaunchConditionManager component");
			return;
		}
		this.ClearConditions();
		this.ConfigureConditions();
	}

	private void ClearConditions()
	{
		foreach (KeyValuePair<RocketLaunchCondition, GameObject> keyValuePair in this.conditionTable)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.conditionTable.Clear();
	}

	private void ConfigureConditions()
	{
		foreach (RocketLaunchCondition rocketLaunchCondition in this.target.GetLaunchConditionList())
		{
			GameObject gameObject = Util.KInstantiateUI(this.prefabConditionLineItem, this.conditionListContainer, true);
			this.conditionTable.Add(rocketLaunchCondition, gameObject);
		}
		this.RefreshConditions();
	}

	public void RefreshConditions()
	{
		bool flag = false;
		List<RocketLaunchCondition> launchConditionList = this.target.GetLaunchConditionList();
		foreach (RocketLaunchCondition rocketLaunchCondition in launchConditionList)
		{
			if (!this.conditionTable.ContainsKey(rocketLaunchCondition))
			{
				flag = true;
				break;
			}
			GameObject gameObject = this.conditionTable[rocketLaunchCondition];
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			if (rocketLaunchCondition.GetParentCondition() != null && !rocketLaunchCondition.GetParentCondition().EvaluateLaunchCondition())
			{
				gameObject.SetActive(false);
			}
			else if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			bool flag2 = rocketLaunchCondition.EvaluateLaunchCondition();
			component.GetReference<LocText>("Label").text = rocketLaunchCondition.GetLaunchStatusMessage(flag2);
			component.GetReference<LocText>("Label").color = ((!flag2) ? Color.red : Color.black);
			component.GetReference<Image>("Box").color = ((!flag2) ? Color.red : Color.black);
			component.GetReference<Image>("Check").gameObject.SetActive(flag2);
			gameObject.GetComponent<ToolTip>().SetSimpleTooltip(rocketLaunchCondition.GetLaunchStatusTooltip(flag2));
		}
		foreach (KeyValuePair<RocketLaunchCondition, GameObject> keyValuePair in this.conditionTable)
		{
			if (!launchConditionList.Contains(keyValuePair.Key))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.ClearConditions();
			this.ConfigureConditions();
		}
		this.destinationButton.onClick = delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
	}

	protected override void OnCleanUp()
	{
		this.updateHandle.ClearScheduler();
		base.OnCleanUp();
	}

	private LaunchConditionManager target;

	public GameObject conditionListContainer;

	public GameObject prefabConditionLineItem;

	public MultiToggle destinationButton;

	private Dictionary<RocketLaunchCondition, GameObject> conditionTable = new Dictionary<RocketLaunchCondition, GameObject>();

	private SchedulerHandle updateHandle;
}
