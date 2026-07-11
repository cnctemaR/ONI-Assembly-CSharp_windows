using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Structure used to get information back from a raycast.</para>
	/// </summary>
	[NativeHeader("Runtime/Dynamics/RaycastHit.h")]
	[NativeHeader("PhysicsScriptingClasses.h")]
	[NativeHeader("Runtime/Interfaces/IRaycast.h")]
	[UsedByNativeCode]
	public struct RaycastHit
	{
		/// <summary>
		///   <para>The Collider that was hit.</para>
		/// </summary>
		public Collider collider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Collider) as Collider;
			}
		}

		/// <summary>
		///   <para>The impact point in world space where the ray hit the collider.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The normal of the surface the ray hit.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The barycentric coordinate of the triangle we hit.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The distance from the ray's origin to the impact point.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The index of the triangle that was hit.</para>
		/// </summary>
		public int triangleIndex
		{
			get
			{
				return (int)this.m_FaceID;
			}
		}

		[FreeFunction]
		private static Vector2 CalculateRaycastTexCoord(Collider collider, Vector2 uv, Vector3 pos, uint face, int textcoord)
		{
			Vector2 vector;
			RaycastHit.CalculateRaycastTexCoord_Injected(collider, ref uv, ref pos, face, textcoord, out vector);
			return vector;
		}

		/// <summary>
		///   <para>The uv texture coordinate at the collision location.</para>
		/// </summary>
		public Vector2 textureCoord
		{
			get
			{
				return RaycastHit.CalculateRaycastTexCoord(this.collider, this.m_UV, this.m_Point, this.m_FaceID, 0);
			}
		}

		/// <summary>
		///   <para>The secondary uv texture coordinate at the impact point.</para>
		/// </summary>
		public Vector2 textureCoord2
		{
			get
			{
				return RaycastHit.CalculateRaycastTexCoord(this.collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
			}
		}

		[Obsolete("Use textureCoord2 instead. (UnityUpgradable) -> textureCoord2")]
		public Vector2 textureCoord1
		{
			get
			{
				return this.textureCoord2;
			}
		}

		/// <summary>
		///   <para>The Transform of the rigidbody or collider that was hit.</para>
		/// </summary>
		public Transform transform
		{
			get
			{
				Rigidbody rigidbody = this.rigidbody;
				Transform transform;
				if (rigidbody != null)
				{
					transform = rigidbody.transform;
				}
				else if (this.collider != null)
				{
					transform = this.collider.transform;
				}
				else
				{
					transform = null;
				}
				return transform;
			}
		}

		/// <summary>
		///   <para>The Rigidbody of the collider that was hit. If the collider is not attached to a rigidbody then it is null.</para>
		/// </summary>
		public Rigidbody rigidbody
		{
			get
			{
				return (!(this.collider != null)) ? null : this.collider.attachedRigidbody;
			}
		}

		/// <summary>
		///   <para>The uv lightmap coordinate at the impact point.</para>
		/// </summary>
		public Vector2 lightmapCoord
		{
			get
			{
				Vector2 vector = RaycastHit.CalculateRaycastTexCoord(this.collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
				if (this.collider.GetComponent<Renderer>() != null)
				{
					Vector4 lightmapScaleOffset = this.collider.GetComponent<Renderer>().lightmapScaleOffset;
					vector.x = vector.x * lightmapScaleOffset.x + lightmapScaleOffset.z;
					vector.y = vector.y * lightmapScaleOffset.y + lightmapScaleOffset.w;
				}
				return vector;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateRaycastTexCoord_Injected(Collider collider, ref Vector2 uv, ref Vector3 pos, uint face, int textcoord, out Vector2 ret);

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
