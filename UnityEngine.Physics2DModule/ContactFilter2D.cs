using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A set of parameters for filtering contact results.</para>
	/// </summary>
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
	[NativeClass("ContactFilter", "struct ContactFilter;")]
	[Serializable]
	public struct ContactFilter2D
	{
		/// <summary>
		///   <para>Sets the contact filter to not filter any ContactPoint2D.</para>
		/// </summary>
		/// <returns>
		///   <para>A copy of the contact filter set to not filter any ContactPoint2D.</para>
		/// </returns>
		public ContactFilter2D NoFilter()
		{
			this.useTriggers = true;
			this.useLayerMask = false;
			this.layerMask = -1;
			this.useDepth = false;
			this.useOutsideDepth = false;
			this.minDepth = float.NegativeInfinity;
			this.maxDepth = float.PositiveInfinity;
			this.useNormalAngle = false;
			this.useOutsideNormalAngle = false;
			this.minNormalAngle = 0f;
			this.maxNormalAngle = 359.9999f;
			return this;
		}

		private void CheckConsistency()
		{
			this.minDepth = ((this.minDepth != float.NegativeInfinity && this.minDepth != float.PositiveInfinity && !float.IsNaN(this.minDepth)) ? this.minDepth : float.MinValue);
			this.maxDepth = ((this.maxDepth != float.NegativeInfinity && this.maxDepth != float.PositiveInfinity && !float.IsNaN(this.maxDepth)) ? this.maxDepth : float.MaxValue);
			if (this.minDepth > this.maxDepth)
			{
				float num = this.minDepth;
				this.minDepth = this.maxDepth;
				this.maxDepth = num;
			}
			this.minNormalAngle = ((!float.IsNaN(this.minNormalAngle)) ? Mathf.Clamp(this.minNormalAngle, 0f, 359.9999f) : 0f);
			this.maxNormalAngle = ((!float.IsNaN(this.maxNormalAngle)) ? Mathf.Clamp(this.maxNormalAngle, 0f, 359.9999f) : 359.9999f);
			if (this.minNormalAngle > this.maxNormalAngle)
			{
				float num2 = this.minNormalAngle;
				this.minNormalAngle = this.maxNormalAngle;
				this.maxNormalAngle = num2;
			}
		}

		/// <summary>
		///   <para>Turns off layer mask filtering by setting useLayerMask to false.  The associated value of layerMask is not changed.</para>
		/// </summary>
		public void ClearLayerMask()
		{
			this.useLayerMask = false;
		}

		/// <summary>
		///   <para>Sets the layerMask filter property using the layerMask parameter provided and also enables layer mask filtering by setting useLayerMask to true.</para>
		/// </summary>
		/// <param name="layerMask">The value used to set the layerMask.</param>
		public void SetLayerMask(LayerMask layerMask)
		{
			this.layerMask = layerMask;
			this.useLayerMask = true;
		}

		/// <summary>
		///   <para>Turns off depth filtering by setting useDepth to false.  The associated values of minDepth and maxDepth are not changed.</para>
		/// </summary>
		public void ClearDepth()
		{
			this.useDepth = false;
		}

		/// <summary>
		///   <para>Sets the minDepth and maxDepth filter properties and turns on depth filtering by setting useDepth to true.</para>
		/// </summary>
		/// <param name="minDepth">The value used to set minDepth.</param>
		/// <param name="maxDepth">The value used to set maxDepth.</param>
		public void SetDepth(float minDepth, float maxDepth)
		{
			this.minDepth = minDepth;
			this.maxDepth = maxDepth;
			this.useDepth = true;
			this.CheckConsistency();
		}

		/// <summary>
		///   <para>Turns off normal angle filtering by setting useNormalAngle to false. The associated values of minNormalAngle and maxNormalAngle are not changed.</para>
		/// </summary>
		public void ClearNormalAngle()
		{
			this.useNormalAngle = false;
		}

		/// <summary>
		///   <para>Sets the minNormalAngle and maxNormalAngle filter properties and turns on normal angle filtering by setting useNormalAngle to true.</para>
		/// </summary>
		/// <param name="minNormalAngle">The value used to set the minNormalAngle.</param>
		/// <param name="maxNormalAngle">The value used to set the maxNormalAngle.</param>
		public void SetNormalAngle(float minNormalAngle, float maxNormalAngle)
		{
			this.minNormalAngle = minNormalAngle;
			this.maxNormalAngle = maxNormalAngle;
			this.useNormalAngle = true;
			this.CheckConsistency();
		}

		/// <summary>
		///   <para>Given the current state of the contact filter, determine whether it would filter anything.</para>
		/// </summary>
		public bool isFiltering
		{
			get
			{
				return !this.useTriggers || this.useLayerMask || this.useDepth || this.useNormalAngle;
			}
		}

		/// <summary>
		///   <para>Checks if the collider is a trigger and should be filtered by the useTriggers to be filtered.</para>
		/// </summary>
		/// <param name="collider">The Collider2D used to check for a trigger.</param>
		/// <returns>
		///   <para>Returns true when collider is excluded by the filter and false if otherwise.</para>
		/// </returns>
		public bool IsFilteringTrigger([Writable] Collider2D collider)
		{
			return !this.useTriggers && collider.isTrigger;
		}

		/// <summary>
		///   <para>Checks if the GameObject.layer for obj is included in the layerMask to be filtered.</para>
		/// </summary>
		/// <param name="obj">The GameObject used to check the GameObject.layer.</param>
		/// <returns>
		///   <para>Returns true when obj is excluded by the filter and false if otherwise.</para>
		/// </returns>
		public bool IsFilteringLayerMask(GameObject obj)
		{
			return this.useLayerMask && (this.layerMask & (1 << obj.layer)) == 0;
		}

		/// <summary>
		///   <para>Checks if the Transform for obj is within the depth range to be filtered.</para>
		/// </summary>
		/// <param name="obj">The GameObject used to check the z-position (depth) of Transform.position.</param>
		/// <returns>
		///   <para>Returns true when obj is excluded by the filter and false if otherwise.</para>
		/// </returns>
		public bool IsFilteringDepth(GameObject obj)
		{
			bool flag;
			if (!this.useDepth)
			{
				flag = false;
			}
			else
			{
				if (this.minDepth > this.maxDepth)
				{
					float num = this.minDepth;
					this.minDepth = this.maxDepth;
					this.maxDepth = num;
				}
				float z = obj.transform.position.z;
				bool flag2 = z < this.minDepth || z > this.maxDepth;
				if (this.useOutsideDepth)
				{
					flag = !flag2;
				}
				else
				{
					flag = flag2;
				}
			}
			return flag;
		}

		/// <summary>
		///   <para>Checks if the angle of normal is within the normal angle range to be filtered.</para>
		/// </summary>
		/// <param name="normal">The normal used to calculate an angle.</param>
		/// <returns>
		///   <para>Returns true when normal is excluded by the filter and false if otherwise.</para>
		/// </returns>
		public bool IsFilteringNormalAngle(Vector2 normal)
		{
			float num = Mathf.Atan2(normal.y, normal.x) * 57.29578f;
			return this.IsFilteringNormalAngle(num);
		}

		/// <summary>
		///   <para>Checks if the angle is within the normal angle range to be filtered.</para>
		/// </summary>
		/// <param name="angle">The angle used for comparison in the filter.</param>
		/// <returns>
		///   <para>Returns true when angle is excluded by the filter and false if otherwise.</para>
		/// </returns>
		public bool IsFilteringNormalAngle(float angle)
		{
			angle -= Mathf.Floor(angle / 359.9999f) * 359.9999f;
			float num = Mathf.Clamp(this.minNormalAngle, 0f, 359.9999f);
			float num2 = Mathf.Clamp(this.maxNormalAngle, 0f, 359.9999f);
			if (num > num2)
			{
				float num3 = num;
				num = num2;
				num2 = num3;
			}
			bool flag = angle < num || angle > num2;
			bool flag2;
			if (this.useOutsideNormalAngle)
			{
				flag2 = !flag;
			}
			else
			{
				flag2 = flag;
			}
			return flag2;
		}

		internal static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(layerMask);
			contactFilter2D.SetDepth(minDepth, maxDepth);
			return contactFilter2D;
		}

		/// <summary>
		///   <para>Sets to filter contact results based on trigger collider involvement.</para>
		/// </summary>
		[NativeName("m_UseTriggers")]
		public bool useTriggers;

		/// <summary>
		///   <para>Sets the contact filter to filter results by layer mask.</para>
		/// </summary>
		[NativeName("m_UseLayerMask")]
		public bool useLayerMask;

		/// <summary>
		///   <para>Sets the contact filter to filter the results by depth using minDepth and maxDepth.</para>
		/// </summary>
		[NativeName("m_UseDepth")]
		public bool useDepth;

		/// <summary>
		///   <para>Sets the contact filter to filter within the minDepth and maxDepth range, or outside that range.</para>
		/// </summary>
		[NativeName("m_UseOutsideDepth")]
		public bool useOutsideDepth;

		/// <summary>
		///   <para>Sets the contact filter to filter the results by the collision's normal angle using minNormalAngle and maxNormalAngle.</para>
		/// </summary>
		[NativeName("m_UseNormalAngle")]
		public bool useNormalAngle;

		/// <summary>
		///   <para>Sets the contact filter to filter within the minNormalAngle and maxNormalAngle range, or outside that range.</para>
		/// </summary>
		[NativeName("m_UseOutsideNormalAngle")]
		public bool useOutsideNormalAngle;

		/// <summary>
		///   <para>Sets the contact filter to filter the results that only include Collider2D on the layers defined by the layer mask.</para>
		/// </summary>
		[NativeName("m_LayerMask")]
		public LayerMask layerMask;

		/// <summary>
		///   <para>Sets the contact filter to filter the results to only include Collider2D with a Z coordinate (depth) greater than this value.</para>
		/// </summary>
		[NativeName("m_MinDepth")]
		public float minDepth;

		/// <summary>
		///   <para>Sets the contact filter to filter the results to only include Collider2D with a Z coordinate (depth) less than this value.</para>
		/// </summary>
		[NativeName("m_MaxDepth")]
		public float maxDepth;

		/// <summary>
		///   <para>Sets the contact filter to filter the results to only include contacts with collision normal angles that are greater than this angle.</para>
		/// </summary>
		[NativeName("m_MinNormalAngle")]
		public float minNormalAngle;

		/// <summary>
		///   <para>Sets the contact filter to filter the results to only include contacts with collision normal angles that are less than this angle.</para>
		/// </summary>
		[NativeName("m_MaxNormalAngle")]
		public float maxNormalAngle;

		public const float NormalAngleUpperLimit = 359.9999f;
	}
}
