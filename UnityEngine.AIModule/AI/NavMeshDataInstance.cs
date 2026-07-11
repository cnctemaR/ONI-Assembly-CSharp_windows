using System;

namespace UnityEngine.AI
{
	public struct NavMeshDataInstance
	{
		public bool valid
		{
			get
			{
				return this.id != 0 && NavMesh.IsValidNavMeshDataHandle(this.id);
			}
		}

		internal int id { get; set; }

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
				int num = ((value != null) ? value.GetInstanceID() : 0);
				bool flag = !NavMesh.InternalSetOwner(this.id, num);
				if (flag)
				{
					Debug.LogError("Cannot set 'owner' on an invalid NavMeshDataInstance");
				}
			}
		}
	}
}
