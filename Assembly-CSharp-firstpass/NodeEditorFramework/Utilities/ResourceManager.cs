using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class ResourceManager
	{
		public static void SetDefaultResourcePath(string defaultResourcePath)
		{
			ResourceManager._ResourcePath = defaultResourcePath;
		}

		public static string PreparePath(string path)
		{
			path = path.Replace(Application.dataPath, "Assets");
			if (path.Contains("Resources"))
			{
				path = path.Substring(path.LastIndexOf("Resources") + 10);
			}
			return path.Substring(0, path.LastIndexOf('.'));
		}

		public static T[] LoadResources<T>(string path) where T : global::UnityEngine.Object
		{
			path = ResourceManager.PreparePath(path);
			throw new NotImplementedException("Currently it is not possible to load subAssets at runtime!");
		}

		public static T LoadResource<T>(string path) where T : global::UnityEngine.Object
		{
			path = ResourceManager.PreparePath(path);
			return Resources.Load<T>(path);
		}

		public static Texture2D LoadTexture(string texPath)
		{
			if (string.IsNullOrEmpty(texPath))
			{
				return null;
			}
			int num = ResourceManager.loadedTextures.FindIndex((ResourceManager.MemoryTexture memTex) => memTex.path == texPath);
			if (num != -1)
			{
				if (!(ResourceManager.loadedTextures[num].texture == null))
				{
					return ResourceManager.loadedTextures[num].texture;
				}
				ResourceManager.loadedTextures.RemoveAt(num);
			}
			Texture2D texture2D = ResourceManager.LoadResource<Texture2D>(texPath);
			ResourceManager.AddTextureToMemory(texPath, texture2D, new string[0]);
			return texture2D;
		}

		public static Texture2D GetTintedTexture(string texPath, Color col)
		{
			string text = "Tint:" + col.ToString();
			Texture2D texture2D = ResourceManager.GetTexture(texPath, new string[] { text });
			if (texture2D == null)
			{
				texture2D = ResourceManager.LoadTexture(texPath);
				ResourceManager.AddTextureToMemory(texPath, texture2D, new string[0]);
				texture2D = RTEditorGUI.Tint(texture2D, col);
				ResourceManager.AddTextureToMemory(texPath, texture2D, new string[] { text });
			}
			return texture2D;
		}

		public static void AddTextureToMemory(string texturePath, Texture2D texture, params string[] modifications)
		{
			if (texture == null)
			{
				return;
			}
			ResourceManager.loadedTextures.Add(new ResourceManager.MemoryTexture(texturePath, texture, modifications));
		}

		public static ResourceManager.MemoryTexture FindInMemory(Texture2D tex)
		{
			int num = ResourceManager.loadedTextures.FindIndex((ResourceManager.MemoryTexture memTex) => memTex.texture == tex);
			return (num == -1) ? null : ResourceManager.loadedTextures[num];
		}

		public static bool HasInMemory(string texturePath, params string[] modifications)
		{
			int num = ResourceManager.loadedTextures.FindIndex((ResourceManager.MemoryTexture memTex) => memTex.path == texturePath);
			return num != -1 && ResourceManager.EqualModifications(ResourceManager.loadedTextures[num].modifications, modifications);
		}

		public static ResourceManager.MemoryTexture GetMemoryTexture(string texturePath, params string[] modifications)
		{
			List<ResourceManager.MemoryTexture> list = ResourceManager.loadedTextures.FindAll((ResourceManager.MemoryTexture memTex) => memTex.path == texturePath);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			foreach (ResourceManager.MemoryTexture memoryTexture in list)
			{
				if (ResourceManager.EqualModifications(memoryTexture.modifications, modifications))
				{
					return memoryTexture;
				}
			}
			return null;
		}

		public static Texture2D GetTexture(string texturePath, params string[] modifications)
		{
			ResourceManager.MemoryTexture memoryTexture = ResourceManager.GetMemoryTexture(texturePath, modifications);
			return (memoryTexture != null) ? memoryTexture.texture : null;
		}

		private static bool EqualModifications(string[] modsA, string[] modsB)
		{
			return modsA.Length == modsB.Length && Array.TrueForAll<string>(modsA, (string mod) => modsB.Count<string>((string oMod) => mod == oMod) == modsA.Count<string>((string oMod) => mod == oMod));
		}

		private static string _ResourcePath = string.Empty;

		private static List<ResourceManager.MemoryTexture> loadedTextures = new List<ResourceManager.MemoryTexture>();

		public class MemoryTexture
		{
			public MemoryTexture(string texPath, Texture2D tex, params string[] mods)
			{
				this.path = texPath;
				this.texture = tex;
				this.modifications = mods;
			}

			public string path;

			public Texture2D texture;

			public string[] modifications;
		}
	}
}
