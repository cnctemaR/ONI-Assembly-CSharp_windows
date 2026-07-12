using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestCriteria
{
	public string Text { get; private set; }

	public string Tooltip { get; private set; }

	public QuestCriteria(Tag id, float[] targetValues = null, int requiredCount = 1, HashSet<Tag> acceptedTags = null, QuestCriteria.BehaviorFlags flags = QuestCriteria.BehaviorFlags.None)
	{
		global::Debug.Assert(targetValues == null || (targetValues.Length != 0 && targetValues.Length <= 32));
		this.CriteriaId = id;
		this.EvaluationBehaviors = flags;
		this.TargetValues = targetValues;
		this.AcceptedTags = acceptedTags;
		this.RequiredCount = requiredCount;
	}

	public bool ValueSatisfies(float value, int valueHandle)
	{
		if (float.IsNaN(value))
		{
			return false;
		}
		float num = ((this.TargetValues == null) ? 0f : this.TargetValues[valueHandle]);
		return this.ValueSatisfies_Internal(value, num);
	}

	protected virtual bool ValueSatisfies_Internal(float current, float target)
	{
		return true;
	}

	public bool IsSatisfied(uint satisfactionState, uint satisfactionMask)
	{
		return (satisfactionState & satisfactionMask) == satisfactionMask;
	}

	public void PopulateStrings(string prefix)
	{
		string text = this.CriteriaId.Name.ToUpperInvariant();
		StringEntry stringEntry;
		if (Strings.TryGet(prefix + "CRITERIA." + text + ".NAME", out stringEntry))
		{
			this.Text = stringEntry.String;
		}
		if (Strings.TryGet(prefix + "CRITERIA." + text + ".TOOLTIP", out stringEntry))
		{
			this.Tooltip = stringEntry.String;
		}
	}

	public uint GetSatisfactionMask()
	{
		if (this.TargetValues == null)
		{
			return 1U;
		}
		return (uint)Mathf.Pow(2f, (float)(this.TargetValues.Length - 1));
	}

	public uint GetValueMask(int valueHandle)
	{
		if (this.TargetValues == null)
		{
			return 1U;
		}
		if (!QuestCriteria.HasBehavior(this.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea))
		{
			valueHandle %= this.TargetValues.Length;
		}
		return 1U << valueHandle;
	}

	public static bool HasBehavior(QuestCriteria.BehaviorFlags flags, QuestCriteria.BehaviorFlags behavior)
	{
		return (flags & behavior) == behavior;
	}

	public const int MAX_VALUES = 32;

	public const int INVALID_VALUE = -1;

	public readonly Tag CriteriaId;

	public readonly QuestCriteria.BehaviorFlags EvaluationBehaviors;

	public readonly float[] TargetValues;

	public readonly int RequiredCount = 1;

	public readonly HashSet<Tag> AcceptedTags;

	public enum BehaviorFlags
	{
		None,
		TrackArea,
		AllowsRegression,
		TrackValues = 4,
		TrackItems = 8,
		UniqueItems = 24
	}
}
