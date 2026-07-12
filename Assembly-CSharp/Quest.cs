using System;

public class Quest : Resource
{
	public Quest(string id, QuestCriteria[] criteria)
		: base(id, id)
	{
		Debug.Assert(criteria.Length != 0);
		this.Criteria = criteria;
		string text = "STRINGS.CODEX.QUESTS." + id.ToUpperInvariant();
		StringEntry stringEntry;
		if (Strings.TryGet(text + ".NAME", out stringEntry))
		{
			this.Title = stringEntry.String;
		}
		if (Strings.TryGet(text + ".COMPLETE", out stringEntry))
		{
			this.CompletionText = stringEntry.String;
		}
		for (int i = 0; i < this.Criteria.Length; i++)
		{
			this.Criteria[i].PopulateStrings("STRINGS.CODEX.QUESTS.");
		}
	}

	public const string STRINGS_PREFIX = "STRINGS.CODEX.QUESTS.";

	public readonly QuestCriteria[] Criteria;

	public readonly string Title;

	public readonly string CompletionText;

	public struct ItemData
	{
		public int ValueHandle
		{
			get
			{
				return this.valueHandle - 1;
			}
			set
			{
				this.valueHandle = value + 1;
			}
		}

		public int LocalCellId;

		public float CurrentValue;

		public Tag SatisfyingItem;

		public Tag QualifyingTag;

		public HashedString CriteriaId;

		private int valueHandle;
	}

	public enum State
	{
		NotStarted,
		InProgress,
		Completed
	}
}
