using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements.UIR.Implementation;

namespace UnityEngine.UIElements.UIR
{
	internal struct RenderChainVEData
	{
		internal RenderChainCommand lastClosingOrLastCommand
		{
			get
			{
				return this.lastClosingCommand ?? this.lastCommand;
			}
		}

		internal static bool AllocatesID(BMPAlloc alloc)
		{
			return alloc.ownedState == OwnedState.Owned && alloc.IsValid();
		}

		internal static bool InheritsID(BMPAlloc alloc)
		{
			return alloc.ownedState == OwnedState.Inherited && alloc.IsValid();
		}

		public bool isIgnoringDynamicColorHint
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.IsIgnoringDynamicColorHint) == RenderDataFlags.IsIgnoringDynamicColorHint;
			}
		}

		internal VisualElement prev;

		internal VisualElement next;

		internal VisualElement groupTransformAncestor;

		internal VisualElement boneTransformAncestor;

		internal VisualElement prevDirty;

		internal VisualElement nextDirty;

		internal RenderDataFlags flags;

		internal int hierarchyDepth;

		internal RenderDataDirtyTypes dirtiedValues;

		internal uint dirtyID;

		internal RenderChainCommand firstCommand;

		internal RenderChainCommand lastCommand;

		internal RenderChainCommand firstClosingCommand;

		internal RenderChainCommand lastClosingCommand;

		internal bool isInChain;

		internal bool isHierarchyHidden;

		internal bool localFlipsWinding;

		internal bool localTransformScaleZero;

		internal bool worldFlipsWinding;

		public bool worldTransformScaleZero;

		internal ClipMethod clipMethod;

		internal int childrenStencilRef;

		internal int childrenMaskDepth;

		internal bool disableNudging;

		internal MeshHandle data;

		internal MeshHandle closingData;

		internal Matrix4x4 verticesSpace;

		internal int displacementUVStart;

		internal int displacementUVEnd;

		internal BMPAlloc transformID;

		internal BMPAlloc clipRectID;

		internal BMPAlloc opacityID;

		internal BMPAlloc textCoreSettingsID;

		internal BMPAlloc colorID;

		internal BMPAlloc backgroundColorID;

		internal BMPAlloc borderLeftColorID;

		internal BMPAlloc borderTopColorID;

		internal BMPAlloc borderRightColorID;

		internal BMPAlloc borderBottomColorID;

		internal BMPAlloc tintColorID;

		internal float compositeOpacity;

		internal Color backgroundColor;

		internal BasicNode<TextureEntry> textures;
	}
}
