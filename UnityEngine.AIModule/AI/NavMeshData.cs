using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Contains and represents NavMesh data.</para>
	/// </summary>
	public sealed class NavMeshData : Object
	{
		/// <summary>
		///   <para>Constructs a new object for representing a NavMesh for the default agent type.</para>
		/// </summary>
		public NavMeshData()
		{
			NavMeshData.Internal_Create(this, 0);
		}

		/// <summary>
		///   <para>Constructs a new object representing a NavMesh for the specified agent type.</para>
		/// </summary>
		/// <param name="agentTypeID">The agent type ID to create a NavMesh for.</param>
		public NavMeshData(int agentTypeID)
		{
			NavMeshData.Internal_Create(this, agentTypeID);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] NavMeshData mono, int agentTypeID);

		/// <summary>
		///   <para>Returns the bounding volume of the input geometry used to build this NavMesh (Read Only).</para>
		/// </summary>
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

		/// <summary>
		///   <para>Gets or sets the world space position of the NavMesh data.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Gets or sets the orientation of the NavMesh data.</para>
		/// </summary>
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
