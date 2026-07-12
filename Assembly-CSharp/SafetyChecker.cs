using System;

public class SafetyChecker
{
	public SafetyChecker.Condition[] conditions { get; private set; }

	public SafetyChecker(SafetyChecker.Condition[] conditions)
	{
		this.conditions = conditions;
	}

	public int GetSafetyConditions(int cell, int cost, SafetyChecker.Context context, out bool all_conditions_met)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.conditions.Length; i++)
		{
			SafetyChecker.Condition condition = this.conditions[i];
			if (condition.callback(cell, cost, context))
			{
				num |= condition.mask;
				num2++;
			}
		}
		all_conditions_met = num2 == this.conditions.Length;
		return num;
	}

	public struct Condition
	{
		public SafetyChecker.Condition.Callback callback { readonly get; private set; }

		public int mask { readonly get; private set; }

		public Condition(string id, int condition_mask, SafetyChecker.Condition.Callback condition_callback)
		{
			this = default(SafetyChecker.Condition);
			this.callback = condition_callback;
			this.mask = condition_mask;
		}

		public delegate bool Callback(int cell, int cost, SafetyChecker.Context context);
	}

	public struct Context
	{
		public Context(KMonoBehaviour cmp)
		{
			this.cell = Grid.PosToCell(cmp);
			this.navigator = cmp.GetComponent<Navigator>();
			this.oxygenBreather = cmp.GetComponent<OxygenBreather>();
			this.minionBrain = cmp.GetComponent<MinionBrain>();
			this.temperatureTransferer = cmp.GetComponent<SimTemperatureTransfer>();
			this.primaryElement = cmp.GetComponent<PrimaryElement>();
			this.worldID = this.navigator.GetMyWorldId();
		}

		public Navigator navigator;

		public OxygenBreather oxygenBreather;

		public SimTemperatureTransfer temperatureTransferer;

		public PrimaryElement primaryElement;

		public MinionBrain minionBrain;

		public int worldID;

		public int cell;
	}
}
