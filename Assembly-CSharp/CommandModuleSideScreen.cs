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
			global::Debug.LogError("Invalid gameObject received", null);
			return;
		}
		this.target = new_target.GetComponent<LaunchConditionManager>();
		if (this.target == null)
		{
			global::Debug.LogError("The gameObject received does not contain a LaunchConditionManager component", null);
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
		this.target.EvaluateConditions();
		for (int i = 0; i < this.target.NumConditions; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.prefabConditionLineItem, this.conditionListContainer, true);
			this.conditionTable.Add(this.target.conditions[i], gameObject);
		}
		this.RefreshConditions();
	}

	public void RefreshConditions()
	{
		bool flag = false;
		foreach (RocketLaunchCondition rocketLaunchCondition in this.target.conditions)
		{
			bool flag2 = false;
			foreach (KeyValuePair<RocketLaunchCondition, GameObject> keyValuePair in this.conditionTable)
			{
				if (keyValuePair.Key == rocketLaunchCondition)
				{
					flag2 = true;
					HierarchyReferences component = keyValuePair.Value.GetComponent<HierarchyReferences>();
					bool flag3 = false;
					bool flag4 = false;
					for (int i = 0; i < this.target.conditions.Count; i++)
					{
						if (this.target.conditions[i] == keyValuePair.Key)
						{
							flag4 = true;
							flag3 = this.target.conditions[i].EvaluateLaunchCondition();
						}
					}
					if (!flag4)
					{
						flag = true;
						break;
					}
					if (rocketLaunchCondition.GetParentCondition() != null && !rocketLaunchCondition.GetParentCondition().EvaluateLaunchCondition())
					{
						keyValuePair.Value.SetActive(false);
					}
					else if (!keyValuePair.Value.activeSelf)
					{
						keyValuePair.Value.SetActive(true);
					}
					component.GetReference<LocText>("Label").text = keyValuePair.Key.GetLaunchStatusMessage(true);
					component.GetReference<LocText>("Label").color = ((!flag3) ? Color.red : Color.black);
					component.GetReference<Image>("Box").color = ((!flag3) ? Color.red : Color.black);
					component.GetReference<Image>("Check").gameObject.SetActive(flag3);
					keyValuePair.Value.GetComponent<ToolTip>().SetSimpleTooltip(keyValuePair.Key.GetLaunchStatusTooltip(flag3));
				}
			}
			flag = flag || !flag2;
		}
		this.destinationButton.onClick = delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		if (flag)
		{
			this.ClearConditions();
			this.ConfigureConditions();
		}
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
