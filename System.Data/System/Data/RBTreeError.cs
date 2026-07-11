using System;

namespace System.Data
{
	internal enum RBTreeError
	{
		InvalidPageSize = 1,
		PagePositionInSlotInUse = 3,
		NoFreeSlots,
		InvalidStateinInsert,
		InvalidNextSizeInDelete = 7,
		InvalidStateinDelete,
		InvalidNodeSizeinDelete,
		InvalidStateinEndDelete,
		CannotRotateInvalidsuccessorNodeinDelete,
		IndexOutOFRangeinGetNodeByIndex = 13,
		RBDeleteFixup,
		UnsupportedAccessMethod1,
		UnsupportedAccessMethod2,
		UnsupportedAccessMethodInNonNillRootSubtree,
		AttachedNodeWithZerorbTreeNodeId,
		CompareNodeInDataRowTree,
		CompareSateliteTreeNodeInDataRowTree,
		NestedSatelliteTreeEnumerator
	}
}
