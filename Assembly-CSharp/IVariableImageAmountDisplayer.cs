using System;
using Klei.AI;
using UnityEngine;

public interface IVariableImageAmountDisplayer : IAmountDisplayer
{
	Sprite GetIcon(Amount master, AmountInstance instance);
}
