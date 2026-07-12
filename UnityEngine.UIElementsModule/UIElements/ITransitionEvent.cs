using System;

namespace UnityEngine.UIElements
{
	public interface ITransitionEvent
	{
		StylePropertyNameCollection stylePropertyNames { get; }

		double elapsedTime { get; }
	}
}
