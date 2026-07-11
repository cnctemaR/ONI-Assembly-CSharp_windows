using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>State of OffMeshLink.</para>
	/// </summary>
	[MovedFrom("UnityEngine")]
	public struct OffMeshLinkData
	{
		/// <summary>
		///   <para>Is link valid (Read Only).</para>
		/// </summary>
		public bool valid
		{
			get
			{
				return this.m_Valid != 0;
			}
		}

		/// <summary>
		///   <para>Is link active (Read Only).</para>
		/// </summary>
		public bool activated
		{
			get
			{
				return this.m_Activated != 0;
			}
		}

		/// <summary>
		///   <para>Link type specifier (Read Only).</para>
		/// </summary>
		public OffMeshLinkType linkType
		{
			get
			{
				return this.m_LinkType;
			}
		}

		/// <summary>
		///   <para>Link start world position (Read Only).</para>
		/// </summary>
		public Vector3 startPos
		{
			get
			{
				return this.m_StartPos;
			}
		}

		/// <summary>
		///   <para>Link end world position (Read Only).</para>
		/// </summary>
		public Vector3 endPos
		{
			get
			{
				return this.m_EndPos;
			}
		}

		/// <summary>
		///   <para>The OffMeshLink if the link type is a manually placed Offmeshlink (Read Only).</para>
		/// </summary>
		public OffMeshLink offMeshLink
		{
			get
			{
				return this.GetOffMeshLinkInternal(this.m_InstanceID);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern OffMeshLink GetOffMeshLinkInternal(int instanceID);

		private int m_Valid;

		private int m_Activated;

		private int m_InstanceID;

		private OffMeshLinkType m_LinkType;

		private Vector3 m_StartPos;

		private Vector3 m_EndPos;
	}
}
