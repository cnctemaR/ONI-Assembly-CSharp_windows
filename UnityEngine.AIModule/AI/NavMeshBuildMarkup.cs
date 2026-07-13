using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	[NativeHeader("Modules/AI/Public/NavMeshBindingTypes.h")]
	public struct NavMeshBuildMarkup
	{
		public bool overrideArea
		{
			get
			{
				return this.m_OverrideArea != 0;
			}
			set
			{
				this.m_OverrideArea = (value ? 1 : 0);
			}
		}

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

		public bool overrideIgnore
		{
			get
			{
				return this.m_InheritIgnoreFromBuild == 0;
			}
			set
			{
				this.m_InheritIgnoreFromBuild = (value ? 0 : 1);
			}
		}

		public bool ignoreFromBuild
		{
			get
			{
				return this.m_IgnoreFromBuild != 0;
			}
			set
			{
				this.m_IgnoreFromBuild = (value ? 1 : 0);
			}
		}

		public bool overrideGenerateLinks
		{
			get
			{
				return this.m_OverrideGenerateLinks != 0;
			}
			set
			{
				this.m_OverrideGenerateLinks = (value ? 1 : 0);
			}
		}

		public bool generateLinks
		{
			get
			{
				return this.m_GenerateLinks != 0;
			}
			set
			{
				this.m_GenerateLinks = (value ? 1 : 0);
			}
		}

		public bool applyToChildren
		{
			get
			{
				return this.m_IgnoreChildren == 0;
			}
			set
			{
				this.m_IgnoreChildren = (value ? 0 : 1);
			}
		}

		public Transform root
		{
			get
			{
				return NavMeshBuildMarkup.InternalGetRootGO(this.m_InstanceID);
			}
			set
			{
				this.m_InstanceID = ((value != null) ? value.GetInstanceID() : 0);
			}
		}

		[StaticAccessor("NavMeshBuildMarkup", StaticAccessorType.DoubleColon)]
		private static Transform InternalGetRootGO(EntityId instanceID)
		{
			return Unmarshal.UnmarshalUnityObject<Transform>(NavMeshBuildMarkup.InternalGetRootGO_Injected(ref instanceID));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetRootGO_Injected([In] ref EntityId instanceID);

		private int m_OverrideArea;

		private int m_Area;

		private int m_InheritIgnoreFromBuild;

		private int m_IgnoreFromBuild;

		private int m_OverrideGenerateLinks;

		private int m_GenerateLinks;

		private int m_InstanceID;

		private int m_IgnoreChildren;
	}
}
