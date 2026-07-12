using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.TerrainAPI
{
	public static class TerrainCallbacks
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event TerrainCallbacks.HeightmapChangedCallback heightmapChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event TerrainCallbacks.TextureChangedCallback textureChanged;

		[RequiredByNativeCode]
		internal static void InvokeHeightmapChangedCallback(TerrainData terrainData, RectInt heightRegion, bool synched)
		{
			bool flag = TerrainCallbacks.heightmapChanged != null;
			if (flag)
			{
				foreach (Terrain terrain in terrainData.users)
				{
					TerrainCallbacks.heightmapChanged(terrain, heightRegion, synched);
				}
			}
		}

		[RequiredByNativeCode]
		internal static void InvokeTextureChangedCallback(TerrainData terrainData, string textureName, RectInt texelRegion, bool synched)
		{
			bool flag = TerrainCallbacks.textureChanged != null;
			if (flag)
			{
				foreach (Terrain terrain in terrainData.users)
				{
					TerrainCallbacks.textureChanged(terrain, textureName, texelRegion, synched);
				}
			}
		}

		public delegate void HeightmapChangedCallback(Terrain terrain, RectInt heightRegion, bool synched);

		public delegate void TextureChangedCallback(Terrain terrain, string textureName, RectInt texelRegion, bool synched);
	}
}
