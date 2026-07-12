using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.UIR;

namespace UnityEditor.UIElements
{
	internal static class PackageEditorAtlasMonitor
	{
		public static void StaticInit()
		{
			RenderChain.OnPreRender = (Action)Delegate.Combine(RenderChain.OnPreRender, new Action(PackageEditorAtlasMonitor.OnPreRender));
		}

		public static void OnPreRender()
		{
			bool flag = PackageEditorAtlasMonitor.CheckForColorSpaceChange();
			bool flag2 = PackageEditorAtlasMonitor.CheckForImportedTextures();
			bool flag3 = PackageEditorAtlasMonitor.CheckForImportedVectorImages();
			bool flag4 = PackageEditorAtlasMonitor.CheckForRenderTexturesTrashed();
			bool flag5 = flag || flag2 || flag4;
			if (flag5)
			{
				UIRAtlasManager.MarkAllForReset();
				VectorImageManager.MarkAllForReset();
			}
			else
			{
				bool flag6 = flag || flag3;
				if (flag6)
				{
					VectorImageManager.MarkAllForReset();
				}
			}
		}

		private static bool CheckForColorSpaceChange()
		{
			ColorSpace activeColorSpace = QualitySettings.activeColorSpace;
			bool flag = PackageEditorAtlasMonitor.m_LastColorSpace == activeColorSpace;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				PackageEditorAtlasMonitor.m_LastColorSpace = activeColorSpace;
				flag2 = true;
			}
			return flag2;
		}

		private static bool CheckForImportedTextures()
		{
			int importedTexturesCount = PackageEditorAtlasMonitor.TexturePostProcessor.importedTexturesCount;
			bool flag = PackageEditorAtlasMonitor.m_LastImportedTexturesCount == importedTexturesCount;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				PackageEditorAtlasMonitor.m_LastImportedTexturesCount = importedTexturesCount;
				flag2 = true;
			}
			return flag2;
		}

		private static bool CheckForImportedVectorImages()
		{
			int importedVectorImagesCount = PackageEditorAtlasMonitor.TexturePostProcessor.importedVectorImagesCount;
			bool flag = PackageEditorAtlasMonitor.m_LastImportedVectorImagesCount == importedVectorImagesCount;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				PackageEditorAtlasMonitor.m_LastImportedVectorImagesCount = importedVectorImagesCount;
				flag2 = true;
			}
			return flag2;
		}

		private static bool CheckForRenderTexturesTrashed()
		{
			bool flag = PackageEditorAtlasMonitor.m_RenderTexture == null;
			bool flag2;
			if (flag)
			{
				PackageEditorAtlasMonitor.m_RenderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
				PackageEditorAtlasMonitor.m_RenderTexture.Create();
				flag2 = true;
			}
			else
			{
				bool flag3 = !PackageEditorAtlasMonitor.m_RenderTexture.IsCreated();
				if (flag3)
				{
					PackageEditorAtlasMonitor.m_RenderTexture.Create();
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		private static PackageEditorAtlasMonitor.TexturePostProcessor s_TexturePostProcessor = new PackageEditorAtlasMonitor.TexturePostProcessor();

		private static ColorSpace m_LastColorSpace;

		private static int m_LastImportedTexturesCount;

		private static int m_LastImportedVectorImagesCount;

		private static RenderTexture m_RenderTexture;

		private class TexturePostProcessor
		{
			public TexturePostProcessor()
			{
				EditorAtlasMonitorBridge.OnPostprocessTexture = new Action<Texture2D>(this.OnPostprocessTexture);
				EditorAtlasMonitorBridge.OnPostprocessAllAssets = new Action<string[], string[], string[], string[]>(PackageEditorAtlasMonitor.TexturePostProcessor.OnPostprocessAllAssets);
			}

			public void OnPostprocessTexture(Texture2D texture)
			{
				PackageEditorAtlasMonitor.TexturePostProcessor.importedTexturesCount++;
			}

			private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
				foreach (string text in importedAssets)
				{
					bool flag = Path.GetExtension(text) == ".svg";
					if (flag)
					{
						PackageEditorAtlasMonitor.TexturePostProcessor.importedVectorImagesCount++;
					}
				}
			}

			public static int importedTexturesCount;

			public static int importedVectorImagesCount;
		}
	}
}
