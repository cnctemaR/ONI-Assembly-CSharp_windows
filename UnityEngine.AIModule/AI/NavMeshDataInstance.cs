using System;

namespace UnityEngine.AI
{
	public struct NavMeshDataInstance
	{
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

		public void Remove()
		{
			NavMesh.RemoveNavMeshDataInternal(this.id);
		}

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
