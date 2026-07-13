using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	internal static class EntityIdExtensions
	{
		public static int[] ToIntArray(this InstanceID[] instanceIds)
		{
			return Array.ConvertAll<InstanceID, int>(instanceIds, (InstanceID input) => input);
		}

		public static InstanceID[] ToInstanceIDArray(this int[] instanceIdInts)
		{
			return Array.ConvertAll<int, InstanceID>(instanceIdInts, (int input) => input);
		}

		public static List<int> ToIntList(this List<InstanceID> instanceIds)
		{
			return instanceIds.ConvertAll<int>((InstanceID input) => input);
		}

		public static List<InstanceID> ToInstanceIDList(this List<int> instanceIdInts)
		{
			return instanceIdInts.ConvertAll<InstanceID>((int input) => input);
		}

		public static int[] ToIntArray(this EntityId[] entityIds)
		{
			return Array.ConvertAll<EntityId, int>(entityIds, (EntityId input) => input);
		}

		public static EntityId[] ToEntityIdArray(this int[] entityIdInts)
		{
			return Array.ConvertAll<int, EntityId>(entityIdInts, (int input) => input);
		}

		public static List<int> ToIntList(this List<EntityId> entityIds)
		{
			return entityIds.ConvertAll<int>((EntityId input) => input);
		}

		public static List<EntityId> ToEntityIdList(this List<int> entityIdInts)
		{
			return entityIdInts.ConvertAll<EntityId>((int input) => input);
		}

		internal static EntityId[] AsEntityIdArray(this int[] instanceIds)
		{
			return new EntityIdExtensions.UnsafeTypeCastInstanceIDArray
			{
				ulongs = instanceIds
			}.instance_ids;
		}

		internal static int[] AsIntArray(this EntityId[] instanceIds)
		{
			return new EntityIdExtensions.UnsafeTypeCastInstanceIDArray
			{
				instance_ids = instanceIds
			}.ulongs;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct UnsafeTypeCastInstanceIDArray
		{
			[FieldOffset(0)]
			public EntityId[] instance_ids;

			[FieldOffset(0)]
			public int[] ulongs;
		}
	}
}
