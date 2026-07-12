using System;

namespace UnityEngine.VFX
{
	public struct VFXOutputEventArgs
	{
		public readonly int nameId { get; }

		public readonly VFXEventAttribute eventAttribute { get; }

		public VFXOutputEventArgs(int nameId, VFXEventAttribute eventAttribute)
		{
			this.nameId = nameId;
			this.eventAttribute = eventAttribute;
		}
	}
}
