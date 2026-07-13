using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualNodePseudoStateData.h")]
	internal struct VisualNodePseudoStateData
	{
		public PseudoStates States;

		public PseudoStates TriggerMask;

		public PseudoStates DependencyMask;
	}
}
