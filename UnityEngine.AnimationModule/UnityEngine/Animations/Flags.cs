using System;

namespace UnityEngine.Animations
{
	internal enum Flags
	{
		kNone,
		kDiscrete,
		kPPtr,
		kSerializeReference = 4,
		kPhantom = 8,
		kUnknown = 16
	}
}
