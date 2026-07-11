using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Parameters controlling culling process in CullResults.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct ScriptableCullingParameters
	{
		/// <summary>
		///   <para>Number of culling planes to use.</para>
		/// </summary>
		public int cullingPlaneCount
		{
			get
			{
				return this.m_CullingPlaneCount;
			}
			set
			{
				if (value < 0 || value > 10)
				{
					throw new IndexOutOfRangeException("Invalid plane count (0 <= count <= 10)");
				}
				this.m_CullingPlaneCount = value;
			}
		}

		/// <summary>
		///   <para>Is the cull orthographic.</para>
		/// </summary>
		public bool isOrthographic
		{
			get
			{
				return Convert.ToBoolean(this.m_IsOrthographic);
			}
			set
			{
				this.m_IsOrthographic = Convert.ToInt32(value);
			}
		}

		/// <summary>
		///   <para>LODParameters for culling.</para>
		/// </summary>
		public LODParameters lodParameters
		{
			get
			{
				return this.m_LodParameters;
			}
			set
			{
				this.m_LodParameters = value;
			}
		}

		/// <summary>
		///   <para>CullingMask used for culling.</para>
		/// </summary>
		public int cullingMask
		{
			get
			{
				return this.m_CullingMask;
			}
			set
			{
				this.m_CullingMask = value;
			}
		}

		/// <summary>
		///   <para>Scene Mask to use for the cull.</para>
		/// </summary>
		public long sceneMask
		{
			get
			{
				return this.m_SceneMask;
			}
			set
			{
				this.m_SceneMask = value;
			}
		}

		/// <summary>
		///   <para>Layers to cull.</para>
		/// </summary>
		public int layerCull
		{
			get
			{
				return this.m_LayerCull;
			}
			set
			{
				this.m_LayerCull = value;
			}
		}

		/// <summary>
		///   <para>CullingMatrix used for culling.</para>
		/// </summary>
		public Matrix4x4 cullingMatrix
		{
			get
			{
				return this.m_CullingMatrix;
			}
			set
			{
				this.m_CullingMatrix = value;
			}
		}

		/// <summary>
		///   <para>Position for the origin of th cull.</para>
		/// </summary>
		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		/// <summary>
		///   <para>Shadow distance to use for the cull.</para>
		/// </summary>
		public float shadowDistance
		{
			get
			{
				return this.m_shadowDistance;
			}
			set
			{
				this.m_shadowDistance = value;
			}
		}

		/// <summary>
		///   <para>Culling Flags for the culling.</para>
		/// </summary>
		public CullFlag cullingFlags
		{
			get
			{
				return this.m_CullingFlags;
			}
			set
			{
				this.m_CullingFlags = value;
			}
		}

		/// <summary>
		///   <para>Reflection Probe Sort options for the cull.</para>
		/// </summary>
		public ReflectionProbeSortOptions reflectionProbeSortOptions
		{
			get
			{
				return this.m_ReflectionProbeSortOptions;
			}
			set
			{
				this.m_ReflectionProbeSortOptions = value;
			}
		}

		/// <summary>
		///   <para>Camera Properties used for culling.</para>
		/// </summary>
		public CameraProperties cameraProperties
		{
			get
			{
				return this.m_CameraProperties;
			}
			set
			{
				this.m_CameraProperties = value;
			}
		}

		/// <summary>
		///   <para>Get the distance for the culling of a specific layer.</para>
		/// </summary>
		/// <param name="layerIndex"></param>
		public unsafe float GetLayerCullDistance(int layerIndex)
		{
			if (layerIndex < 0 || layerIndex >= 32)
			{
				throw new IndexOutOfRangeException("Invalid layer index");
			}
			fixed (float* ptr = &this.m_LayerFarCullDistances.FixedElementField)
			{
				return ptr[(IntPtr)layerIndex * 4];
			}
		}

		/// <summary>
		///   <para>Set the distance for the culling of a specific layer.</para>
		/// </summary>
		/// <param name="layerIndex"></param>
		/// <param name="distance"></param>
		public unsafe void SetLayerCullDistance(int layerIndex, float distance)
		{
			if (layerIndex < 0 || layerIndex >= 32)
			{
				throw new IndexOutOfRangeException("Invalid layer index");
			}
			fixed (float* ptr = &this.m_LayerFarCullDistances.FixedElementField)
			{
				ptr[(IntPtr)layerIndex * 4] = distance;
			}
		}

		/// <summary>
		///   <para>Fetch the culling plane at the given index.</para>
		/// </summary>
		/// <param name="index"></param>
		public unsafe Plane GetCullingPlane(int index)
		{
			if (index < 0 || index >= this.cullingPlaneCount || index >= 10)
			{
				throw new IndexOutOfRangeException("Invalid plane index");
			}
			fixed (float* ptr = &this.m_CullingPlanes.FixedElementField)
			{
				return new Plane(new Vector3(ptr[(IntPtr)(index * 4) * 4], ptr[(IntPtr)(index * 4 + 1) * 4], ptr[(IntPtr)(index * 4 + 2) * 4]), ptr[(IntPtr)(index * 4 + 3) * 4]);
			}
		}

		/// <summary>
		///   <para>Set the culling plane at a given index.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="plane"></param>
		public unsafe void SetCullingPlane(int index, Plane plane)
		{
			if (index < 0 || index >= this.cullingPlaneCount || index >= 10)
			{
				throw new IndexOutOfRangeException("Invalid plane index");
			}
			fixed (float* ptr = &this.m_CullingPlanes.FixedElementField)
			{
				ptr[(IntPtr)(index * 4) * 4] = plane.normal.x;
				ptr[(IntPtr)(index * 4 + 1) * 4] = plane.normal.y;
				ptr[(IntPtr)(index * 4 + 2) * 4] = plane.normal.z;
				ptr[(IntPtr)(index * 4 + 3) * 4] = plane.distance;
			}
		}

		private int m_IsOrthographic;

		private LODParameters m_LodParameters;

		private ScriptableCullingParameters.<m_CullingPlanes>__FixedBuffer5 m_CullingPlanes;

		private int m_CullingPlaneCount;

		private int m_CullingMask;

		private long m_SceneMask;

		private ScriptableCullingParameters.<m_LayerFarCullDistances>__FixedBuffer6 m_LayerFarCullDistances;

		private int m_LayerCull;

		private Matrix4x4 m_CullingMatrix;

		private Vector3 m_Position;

		private float m_shadowDistance;

		private CullFlag m_CullingFlags;

		private ReflectionProbeSortOptions m_ReflectionProbeSortOptions;

		private CameraProperties m_CameraProperties;

		/// <summary>
		///   <para>The view matrix generated for single-pass stereo culling.</para>
		/// </summary>
		public Matrix4x4 cullStereoView;

		/// <summary>
		///   <para>The projection matrix generated for single-pass stereo culling.</para>
		/// </summary>
		public Matrix4x4 cullStereoProj;

		/// <summary>
		///   <para>Distance between the virtual eyes.</para>
		/// </summary>
		public float cullStereoSeparation;

		private int padding2;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 160)]
		public struct <m_CullingPlanes>__FixedBuffer5
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <m_LayerFarCullDistances>__FixedBuffer6
		{
			public float FixedElementField;
		}
	}
}
