using System;
using KSerialization.Converters;

namespace ProcGen
{
	public struct ChangeFloats
	{
		[StringEnumConverter]
		public ChangeFloats.ChangeType change { get; private set; }

		public MinMax value { get; private set; }

		public enum ChangeType
		{
			NoChange,
			OverrideRange,
			OverrideSet,
			TakeNoiseVal
		}
	}
}
