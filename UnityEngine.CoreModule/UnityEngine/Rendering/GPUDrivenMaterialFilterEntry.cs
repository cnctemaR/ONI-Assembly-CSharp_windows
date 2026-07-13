using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	internal struct GPUDrivenMaterialFilterEntry
	{
		public GPUDrivenBitOpType op;

		public int minQueueValue;

		public int maxQueueValue;

		public ShaderTagId keyTag;

		public ShaderTagId valueTag;

		public int flags;

		public string keyword;
	}
}
