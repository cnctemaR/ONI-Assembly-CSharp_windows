using System;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace TMPro.Examples
{
	public class Benchmark03 : MonoBehaviour
	{
		private void Awake()
		{
		}

		private void Start()
		{
			TMP_FontAsset tmp_FontAsset = null;
			switch (this.Benchmark)
			{
			case Benchmark03.BenchmarkType.TMP_SDF_MOBILE:
				tmp_FontAsset = TMP_FontAsset.CreateFontAsset(this.SourceFont, 90, 9, GlyphRenderMode.SDFAA, 256, 256, AtlasPopulationMode.Dynamic, true);
				break;
			case Benchmark03.BenchmarkType.TMP_SDF__MOBILE_SSD:
				tmp_FontAsset = TMP_FontAsset.CreateFontAsset(this.SourceFont, 90, 9, GlyphRenderMode.SDFAA, 256, 256, AtlasPopulationMode.Dynamic, true);
				tmp_FontAsset.material.shader = Shader.Find("TextMeshPro/Mobile/Distance Field SSD");
				break;
			case Benchmark03.BenchmarkType.TMP_SDF:
				tmp_FontAsset = TMP_FontAsset.CreateFontAsset(this.SourceFont, 90, 9, GlyphRenderMode.SDFAA, 256, 256, AtlasPopulationMode.Dynamic, true);
				tmp_FontAsset.material.shader = Shader.Find("TextMeshPro/Distance Field");
				break;
			case Benchmark03.BenchmarkType.TMP_BITMAP_MOBILE:
				tmp_FontAsset = TMP_FontAsset.CreateFontAsset(this.SourceFont, 90, 9, GlyphRenderMode.SMOOTH, 256, 256, AtlasPopulationMode.Dynamic, true);
				break;
			}
			for (int i = 0; i < this.NumberOfSamples; i++)
			{
				Benchmark03.BenchmarkType benchmark = this.Benchmark;
				if (benchmark > Benchmark03.BenchmarkType.TMP_BITMAP_MOBILE)
				{
					if (benchmark == Benchmark03.BenchmarkType.TEXTMESH_BITMAP)
					{
						TextMesh textMesh = new GameObject
						{
							transform = 
							{
								position = new Vector3(0f, 1.2f, 0f)
							}
						}.AddComponent<TextMesh>();
						textMesh.GetComponent<Renderer>().sharedMaterial = this.SourceFont.material;
						textMesh.font = this.SourceFont;
						textMesh.anchor = TextAnchor.MiddleCenter;
						textMesh.fontSize = 130;
						textMesh.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
						textMesh.text = "@";
					}
				}
				else
				{
					TextMeshPro textMeshPro = new GameObject
					{
						transform = 
						{
							position = new Vector3(0f, 1.2f, 0f)
						}
					}.AddComponent<TextMeshPro>();
					textMeshPro.font = tmp_FontAsset;
					textMeshPro.fontSize = 128f;
					textMeshPro.text = "@";
					textMeshPro.alignment = TextAlignmentOptions.Center;
					textMeshPro.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
					if (this.Benchmark == Benchmark03.BenchmarkType.TMP_BITMAP_MOBILE)
					{
						textMeshPro.fontSize = 132f;
					}
				}
			}
		}

		public int NumberOfSamples = 100;

		public Benchmark03.BenchmarkType Benchmark;

		public Font SourceFont;

		public enum BenchmarkType
		{
			TMP_SDF_MOBILE,
			TMP_SDF__MOBILE_SSD,
			TMP_SDF,
			TMP_BITMAP_MOBILE,
			TEXTMESH_BITMAP
		}
	}
}
