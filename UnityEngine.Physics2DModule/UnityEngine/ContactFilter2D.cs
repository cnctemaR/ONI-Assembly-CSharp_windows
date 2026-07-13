using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeClass("ContactFilter", "struct ContactFilter;")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
	[Serializable]
	public struct ContactFilter2D
	{
		public static ContactFilter2D noFilter
		{
			get
			{
				return ContactFilter2D._noFilter;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CheckConsistency();

		public void ClearLayerMask()
		{
			this.useLayerMask = false;
		}

		public void SetLayerMask(LayerMask layerMask)
		{
			this.layerMask = layerMask;
			this.useLayerMask = true;
		}

		public void ClearDepth()
		{
			this.useDepth = false;
		}

		public void SetDepth(float minDepth, float maxDepth)
		{
			this.minDepth = minDepth;
			this.maxDepth = maxDepth;
			this.useDepth = true;
			this.CheckConsistency();
		}

		public void ClearNormalAngle()
		{
			this.useNormalAngle = false;
		}

		public void SetNormalAngle(float minNormalAngle, float maxNormalAngle)
		{
			this.minNormalAngle = minNormalAngle;
			this.maxNormalAngle = maxNormalAngle;
			this.useNormalAngle = true;
			this.CheckConsistency();
		}

		public bool isFiltering
		{
			get
			{
				return !this.useTriggers || this.useLayerMask || this.useDepth || this.useNormalAngle;
			}
		}

		public bool IsFilteringTrigger(Collider2D collider)
		{
			return !this.useTriggers && collider.isTrigger;
		}

		public bool IsFilteringLayerMask(GameObject obj)
		{
			return this.useLayerMask && (this.layerMask & (1 << obj.layer)) == 0;
		}

		public bool IsFilteringDepth(GameObject obj)
		{
			bool flag = !this.useDepth;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.minDepth > this.maxDepth;
				if (flag3)
				{
					float num = this.minDepth;
					this.minDepth = this.maxDepth;
					this.maxDepth = num;
				}
				float z = obj.transform.position.z;
				bool flag4 = z < this.minDepth || z > this.maxDepth;
				bool flag5 = this.useOutsideDepth;
				if (flag5)
				{
					flag2 = !flag4;
				}
				else
				{
					flag2 = flag4;
				}
			}
			return flag2;
		}

		public bool IsFilteringNormalAngle(Vector2 normal)
		{
			return ContactFilter2D.IsFilteringNormalAngle_Injected(ref this, ref normal);
		}

		public bool IsFilteringNormalAngle(float angle)
		{
			return this.IsFilteringNormalAngleUsingAngle(angle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsFilteringNormalAngleUsingAngle(float angle);

		internal static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(layerMask);
			contactFilter2D.SetDepth(minDepth, maxDepth);
			return contactFilter2D;
		}

		[Obsolete("ContactFilter2D.NoFilter method has been deprecated. Please use the static ContactFilter2D.noFilter property.", false)]
		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ContactFilter2D NoFilter()
		{
			return ContactFilter2D.noFilter;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsFilteringNormalAngle_Injected(ref ContactFilter2D _unity_self, [In] ref Vector2 normal);

		private static ContactFilter2D _noFilter = new ContactFilter2D
		{
			useTriggers = true,
			useLayerMask = false,
			layerMask = -1,
			useDepth = false,
			useOutsideDepth = false,
			minDepth = float.NegativeInfinity,
			maxDepth = float.PositiveInfinity,
			useNormalAngle = false,
			useOutsideNormalAngle = false,
			minNormalAngle = 0f,
			maxNormalAngle = 359.9999f
		};

		[NativeName("m_UseTriggers")]
		public bool useTriggers;

		[NativeName("m_UseLayerMask")]
		public bool useLayerMask;

		[NativeName("m_UseDepth")]
		public bool useDepth;

		[NativeName("m_UseOutsideDepth")]
		public bool useOutsideDepth;

		[NativeName("m_UseNormalAngle")]
		public bool useNormalAngle;

		[NativeName("m_UseOutsideNormalAngle")]
		public bool useOutsideNormalAngle;

		[NativeName("m_LayerMask")]
		public LayerMask layerMask;

		[NativeName("m_MinDepth")]
		public float minDepth;

		[NativeName("m_MaxDepth")]
		public float maxDepth;

		[NativeName("m_MinNormalAngle")]
		public float minNormalAngle;

		[NativeName("m_MaxNormalAngle")]
		public float maxNormalAngle;

		public const float NormalAngleUpperLimit = 359.9999f;
	}
}
