using System;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>An instance representing a link available for pathfinding.</para>
	/// </summary>
	public struct NavMeshLinkInstance
	{
		/// <summary>
		///   <para>True if the NavMesh link is added to the navigation system - otherwise false (Read Only).</para>
		/// </summary>
		public bool valid
		{
			get
			{
				return this.m_Handle != 0 && NavMesh.IsValidLinkHandle(this.m_Handle);
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
		///   <para>Removes this instance from the game.</para>
		/// </summary>
		public void Remove()
		{
			NavMesh.RemoveLinkInternal(this.id);
		}

		/// <summary>
		///   <para>Get or set the owning Object.</para>
		/// </summary>
		public Object owner
		{
			get
			{
				return NavMesh.InternalGetLinkOwner(this.id);
			}
			set
			{
				int num = ((!(value != null)) ? 0 : value.GetInstanceID());
				if (!NavMesh.InternalSetLinkOwner(this.id, num))
				{
					Debug.LogError("Cannot set 'owner' on an invalid NavMeshLinkInstance");
				}
			}
		}

		private int m_Handle;
	}
}
