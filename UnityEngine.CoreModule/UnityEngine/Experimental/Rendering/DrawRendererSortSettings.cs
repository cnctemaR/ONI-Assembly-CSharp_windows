using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Describes how to sort objects during rendering.</para>
	/// </summary>
	public struct DrawRendererSortSettings
	{
		/// <summary>
		///   <para>Should orthographic sorting be used?</para>
		/// </summary>
		public bool sortOrthographic
		{
			get
			{
				return this._sortOrthographic != 0;
			}
			set
			{
				this._sortOrthographic = ((!value) ? 0 : 1);
			}
		}

		/// <summary>
		///   <para>Camera view matrix, used to determine distances to objects.</para>
		/// </summary>
		public Matrix4x4 worldToCameraMatrix;

		/// <summary>
		///   <para>Camera position, used to determine distances to objects.</para>
		/// </summary>
		public Vector3 cameraPosition;

		/// <summary>
		///   <para>What kind of sorting to do while rendering.</para>
		/// </summary>
		public SortFlags flags;

		private int _sortOrthographic;

		private Matrix4x4 _previousVPMatrix;

		private Matrix4x4 _nonJitteredVPMatrix;
	}
}
