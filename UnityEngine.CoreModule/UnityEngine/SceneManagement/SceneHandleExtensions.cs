using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.SceneManagement
{
	internal static class SceneHandleExtensions
	{
		public static SceneHandle[] ToSceneHandleArray(this int[] integers)
		{
			return integers;
		}

		public static int[] ToIntArray(this SceneHandle[] sceneHandles)
		{
			return sceneHandles;
		}

		public static SceneHandle[] ToSceneHandleArray(this EntityId[] entityIds)
		{
			return entityIds;
		}

		public static EntityId[] ToEntityIdArray(this SceneHandle[] sceneHandles)
		{
			return sceneHandles;
		}

		public static List<SceneHandle> ToSceneHandleList(this List<int> integers)
		{
			return integers;
		}

		public static List<int> ToIntList(this List<SceneHandle> sceneHandles)
		{
			return sceneHandles;
		}

		public static List<SceneHandle> ToSceneHandleList(this List<EntityId> entityIds)
		{
			return entityIds;
		}

		public static List<EntityId> ToEntityIdList(this List<SceneHandle> sceneHandles)
		{
			return sceneHandles;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SceneHandleToIntArray
		{
			public static implicit operator SceneHandleExtensions.SceneHandleToIntArray(int[] integers)
			{
				return new SceneHandleExtensions.SceneHandleToIntArray
				{
					_integers = integers
				};
			}

			public static implicit operator SceneHandleExtensions.SceneHandleToIntArray(SceneHandle[] sceneHandles)
			{
				return new SceneHandleExtensions.SceneHandleToIntArray
				{
					_sceneHandles = sceneHandles
				};
			}

			public static implicit operator int[](SceneHandleExtensions.SceneHandleToIntArray value)
			{
				return value._integers;
			}

			public static implicit operator SceneHandle[](SceneHandleExtensions.SceneHandleToIntArray value)
			{
				return value._sceneHandles;
			}

			[FieldOffset(0)]
			private int[] _integers;

			[FieldOffset(0)]
			private SceneHandle[] _sceneHandles;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SceneHandleToEntityIdArray
		{
			public static implicit operator SceneHandleExtensions.SceneHandleToEntityIdArray(EntityId[] entityIds)
			{
				return new SceneHandleExtensions.SceneHandleToEntityIdArray
				{
					_entityIds = entityIds
				};
			}

			public static implicit operator SceneHandleExtensions.SceneHandleToEntityIdArray(SceneHandle[] sceneHandles)
			{
				return new SceneHandleExtensions.SceneHandleToEntityIdArray
				{
					_sceneHandles = sceneHandles
				};
			}

			public static implicit operator EntityId[](SceneHandleExtensions.SceneHandleToEntityIdArray value)
			{
				return value._entityIds;
			}

			public static implicit operator SceneHandle[](SceneHandleExtensions.SceneHandleToEntityIdArray value)
			{
				return value._sceneHandles;
			}

			[FieldOffset(0)]
			private EntityId[] _entityIds;

			[FieldOffset(0)]
			private SceneHandle[] _sceneHandles;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SceneHandleToIntList
		{
			public static implicit operator SceneHandleExtensions.SceneHandleToIntList(List<int> integers)
			{
				return new SceneHandleExtensions.SceneHandleToIntList
				{
					_integers = integers
				};
			}

			public static implicit operator SceneHandleExtensions.SceneHandleToIntList(List<SceneHandle> sceneHandles)
			{
				return new SceneHandleExtensions.SceneHandleToIntList
				{
					_sceneHandles = sceneHandles
				};
			}

			public static implicit operator List<int>(SceneHandleExtensions.SceneHandleToIntList value)
			{
				return value._integers;
			}

			public static implicit operator List<SceneHandle>(SceneHandleExtensions.SceneHandleToIntList value)
			{
				return value._sceneHandles;
			}

			[FieldOffset(0)]
			private List<int> _integers;

			[FieldOffset(0)]
			private List<SceneHandle> _sceneHandles;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SceneHandleToEntityIdList
		{
			public static implicit operator SceneHandleExtensions.SceneHandleToEntityIdList(List<EntityId> entityIds)
			{
				return new SceneHandleExtensions.SceneHandleToEntityIdList
				{
					_entityIds = entityIds
				};
			}

			public static implicit operator SceneHandleExtensions.SceneHandleToEntityIdList(List<SceneHandle> sceneHandles)
			{
				return new SceneHandleExtensions.SceneHandleToEntityIdList
				{
					_sceneHandles = sceneHandles
				};
			}

			public static implicit operator List<EntityId>(SceneHandleExtensions.SceneHandleToEntityIdList value)
			{
				return value._entityIds;
			}

			public static implicit operator List<SceneHandle>(SceneHandleExtensions.SceneHandleToEntityIdList value)
			{
				return value._sceneHandles;
			}

			[FieldOffset(0)]
			private List<EntityId> _entityIds;

			[FieldOffset(0)]
			private List<SceneHandle> _sceneHandles;
		}
	}
}
