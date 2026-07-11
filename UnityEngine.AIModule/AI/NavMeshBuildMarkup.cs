using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>The NavMesh build markup allows you to control how certain objects are treated during the NavMesh build process, specifically when collecting sources for building.</para>
	/// </summary>
	[NativeHeader("Modules/AI/Public/NavMeshBindingTypes.h")]
	public struct NavMeshBuildMarkup
	{
		/// <summary>
		///   <para>Use this to specify whether the area type of the GameObject and its children should be overridden by the area type specified in this struct.</para>
		/// </summary>
		public bool overrideArea
		{
			get
			{
				return this.m_OverrideArea != 0;
			}
			set
			{
				this.m_OverrideArea = ((!value) ? 0 : 1);
			}
		}

		/// <summary>
		///   <para>The area type to use when override area is enabled.</para>
		/// </summary>
		public int area
		{
			get
			{
				return this.m_Area;
			}
			set
			{
				this.m_Area = value;
			}
		}

		/// <summary>
		///   <para>Use this to specify whether the GameObject and its children should be ignored.</para>
		/// </summary>
		public bool ignoreFromBuild
		{
			get
			{
				return this.m_IgnoreFromBuild != 0;
			}
			set
			{
				this.m_IgnoreFromBuild = ((!value) ? 0 : 1);
			}
		}

		/// <summary>
		///   <para>Use this to specify which GameObject (including the GameObject’s children) the markup should be applied to.</para>
		/// </summary>
		public Transform root
		{
			get
			{
				return NavMeshBuildMarkup.InternalGetRootGO(this.m_InstanceID);
			}
			set
			{
				this.m_InstanceID = ((!(value != null)) ? 0 : value.GetInstanceID());
			}
		}

		[StaticAccessor("NavMeshBuildMarkup", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Transform InternalGetRootGO(int instanceID);

		private int m_OverrideArea;

		private int m_Area;

		private int m_IgnoreFromBuild;

		private int m_InstanceID;
	}
}
