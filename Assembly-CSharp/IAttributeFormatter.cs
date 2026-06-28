using System;
using Klei.AI;

public interface IAttributeFormatter
{
	string GetFormattedAttribute(AttributeInstance instance);

	string GetFormattedModifier(AttributeModifier modifier);
}
