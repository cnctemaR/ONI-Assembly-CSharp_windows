using System;
using Klei.AI;

public interface IAmountDisplayer : IAttributeFormatter
{
	string GetValueString(Amount master, AmountInstance instance);

	string GetDescription(Amount master, AmountInstance instance);

	string GetTooltip(Amount master, AmountInstance instance);
}
