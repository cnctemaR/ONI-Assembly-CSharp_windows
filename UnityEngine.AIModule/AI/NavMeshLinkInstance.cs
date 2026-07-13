using System;

namespace UnityEngine.AI
{
	public struct NavMeshLinkInstance
	{
		internal int id { readonly get; set; }

		[Obsolete("valid has been deprecated. Use NavMesh.IsLinkValid() instead.")]
		public bool valid
		{
			get
			{
				return NavMesh.IsValidLinkHandle(this.id);
			}
		}

		[Obsolete("Remove() has been deprecated. Use NavMesh.RemoveLink() instead.")]
		public void Remove()
		{
			NavMesh.RemoveLinkInternal(this.id);
		}

		[Obsolete("owner has been deprecated. Use NavMesh.GetLinkOwner() and NavMesh.SetLinkOwner() instead.")]
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
