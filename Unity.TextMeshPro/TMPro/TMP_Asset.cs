using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore;

namespace TMPro
{
	[Serializable]
	public abstract class TMP_Asset : ScriptableObject
	{
		public string version
		{
			get
			{
				return this.m_Version;
			}
			internal set
			{
				this.m_Version = value;
			}
		}

		public int instanceID
		{
			get
			{
				if (this.m_InstanceID == 0)
				{
					this.m_InstanceID = base.GetInstanceID();
				}
				return this.m_InstanceID;
			}
		}

		public int hashCode
		{
			get
			{
				if (this.m_HashCode == 0)
				{
					this.m_HashCode = TMP_TextUtilities.GetHashCode(base.name);
				}
				return this.m_HashCode;
			}
			set
			{
				this.m_HashCode = value;
			}
		}

		public FaceInfo faceInfo
		{
			get
			{
				return this.m_FaceInfo;
			}
			set
			{
				this.m_FaceInfo = value;
			}
		}

		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
			}
		}

		public int materialHashCode
		{
			get
			{
				if (this.m_MaterialHashCode == 0)
				{
					if (this.m_Material == null)
					{
						return 0;
					}
					this.m_MaterialHashCode = TMP_TextUtilities.GetSimpleHashCode(this.m_Material.name);
				}
				return this.m_MaterialHashCode;
			}
			set
			{
				this.m_MaterialHashCode = value;
			}
		}

		[SerializeField]
		internal string m_Version;

		internal int m_InstanceID;

		internal int m_HashCode;

		[SerializeField]
		internal FaceInfo m_FaceInfo;

		[SerializeField]
		[FormerlySerializedAs("material")]
		internal Material m_Material;

		internal int m_MaterialHashCode;
	}
}
