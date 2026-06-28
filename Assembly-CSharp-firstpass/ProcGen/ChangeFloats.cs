using System;
using System.Runtime.InteropServices;
using KSerialization.Converters;

namespace ProcGen
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
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
