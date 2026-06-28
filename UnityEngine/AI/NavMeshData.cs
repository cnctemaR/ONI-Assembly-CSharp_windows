using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.AI
{
	public sealed class NavMeshData : Object
	{
		public NavMeshData()
		{
			NavMeshData.Internal_Create(this, 0);
		}

		public NavMeshData(int agentTypeID)
		{
			NavMeshData.Internal_Create(this, agentTypeID);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] NavMeshData mono, int agentTypeID);

		public Bounds sourceBounds
		{
			get
			{
				Bounds bounds;
				this.INTERNAL_get_sourceBounds(out bounds);
				return bounds;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_sourceBounds(out Bounds value);

		public Vector3 position
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_position(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_position(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_position(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_position(ref Vector3 value);

		public Quaternion rotation
		{
			get
			{
				Quaternion quaternion;
				this.INTERNAL_get_rotation(out quaternion);
				return quaternion;
			}
			set
			{
				this.INTERNAL_set_rotation(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rotation(out Quaternion value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rotation(ref Quaternion value);
	}
}
