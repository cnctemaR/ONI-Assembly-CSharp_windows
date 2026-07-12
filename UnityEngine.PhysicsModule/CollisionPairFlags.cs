using System;

namespace UnityEngine
{
	internal enum CollisionPairFlags : ushort
	{
		RemovedShape = 1,
		RemovedOtherShape,
		ActorPairHasFirstTouch = 4,
		ActorPairLostTouch = 8,
		InternalHasImpulses = 16,
		InternalContactsAreFlipped = 32
	}
}
