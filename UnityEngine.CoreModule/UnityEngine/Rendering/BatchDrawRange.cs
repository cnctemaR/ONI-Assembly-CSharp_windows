using System;

namespace UnityEngine.Rendering
{
	public struct BatchDrawRange
	{
		public BatchDrawCommandType drawCommandsType;

		public uint drawCommandsBegin;

		public uint drawCommandsCount;

		public BatchFilterSettings filterSettings;
	}
}
