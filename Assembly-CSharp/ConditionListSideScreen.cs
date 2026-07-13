using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionListSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		if (target != null)
		{
			this.targetConditionSet = target.GetComponent<IProcessConditionSet>();
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.Refresh();
		}
	}

	private void Refresh()
	{
		bool flag = false;
		List<ProcessCondition> list;
		using (ProcessCondition.ListPool.Get(out list))
		{
			this.targetConditionSet.PopulateConditionSet(ProcessCondition.ProcessConditionType.All, list);
			foreach (ProcessCondition processCondition in list)
			{
				if (!this.rows.ContainsKey(processCondition))
				{
					flag = true;
					break;
				}
			}
			foreach (KeyValuePair<ProcessCondition, GameObject> keyValuePair in this.rows)
			{
				if (!list.Contains(keyValuePair.Key))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.Rebuild();
			}
			foreach (KeyValuePair<ProcessCondition, GameObject> keyValuePair2 in this.rows)
			{
				ConditionListSideScreen.SetRowState(keyValuePair2.Value, keyValuePair2.Key);
			}
		}
	}

	public static void SetRowState(GameObject row, ProcessCondition condition)
	{
		HierarchyReferences component = row.GetComponent<HierarchyReferences>();
		ProcessCondition.Status status = condition.EvaluateCondition();
		component.GetReference<LocText>("Label").text = condition.GetStatusMessage(status);
		switch (status)
		{
		case ProcessCondition.Status.Failure:
			component.GetReference<LocText>("Label").color = ConditionListSideScreen.failedColor;
			component.GetReference<Image>("Box").color = ConditionListSideScreen.failedColor;
			break;
		case ProcessCondition.Status.Warning:
			component.GetReference<LocText>("Label").color = ConditionListSideScreen.warningColor;
			component.GetReference<Image>("Box").color = ConditionListSideScreen.warningColor;
			break;
		case ProcessCondition.Status.Ready:
			component.GetReference<LocText>("Label").color = ConditionListSideScreen.readyColor;
			component.GetReference<Image>("Box").color = ConditionListSideScreen.readyColor;
			break;
		}
		component.GetReference<Image>("Check").gameObject.SetActive(status == ProcessCondition.Status.Ready);
		component.GetReference<Image>("Dash").gameObject.SetActive(false);
		row.GetComponent<ToolTip>().SetSimpleTooltip(condition.GetStatusTooltip(status));
	}

	private void Rebuild()
	{
		this.ClearRows();
		this.BuildRows();
	}

	private void ClearRows()
	{
		foreach (KeyValuePair<ProcessCondition, GameObject> keyValuePair in this.rows)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.rows.Clear();
	}

	private void BuildRows()
	{
		List<ProcessCondition> list;
		using (ProcessCondition.ListPool.Get(out list))
		{
			this.targetConditionSet.PopulateConditionSet(ProcessCondition.ProcessConditionType.All, list);
			foreach (ProcessCondition processCondition in list)
			{
				if (processCondition.ShowInUI())
				{
					GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.rowContainer, true);
					this.rows.Add(processCondition, gameObject);
				}
			}
		}
	}

	public GameObject rowPrefab;

	public GameObject rowContainer;

	[Tooltip("This list is indexed by the ProcessCondition.Status enum")]
	public static Color readyColor = Color.black;

	public static Color failedColor = Color.red;

	public static Color warningColor = new Color(1f, 0.3529412f, 0f, 1f);

	private IProcessConditionSet targetConditionSet;

	private Dictionary<ProcessCondition, GameObject> rows = new Dictionary<ProcessCondition, GameObject>();
}
