using System;
using Klei.AI;

public interface IAttributeFormatter
{
	string GetFormattedAttribute(AttributeInstance instance, bool tooltip);

	string GetFormattedModifier(AttributeModifier modifier);
}
