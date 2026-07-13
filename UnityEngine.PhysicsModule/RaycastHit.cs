using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Interfaces/IRaycast.h")]
	[NativeHeader("PhysicsScriptingClasses.h")]
	[UsedByNativeCode]
	[NativeHeader("Modules/Physics/RaycastHit.h")]
	public struct RaycastHit
	{
		public Collider collider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Collider) as Collider;
			}
		}

		public int colliderInstanceID
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Vector3 point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		public Vector3 barycentricCoordinate
		{
			get
			{
				return new Vector3(1f - (this.m_UV.y + this.m_UV.x), this.m_UV.x, this.m_UV.y);
			}
			set
			{
				this.m_UV = value;
			}
		}

		public float distance
		{
			get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = value;
			}
		}

		public int triangleIndex
		{
			get
			{
				return (int)this.m_FaceID;
			}
		}

		[NativeMethod("CalculateRaycastTexCoord", true, true)]
		private static Vector2 CalculateRaycastTexCoord(int colliderInstanceID, Vector2 uv, Vector3 pos, uint face, int textcoord)
		{
			Vector2 vector;
			RaycastHit.CalculateRaycastTexCoord_Injected(colliderInstanceID, ref uv, ref pos, face, textcoord, out vector);
			return vector;
		}

		public Vector2 textureCoord
		{
			get
			{
				return RaycastHit.CalculateRaycastTexCoord(this.m_Collider, this.m_UV, this.m_Point, this.m_FaceID, 0);
			}
		}

		public Vector2 textureCoord2
		{
			get
			{
				return RaycastHit.CalculateRaycastTexCoord(this.m_Collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
			}
		}

		public Transform transform
		{
			get
			{
				Rigidbody rigidbody = this.rigidbody;
				bool flag = rigidbody != null;
				Transform transform;
				if (flag)
				{
					transform = rigidbody.transform;
				}
				else
				{
					bool flag2 = this.collider != null;
					if (flag2)
					{
						transform = this.collider.transform;
					}
					else
					{
						transform = null;
					}
				}
				return transform;
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				return (this.collider != null) ? this.collider.attachedRigidbody : null;
			}
		}

		public ArticulationBody articulationBody
		{
			get
			{
				return (this.collider != null) ? this.collider.attachedArticulationBody : null;
			}
		}

		public Vector2 lightmapCoord
		{
			get
			{
				Vector2 vector = RaycastHit.CalculateRaycastTexCoord(this.m_Collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
				bool flag = this.collider.GetComponent<Renderer>() != null;
				if (flag)
				{
					Vector4 lightmapScaleOffset = this.collider.GetComponent<Renderer>().lightmapScaleOffset;
					vector.x = vector.x * lightmapScaleOffset.x + lightmapScaleOffset.z;
					vector.y = vector.y * lightmapScaleOffset.y + lightmapScaleOffset.w;
				}
				return vector;
			}
		}

		[Obsolete("Use textureCoord2 instead. (UnityUpgradable) -> textureCoord2")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Vector2 textureCoord1
		{
			get
			{
				return this.textureCoord2;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateRaycastTexCoord_Injected(int colliderInstanceID, ref Vector2 uv, ref Vector3 pos, uint face, int textcoord, out Vector2 ret);

		[NativeName("point")]
		internal Vector3 m_Point;

		[NativeName("normal")]
		internal Vector3 m_Normal;

		[NativeName("faceID")]
		internal uint m_FaceID;

		[NativeName("distance")]
		internal float m_Distance;

		[NativeName("uv")]
		internal Vector2 m_UV;

		[NativeName("collider")]
		internal int m_Collider;
	}
}
