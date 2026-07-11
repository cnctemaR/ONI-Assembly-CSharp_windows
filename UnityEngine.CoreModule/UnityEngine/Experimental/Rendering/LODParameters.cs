using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>LODGroup culling parameters.</para>
	/// </summary>
	public struct LODParameters
	{
		/// <summary>
		///   <para>Indicates whether camera is orthographic.</para>
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
		///   <para>Camera position.</para>
		/// </summary>
		public Vector3 cameraPosition
		{
			get
			{
				return this.m_CameraPosition;
			}
			set
			{
				this.m_CameraPosition = value;
			}
		}

		/// <summary>
		///   <para>Camera's field of view.</para>
		/// </summary>
		public float fieldOfView
		{
			get
			{
				return this.m_FieldOfView;
			}
			set
			{
				this.m_FieldOfView = value;
			}
		}

		/// <summary>
		///   <para>Orhographic camera size.</para>
		/// </summary>
		public float orthoSize
		{
			get
			{
				return this.m_OrthoSize;
			}
			set
			{
				this.m_OrthoSize = value;
			}
		}

		/// <summary>
		///   <para>Rendering view height in pixels.</para>
		/// </summary>
		public int cameraPixelHeight
		{
			get
			{
				return this.m_CameraPixelHeight;
			}
			set
			{
				this.m_CameraPixelHeight = value;
			}
		}

		private int m_IsOrthographic;

		private Vector3 m_CameraPosition;

		private float m_FieldOfView;

		private float m_OrthoSize;

		private int m_CameraPixelHeight;
	}
}
