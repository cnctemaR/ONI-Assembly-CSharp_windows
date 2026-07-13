using System;
using Unity.IntegerTime;

namespace UnityEngine.InputForUI
{
	internal interface IEventProperties
	{
		DiscreteTime timestamp { get; }

		EventSource eventSource { get; }

		uint playerId { get; }

		EventModifiers eventModifiers { get; }
	}
}
