using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class QuestInstance : ISaveLoadable
{
	public HashedString Id
	{
		get
		{
			return this.quest.IdHash;
		}
	}

	public int CriteriaCount
	{
		get
		{
			return this.quest.Criteria.Length;
		}
	}

	public string Name
	{
		get
		{
			return this.quest.Name;
		}
	}

	public string CompletionText
	{
		get
		{
			return this.quest.CompletionText;
		}
	}

	public bool IsStarted
	{
		get
		{
			return this.currentState > Quest.State.NotStarted;
		}
	}

	public bool IsComplete
	{
		get
		{
			return this.currentState == Quest.State.Completed;
		}
	}

	public float CurrentProgress { get; private set; }

	public Quest.State CurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	public QuestInstance(Quest quest)
	{
		this.quest = quest;
		this.criteriaStates = new Dictionary<int, QuestInstance.CriteriaState>(quest.Criteria.Length);
		for (int i = 0; i < quest.Criteria.Length; i++)
		{
			QuestCriteria questCriteria = quest.Criteria[i];
			QuestInstance.CriteriaState criteriaState = new QuestInstance.CriteriaState
			{
				Handle = i
			};
			if (questCriteria.TargetValues != null)
			{
				if ((questCriteria.EvaluationBehaviors & QuestCriteria.BehaviorFlags.TrackItems) == QuestCriteria.BehaviorFlags.TrackItems)
				{
					criteriaState.SatisfyingItems = new Tag[questCriteria.TargetValues.Length * questCriteria.RequiredCount];
				}
				if ((questCriteria.EvaluationBehaviors & QuestCriteria.BehaviorFlags.TrackValues) == QuestCriteria.BehaviorFlags.TrackValues)
				{
					criteriaState.CurrentValues = new float[questCriteria.TargetValues.Length * questCriteria.RequiredCount];
				}
			}
			this.criteriaStates[questCriteria.CriteriaId.GetHash()] = criteriaState;
		}
	}

	public void Initialize(Quest quest)
	{
		this.quest = quest;
		this.ValidateCriteriasOnLoad();
		this.UpdateQuestProgress(false);
	}

	public bool HasCriteria(HashedString criteriaId)
	{
		return this.criteriaStates.ContainsKey(criteriaId.HashValue);
	}

	public bool HasBehavior(QuestCriteria.BehaviorFlags behavior)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < this.quest.Criteria.Length)
		{
			flag = (this.quest.Criteria[num].EvaluationBehaviors & behavior) > QuestCriteria.BehaviorFlags.None;
			num++;
		}
		return flag;
	}

	public int GetTargetCount(HashedString criteriaId)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState))
		{
			return 0;
		}
		return this.quest.Criteria[criteriaState.Handle].RequiredCount;
	}

	public int GetCurrentCount(HashedString criteriaId)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState))
		{
			return 0;
		}
		return criteriaState.CurrentCount;
	}

	public float GetCurrentValue(HashedString criteriaId, int valueHandle = 0)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState) || criteriaState.CurrentValues == null)
		{
			return float.NaN;
		}
		return criteriaState.CurrentValues[valueHandle];
	}

	public float GetTargetValue(HashedString criteriaId, int valueHandle = 0)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState))
		{
			return float.NaN;
		}
		if (this.quest.Criteria[criteriaState.Handle].TargetValues == null)
		{
			return float.NaN;
		}
		return this.quest.Criteria[criteriaState.Handle].TargetValues[valueHandle];
	}

	public Tag GetSatisfyingItem(HashedString criteriaId, int valueHandle = 0)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState) || criteriaState.SatisfyingItems == null)
		{
			return default(Tag);
		}
		return criteriaState.SatisfyingItems[valueHandle];
	}

	public float GetAreaAverage(HashedString criteriaId)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState))
		{
			return float.NaN;
		}
		if (!QuestCriteria.HasBehavior(this.quest.Criteria[criteriaState.Handle].EvaluationBehaviors, (QuestCriteria.BehaviorFlags)5))
		{
			return float.NaN;
		}
		float num = 0f;
		for (int i = 0; i < criteriaState.CurrentValues.Length; i++)
		{
			num += criteriaState.CurrentValues[i];
		}
		return num / (float)criteriaState.CurrentValues.Length;
	}

	public bool IsItemRedundant(HashedString criteriaId, Tag item)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState) || criteriaState.SatisfyingItems == null)
		{
			return false;
		}
		bool flag = false;
		int num = 0;
		while (!flag && num < criteriaState.SatisfyingItems.Length)
		{
			flag = criteriaState.SatisfyingItems[num] == item;
			num++;
		}
		return flag;
	}

	public bool IsCriteriaSatisfied(HashedString id)
	{
		QuestInstance.CriteriaState criteriaState;
		return this.criteriaStates.TryGetValue(id.HashValue, out criteriaState) && this.quest.Criteria[criteriaState.Handle].IsSatisfied(criteriaState.SatisfactionState, this.GetSatisfactionMask(criteriaState));
	}

	public bool IsCriteriaSatisfied(Tag id)
	{
		QuestInstance.CriteriaState criteriaState;
		return this.criteriaStates.TryGetValue(id.GetHash(), out criteriaState) && this.quest.Criteria[criteriaState.Handle].IsSatisfied(criteriaState.SatisfactionState, this.GetSatisfactionMask(criteriaState));
	}

	public void TrackAreaForCriteria(HashedString criteriaId, Extents area)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(criteriaId.HashValue, out criteriaState))
		{
			return;
		}
		int num = area.width * area.height;
		QuestCriteria questCriteria = this.quest.Criteria[criteriaState.Handle];
		global::Debug.Assert(num <= 32);
		if (QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues))
		{
			criteriaState.CurrentValues = new float[num];
		}
		if (QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackItems))
		{
			criteriaState.SatisfyingItems = new Tag[num];
		}
		this.criteriaStates[criteriaId.HashValue] = criteriaState;
	}

	private uint GetSatisfactionMask(QuestInstance.CriteriaState state)
	{
		QuestCriteria questCriteria = this.quest.Criteria[state.Handle];
		if (QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea))
		{
			int num = 0;
			if (state.SatisfyingItems != null)
			{
				num = state.SatisfyingItems.Length;
			}
			else if (state.CurrentValues != null)
			{
				num = state.CurrentValues.Length;
			}
			return (uint)(Mathf.Pow(2f, (float)num) - 1f);
		}
		return questCriteria.GetSatisfactionMask();
	}

	public int TrackProgress(Quest.ItemData data, out bool dataSatisfies, out bool itemIsRedundant)
	{
		dataSatisfies = false;
		itemIsRedundant = false;
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(data.CriteriaId.HashValue, out criteriaState))
		{
			return -1;
		}
		int valueHandle = data.ValueHandle;
		QuestCriteria questCriteria = this.quest.Criteria[criteriaState.Handle];
		dataSatisfies = this.DataSatisfiesCriteria(data, ref valueHandle);
		if (valueHandle == -1)
		{
			return valueHandle;
		}
		bool flag = QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.AllowsRegression);
		bool flag2 = QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackItems);
		Tag tag = (flag2 ? criteriaState.SatisfyingItems[valueHandle] : default(Tag));
		if (dataSatisfies)
		{
			itemIsRedundant = QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.UniqueItems) && this.IsItemRedundant(data.CriteriaId, data.SatisfyingItem);
			if (itemIsRedundant)
			{
				return valueHandle;
			}
			tag = data.SatisfyingItem;
			criteriaState.SatisfactionState |= questCriteria.GetValueMask(valueHandle);
		}
		else if (flag)
		{
			criteriaState.SatisfactionState &= ~questCriteria.GetValueMask(valueHandle);
		}
		if (QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues))
		{
			criteriaState.CurrentValues[valueHandle] = data.CurrentValue;
		}
		if (flag2)
		{
			criteriaState.SatisfyingItems[valueHandle] = tag;
		}
		bool flag3 = this.IsCriteriaSatisfied(data.CriteriaId);
		bool flag4 = questCriteria.IsSatisfied(criteriaState.SatisfactionState, this.GetSatisfactionMask(criteriaState));
		if (flag3 != flag4)
		{
			criteriaState.CurrentCount += (flag3 ? (-1) : 1);
			if (flag4 && criteriaState.CurrentCount < questCriteria.RequiredCount)
			{
				criteriaState.SatisfactionState = 0U;
			}
		}
		this.criteriaStates[data.CriteriaId.HashValue] = criteriaState;
		this.UpdateQuestProgress(true);
		return valueHandle;
	}

	public bool DataSatisfiesCriteria(Quest.ItemData data, ref int valueHandle)
	{
		QuestInstance.CriteriaState criteriaState;
		if (!this.criteriaStates.TryGetValue(data.CriteriaId.HashValue, out criteriaState))
		{
			return false;
		}
		QuestCriteria questCriteria = this.quest.Criteria[criteriaState.Handle];
		bool flag = questCriteria.AcceptedTags == null || (data.QualifyingTag.IsValid && questCriteria.AcceptedTags.Contains(data.QualifyingTag));
		if (flag && questCriteria.TargetValues == null)
		{
			valueHandle = 0;
		}
		if (!flag || valueHandle != -1)
		{
			return flag && questCriteria.ValueSatisfies(data.CurrentValue, valueHandle);
		}
		if (QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea))
		{
			valueHandle = data.LocalCellId;
		}
		int num = -1;
		bool flag2 = QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues);
		bool flag3 = false;
		int num2 = 0;
		while (!flag3 && num2 < questCriteria.TargetValues.Length)
		{
			if (questCriteria.ValueSatisfies(data.CurrentValue, num2))
			{
				flag3 = true;
				num = num2;
				break;
			}
			if (flag2 && (num == -1 || criteriaState.CurrentValues[num] > criteriaState.CurrentValues[num2]))
			{
				num = num2;
			}
			num2++;
		}
		if (valueHandle == -1 && num != -1)
		{
			valueHandle = questCriteria.RequiredCount * num + Mathf.Min(criteriaState.CurrentCount, questCriteria.RequiredCount - 1);
		}
		return flag3;
	}

	private void UpdateQuestProgress(bool startQuest = false)
	{
		if (!this.IsStarted && !startQuest)
		{
			return;
		}
		float currentProgress = this.CurrentProgress;
		Quest.State state = this.currentState;
		this.currentState = Quest.State.InProgress;
		this.CurrentProgress = 0f;
		float num = 0f;
		for (int i = 0; i < this.quest.Criteria.Length; i++)
		{
			QuestCriteria questCriteria = this.quest.Criteria[i];
			QuestInstance.CriteriaState criteriaState = this.criteriaStates[questCriteria.CriteriaId.GetHash()];
			float num2 = (float)((questCriteria.TargetValues != null) ? questCriteria.TargetValues.Length : 1);
			num += (float)questCriteria.RequiredCount;
			this.CurrentProgress += (float)criteriaState.CurrentCount;
			if (!this.IsCriteriaSatisfied(questCriteria.CriteriaId))
			{
				float num3 = 0f;
				int num4 = 0;
				while (questCriteria.TargetValues != null && (float)num4 < num2)
				{
					if ((criteriaState.SatisfactionState & questCriteria.GetValueMask(num4)) == 0U)
					{
						if (QuestCriteria.HasBehavior(questCriteria.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues))
						{
							int num5 = questCriteria.RequiredCount * num4 + Mathf.Min(criteriaState.CurrentCount, questCriteria.RequiredCount - 1);
							num3 += Mathf.Max(0f, criteriaState.CurrentValues[num5] / questCriteria.TargetValues[num4]);
						}
					}
					else
					{
						num3 += 1f;
					}
					num4++;
				}
				this.CurrentProgress += num3 / num2;
			}
		}
		this.CurrentProgress = Mathf.Clamp01(this.CurrentProgress / num);
		if (this.CurrentProgress == 1f)
		{
			this.currentState = Quest.State.Completed;
		}
		float num6 = this.CurrentProgress - currentProgress;
		if (state != this.currentState || Mathf.Abs(num6) > Mathf.Epsilon)
		{
			Action<QuestInstance, Quest.State, float> questProgressChanged = this.QuestProgressChanged;
			if (questProgressChanged == null)
			{
				return;
			}
			questProgressChanged(this, state, num6);
		}
	}

	public ICheckboxListGroupControl.CheckboxItem[] GetCheckBoxData(Func<int, string, QuestInstance, string> resolveToolTip = null)
	{
		ICheckboxListGroupControl.CheckboxItem[] array = new ICheckboxListGroupControl.CheckboxItem[this.quest.Criteria.Length];
		for (int i = 0; i < this.quest.Criteria.Length; i++)
		{
			QuestCriteria c = this.quest.Criteria[i];
			array[i] = new ICheckboxListGroupControl.CheckboxItem
			{
				text = c.Text,
				isOn = this.IsCriteriaSatisfied(c.CriteriaId),
				tooltip = c.Tooltip
			};
			if (resolveToolTip != null)
			{
				array[i].resolveTooltipCallback = (string tooltip, object owner) => resolveToolTip(c.CriteriaId.GetHash(), c.Tooltip, this);
			}
		}
		return array;
	}

	public void ValidateCriteriasOnLoad()
	{
		if (this.criteriaStates.Count != this.quest.Criteria.Length)
		{
			Dictionary<int, QuestInstance.CriteriaState> dictionary = new Dictionary<int, QuestInstance.CriteriaState>(this.quest.Criteria.Length);
			for (int i = 0; i < this.quest.Criteria.Length; i++)
			{
				QuestCriteria questCriteria = this.quest.Criteria[i];
				int hash = questCriteria.CriteriaId.GetHash();
				if (this.criteriaStates.ContainsKey(hash))
				{
					dictionary[hash] = this.criteriaStates[hash];
				}
				else
				{
					QuestInstance.CriteriaState criteriaState = new QuestInstance.CriteriaState
					{
						Handle = i
					};
					if (questCriteria.TargetValues != null)
					{
						if ((questCriteria.EvaluationBehaviors & QuestCriteria.BehaviorFlags.TrackItems) == QuestCriteria.BehaviorFlags.TrackItems)
						{
							criteriaState.SatisfyingItems = new Tag[questCriteria.TargetValues.Length * questCriteria.RequiredCount];
						}
						if ((questCriteria.EvaluationBehaviors & QuestCriteria.BehaviorFlags.TrackValues) == QuestCriteria.BehaviorFlags.TrackValues)
						{
							criteriaState.CurrentValues = new float[questCriteria.TargetValues.Length * questCriteria.RequiredCount];
						}
					}
					dictionary[hash] = criteriaState;
				}
			}
			this.criteriaStates = dictionary;
		}
	}

	public Action<QuestInstance, Quest.State, float> QuestProgressChanged;

	private Quest quest;

	[Serialize]
	private Dictionary<int, QuestInstance.CriteriaState> criteriaStates;

	[Serialize]
	private Quest.State currentState;

	private struct CriteriaState
	{
		public static bool ItemAlreadySatisfying(QuestInstance.CriteriaState state, Tag item)
		{
			bool flag = false;
			int num = 0;
			while (state.SatisfyingItems != null && num < state.SatisfyingItems.Length)
			{
				if (state.SatisfyingItems[num] == item)
				{
					flag = true;
					break;
				}
				num++;
			}
			return flag;
		}

		public int Handle;

		public int CurrentCount;

		public uint SatisfactionState;

		public Tag[] SatisfyingItems;

		public float[] CurrentValues;
	}
}
