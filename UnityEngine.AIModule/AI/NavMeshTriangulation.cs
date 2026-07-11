using System;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	[UsedByNativeCode]
	public struct NavMeshTriangulation
	{
		[Obsolete("Use areas instead.")]
		public int[] layers
		{
			get
			{
				return this.areas;
			}
		}

		public Vector3[] vertices;

		public int[] indices;

		public int[] areas;
	}
}
