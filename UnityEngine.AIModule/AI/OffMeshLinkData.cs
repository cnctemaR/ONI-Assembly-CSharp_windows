using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	[NativeHeader("Modules/AI/Components/OffMeshLink.bindings.h")]
	public struct OffMeshLinkData
	{
		public bool valid
		{
			get
			{
				return this.m_Valid != 0;
			}
		}

		public bool activated
		{
			get
			{
				return this.m_Activated != 0;
			}
		}

		public OffMeshLinkType linkType
		{
			get
			{
				return this.m_LinkType;
			}
		}

		public Vector3 startPos
		{
			get
			{
				return this.m_StartPos;
			}
		}

		public Vector3 endPos
		{
			get
			{
				return this.m_EndPos;
			}
		}

		public Object owner
		{
			get
			{
				return OffMeshLinkData.GetLinkOwnerInternal(this.m_InstanceID);
			}
		}

		[FreeFunction("OffMeshLinkScriptBindings::GetLinkOwnerInternal")]
		private static Object GetLinkOwnerInternal(EntityId instanceID)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(OffMeshLinkData.GetLinkOwnerInternal_Injected(ref instanceID));
		}

		[Obsolete("offMeshLink has been deprecated. Use 'owner' instead.")]
		public OffMeshLink offMeshLink
		{
			get
			{
				return OffMeshLinkData.GetOffMeshLinkInternal(this.m_InstanceID);
			}
		}

		[FreeFunction("OffMeshLinkScriptBindings::GetOffMeshLinkInternal")]
		private static OffMeshLink GetOffMeshLinkInternal(EntityId instanceID)
		{
			return Unmarshal.UnmarshalUnityObject<OffMeshLink>(OffMeshLinkData.GetOffMeshLinkInternal_Injected(ref instanceID));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetLinkOwnerInternal_Injected([In] ref EntityId instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetOffMeshLinkInternal_Injected([In] ref EntityId instanceID);

		internal int m_Valid;

		internal int m_Activated;

		internal EntityId m_InstanceID;

		internal OffMeshLinkType m_LinkType;

		internal Vector3 m_StartPos;

		internal Vector3 m_EndPos;
	}
}
