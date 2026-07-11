using System;
using Klei.AI;

public interface IAmountDisplayer
{
	string GetValueString(Amount master, AmountInstance instance);

	string GetDescription(Amount master, AmountInstance instance);

	string GetTooltip(Amount master, AmountInstance instance);

	IAttributeFormatter Formatter { get; }
}
