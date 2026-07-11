using System;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>The instance is returned when adding NavMesh data.</para>
	/// </summary>
	public struct NavMeshDataInstance
	{
		/// <summary>
		///   <para>True if the NavMesh data is added to the navigation system - otherwise false (Read Only).</para>
		/// </summary>
		public bool valid
		{
			get
			{
				return this.m_Handle != 0 && NavMesh.IsValidNavMeshDataHandle(this.m_Handle);
			}
		}

		internal int id
		{
			get
			{
				return this.m_Handle;
			}
			set
			{
				this.m_Handle = value;
			}
		}

		/// <summary>
		///   <para>Removes this instance from the NavMesh system.</para>
		/// </summary>
		public void Remove()
		{
			NavMesh.RemoveNavMeshDataInternal(this.id);
		}

		/// <summary>
		///   <para>Get or set the owning Object.</para>
		/// </summary>
		public Object owner
		{
			get
			{
				return NavMesh.InternalGetOwner(this.id);
			}
			set
			{
				int num = ((!(value != null)) ? 0 : value.GetInstanceID());
				if (!NavMesh.InternalSetOwner(this.id, num))
				{
					Debug.LogError("Cannot set 'owner' on an invalid NavMeshDataInstance");
				}
			}
		}

		private int m_Handle;
	}
}
