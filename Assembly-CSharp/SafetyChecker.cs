using System;
using System.Runtime.InteropServices;

public class SafetyChecker
{
	public SafetyChecker(SafetyChecker.Condition[] conditions)
	{
		this.conditions = conditions;
	}

	public SafetyChecker.Condition[] conditions { get; private set; }

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

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Condition
	{
		public Condition(string id, int condition_mask, SafetyChecker.Condition.Callback condition_callback)
		{
			this.callback = condition_callback;
			this.mask = condition_mask;
		}

		public SafetyChecker.Condition.Callback callback { get; private set; }

		public int mask { get; private set; }

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
		}

		public Navigator navigator;

		public OxygenBreather oxygenBreather;

		public SimTemperatureTransfer temperatureTransferer;

		public PrimaryElement primaryElement;

		public MinionBrain minionBrain;

		public int cell;
	}
}
