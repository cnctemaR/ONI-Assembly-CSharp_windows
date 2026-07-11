using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>The input to the NavMesh builder is a list of NavMesh build sources.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Modules/AI/Public/NavMeshBindingTypes.h")]
	public struct NavMeshBuildSource
	{
		/// <summary>
		///   <para>Describes the local to world transformation matrix of the build source. That is, position and orientation and scale of the shape.</para>
		/// </summary>
		public Matrix4x4 transform
		{
			get
			{
				return this.m_Transform;
			}
			set
			{
				this.m_Transform = value;
			}
		}

		/// <summary>
		///   <para>Describes the dimensions of the shape.</para>
		/// </summary>
		public Vector3 size
		{
			get
			{
				return this.m_Size;
			}
			set
			{
				this.m_Size = value;
			}
		}

		/// <summary>
		///   <para>The type of the shape this source describes. See Also: NavMeshBuildSourceShape.</para>
		/// </summary>
		public NavMeshBuildSourceShape shape
		{
			get
			{
				return this.m_Shape;
			}
			set
			{
				this.m_Shape = value;
			}
		}

		/// <summary>
		///   <para>Describes the area type of the NavMesh surface for this object.</para>
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
		///   <para>Describes the object referenced for Mesh and Terrain types of input sources.</para>
		/// </summary>
		public Object sourceObject
		{
			get
			{
				return NavMeshBuildSource.InternalGetObject(this.m_InstanceID);
			}
			set
			{
				this.m_InstanceID = ((!(value != null)) ? 0 : value.GetInstanceID());
			}
		}

		/// <summary>
		///   <para>Points to the owning component - if available, otherwise null.</para>
		/// </summary>
		public Component component
		{
			get
			{
				return NavMeshBuildSource.InternalGetComponent(this.m_ComponentID);
			}
			set
			{
				this.m_ComponentID = ((!(value != null)) ? 0 : value.GetInstanceID());
			}
		}

		[StaticAccessor("NavMeshBuildSource", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Component InternalGetComponent(int instanceID);

		[StaticAccessor("NavMeshBuildSource", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object InternalGetObject(int instanceID);

		private Matrix4x4 m_Transform;

		private Vector3 m_Size;

		private NavMeshBuildSourceShape m_Shape;

		private int m_Area;

		private int m_InstanceID;

		private int m_ComponentID;
	}
}
