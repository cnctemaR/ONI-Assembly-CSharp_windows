using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.LowLevel
{
	[NativeHeader("Modules/TextCore/Native/FontEngine/FontEngine.h")]
	public sealed class FontEngine
	{
		internal FontEngine()
		{
		}

		internal static FontEngine GetInstance()
		{
			return FontEngine.s_Instance;
		}

		public static FontEngineError InitializeFontEngine()
		{
			return (FontEngineError)FontEngine.InitializeFontEngine_Internal();
		}

		[NativeMethod(Name = "TextCore::FontEngine::InitFontEngine", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InitializeFontEngine_Internal();

		public static FontEngineError DestroyFontEngine()
		{
			return (FontEngineError)FontEngine.DestroyFontEngine_Internal();
		}

		[NativeMethod(Name = "TextCore::FontEngine::DestroyFontEngine", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int DestroyFontEngine_Internal();

		internal static void SendCancellationRequest()
		{
			FontEngine.SendCancellationRequest_Internal();
		}

		[NativeMethod(Name = "TextCore::FontEngine::SendCancellationRequest", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendCancellationRequest_Internal();

		internal static extern bool isProcessingDone
		{
			[NativeMethod(Name = "TextCore::FontEngine::GetIsProcessingDone", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern float generationProgress
		{
			[NativeMethod(Name = "TextCore::FontEngine::GetGenerationProgress", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static FontEngineError LoadFontFace(string filePath)
		{
			return (FontEngineError)FontEngine.LoadFontFace_Internal(filePath);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_Internal(string filePath);

		public static FontEngineError LoadFontFace(string filePath, int pointSize)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_Internal(filePath, pointSize);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_Internal(string filePath, int pointSize);

		public static FontEngineError LoadFontFace(byte[] sourceFontFile)
		{
			FontEngineError fontEngineError;
			if (sourceFontFile.Length == 0)
			{
				fontEngineError = FontEngineError.Invalid_File;
			}
			else
			{
				fontEngineError = (FontEngineError)FontEngine.LoadFontFace_FromSourceFontFile_Internal(sourceFontFile);
			}
			return fontEngineError;
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_FromSourceFontFile_Internal(byte[] sourceFontFile);

		public static FontEngineError LoadFontFace(byte[] sourceFontFile, int pointSize)
		{
			FontEngineError fontEngineError;
			if (sourceFontFile.Length == 0)
			{
				fontEngineError = FontEngineError.Invalid_File;
			}
			else
			{
				fontEngineError = (FontEngineError)FontEngine.LoadFontFace_With_Size_FromSourceFontFile_Internal(sourceFontFile, pointSize);
			}
			return fontEngineError;
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_FromSourceFontFile_Internal(byte[] sourceFontFile, int pointSize);

		public static FontEngineError LoadFontFace(Font font)
		{
			return (FontEngineError)FontEngine.LoadFontFace_FromFont_Internal(font);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_FromFont_Internal(Font font);

		public static FontEngineError LoadFontFace(Font font, int pointSize)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_FromFont_Internal(font, pointSize);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_FromFont_Internal(Font font, int pointSize);

		public static FontEngineError SetFaceSize(int pointSize)
		{
			return (FontEngineError)FontEngine.SetFaceSize_Internal(pointSize);
		}

		[NativeMethod(Name = "TextCore::FontEngine::SetFaceSize", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SetFaceSize_Internal(int pointSize);

		public static FaceInfo GetFaceInfo()
		{
			FaceInfo faceInfo = default(FaceInfo);
			FontEngine.GetFaceInfo_Internal(ref faceInfo);
			return faceInfo;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetFaceInfo", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFaceInfo_Internal(ref FaceInfo faceInfo);

		[NativeMethod(Name = "TextCore::FontEngine::GetGlyphIndex", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetGlyphIndex(uint unicode);

		[NativeMethod(Name = "TextCore::FontEngine::TryGetGlyphIndex", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetGlyphIndex(uint unicode, out uint glyphIndex);

		internal static FontEngineError LoadGlyph(uint unicode, GlyphLoadFlags flags)
		{
			return (FontEngineError)FontEngine.LoadGlyph_Internal(unicode, flags);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadGlyph", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadGlyph_Internal(uint unicode, GlyphLoadFlags loadFlags);

		public static bool TryGetGlyphWithUnicodeValue(uint unicode, GlyphLoadFlags flags, out Glyph glyph)
		{
			GlyphMarshallingStruct glyphMarshallingStruct = default(GlyphMarshallingStruct);
			bool flag;
			if (FontEngine.TryGetGlyphWithUnicodeValue_Internal(unicode, flags, ref glyphMarshallingStruct))
			{
				glyph = new Glyph(glyphMarshallingStruct);
				flag = true;
			}
			else
			{
				glyph = null;
				flag = false;
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryGetGlyphWithUnicodeValue", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetGlyphWithUnicodeValue_Internal(uint unicode, GlyphLoadFlags loadFlags, ref GlyphMarshallingStruct glyphStruct);

		public static bool TryGetGlyphWithIndexValue(uint glyphIndex, GlyphLoadFlags flags, out Glyph glyph)
		{
			GlyphMarshallingStruct glyphMarshallingStruct = default(GlyphMarshallingStruct);
			bool flag;
			if (FontEngine.TryGetGlyphWithIndexValue_Internal(glyphIndex, flags, ref glyphMarshallingStruct))
			{
				glyph = new Glyph(glyphMarshallingStruct);
				flag = true;
			}
			else
			{
				glyph = null;
				flag = false;
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryGetGlyphWithIndexValue", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetGlyphWithIndexValue_Internal(uint glyphIndex, GlyphLoadFlags loadFlags, ref GlyphMarshallingStruct glyphStruct);

		internal static bool TryPackGlyphInAtlas(Glyph glyph, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects)
		{
			GlyphMarshallingStruct glyphMarshallingStruct = new GlyphMarshallingStruct(glyph);
			int count = freeGlyphRects.Count;
			int count2 = usedGlyphRects.Count;
			int num = count + count2;
			if (FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num)
			{
				int num2 = Mathf.NextPowerOfTwo(num + 1);
				FontEngine.s_FreeGlyphRects = new GlyphRect[num2];
				FontEngine.s_UsedGlyphRects = new GlyphRect[num2];
			}
			int num3 = Mathf.Max(count, count2);
			for (int i = 0; i < num3; i++)
			{
				if (i < count)
				{
					FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
				}
				if (i < count2)
				{
					FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
				}
			}
			bool flag;
			if (FontEngine.TryPackGlyphInAtlas_Internal(ref glyphMarshallingStruct, padding, packingMode, renderMode, width, height, FontEngine.s_FreeGlyphRects, ref count, FontEngine.s_UsedGlyphRects, ref count2))
			{
				glyph.glyphRect = glyphMarshallingStruct.glyphRect;
				freeGlyphRects.Clear();
				usedGlyphRects.Clear();
				num3 = Mathf.Max(count, count2);
				for (int j = 0; j < num3; j++)
				{
					if (j < count)
					{
						freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
					}
					if (j < count2)
					{
						usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
					}
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryPackGlyph", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryPackGlyphInAtlas_Internal(ref GlyphMarshallingStruct glyph, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount);

		internal static bool TryPackGlyphsInAtlas(List<Glyph> glyphsToAdd, List<Glyph> glyphsAdded, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects)
		{
			int count = glyphsToAdd.Count;
			int count2 = glyphsAdded.Count;
			int count3 = freeGlyphRects.Count;
			int count4 = usedGlyphRects.Count;
			int num = count + count2 + count3 + count4;
			if (FontEngine.s_GlyphMarshallingStruct_IN.Length < num || FontEngine.s_GlyphMarshallingStruct_OUT.Length < num || FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num)
			{
				int num2 = Mathf.NextPowerOfTwo(num + 1);
				FontEngine.s_GlyphMarshallingStruct_IN = new GlyphMarshallingStruct[num2];
				FontEngine.s_GlyphMarshallingStruct_OUT = new GlyphMarshallingStruct[num2];
				FontEngine.s_FreeGlyphRects = new GlyphRect[num2];
				FontEngine.s_UsedGlyphRects = new GlyphRect[num2];
			}
			FontEngine.s_GlyphLookupDictionary.Clear();
			for (int i = 0; i < num; i++)
			{
				if (i < count)
				{
					GlyphMarshallingStruct glyphMarshallingStruct = new GlyphMarshallingStruct(glyphsToAdd[i]);
					FontEngine.s_GlyphMarshallingStruct_IN[i] = glyphMarshallingStruct;
					if (!FontEngine.s_GlyphLookupDictionary.ContainsKey(glyphMarshallingStruct.index))
					{
						FontEngine.s_GlyphLookupDictionary.Add(glyphMarshallingStruct.index, glyphsToAdd[i]);
					}
				}
				if (i < count2)
				{
					GlyphMarshallingStruct glyphMarshallingStruct2 = new GlyphMarshallingStruct(glyphsAdded[i]);
					FontEngine.s_GlyphMarshallingStruct_OUT[i] = glyphMarshallingStruct2;
					if (!FontEngine.s_GlyphLookupDictionary.ContainsKey(glyphMarshallingStruct2.index))
					{
						FontEngine.s_GlyphLookupDictionary.Add(glyphMarshallingStruct2.index, glyphsAdded[i]);
					}
				}
				if (i < count3)
				{
					FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
				}
				if (i < count4)
				{
					FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
				}
			}
			bool flag = FontEngine.TryPackGlyphsInAtlas_Internal(FontEngine.s_GlyphMarshallingStruct_IN, ref count, FontEngine.s_GlyphMarshallingStruct_OUT, ref count2, padding, packingMode, renderMode, width, height, FontEngine.s_FreeGlyphRects, ref count3, FontEngine.s_UsedGlyphRects, ref count4);
			glyphsToAdd.Clear();
			glyphsAdded.Clear();
			freeGlyphRects.Clear();
			usedGlyphRects.Clear();
			for (int j = 0; j < num; j++)
			{
				if (j < count)
				{
					GlyphMarshallingStruct glyphMarshallingStruct3 = FontEngine.s_GlyphMarshallingStruct_IN[j];
					Glyph glyph = FontEngine.s_GlyphLookupDictionary[glyphMarshallingStruct3.index];
					glyph.metrics = glyphMarshallingStruct3.metrics;
					glyph.glyphRect = glyphMarshallingStruct3.glyphRect;
					glyph.scale = glyphMarshallingStruct3.scale;
					glyph.atlasIndex = glyphMarshallingStruct3.atlasIndex;
					glyphsToAdd.Add(glyph);
				}
				if (j < count2)
				{
					GlyphMarshallingStruct glyphMarshallingStruct4 = FontEngine.s_GlyphMarshallingStruct_OUT[j];
					Glyph glyph2 = FontEngine.s_GlyphLookupDictionary[glyphMarshallingStruct4.index];
					glyph2.metrics = glyphMarshallingStruct4.metrics;
					glyph2.glyphRect = glyphMarshallingStruct4.glyphRect;
					glyph2.scale = glyphMarshallingStruct4.scale;
					glyph2.atlasIndex = glyphMarshallingStruct4.atlasIndex;
					glyphsAdded.Add(glyph2);
				}
				if (j < count3)
				{
					freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
				}
				if (j < count4)
				{
					usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
				}
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryPackGlyphs", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryPackGlyphsInAtlas_Internal([Out] GlyphMarshallingStruct[] glyphsToAdd, ref int glyphsToAddCount, [Out] GlyphMarshallingStruct[] glyphsAdded, ref int glyphsAddedCount, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount);

		internal static FontEngineError RenderGlyphToTexture(Glyph glyph, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			GlyphMarshallingStruct glyphMarshallingStruct = new GlyphMarshallingStruct(glyph);
			return (FontEngineError)FontEngine.RenderGlyphToTexture_Internal(glyphMarshallingStruct, padding, renderMode, texture);
		}

		[NativeMethod(Name = "TextCore::FontEngine::RenderGlyphToTexture", IsFreeFunction = true)]
		private static int RenderGlyphToTexture_Internal(GlyphMarshallingStruct glyphStruct, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			return FontEngine.RenderGlyphToTexture_Internal_Injected(ref glyphStruct, padding, renderMode, texture);
		}

		internal static FontEngineError RenderGlyphsToTexture(List<Glyph> glyphs, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			int count = glyphs.Count;
			if (FontEngine.s_GlyphMarshallingStruct_IN.Length < count)
			{
				int num = Mathf.NextPowerOfTwo(count + 1);
				FontEngine.s_GlyphMarshallingStruct_IN = new GlyphMarshallingStruct[num];
			}
			for (int i = 0; i < count; i++)
			{
				FontEngine.s_GlyphMarshallingStruct_IN[i] = new GlyphMarshallingStruct(glyphs[i]);
			}
			return (FontEngineError)FontEngine.RenderGlyphsToTexture_Internal(FontEngine.s_GlyphMarshallingStruct_IN, count, padding, renderMode, texture);
		}

		[NativeMethod(Name = "TextCore::FontEngine::RenderGlyphsToTexture", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphsToTexture_Internal(GlyphMarshallingStruct[] glyphs, int glyphCount, int padding, GlyphRenderMode renderMode, Texture2D texture);

		internal static FontEngineError RenderGlyphsToTexture(List<Glyph> glyphs, int padding, GlyphRenderMode renderMode, byte[] texBuffer, int texWidth, int texHeight)
		{
			int count = glyphs.Count;
			if (FontEngine.s_GlyphMarshallingStruct_IN.Length < count)
			{
				int num = Mathf.NextPowerOfTwo(count + 1);
				FontEngine.s_GlyphMarshallingStruct_IN = new GlyphMarshallingStruct[num];
			}
			for (int i = 0; i < count; i++)
			{
				FontEngine.s_GlyphMarshallingStruct_IN[i] = new GlyphMarshallingStruct(glyphs[i]);
			}
			return (FontEngineError)FontEngine.RenderGlyphsToTextureBuffer_Internal(FontEngine.s_GlyphMarshallingStruct_IN, count, padding, renderMode, texBuffer, texWidth, texHeight);
		}

		[NativeMethod(Name = "TextCore::FontEngine::RenderGlyphsToTextureBuffer", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphsToTextureBuffer_Internal(GlyphMarshallingStruct[] glyphs, int glyphCount, int padding, GlyphRenderMode renderMode, [Out] byte[] texBuffer, int texWidth, int texHeight);

		internal static FontEngineError RenderGlyphsToSharedTexture(List<Glyph> glyphs, int padding, GlyphRenderMode renderMode)
		{
			int count = glyphs.Count;
			if (FontEngine.s_GlyphMarshallingStruct_IN.Length < count)
			{
				int num = Mathf.NextPowerOfTwo(count + 1);
				FontEngine.s_GlyphMarshallingStruct_IN = new GlyphMarshallingStruct[num];
			}
			for (int i = 0; i < count; i++)
			{
				FontEngine.s_GlyphMarshallingStruct_IN[i] = new GlyphMarshallingStruct(glyphs[i]);
			}
			return (FontEngineError)FontEngine.RenderGlyphsToSharedTexture_Internal(FontEngine.s_GlyphMarshallingStruct_IN, count, padding, renderMode);
		}

		[NativeMethod(Name = "TextCore::FontEngine::RenderGlyphsToSharedTexture", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphsToSharedTexture_Internal(GlyphMarshallingStruct[] glyphs, int glyphCount, int padding, GlyphRenderMode renderMode);

		[NativeMethod(Name = "TextCore::FontEngine::SetSharedTextureData", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSharedTexture(Texture2D texture);

		[NativeMethod(Name = "TextCore::FontEngine::ReleaseSharedTextureData", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReleaseSharedTexture();

		internal static bool TryAddGlyphToTexture(uint glyphIndex, int padding, GlyphPackingMode packingMode, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects, GlyphRenderMode renderMode, Texture2D texture, out Glyph glyph)
		{
			int count = freeGlyphRects.Count;
			int count2 = usedGlyphRects.Count;
			int num = count + count2;
			if (FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num)
			{
				int num2 = Mathf.NextPowerOfTwo(num + 1);
				FontEngine.s_FreeGlyphRects = new GlyphRect[num2];
				FontEngine.s_UsedGlyphRects = new GlyphRect[num2];
			}
			int num3 = Mathf.Max(count, count2);
			for (int i = 0; i < num3; i++)
			{
				if (i < count)
				{
					FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
				}
				if (i < count2)
				{
					FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
				}
			}
			GlyphMarshallingStruct glyphMarshallingStruct;
			bool flag;
			if (FontEngine.TryAddGlyphToTexture_Internal(glyphIndex, padding, packingMode, FontEngine.s_FreeGlyphRects, ref count, FontEngine.s_UsedGlyphRects, ref count2, renderMode, texture, out glyphMarshallingStruct))
			{
				glyph = new Glyph(glyphMarshallingStruct);
				freeGlyphRects.Clear();
				usedGlyphRects.Clear();
				num3 = Mathf.Max(count, count2);
				for (int j = 0; j < num3; j++)
				{
					if (j < count)
					{
						freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
					}
					if (j < count2)
					{
						usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
					}
				}
				flag = true;
			}
			else
			{
				glyph = null;
				flag = false;
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryAddGlyphToTexture", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryAddGlyphToTexture_Internal(uint glyphIndex, int padding, GlyphPackingMode packingMode, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, Texture2D texture, out GlyphMarshallingStruct glyph);

		internal static bool TryAddGlyphsToTexture(List<uint> glyphIndexes, int padding, GlyphPackingMode packingMode, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects, GlyphRenderMode renderMode, Texture2D texture, out Glyph[] glyphs)
		{
			glyphs = null;
			bool flag;
			if (glyphIndexes == null || glyphIndexes.Count == 0)
			{
				flag = false;
			}
			else
			{
				int count = glyphIndexes.Count;
				if (FontEngine.s_GlyphIndexes_MarshallingArray == null || FontEngine.s_GlyphIndexes_MarshallingArray.Length < count)
				{
					if (FontEngine.s_GlyphIndexes_MarshallingArray == null)
					{
						FontEngine.s_GlyphIndexes_MarshallingArray = new uint[count];
					}
					else
					{
						int num = Mathf.NextPowerOfTwo(count + 1);
						FontEngine.s_GlyphIndexes_MarshallingArray = new uint[num];
					}
				}
				int count2 = freeGlyphRects.Count;
				int count3 = usedGlyphRects.Count;
				int num2 = count2 + count3 + count;
				if (FontEngine.s_FreeGlyphRects.Length < num2 || FontEngine.s_UsedGlyphRects.Length < num2)
				{
					int num3 = Mathf.NextPowerOfTwo(num2 + 1);
					FontEngine.s_FreeGlyphRects = new GlyphRect[num3];
					FontEngine.s_UsedGlyphRects = new GlyphRect[num3];
				}
				if (FontEngine.s_GlyphMarshallingStruct_OUT.Length < count)
				{
					int num4 = Mathf.NextPowerOfTwo(count + 1);
					FontEngine.s_GlyphMarshallingStruct_OUT = new GlyphMarshallingStruct[num4];
				}
				int num5 = FontEngineUtilities.MaxValue(count2, count3, count);
				for (int i = 0; i < num5; i++)
				{
					if (i < count)
					{
						FontEngine.s_GlyphIndexes_MarshallingArray[i] = glyphIndexes[i];
					}
					if (i < count2)
					{
						FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
					}
					if (i < count3)
					{
						FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
					}
				}
				bool flag2 = FontEngine.TryAddGlyphsToTexture_Internal(FontEngine.s_GlyphIndexes_MarshallingArray, padding, packingMode, FontEngine.s_FreeGlyphRects, ref count2, FontEngine.s_UsedGlyphRects, ref count3, renderMode, texture, FontEngine.s_GlyphMarshallingStruct_OUT, ref count);
				if (FontEngine.s_Glyphs == null || FontEngine.s_Glyphs.Length <= count)
				{
					FontEngine.s_Glyphs = new Glyph[Mathf.NextPowerOfTwo(count + 1)];
				}
				FontEngine.s_Glyphs[count] = null;
				freeGlyphRects.Clear();
				usedGlyphRects.Clear();
				num5 = FontEngineUtilities.MaxValue(count2, count3, count);
				for (int j = 0; j < num5; j++)
				{
					if (j < count)
					{
						FontEngine.s_Glyphs[j] = new Glyph(FontEngine.s_GlyphMarshallingStruct_OUT[j]);
					}
					if (j < count2)
					{
						freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
					}
					if (j < count3)
					{
						usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
					}
				}
				glyphs = FontEngine.s_Glyphs;
				flag = flag2;
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryAddGlyphsToTexture", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryAddGlyphsToTexture_Internal(uint[] glyphIndex, int padding, GlyphPackingMode packingMode, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, Texture2D texture, [Out] GlyphMarshallingStruct[] glyphs, ref int glyphCount);

		internal static GlyphPairAdjustmentRecord[] GetGlyphPairAdjustmentTable(uint[] glyphIndexes)
		{
			int num = glyphIndexes.Length * glyphIndexes.Length;
			if (FontEngine.s_GlyphPairAdjustmentRecords_MarshallingArray == null || FontEngine.s_GlyphPairAdjustmentRecords_MarshallingArray.Length < num)
			{
				FontEngine.s_GlyphPairAdjustmentRecords_MarshallingArray = new GlyphPairAdjustmentRecord[num];
			}
			int num2;
			GlyphPairAdjustmentRecord[] array;
			if (FontEngine.GetGlyphPairAdjustmentTable_Internal(glyphIndexes, FontEngine.s_GlyphPairAdjustmentRecords_MarshallingArray, out num2) != 0)
			{
				array = null;
			}
			else
			{
				GlyphPairAdjustmentRecord[] array2 = new GlyphPairAdjustmentRecord[num2];
				for (int i = 0; i < num2; i++)
				{
					array2[i] = FontEngine.s_GlyphPairAdjustmentRecords_MarshallingArray[i];
				}
				array = array2;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetGlyphPairAdjustmentTable", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGlyphPairAdjustmentTable_Internal(uint[] glyphIndexes, [Out] GlyphPairAdjustmentRecord[] glyphPairAdjustmentRecords, out int adjustmentRecordCount);

		[NativeMethod(Name = "TextCore::FontEngine::ResetAtlasTexture", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ResetAtlasTexture(Texture2D texture);

		[NativeMethod(Name = "TextCore::FontEngine::RenderToTexture", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RenderBufferToTexture(Texture2D srcTexture, int padding, GlyphRenderMode renderMode, Texture2D dstTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphToTexture_Internal_Injected(ref GlyphMarshallingStruct glyphStruct, int padding, GlyphRenderMode renderMode, Texture2D texture);

		private static readonly FontEngine s_Instance = new FontEngine();

		private static Glyph[] s_Glyphs = new Glyph[16];

		private static uint[] s_GlyphIndexes_MarshallingArray = new uint[16];

		private static GlyphMarshallingStruct[] s_GlyphMarshallingStruct_IN = new GlyphMarshallingStruct[16];

		private static GlyphMarshallingStruct[] s_GlyphMarshallingStruct_OUT = new GlyphMarshallingStruct[16];

		private static GlyphRect[] s_FreeGlyphRects = new GlyphRect[16];

		private static GlyphRect[] s_UsedGlyphRects = new GlyphRect[16];

		private static GlyphPairAdjustmentRecord[] s_GlyphPairAdjustmentRecords_MarshallingArray;

		private static Dictionary<uint, Glyph> s_GlyphLookupDictionary = new Dictionary<uint, Glyph>();
	}
}
