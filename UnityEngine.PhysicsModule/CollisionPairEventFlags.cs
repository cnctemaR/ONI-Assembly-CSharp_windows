using System;

namespace UnityEngine
{
	internal enum CollisionPairEventFlags : ushort
	{
		SolveContacts = 1,
		ModifyContacts,
		NotifyTouchFound = 4,
		NotifyTouchPersists = 8,
		NotifyTouchLost = 16,
		NotifyTouchCCD = 32,
		NotifyThresholdForceFound = 64,
		NotifyThresholdForcePersists = 128,
		NotifyThresholdForceLost = 256,
		NotifyContactPoint = 512,
		DetectDiscreteContact = 1024,
		DetectCCDContact = 2048,
		PreSolverVelocity = 4096,
		PostSolverVelocity = 8192,
		ContactEventPose = 16384,
		NextFree = 32768,
		ContactDefault = 1025,
		TriggerDefault = 1044
	}
}
