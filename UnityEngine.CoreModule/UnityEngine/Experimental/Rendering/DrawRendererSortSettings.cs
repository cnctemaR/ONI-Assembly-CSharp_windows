using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct DrawRendererSortSettings
	{
		[Obsolete("Use sortMode instead")]
		public bool sortOrthographic
		{
			get
			{
				return this.sortMode == DrawRendererSortMode.Orthographic;
			}
			set
			{
				this.sortMode = ((!value) ? DrawRendererSortMode.Perspective : DrawRendererSortMode.Orthographic);
			}
		}

		public Matrix4x4 worldToCameraMatrix;

		public Vector3 cameraPosition;

		public Vector3 cameraCustomSortAxis;

		public SortFlags flags;

		public DrawRendererSortMode sortMode;

		private Matrix4x4 _previousVPMatrix;

		private Matrix4x4 _nonJitteredVPMatrix;
	}
}
