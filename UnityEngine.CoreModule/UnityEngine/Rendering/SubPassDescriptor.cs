using System;

namespace UnityEngine.Rendering
{
	public struct SubPassDescriptor
	{
		public AttachmentIndexArray inputs;

		public AttachmentIndexArray colorOutputs;

		public SubPassFlags flags;
	}
}
