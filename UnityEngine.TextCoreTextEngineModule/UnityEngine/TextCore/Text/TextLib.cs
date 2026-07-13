using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "Unity.UIElements.PlayModeTests" })]
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextLib.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal class TextLib
	{
		public TextLib(byte[] icuData)
		{
			this.m_Ptr = TextLib.GetInstance(icuData);
		}

		private unsafe static IntPtr GetInstance(byte[] icuData)
		{
			Span<byte> span = new Span<byte>(icuData);
			IntPtr instance_Injected;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				instance_Injected = TextLib.GetInstance_Injected(ref managedSpanWrapper);
			}
			return instance_Injected;
		}

		public NativeTextInfo GenerateText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo, ref bool wasCached)
		{
			Debug.Assert((settings.fontStyle & FontStyles.Bold) == FontStyles.Normal);
			return this.GenerateTextInternal(settings, textGenerationInfo, ref wasCached);
		}

		public bool HasMissingGlyphs(NativeTextInfo textInfo, ref Dictionary<int, HashSet<uint>> missingGlyphsPerFontAsset)
		{
			Span<ATGMeshInfo> meshInfos = textInfo.meshInfos;
			bool flag = false;
			Span<ATGMeshInfo> span = meshInfos;
			for (int i = 0; i < span.Length; i++)
			{
				ref ATGMeshInfo ptr = ref span[i];
				TextAsset textAssetByID = TextAsset.GetTextAssetByID(ptr.textAssetId);
				HashSet<uint> hashSet = null;
				bool flag2 = textAssetByID is SpriteAsset || textAssetByID == null;
				if (!flag2)
				{
					Span<NativeTextElementInfo> textElementInfos = ptr.textElementInfos;
					for (int j = 0; j < textElementInfos.Length; j++)
					{
						ref NativeTextElementInfo ptr2 = ref textElementInfos[j];
						int glyphID = ptr2.glyphID;
						Glyph glyphInCache = ((FontAsset)textAssetByID).GetGlyphInCache((uint)glyphID);
						bool flag3 = glyphInCache == null;
						if (flag3)
						{
							flag = true;
							bool flag4 = hashSet == null;
							if (flag4)
							{
								bool flag5 = !missingGlyphsPerFontAsset.TryGetValue(ptr.textAssetId, out hashSet);
								if (flag5)
								{
									hashSet = new HashSet<uint>();
									missingGlyphsPerFontAsset.Add(ptr.textAssetId, hashSet);
								}
							}
							hashSet.Add((uint)glyphID);
						}
					}
				}
			}
			return flag;
		}

		public void ProcessMeshInfos(NativeTextInfo textInfo, NativeTextGenerationSettings settings, ref List<List<List<int>>> textElementIndicesByMesh, ref List<bool> hasMultipleColorsByMesh, bool uvsAreGenerated)
		{
			Span<ATGMeshInfo> meshInfos = textInfo.meshInfos;
			int num = 0;
			Span<ATGMeshInfo> span = meshInfos;
			for (int i = 0; i < span.Length; i++)
			{
				ref ATGMeshInfo ptr = ref span[i];
				TextAsset textAssetByID = TextAsset.GetTextAssetByID(ptr.textAssetId);
				bool flag = textAssetByID == null;
				if (!flag)
				{
					float num2 = 0f;
					float num3 = 0f;
					bool flag2 = false;
					int num4 = 1;
					bool flag3 = num < textElementIndicesByMesh.Count;
					List<List<int>> list;
					if (flag3)
					{
						list = textElementIndicesByMesh[num];
					}
					else
					{
						list = new List<List<int>>();
						textElementIndicesByMesh.Add(list);
					}
					FontAsset fontAsset = textAssetByID as FontAsset;
					bool flag4 = fontAsset != null;
					if (flag4)
					{
						flag2 = false;
						num4 = fontAsset.atlasTextures.Length;
						num2 = 1f / (float)fontAsset.atlasWidth;
						num3 = 1f / (float)fontAsset.atlasHeight;
					}
					else
					{
						SpriteAsset spriteAsset = textAssetByID as SpriteAsset;
						bool flag5 = spriteAsset != null;
						if (flag5)
						{
							flag2 = true;
							num4 = 1;
							num2 = 1f / (float)spriteAsset.m_SpriteAtlasTexture.width;
							num3 = 1f / (float)spriteAsset.m_SpriteAtlasTexture.height;
						}
					}
					while (list.Count < num4)
					{
						list.Add(new List<int>());
					}
					float num5 = (float)settings.vertexPadding / 64f;
					bool flag6 = false;
					Color? color = null;
					Span<NativeTextElementInfo> textElementInfos = ptr.textElementInfos;
					int j = 0;
					while (j < textElementInfos.Length)
					{
						ref NativeTextElementInfo ptr2 = ref textElementInfos[j];
						int glyphID = ptr2.glyphID;
						bool flag7 = flag2;
						Glyph glyph;
						if (flag7)
						{
							int num6 = glyphID - 57344;
							num5 = 0f;
							glyph = ((SpriteAsset)textAssetByID).spriteCharacterTable[num6].glyph;
							bool flag8 = num6 == -1;
							if (!flag8)
							{
								goto IL_01E3;
							}
						}
						else
						{
							glyph = ((FontAsset)textAssetByID).GetGlyphInCache((uint)glyphID);
							bool flag9 = glyph == null;
							if (!flag9)
							{
								goto IL_01E3;
							}
						}
						IL_061A:
						j++;
						continue;
						IL_01E3:
						Color32 color2 = ptr2.topLeft.color;
						bool flag10 = color != null && color.Value != color2;
						if (flag10)
						{
							flag6 = true;
						}
						color = new Color?(color2);
						GlyphRect glyphRect = glyph.glyphRect;
						textElementIndicesByMesh[num][glyph.atlasIndex].Add(j);
						if (uvsAreGenerated)
						{
							goto IL_061A;
						}
						bool flag11 = (ptr2.bottomLeft.uv0.x == 0f || ptr2.bottomLeft.uv0.x == 1f) && (ptr2.bottomLeft.uv0.y == 0f || ptr2.bottomLeft.uv0.y == 1f) && (ptr2.topLeft.uv0.x == 0f || ptr2.topLeft.uv0.x == 1f) && (ptr2.topLeft.uv0.y == 0f || ptr2.topLeft.uv0.y == 1f) && (ptr2.topRight.uv0.x == 0f || ptr2.topRight.uv0.x == 1f) && (ptr2.topRight.uv0.y == 0f || ptr2.topRight.uv0.y == 1f) && (ptr2.bottomRight.uv0.x == 0f || ptr2.bottomRight.uv0.x == 1f) && (ptr2.bottomRight.uv0.y == 0f || ptr2.bottomRight.uv0.y == 1f);
						bool flag12 = flag11;
						if (flag12)
						{
							float num7 = ((float)glyphRect.x - num5) * num2;
							float num8 = ((float)glyphRect.y - num5) * num3;
							float num9 = ((float)(glyphRect.x + glyphRect.width) + num5) * num2;
							float num10 = ((float)(glyphRect.y + glyphRect.height) + num5) * num3;
							ptr2.bottomLeft.uv0 = new Vector2(num7, num8);
							ptr2.topLeft.uv0 = new Vector2(num7, num10);
							ptr2.topRight.uv0 = new Vector2(num9, num10);
							ptr2.bottomRight.uv0 = new Vector2(num9, num8);
						}
						else
						{
							Vector2 vector = new Vector2(((float)glyphRect.x - num5) * num2, ((float)glyphRect.y - num5) * num3);
							Vector2 vector2 = new Vector2(vector.x, ((float)(glyphRect.y + glyphRect.height) + num5) * num3);
							Vector2 vector3 = new Vector2(((float)(glyphRect.x + glyphRect.width) + num5) * num2, vector2.y);
							ptr2.bottomLeft.uv0 = vector3 * ptr2.bottomLeft.uv0 + vector * (Vector2.one - ptr2.bottomLeft.uv0);
							ptr2.topLeft.uv0 = vector3 * ptr2.topLeft.uv0 + vector * (Vector2.one - ptr2.topLeft.uv0);
							ptr2.topRight.uv0 = vector3 * ptr2.topRight.uv0 + vector * (Vector2.one - ptr2.topRight.uv0);
							ptr2.bottomRight.uv0 = vector3 * ptr2.bottomRight.uv0 + vector * (Vector2.one - ptr2.bottomRight.uv0);
						}
						goto IL_061A;
					}
					hasMultipleColorsByMesh.Add(flag6);
					num++;
				}
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		public void ShapeText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
		{
			IntPtr intPtr = TextLib.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TextLib.ShapeText_Injected(intPtr, ref settings, textGenerationInfo);
		}

		[NativeMethod(Name = "TextLib::GenerateTextMesh", IsThreadSafe = true)]
		private NativeTextInfo GenerateTextInternal(NativeTextGenerationSettings settings, IntPtr textGenerationInfo, ref bool uvsAreGenerated)
		{
			IntPtr intPtr = TextLib.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NativeTextInfo nativeTextInfo;
			TextLib.GenerateTextInternal_Injected(intPtr, ref settings, textGenerationInfo, ref uvsAreGenerated, out nativeTextInfo);
			return nativeTextInfo;
		}

		[NativeMethod(Name = "TextLib::MeasureText")]
		public Vector2 MeasureText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
		{
			IntPtr intPtr = TextLib.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			TextLib.MeasureText_Injected(intPtr, ref settings, textGenerationInfo, out vector);
			return vector;
		}

		[NativeMethod(Name = "TextLib::FindIntersectingLink")]
		public static int FindIntersectingLink(Vector2 point, IntPtr textGenerationInfo)
		{
			return TextLib.FindIntersectingLink_Injected(ref point, textGenerationInfo);
		}

		[NativeMethod(Name = "TextLib::GetCharacterCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCharacterCount(IntPtr textGenerationInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInstance_Injected(ref ManagedSpanWrapper icuData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ShapeText_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateTextInternal_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, ref bool uvsAreGenerated, out NativeTextInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MeasureText_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FindIntersectingLink_Injected([In] ref Vector2 point, IntPtr textGenerationInfo);

		public const int k_unconstrainedScreenSize = -1;

		private readonly IntPtr m_Ptr;

		public static Func<TextAsset> GetICUAssetEditorDelegate;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(TextLib textLib)
			{
				return textLib.m_Ptr;
			}
		}
	}
}
