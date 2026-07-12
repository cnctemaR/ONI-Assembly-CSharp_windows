using System;

namespace UnityEngine.AI
{
	public struct NavMeshLinkInstance
	{
		public bool valid
		{
			get
			{
				return this.id != 0 && NavMesh.IsValidLinkHandle(this.id);
			}
		}

		internal int id { readonly get; set; }

		public void Remove()
		{
			NavMesh.RemoveLinkInternal(this.id);
		}

		public Object owner
		{
			get
			{
				return NavMesh.InternalGetLinkOwner(this.id);
			}
			set
			{
				int num = ((value != null) ? value.GetInstanceID() : 0);
				bool flag = !NavMesh.InternalSetLinkOwner(this.id, num);
				if (flag)
				{
					Debug.LogError("Cannot set 'owner' on an invalid NavMeshLinkInstance");
				}
			}
		}
	}
}
