using System;

namespace UnityEngine.UIElements
{
	[Flags]
	internal enum VisualElementFlags
	{
		WorldTransformDirty = 1,
		WorldTransformInverseDirty = 2,
		WorldClipDirty = 4,
		BoundingBoxDirty = 8,
		WorldBoundingBoxDirty = 16,
		EventInterestParentCategoriesDirty = 32,
		LayoutManual = 64,
		CompositeRoot = 128,
		RequireMeasureFunction = 256,
		EnableViewDataPersistence = 512,
		DisableClipping = 1024,
		NeedsAttachToPanelEvent = 2048,
		HierarchyDisplayed = 4096,
		StyleInitialized = 8192,
		DisableRendering = 16384,
		Needs3DBounds = 32768,
		LocalBounds3DDirty = 65536,
		LocalBoundsWithoutNested3DDirty = 131072,
		DetachedDataSource = 262144,
		PointerCapture = 524288,
		IsWorldSpaceRootUIDocument = 1048576,
		ReceivesHierarchyGeometryChangedEvents = 2097152,
		BoundingBoxDirtiedSinceLastLayoutPass = 4194304,
		Init = 458815
	}
}
