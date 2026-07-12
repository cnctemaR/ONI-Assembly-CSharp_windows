using System;

namespace UnityEngine.Rendering
{
	public class ObjectIdRequest
	{
		public RenderTexture destination { get; set; }

		public int mipLevel { get; set; }

		public CubemapFace face { get; set; }

		public int slice { get; set; }

		public ObjectIdResult result { get; internal set; }

		public ObjectIdRequest(RenderTexture destination, int mipLevel = 0, CubemapFace face = CubemapFace.Unknown, int slice = 0)
		{
			this.destination = destination;
			this.mipLevel = mipLevel;
			this.face = face;
			this.slice = slice;
		}
	}
}
