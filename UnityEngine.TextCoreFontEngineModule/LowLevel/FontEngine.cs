using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.LowLevel
{
	[NativeHeader("Modules/TextCoreFontEngine/Native/FontEngine.h")]
	public sealed class FontEngine
	{
		internal FontEngine()
		{
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
		private unsafe static int LoadFontFace_Internal(string filePath)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = FontEngine.LoadFontFace_Internal_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public static FontEngineError LoadFontFace(string filePath, int pointSize)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_Internal(filePath, pointSize);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private unsafe static int LoadFontFace_With_Size_Internal(string filePath, int pointSize)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = FontEngine.LoadFontFace_With_Size_Internal_Injected(ref managedSpanWrapper, pointSize);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public static FontEngineError LoadFontFace(string filePath, float pointSize, int faceIndex)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_And_FaceIndex_Internal(filePath, (int)Math.Round((double)pointSize, MidpointRounding.AwayFromZero), faceIndex);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private unsafe static int LoadFontFace_With_Size_And_FaceIndex_Internal(string filePath, int pointSize, int faceIndex)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = FontEngine.LoadFontFace_With_Size_And_FaceIndex_Internal_Injected(ref managedSpanWrapper, pointSize, faceIndex);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public static FontEngineError LoadFontFace(byte[] sourceFontFile)
		{
			bool flag = sourceFontFile.Length == 0;
			FontEngineError fontEngineError;
			if (flag)
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
		private unsafe static int LoadFontFace_FromSourceFontFile_Internal(byte[] sourceFontFile)
		{
			Span<byte> span = new Span<byte>(sourceFontFile);
			int num;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.LoadFontFace_FromSourceFontFile_Internal_Injected(ref managedSpanWrapper);
			}
			return num;
		}

		public static FontEngineError LoadFontFace(byte[] sourceFontFile, int pointSize)
		{
			bool flag = sourceFontFile.Length == 0;
			FontEngineError fontEngineError;
			if (flag)
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
		private unsafe static int LoadFontFace_With_Size_FromSourceFontFile_Internal(byte[] sourceFontFile, int pointSize)
		{
			Span<byte> span = new Span<byte>(sourceFontFile);
			int num;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.LoadFontFace_With_Size_FromSourceFontFile_Internal_Injected(ref managedSpanWrapper, pointSize);
			}
			return num;
		}

		public static FontEngineError LoadFontFace(byte[] sourceFontFile, float pointSize, int faceIndex)
		{
			bool flag = sourceFontFile.Length == 0;
			FontEngineError fontEngineError;
			if (flag)
			{
				fontEngineError = FontEngineError.Invalid_File;
			}
			else
			{
				fontEngineError = (FontEngineError)FontEngine.LoadFontFace_With_Size_And_FaceIndex_FromSourceFontFile_Internal(sourceFontFile, (int)Math.Round((double)pointSize, MidpointRounding.AwayFromZero), faceIndex);
			}
			return fontEngineError;
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private unsafe static int LoadFontFace_With_Size_And_FaceIndex_FromSourceFontFile_Internal(byte[] sourceFontFile, int pointSize, int faceIndex)
		{
			Span<byte> span = new Span<byte>(sourceFontFile);
			int num;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.LoadFontFace_With_Size_And_FaceIndex_FromSourceFontFile_Internal_Injected(ref managedSpanWrapper, pointSize, faceIndex);
			}
			return num;
		}

		public static FontEngineError LoadFontFace(Font font)
		{
			return (FontEngineError)FontEngine.LoadFontFace_FromFont_Internal(font);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private static int LoadFontFace_FromFont_Internal(Font font)
		{
			return FontEngine.LoadFontFace_FromFont_Internal_Injected(Object.MarshalledUnityObject.Marshal<Font>(font));
		}

		public static FontEngineError LoadFontFace(Font font, int pointSize)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_FromFont_Internal(font, pointSize);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private static int LoadFontFace_With_Size_FromFont_Internal(Font font, int pointSize)
		{
			return FontEngine.LoadFontFace_With_Size_FromFont_Internal_Injected(Object.MarshalledUnityObject.Marshal<Font>(font), pointSize);
		}

		public static FontEngineError LoadFontFace(Font font, float pointSize, int faceIndex)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_and_FaceIndex_FromFont_Internal(font, (int)Math.Round((double)pointSize, MidpointRounding.AwayFromZero), faceIndex);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private static int LoadFontFace_With_Size_and_FaceIndex_FromFont_Internal(Font font, int pointSize, int faceIndex)
		{
			return FontEngine.LoadFontFace_With_Size_and_FaceIndex_FromFont_Internal_Injected(Object.MarshalledUnityObject.Marshal<Font>(font), pointSize, faceIndex);
		}

		public static FontEngineError LoadFontFace(string familyName, string styleName)
		{
			return (FontEngineError)FontEngine.LoadFontFace_by_FamilyName_and_StyleName_Internal(familyName, styleName);
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private unsafe static int LoadFontFace_by_FamilyName_and_StyleName_Internal(string familyName, string styleName)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(familyName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = familyName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(styleName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = styleName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				num = FontEngine.LoadFontFace_by_FamilyName_and_StyleName_Internal_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return num;
		}

		public static FontEngineError LoadFontFace(string familyName, string styleName, float pointSize)
		{
			return (FontEngineError)FontEngine.LoadFontFace_With_Size_by_FamilyName_and_StyleName_Internal(familyName, styleName, (int)Math.Round((double)pointSize, MidpointRounding.AwayFromZero));
		}

		[NativeMethod(Name = "TextCore::FontEngine::LoadFontFace", IsFreeFunction = true)]
		private unsafe static int LoadFontFace_With_Size_by_FamilyName_and_StyleName_Internal(string familyName, string styleName, int pointSize)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(familyName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = familyName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(styleName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = styleName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				num = FontEngine.LoadFontFace_With_Size_by_FamilyName_and_StyleName_Internal_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, pointSize);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return num;
		}

		public static FontEngineError UnloadFontFace()
		{
			return (FontEngineError)FontEngine.UnloadFontFace_Internal();
		}

		[NativeMethod(Name = "TextCore::FontEngine::UnloadFontFace", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int UnloadFontFace_Internal();

		public static FontEngineError UnloadAllFontFaces()
		{
			return (FontEngineError)FontEngine.UnloadAllFontFaces_Internal();
		}

		[NativeMethod(Name = "TextCore::FontEngine::UnloadAllFontFaces", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int UnloadAllFontFaces_Internal();

		public static string[] GetSystemFontNames()
		{
			string[] systemFontNames_Internal = FontEngine.GetSystemFontNames_Internal();
			bool flag = systemFontNames_Internal != null && systemFontNames_Internal.Length == 0;
			string[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				array = systemFontNames_Internal;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetSystemFontNames", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetSystemFontNames_Internal();

		[NativeMethod(Name = "TextCore::FontEngine::GetSystemFontReferences", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern FontReference[] GetSystemFontReferences();

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsColorFontFace();

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static bool TryGetSystemFontReference(string familyName, string styleName, out FontReference fontRef)
		{
			return FontEngine.TryGetSystemFontReference_Internal(familyName, styleName, out fontRef);
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryGetSystemFontReference", IsThreadSafe = true, IsFreeFunction = true)]
		private unsafe static bool TryGetSystemFontReference_Internal(string familyName, string styleName, out FontReference fontRef)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(familyName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = familyName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(styleName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = styleName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				flag = FontEngine.TryGetSystemFontReference_Internal_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, out fontRef);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return flag;
		}

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

		[NativeMethod(Name = "TextCore::FontEngine::GetFaceCount", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetFaceCount();

		public static string[] GetFontFaces()
		{
			string[] fontFaces_Internal = FontEngine.GetFontFaces_Internal();
			bool flag = fontFaces_Internal != null && fontFaces_Internal.Length == 0;
			string[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				array = fontFaces_Internal;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetFontFaces", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetFontFaces_Internal();

		[NativeMethod(Name = "TextCore::FontEngine::GetVariantGlyphIndex", IsThreadSafe = true, IsFreeFunction = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetVariantGlyphIndex(uint unicode, uint variantSelectorUnicode);

		[NativeMethod(Name = "TextCore::FontEngine::GetGlyphIndex", IsThreadSafe = true, IsFreeFunction = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetGlyphIndex(uint unicode);

		[NativeMethod(Name = "TextCore::FontEngine::TryGetGlyphIndex", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetGlyphIndex(uint unicode, out uint glyphIndex);

		internal static Dictionary<uint, List<int>> GetCharacterMap()
		{
			GlyphIndexCodePointMap[] fontCharacterMap_Internal = FontEngine.GetFontCharacterMap_Internal();
			Dictionary<uint, List<int>> dictionary = new Dictionary<uint, List<int>>();
			for (int i = 0; i < fontCharacterMap_Internal.Length; i++)
			{
				uint glyphIndex = fontCharacterMap_Internal[i].glyphIndex;
				uint unicode = fontCharacterMap_Internal[i].unicode;
				bool flag = !dictionary.ContainsKey(glyphIndex);
				if (flag)
				{
					dictionary.Add(glyphIndex, new List<int> { (int)unicode });
				}
				else
				{
					dictionary[glyphIndex].Add((int)unicode);
				}
			}
			return dictionary;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetFontCharacterMap", IsThreadSafe = true, IsFreeFunction = true)]
		internal static GlyphIndexCodePointMap[] GetFontCharacterMap_Internal()
		{
			GlyphIndexCodePointMap[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetFontCharacterMap_Internal_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GlyphIndexCodePointMap[] array;
				blittableArrayWrapper.Unmarshal<GlyphIndexCodePointMap>(ref array);
				array2 = array;
			}
			return array2;
		}

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
			bool flag = FontEngine.TryGetGlyphWithUnicodeValue_Internal(unicode, flags, ref glyphMarshallingStruct);
			bool flag2;
			if (flag)
			{
				glyph = new Glyph(glyphMarshallingStruct);
				flag2 = true;
			}
			else
			{
				glyph = null;
				flag2 = false;
			}
			return flag2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryGetGlyphWithUnicodeValue", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetGlyphWithUnicodeValue_Internal(uint unicode, GlyphLoadFlags loadFlags, ref GlyphMarshallingStruct glyphStruct);

		public static bool TryGetGlyphWithIndexValue(uint glyphIndex, GlyphLoadFlags flags, out Glyph glyph)
		{
			GlyphMarshallingStruct glyphMarshallingStruct = default(GlyphMarshallingStruct);
			bool flag = FontEngine.TryGetGlyphWithIndexValue_Internal(glyphIndex, flags, ref glyphMarshallingStruct);
			bool flag2;
			if (flag)
			{
				glyph = new Glyph(glyphMarshallingStruct);
				flag2 = true;
			}
			else
			{
				glyph = null;
				flag2 = false;
			}
			return flag2;
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
			bool flag = FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num;
			if (flag)
			{
				int num2 = Mathf.NextPowerOfTwo(num + 1);
				FontEngine.s_FreeGlyphRects = new GlyphRect[num2];
				FontEngine.s_UsedGlyphRects = new GlyphRect[num2];
			}
			int num3 = Mathf.Max(count, count2);
			for (int i = 0; i < num3; i++)
			{
				bool flag2 = i < count;
				if (flag2)
				{
					FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
				}
				bool flag3 = i < count2;
				if (flag3)
				{
					FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
				}
			}
			bool flag4 = FontEngine.TryPackGlyphInAtlas_Internal(ref glyphMarshallingStruct, padding, packingMode, renderMode, width, height, FontEngine.s_FreeGlyphRects, ref count, FontEngine.s_UsedGlyphRects, ref count2);
			bool flag7;
			if (flag4)
			{
				glyph.glyphRect = glyphMarshallingStruct.glyphRect;
				freeGlyphRects.Clear();
				usedGlyphRects.Clear();
				num3 = Mathf.Max(count, count2);
				for (int j = 0; j < num3; j++)
				{
					bool flag5 = j < count;
					if (flag5)
					{
						freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
					}
					bool flag6 = j < count2;
					if (flag6)
					{
						usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
					}
				}
				flag7 = true;
			}
			else
			{
				flag7 = false;
			}
			return flag7;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryPackGlyph", IsThreadSafe = true, IsFreeFunction = true)]
		private unsafe static bool TryPackGlyphInAtlas_Internal(ref GlyphMarshallingStruct glyph, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount)
		{
			bool flag;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (freeGlyphRects != null)
				{
					fixed (GlyphRect[] array = freeGlyphRects)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper2;
				if (usedGlyphRects != null)
				{
					fixed (GlyphRect[] array2 = usedGlyphRects)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				flag = FontEngine.TryPackGlyphInAtlas_Internal_Injected(ref glyph, padding, packingMode, renderMode, width, height, out blittableArrayWrapper, ref freeGlyphRectCount, out blittableArrayWrapper2, ref usedGlyphRectCount);
			}
			finally
			{
				GlyphRect[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<GlyphRect>(ref array);
				GlyphRect[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<GlyphRect>(ref array2);
			}
			return flag;
		}

		internal static bool TryPackGlyphsInAtlas(List<Glyph> glyphsToAdd, List<Glyph> glyphsAdded, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects)
		{
			int count = glyphsToAdd.Count;
			int count2 = glyphsAdded.Count;
			int count3 = freeGlyphRects.Count;
			int count4 = usedGlyphRects.Count;
			int num = count + count2 + count3 + count4;
			bool flag = FontEngine.s_GlyphMarshallingStruct_IN.Length < num || FontEngine.s_GlyphMarshallingStruct_OUT.Length < num || FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num;
			if (flag)
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
				bool flag2 = i < count;
				if (flag2)
				{
					GlyphMarshallingStruct glyphMarshallingStruct = new GlyphMarshallingStruct(glyphsToAdd[i]);
					FontEngine.s_GlyphMarshallingStruct_IN[i] = glyphMarshallingStruct;
					bool flag3 = !FontEngine.s_GlyphLookupDictionary.ContainsKey(glyphMarshallingStruct.index);
					if (flag3)
					{
						FontEngine.s_GlyphLookupDictionary.Add(glyphMarshallingStruct.index, glyphsToAdd[i]);
					}
				}
				bool flag4 = i < count2;
				if (flag4)
				{
					GlyphMarshallingStruct glyphMarshallingStruct2 = new GlyphMarshallingStruct(glyphsAdded[i]);
					FontEngine.s_GlyphMarshallingStruct_OUT[i] = glyphMarshallingStruct2;
					bool flag5 = !FontEngine.s_GlyphLookupDictionary.ContainsKey(glyphMarshallingStruct2.index);
					if (flag5)
					{
						FontEngine.s_GlyphLookupDictionary.Add(glyphMarshallingStruct2.index, glyphsAdded[i]);
					}
				}
				bool flag6 = i < count3;
				if (flag6)
				{
					FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
				}
				bool flag7 = i < count4;
				if (flag7)
				{
					FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
				}
			}
			bool flag8 = FontEngine.TryPackGlyphsInAtlas_Internal(FontEngine.s_GlyphMarshallingStruct_IN, ref count, FontEngine.s_GlyphMarshallingStruct_OUT, ref count2, padding, packingMode, renderMode, width, height, FontEngine.s_FreeGlyphRects, ref count3, FontEngine.s_UsedGlyphRects, ref count4);
			glyphsToAdd.Clear();
			glyphsAdded.Clear();
			freeGlyphRects.Clear();
			usedGlyphRects.Clear();
			for (int j = 0; j < num; j++)
			{
				bool flag9 = j < count;
				if (flag9)
				{
					GlyphMarshallingStruct glyphMarshallingStruct3 = FontEngine.s_GlyphMarshallingStruct_IN[j];
					Glyph glyph = FontEngine.s_GlyphLookupDictionary[glyphMarshallingStruct3.index];
					glyph.metrics = glyphMarshallingStruct3.metrics;
					glyph.glyphRect = glyphMarshallingStruct3.glyphRect;
					glyph.scale = glyphMarshallingStruct3.scale;
					glyph.atlasIndex = glyphMarshallingStruct3.atlasIndex;
					glyphsToAdd.Add(glyph);
				}
				bool flag10 = j < count2;
				if (flag10)
				{
					GlyphMarshallingStruct glyphMarshallingStruct4 = FontEngine.s_GlyphMarshallingStruct_OUT[j];
					Glyph glyph2 = FontEngine.s_GlyphLookupDictionary[glyphMarshallingStruct4.index];
					glyph2.metrics = glyphMarshallingStruct4.metrics;
					glyph2.glyphRect = glyphMarshallingStruct4.glyphRect;
					glyph2.scale = glyphMarshallingStruct4.scale;
					glyph2.atlasIndex = glyphMarshallingStruct4.atlasIndex;
					glyphsAdded.Add(glyph2);
				}
				bool flag11 = j < count3;
				if (flag11)
				{
					freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
				}
				bool flag12 = j < count4;
				if (flag12)
				{
					usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
				}
			}
			return flag8;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryPackGlyphs", IsThreadSafe = true, IsFreeFunction = true)]
		private unsafe static bool TryPackGlyphsInAtlas_Internal([Out] GlyphMarshallingStruct[] glyphsToAdd, ref int glyphsToAddCount, [Out] GlyphMarshallingStruct[] glyphsAdded, ref int glyphsAddedCount, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount)
		{
			bool flag;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (glyphsToAdd != null)
				{
					fixed (GlyphMarshallingStruct[] array = glyphsToAdd)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper2;
				if (glyphsAdded != null)
				{
					fixed (GlyphMarshallingStruct[] array2 = glyphsAdded)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper3;
				if (freeGlyphRects != null)
				{
					fixed (GlyphRect[] array3 = freeGlyphRects)
					{
						if (array3.Length != 0)
						{
							blittableArrayWrapper3 = new BlittableArrayWrapper((void*)(&array3[0]), array3.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper4;
				if (usedGlyphRects != null)
				{
					fixed (GlyphRect[] array4 = usedGlyphRects)
					{
						if (array4.Length != 0)
						{
							blittableArrayWrapper4 = new BlittableArrayWrapper((void*)(&array4[0]), array4.Length);
						}
					}
				}
				flag = FontEngine.TryPackGlyphsInAtlas_Internal_Injected(out blittableArrayWrapper, ref glyphsToAddCount, out blittableArrayWrapper2, ref glyphsAddedCount, padding, packingMode, renderMode, width, height, out blittableArrayWrapper3, ref freeGlyphRectCount, out blittableArrayWrapper4, ref usedGlyphRectCount);
			}
			finally
			{
				GlyphMarshallingStruct[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<GlyphMarshallingStruct>(ref array);
				GlyphMarshallingStruct[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<GlyphMarshallingStruct>(ref array2);
				GlyphRect[] array3;
				BlittableArrayWrapper blittableArrayWrapper3;
				blittableArrayWrapper3.Unmarshal<GlyphRect>(ref array3);
				GlyphRect[] array4;
				BlittableArrayWrapper blittableArrayWrapper4;
				blittableArrayWrapper4.Unmarshal<GlyphRect>(ref array4);
			}
			return flag;
		}

		internal static FontEngineError RenderGlyphToTexture(Glyph glyph, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			GlyphMarshallingStruct glyphMarshallingStruct = new GlyphMarshallingStruct(glyph);
			return (FontEngineError)FontEngine.RenderGlyphToTexture_Internal(glyphMarshallingStruct, padding, renderMode, texture);
		}

		[NativeMethod(Name = "TextCore::FontEngine::RenderGlyphToTexture", IsFreeFunction = true)]
		private static int RenderGlyphToTexture_Internal(GlyphMarshallingStruct glyphStruct, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			return FontEngine.RenderGlyphToTexture_Internal_Injected(ref glyphStruct, padding, renderMode, Object.MarshalledUnityObject.Marshal<Texture2D>(texture));
		}

		internal static FontEngineError RenderGlyphsToTexture(List<Glyph> glyphs, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			int count = glyphs.Count;
			bool flag = FontEngine.s_GlyphMarshallingStruct_IN.Length < count;
			if (flag)
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
		private unsafe static int RenderGlyphsToTexture_Internal(GlyphMarshallingStruct[] glyphs, int glyphCount, int padding, GlyphRenderMode renderMode, Texture2D texture)
		{
			Span<GlyphMarshallingStruct> span = new Span<GlyphMarshallingStruct>(glyphs);
			int num;
			fixed (GlyphMarshallingStruct* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.RenderGlyphsToTexture_Internal_Injected(ref managedSpanWrapper, glyphCount, padding, renderMode, Object.MarshalledUnityObject.Marshal<Texture2D>(texture));
			}
			return num;
		}

		internal static FontEngineError RenderGlyphsToTexture(List<Glyph> glyphs, int padding, GlyphRenderMode renderMode, byte[] texBuffer, int texWidth, int texHeight)
		{
			int count = glyphs.Count;
			bool flag = FontEngine.s_GlyphMarshallingStruct_IN.Length < count;
			if (flag)
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
		private unsafe static int RenderGlyphsToTextureBuffer_Internal(GlyphMarshallingStruct[] glyphs, int glyphCount, int padding, GlyphRenderMode renderMode, [Out] byte[] texBuffer, int texWidth, int texHeight)
		{
			int num;
			try
			{
				Span<GlyphMarshallingStruct> span = new Span<GlyphMarshallingStruct>(glyphs);
				fixed (GlyphMarshallingStruct* ptr = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
					BlittableArrayWrapper blittableArrayWrapper;
					if (texBuffer != null)
					{
						fixed (byte[] array = texBuffer)
						{
							if (array.Length != 0)
							{
								blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
							}
						}
					}
					num = FontEngine.RenderGlyphsToTextureBuffer_Internal_Injected(ref managedSpanWrapper, glyphCount, padding, renderMode, out blittableArrayWrapper, texWidth, texHeight);
				}
			}
			finally
			{
				GlyphMarshallingStruct* ptr = null;
				byte[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
			}
			return num;
		}

		internal static FontEngineError RenderGlyphsToSharedTexture(List<Glyph> glyphs, int padding, GlyphRenderMode renderMode)
		{
			int count = glyphs.Count;
			bool flag = FontEngine.s_GlyphMarshallingStruct_IN.Length < count;
			if (flag)
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
		private unsafe static int RenderGlyphsToSharedTexture_Internal(GlyphMarshallingStruct[] glyphs, int glyphCount, int padding, GlyphRenderMode renderMode)
		{
			Span<GlyphMarshallingStruct> span = new Span<GlyphMarshallingStruct>(glyphs);
			int num;
			fixed (GlyphMarshallingStruct* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.RenderGlyphsToSharedTexture_Internal_Injected(ref managedSpanWrapper, glyphCount, padding, renderMode);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::SetSharedTextureData", IsFreeFunction = true)]
		internal static void SetSharedTexture(Texture2D texture)
		{
			FontEngine.SetSharedTexture_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(texture));
		}

		[NativeMethod(Name = "TextCore::FontEngine::ReleaseSharedTextureData", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReleaseSharedTexture();

		[NativeMethod(Name = "TextCore::FontEngine::SetTextureUploadMode", IsThreadSafe = true, IsFreeFunction = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTextureUploadMode(bool shouldUploadImmediately);

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static bool TryAddGlyphToTexture(uint glyphIndex, int padding, GlyphPackingMode packingMode, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects, GlyphRenderMode renderMode, Texture2D texture, out Glyph glyph)
		{
			int count = freeGlyphRects.Count;
			int count2 = usedGlyphRects.Count;
			int num = count + count2;
			bool flag = FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num;
			if (flag)
			{
				int num2 = Mathf.NextPowerOfTwo(num + 1);
				FontEngine.s_FreeGlyphRects = new GlyphRect[num2];
				FontEngine.s_UsedGlyphRects = new GlyphRect[num2];
			}
			int num3 = Mathf.Max(count, count2);
			for (int i = 0; i < num3; i++)
			{
				bool flag2 = i < count;
				if (flag2)
				{
					FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
				}
				bool flag3 = i < count2;
				if (flag3)
				{
					FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
				}
			}
			GlyphMarshallingStruct glyphMarshallingStruct;
			bool flag4 = FontEngine.TryAddGlyphToTexture_Internal(glyphIndex, padding, packingMode, FontEngine.s_FreeGlyphRects, ref count, FontEngine.s_UsedGlyphRects, ref count2, renderMode, texture, out glyphMarshallingStruct);
			bool flag7;
			if (flag4)
			{
				glyph = new Glyph(glyphMarshallingStruct);
				freeGlyphRects.Clear();
				usedGlyphRects.Clear();
				num3 = Mathf.Max(count, count2);
				for (int j = 0; j < num3; j++)
				{
					bool flag5 = j < count;
					if (flag5)
					{
						freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
					}
					bool flag6 = j < count2;
					if (flag6)
					{
						usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
					}
				}
				flag7 = true;
			}
			else
			{
				glyph = null;
				flag7 = false;
			}
			return flag7;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryAddGlyphToTexture", IsThreadSafe = true, IsFreeFunction = true)]
		private unsafe static bool TryAddGlyphToTexture_Internal(uint glyphIndex, int padding, GlyphPackingMode packingMode, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, Texture2D texture, out GlyphMarshallingStruct glyph)
		{
			bool flag;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (freeGlyphRects != null)
				{
					fixed (GlyphRect[] array = freeGlyphRects)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper2;
				if (usedGlyphRects != null)
				{
					fixed (GlyphRect[] array2 = usedGlyphRects)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				flag = FontEngine.TryAddGlyphToTexture_Internal_Injected(glyphIndex, padding, packingMode, out blittableArrayWrapper, ref freeGlyphRectCount, out blittableArrayWrapper2, ref usedGlyphRectCount, renderMode, Object.MarshalledUnityObject.Marshal<Texture2D>(texture), out glyph);
			}
			finally
			{
				GlyphRect[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<GlyphRect>(ref array);
				GlyphRect[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<GlyphRect>(ref array2);
			}
			return flag;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static bool TryAddGlyphsToTexture(List<Glyph> glyphsToAdd, List<Glyph> glyphsAdded, int padding, GlyphPackingMode packingMode, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects, GlyphRenderMode renderMode, Texture2D texture)
		{
			int count = glyphsToAdd.Count;
			int num = 0;
			bool flag = FontEngine.s_GlyphMarshallingStruct_IN.Length < count || FontEngine.s_GlyphMarshallingStruct_OUT.Length < count;
			if (flag)
			{
				int num2 = Mathf.NextPowerOfTwo(count + 1);
				bool flag2 = FontEngine.s_GlyphMarshallingStruct_IN.Length < count;
				if (flag2)
				{
					Array.Resize<GlyphMarshallingStruct>(ref FontEngine.s_GlyphMarshallingStruct_IN, num2);
				}
				bool flag3 = FontEngine.s_GlyphMarshallingStruct_OUT.Length < count;
				if (flag3)
				{
					Array.Resize<GlyphMarshallingStruct>(ref FontEngine.s_GlyphMarshallingStruct_OUT, num2);
				}
			}
			int count2 = freeGlyphRects.Count;
			int count3 = usedGlyphRects.Count;
			int num3 = count2 + count3 + count;
			bool flag4 = FontEngine.s_FreeGlyphRects.Length < num3 || FontEngine.s_UsedGlyphRects.Length < num3;
			if (flag4)
			{
				int num4 = Mathf.NextPowerOfTwo(num3 + 1);
				bool flag5 = FontEngine.s_FreeGlyphRects.Length < num3;
				if (flag5)
				{
					Array.Resize<GlyphRect>(ref FontEngine.s_FreeGlyphRects, num4);
				}
				bool flag6 = FontEngine.s_UsedGlyphRects.Length < num3;
				if (flag6)
				{
					Array.Resize<GlyphRect>(ref FontEngine.s_UsedGlyphRects, num4);
				}
			}
			FontEngine.s_GlyphLookupDictionary.Clear();
			int num5 = 0;
			bool flag7 = true;
			while (flag7)
			{
				flag7 = false;
				bool flag8 = num5 < count;
				if (flag8)
				{
					Glyph glyph = glyphsToAdd[num5];
					FontEngine.s_GlyphMarshallingStruct_IN[num5] = new GlyphMarshallingStruct(glyph);
					FontEngine.s_GlyphLookupDictionary.Add(glyph.index, glyph);
					flag7 = true;
				}
				bool flag9 = num5 < count2;
				if (flag9)
				{
					FontEngine.s_FreeGlyphRects[num5] = freeGlyphRects[num5];
					flag7 = true;
				}
				bool flag10 = num5 < count3;
				if (flag10)
				{
					FontEngine.s_UsedGlyphRects[num5] = usedGlyphRects[num5];
					flag7 = true;
				}
				num5++;
			}
			bool flag11 = FontEngine.TryAddGlyphsToTexture_Internal_MultiThread(FontEngine.s_GlyphMarshallingStruct_IN, ref count, FontEngine.s_GlyphMarshallingStruct_OUT, ref num, padding, packingMode, FontEngine.s_FreeGlyphRects, ref count2, FontEngine.s_UsedGlyphRects, ref count3, renderMode, texture);
			glyphsToAdd.Clear();
			glyphsAdded.Clear();
			freeGlyphRects.Clear();
			usedGlyphRects.Clear();
			num5 = 0;
			flag7 = true;
			while (flag7)
			{
				flag7 = false;
				bool flag12 = num5 < count;
				if (flag12)
				{
					uint index = FontEngine.s_GlyphMarshallingStruct_IN[num5].index;
					glyphsToAdd.Add(FontEngine.s_GlyphLookupDictionary[index]);
					flag7 = true;
				}
				bool flag13 = num5 < num;
				if (flag13)
				{
					uint index2 = FontEngine.s_GlyphMarshallingStruct_OUT[num5].index;
					Glyph glyph2 = FontEngine.s_GlyphLookupDictionary[index2];
					glyph2.atlasIndex = FontEngine.s_GlyphMarshallingStruct_OUT[num5].atlasIndex;
					glyph2.scale = FontEngine.s_GlyphMarshallingStruct_OUT[num5].scale;
					glyph2.glyphRect = FontEngine.s_GlyphMarshallingStruct_OUT[num5].glyphRect;
					glyph2.metrics = FontEngine.s_GlyphMarshallingStruct_OUT[num5].metrics;
					glyphsAdded.Add(glyph2);
					flag7 = true;
				}
				bool flag14 = num5 < count2;
				if (flag14)
				{
					freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[num5]);
					flag7 = true;
				}
				bool flag15 = num5 < count3;
				if (flag15)
				{
					usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[num5]);
					flag7 = true;
				}
				num5++;
			}
			return flag11;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryAddGlyphsToTexture", IsThreadSafe = true, IsFreeFunction = true)]
		private unsafe static bool TryAddGlyphsToTexture_Internal_MultiThread([Out] GlyphMarshallingStruct[] glyphsToAdd, ref int glyphsToAddCount, [Out] GlyphMarshallingStruct[] glyphsAdded, ref int glyphsAddedCount, int padding, GlyphPackingMode packingMode, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, Texture2D texture)
		{
			bool flag;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (glyphsToAdd != null)
				{
					fixed (GlyphMarshallingStruct[] array = glyphsToAdd)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper2;
				if (glyphsAdded != null)
				{
					fixed (GlyphMarshallingStruct[] array2 = glyphsAdded)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper3;
				if (freeGlyphRects != null)
				{
					fixed (GlyphRect[] array3 = freeGlyphRects)
					{
						if (array3.Length != 0)
						{
							blittableArrayWrapper3 = new BlittableArrayWrapper((void*)(&array3[0]), array3.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper4;
				if (usedGlyphRects != null)
				{
					fixed (GlyphRect[] array4 = usedGlyphRects)
					{
						if (array4.Length != 0)
						{
							blittableArrayWrapper4 = new BlittableArrayWrapper((void*)(&array4[0]), array4.Length);
						}
					}
				}
				flag = FontEngine.TryAddGlyphsToTexture_Internal_MultiThread_Injected(out blittableArrayWrapper, ref glyphsToAddCount, out blittableArrayWrapper2, ref glyphsAddedCount, padding, packingMode, out blittableArrayWrapper3, ref freeGlyphRectCount, out blittableArrayWrapper4, ref usedGlyphRectCount, renderMode, Object.MarshalledUnityObject.Marshal<Texture2D>(texture));
			}
			finally
			{
				GlyphMarshallingStruct[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<GlyphMarshallingStruct>(ref array);
				GlyphMarshallingStruct[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<GlyphMarshallingStruct>(ref array2);
				GlyphRect[] array3;
				BlittableArrayWrapper blittableArrayWrapper3;
				blittableArrayWrapper3.Unmarshal<GlyphRect>(ref array3);
				GlyphRect[] array4;
				BlittableArrayWrapper blittableArrayWrapper4;
				blittableArrayWrapper4.Unmarshal<GlyphRect>(ref array4);
			}
			return flag;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static bool TryAddGlyphsToTexture(List<uint> glyphIndexes, int padding, GlyphPackingMode packingMode, List<GlyphRect> freeGlyphRects, List<GlyphRect> usedGlyphRects, GlyphRenderMode renderMode, Texture2D texture, out Glyph[] glyphs)
		{
			glyphs = null;
			bool flag = glyphIndexes == null || glyphIndexes.Count == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int count = glyphIndexes.Count;
				bool flag3 = FontEngine.s_GlyphIndexes_MarshallingArray_A == null || FontEngine.s_GlyphIndexes_MarshallingArray_A.Length < count;
				if (flag3)
				{
					FontEngine.s_GlyphIndexes_MarshallingArray_A = new uint[Mathf.NextPowerOfTwo(count + 1)];
				}
				int count2 = freeGlyphRects.Count;
				int count3 = usedGlyphRects.Count;
				int num = count2 + count3 + count;
				bool flag4 = FontEngine.s_FreeGlyphRects.Length < num || FontEngine.s_UsedGlyphRects.Length < num;
				if (flag4)
				{
					int num2 = Mathf.NextPowerOfTwo(num + 1);
					FontEngine.s_FreeGlyphRects = new GlyphRect[num2];
					FontEngine.s_UsedGlyphRects = new GlyphRect[num2];
				}
				bool flag5 = FontEngine.s_GlyphMarshallingStruct_OUT.Length < count;
				if (flag5)
				{
					int num3 = Mathf.NextPowerOfTwo(count + 1);
					FontEngine.s_GlyphMarshallingStruct_OUT = new GlyphMarshallingStruct[num3];
				}
				int num4 = FontEngineUtilities.MaxValue(count2, count3, count);
				for (int i = 0; i < num4; i++)
				{
					bool flag6 = i < count;
					if (flag6)
					{
						FontEngine.s_GlyphIndexes_MarshallingArray_A[i] = glyphIndexes[i];
					}
					bool flag7 = i < count2;
					if (flag7)
					{
						FontEngine.s_FreeGlyphRects[i] = freeGlyphRects[i];
					}
					bool flag8 = i < count3;
					if (flag8)
					{
						FontEngine.s_UsedGlyphRects[i] = usedGlyphRects[i];
					}
				}
				bool flag9 = FontEngine.TryAddGlyphsToTexture_Internal(FontEngine.s_GlyphIndexes_MarshallingArray_A, padding, packingMode, FontEngine.s_FreeGlyphRects, ref count2, FontEngine.s_UsedGlyphRects, ref count3, renderMode, texture, FontEngine.s_GlyphMarshallingStruct_OUT, ref count);
				bool flag10 = FontEngine.s_Glyphs == null || FontEngine.s_Glyphs.Length <= count;
				if (flag10)
				{
					FontEngine.s_Glyphs = new Glyph[Mathf.NextPowerOfTwo(count + 1)];
				}
				FontEngine.s_Glyphs[count] = null;
				freeGlyphRects.Clear();
				usedGlyphRects.Clear();
				num4 = FontEngineUtilities.MaxValue(count2, count3, count);
				for (int j = 0; j < num4; j++)
				{
					bool flag11 = j < count;
					if (flag11)
					{
						FontEngine.s_Glyphs[j] = new Glyph(FontEngine.s_GlyphMarshallingStruct_OUT[j]);
					}
					bool flag12 = j < count2;
					if (flag12)
					{
						freeGlyphRects.Add(FontEngine.s_FreeGlyphRects[j]);
					}
					bool flag13 = j < count3;
					if (flag13)
					{
						usedGlyphRects.Add(FontEngine.s_UsedGlyphRects[j]);
					}
				}
				glyphs = FontEngine.s_Glyphs;
				flag2 = flag9;
			}
			return flag2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::TryAddGlyphsToTexture", IsThreadSafe = true, IsFreeFunction = true)]
		private unsafe static bool TryAddGlyphsToTexture_Internal(uint[] glyphIndex, int padding, GlyphPackingMode packingMode, [Out] GlyphRect[] freeGlyphRects, ref int freeGlyphRectCount, [Out] GlyphRect[] usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, Texture2D texture, [Out] GlyphMarshallingStruct[] glyphs, ref int glyphCount)
		{
			bool flag;
			try
			{
				Span<uint> span = new Span<uint>(glyphIndex);
				fixed (uint* ptr = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
					BlittableArrayWrapper blittableArrayWrapper;
					if (freeGlyphRects != null)
					{
						fixed (GlyphRect[] array = freeGlyphRects)
						{
							if (array.Length != 0)
							{
								blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
							}
						}
					}
					BlittableArrayWrapper blittableArrayWrapper2;
					if (usedGlyphRects != null)
					{
						fixed (GlyphRect[] array2 = usedGlyphRects)
						{
							if (array2.Length != 0)
							{
								blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
							}
						}
					}
					IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Texture2D>(texture);
					BlittableArrayWrapper blittableArrayWrapper3;
					if (glyphs != null)
					{
						fixed (GlyphMarshallingStruct[] array3 = glyphs)
						{
							if (array3.Length != 0)
							{
								blittableArrayWrapper3 = new BlittableArrayWrapper((void*)(&array3[0]), array3.Length);
							}
						}
					}
					flag = FontEngine.TryAddGlyphsToTexture_Internal_Injected(ref managedSpanWrapper, padding, packingMode, out blittableArrayWrapper, ref freeGlyphRectCount, out blittableArrayWrapper2, ref usedGlyphRectCount, renderMode, intPtr, out blittableArrayWrapper3, ref glyphCount);
				}
			}
			finally
			{
				uint* ptr = null;
				GlyphRect[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<GlyphRect>(ref array);
				GlyphRect[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<GlyphRect>(ref array2);
				GlyphMarshallingStruct[] array3;
				BlittableArrayWrapper blittableArrayWrapper3;
				blittableArrayWrapper3.Unmarshal<GlyphMarshallingStruct>(ref array3);
			}
			return flag;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetOpenTypeLayoutTable", IsFreeFunction = true)]
		internal static OTL_Table GetOpenTypeLayoutTable(OTL_TableType type)
		{
			OTL_Table otl_Table;
			FontEngine.GetOpenTypeLayoutTable_Injected(type, out otl_Table);
			return otl_Table;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetOpenTypeLayoutScripts", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern OTL_Script[] GetOpenTypeLayoutScripts();

		[NativeMethod(Name = "TextCore::FontEngine::GetOpenTypeLayoutFeatures", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern OTL_Feature[] GetOpenTypeLayoutFeatures();

		[NativeMethod(Name = "TextCore::FontEngine::GetOpenTypeLayoutLookups", IsFreeFunction = true)]
		internal static OTL_Lookup[] GetOpenTypeLayoutLookups()
		{
			OTL_Lookup[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetOpenTypeLayoutLookups_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				OTL_Lookup[] array;
				blittableArrayWrapper.Unmarshal<OTL_Lookup>(ref array);
				array2 = array;
			}
			return array2;
		}

		internal static OpenTypeFeature[] GetOpenTypeFontFeatureList()
		{
			throw new NotImplementedException();
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetAllSingleSubstitutionRecords", IsThreadSafe = true, IsFreeFunction = true)]
		internal static SingleSubstitutionRecord[] GetAllSingleSubstitutionRecords()
		{
			SingleSubstitutionRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetAllSingleSubstitutionRecords_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				SingleSubstitutionRecord[] array;
				blittableArrayWrapper.Unmarshal<SingleSubstitutionRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		internal static SingleSubstitutionRecord[] GetSingleSubstitutionRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetSingleSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static SingleSubstitutionRecord[] GetSingleSubstitutionRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetSingleSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static SingleSubstitutionRecord[] GetSingleSubstitutionRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateSingleSubstitutionRecordMarshallingArray_from_GlyphIndexes(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			SingleSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<SingleSubstitutionRecord>(ref FontEngine.s_SingleSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetSingleSubstitutionRecordsFromMarshallingArray(FontEngine.s_SingleSubstitutionRecords_MarshallingArray.AsSpan<SingleSubstitutionRecord>());
				FontEngine.s_SingleSubstitutionRecords_MarshallingArray[num] = default(SingleSubstitutionRecord);
				array = FontEngine.s_SingleSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateSingleSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateSingleSubstitutionRecordMarshallingArray_from_GlyphIndexes(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateSingleSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetSingleSubstitutionRecordsFromMarshallingArray", IsFreeFunction = true)]
		private unsafe static int GetSingleSubstitutionRecordsFromMarshallingArray(Span<SingleSubstitutionRecord> singleSubstitutionRecords)
		{
			Span<SingleSubstitutionRecord> span = singleSubstitutionRecords;
			int singleSubstitutionRecordsFromMarshallingArray_Injected;
			fixed (SingleSubstitutionRecord* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				singleSubstitutionRecordsFromMarshallingArray_Injected = FontEngine.GetSingleSubstitutionRecordsFromMarshallingArray_Injected(ref managedSpanWrapper);
			}
			return singleSubstitutionRecordsFromMarshallingArray_Injected;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetAllMultipleSubstitutionRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MultipleSubstitutionRecord[] GetAllMultipleSubstitutionRecords();

		internal static MultipleSubstitutionRecord[] GetMultipleSubstitutionRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMultipleSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static MultipleSubstitutionRecord[] GetMultipleSubstitutionRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMultipleSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static MultipleSubstitutionRecord[] GetMultipleSubstitutionRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMultipleSubstitutionRecordMarshallingArray_from_GlyphIndexes(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			MultipleSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MultipleSubstitutionRecord>(ref FontEngine.s_MultipleSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetMultipleSubstitutionRecordsFromMarshallingArray(FontEngine.s_MultipleSubstitutionRecords_MarshallingArray);
				FontEngine.s_MultipleSubstitutionRecords_MarshallingArray[num] = default(MultipleSubstitutionRecord);
				array = FontEngine.s_MultipleSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMultipleSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMultipleSubstitutionRecordMarshallingArray_from_GlyphIndexes(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMultipleSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMultipleSubstitutionRecordsFromMarshallingArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMultipleSubstitutionRecordsFromMarshallingArray([Out] MultipleSubstitutionRecord[] substitutionRecords);

		[NativeMethod(Name = "TextCore::FontEngine::GetAllAlternateSubstitutionRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AlternateSubstitutionRecord[] GetAllAlternateSubstitutionRecords();

		internal static AlternateSubstitutionRecord[] GetAlternateSubstitutionRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetAlternateSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static AlternateSubstitutionRecord[] GetAlternateSubstitutionRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetAlternateSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static AlternateSubstitutionRecord[] GetAlternateSubstitutionRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateAlternateSubstitutionRecordMarshallingArray_from_GlyphIndexes(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			AlternateSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<AlternateSubstitutionRecord>(ref FontEngine.s_AlternateSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetAlternateSubstitutionRecordsFromMarshallingArray(FontEngine.s_AlternateSubstitutionRecords_MarshallingArray);
				FontEngine.s_AlternateSubstitutionRecords_MarshallingArray[num] = default(AlternateSubstitutionRecord);
				array = FontEngine.s_AlternateSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateAlternateSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateAlternateSubstitutionRecordMarshallingArray_from_GlyphIndexes(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateAlternateSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetAlternateSubstitutionRecordsFromMarshallingArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAlternateSubstitutionRecordsFromMarshallingArray([Out] AlternateSubstitutionRecord[] singleSubstitutionRecords);

		[NativeMethod(Name = "TextCore::FontEngine::GetAllLigatureSubstitutionRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern LigatureSubstitutionRecord[] GetAllLigatureSubstitutionRecords();

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static LigatureSubstitutionRecord[] GetLigatureSubstitutionRecords(uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetLigatureSubstitutionRecords(FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static LigatureSubstitutionRecord[] GetLigatureSubstitutionRecords(List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetLigatureSubstitutionRecords(FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static LigatureSubstitutionRecord[] GetLigatureSubstitutionRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetLigatureSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static LigatureSubstitutionRecord[] GetLigatureSubstitutionRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetLigatureSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static LigatureSubstitutionRecord[] GetLigatureSubstitutionRecords(uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateLigatureSubstitutionRecordMarshallingArray(glyphIndexes, out num);
			bool flag = num == 0;
			LigatureSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<LigatureSubstitutionRecord>(ref FontEngine.s_LigatureSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetLigatureSubstitutionRecordsFromMarshallingArray(FontEngine.s_LigatureSubstitutionRecords_MarshallingArray);
				FontEngine.s_LigatureSubstitutionRecords_MarshallingArray[num] = default(LigatureSubstitutionRecord);
				array = FontEngine.s_LigatureSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		private static LigatureSubstitutionRecord[] GetLigatureSubstitutionRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateLigatureSubstitutionRecordMarshallingArray_for_LookupIndex(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			LigatureSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<LigatureSubstitutionRecord>(ref FontEngine.s_LigatureSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetLigatureSubstitutionRecordsFromMarshallingArray(FontEngine.s_LigatureSubstitutionRecords_MarshallingArray);
				FontEngine.s_LigatureSubstitutionRecords_MarshallingArray[num] = default(LigatureSubstitutionRecord);
				array = FontEngine.s_LigatureSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateLigatureSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateLigatureSubstitutionRecordMarshallingArray(uint[] glyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateLigatureSubstitutionRecordMarshallingArray_Injected(ref managedSpanWrapper, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateLigatureSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateLigatureSubstitutionRecordMarshallingArray_for_LookupIndex(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateLigatureSubstitutionRecordMarshallingArray_for_LookupIndex_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetLigatureSubstitutionRecordsFromMarshallingArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLigatureSubstitutionRecordsFromMarshallingArray([Out] LigatureSubstitutionRecord[] ligatureSubstitutionRecords);

		[NativeMethod(Name = "TextCore::FontEngine::GetAllContextualSubstitutionRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ContextualSubstitutionRecord[] GetAllContextualSubstitutionRecords();

		internal static ContextualSubstitutionRecord[] GetContextualSubstitutionRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetContextualSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static ContextualSubstitutionRecord[] GetContextualSubstitutionRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetContextualSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static ContextualSubstitutionRecord[] GetContextualSubstitutionRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			ContextualSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<ContextualSubstitutionRecord>(ref FontEngine.s_ContextualSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetContextualSubstitutionRecordsFromMarshallingArray(FontEngine.s_ContextualSubstitutionRecords_MarshallingArray);
				FontEngine.s_ContextualSubstitutionRecords_MarshallingArray[num] = default(ContextualSubstitutionRecord);
				array = FontEngine.s_ContextualSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateContextualSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetContextualSubstitutionRecordsFromMarshallingArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetContextualSubstitutionRecordsFromMarshallingArray([Out] ContextualSubstitutionRecord[] substitutionRecords);

		[NativeMethod(Name = "TextCore::FontEngine::GetAllChainingContextualSubstitutionRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ChainingContextualSubstitutionRecord[] GetAllChainingContextualSubstitutionRecords();

		internal static ChainingContextualSubstitutionRecord[] GetChainingContextualSubstitutionRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetChainingContextualSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static ChainingContextualSubstitutionRecord[] GetChainingContextualSubstitutionRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetChainingContextualSubstitutionRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static ChainingContextualSubstitutionRecord[] GetChainingContextualSubstitutionRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateChainingContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			ChainingContextualSubstitutionRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<ChainingContextualSubstitutionRecord>(ref FontEngine.s_ChainingContextualSubstitutionRecords_MarshallingArray, num);
				FontEngine.GetChainingContextualSubstitutionRecordsFromMarshallingArray(FontEngine.s_ChainingContextualSubstitutionRecords_MarshallingArray);
				FontEngine.s_ChainingContextualSubstitutionRecords_MarshallingArray[num] = default(ChainingContextualSubstitutionRecord);
				array = FontEngine.s_ChainingContextualSubstitutionRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateChainingContextualSubstitutionRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateChainingContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateChainingContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetChainingContextualSubstitutionRecordsFromMarshallingArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChainingContextualSubstitutionRecordsFromMarshallingArray([Out] ChainingContextualSubstitutionRecord[] substitutionRecords);

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static GlyphPairAdjustmentRecord[] GetGlyphPairAdjustmentTable(uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulatePairAdjustmentRecordMarshallingArray_from_KernTable(glyphIndexes, out num);
			bool flag = num == 0;
			GlyphPairAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphPairAdjustmentRecord>(ref FontEngine.s_PairAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetPairAdjustmentRecordsFromMarshallingArray(FontEngine.s_PairAdjustmentRecords_MarshallingArray);
				FontEngine.s_PairAdjustmentRecords_MarshallingArray[num] = default(GlyphPairAdjustmentRecord);
				array = FontEngine.s_PairAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		internal static GlyphPairAdjustmentRecord[] GetGlyphPairAdjustmentRecords(List<uint> glyphIndexes, out int recordCount)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			FontEngine.PopulatePairAdjustmentRecordMarshallingArray_from_KernTable(FontEngine.s_GlyphIndexes_MarshallingArray_A, out recordCount);
			bool flag = recordCount == 0;
			GlyphPairAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphPairAdjustmentRecord>(ref FontEngine.s_PairAdjustmentRecords_MarshallingArray, recordCount);
				FontEngine.GetPairAdjustmentRecordsFromMarshallingArray(FontEngine.s_PairAdjustmentRecords_MarshallingArray);
				FontEngine.s_PairAdjustmentRecords_MarshallingArray[recordCount] = default(GlyphPairAdjustmentRecord);
				array = FontEngine.s_PairAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulatePairAdjustmentRecordMarshallingArrayFromKernTable", IsFreeFunction = true)]
		private unsafe static int PopulatePairAdjustmentRecordMarshallingArray_from_KernTable(uint[] glyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulatePairAdjustmentRecordMarshallingArray_from_KernTable_Injected(ref managedSpanWrapper, out recordCount);
			}
			return num;
		}

		internal static GlyphPairAdjustmentRecord[] GetGlyphPairAdjustmentRecords(uint glyphIndex, out int recordCount)
		{
			FontEngine.PopulatePairAdjustmentRecordMarshallingArray_from_GlyphIndex(glyphIndex, out recordCount);
			bool flag = recordCount == 0;
			GlyphPairAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphPairAdjustmentRecord>(ref FontEngine.s_PairAdjustmentRecords_MarshallingArray, recordCount);
				FontEngine.GetPairAdjustmentRecordsFromMarshallingArray(FontEngine.s_PairAdjustmentRecords_MarshallingArray);
				FontEngine.s_PairAdjustmentRecords_MarshallingArray[recordCount] = default(GlyphPairAdjustmentRecord);
				array = FontEngine.s_PairAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulatePairAdjustmentRecordMarshallingArrayFromKernTable", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulatePairAdjustmentRecordMarshallingArray_from_GlyphIndex(uint glyphIndex, out int recordCount);

		internal static GlyphPairAdjustmentRecord[] GetGlyphPairAdjustmentRecords(List<uint> newGlyphIndexes, List<uint> allGlyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref newGlyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			FontEngine.GenericListToMarshallingArray<uint>(ref allGlyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_B);
			int num;
			FontEngine.PopulatePairAdjustmentRecordMarshallingArray_for_NewlyAddedGlyphIndexes(FontEngine.s_GlyphIndexes_MarshallingArray_A, FontEngine.s_GlyphIndexes_MarshallingArray_B, out num);
			bool flag = num == 0;
			GlyphPairAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphPairAdjustmentRecord>(ref FontEngine.s_PairAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetPairAdjustmentRecordsFromMarshallingArray(FontEngine.s_PairAdjustmentRecords_MarshallingArray);
				FontEngine.s_PairAdjustmentRecords_MarshallingArray[num] = default(GlyphPairAdjustmentRecord);
				array = FontEngine.s_PairAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulatePairAdjustmentRecordMarshallingArrayFromKernTable", IsFreeFunction = true)]
		private unsafe static int PopulatePairAdjustmentRecordMarshallingArray_for_NewlyAddedGlyphIndexes(uint[] newGlyphIndexes, uint[] allGlyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(newGlyphIndexes);
			fixed (uint* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<uint> span2 = new Span<uint>(allGlyphIndexes);
				int num;
				fixed (uint* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					num = FontEngine.PopulatePairAdjustmentRecordMarshallingArray_for_NewlyAddedGlyphIndexes_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, out recordCount);
					ptr = null;
				}
				return num;
			}
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetGlyphPairAdjustmentRecord", IsFreeFunction = true)]
		internal static GlyphPairAdjustmentRecord GetGlyphPairAdjustmentRecord(uint firstGlyphIndex, uint secondGlyphIndex)
		{
			GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
			FontEngine.GetGlyphPairAdjustmentRecord_Injected(firstGlyphIndex, secondGlyphIndex, out glyphPairAdjustmentRecord);
			return glyphPairAdjustmentRecord;
		}

		internal static GlyphAdjustmentRecord[] GetSingleAdjustmentRecords(int lookupIndex, uint glyphIndex)
		{
			bool flag = FontEngine.s_GlyphIndexes_MarshallingArray_A == null;
			if (flag)
			{
				FontEngine.s_GlyphIndexes_MarshallingArray_A = new uint[8];
			}
			FontEngine.s_GlyphIndexes_MarshallingArray_A[0] = glyphIndex;
			FontEngine.s_GlyphIndexes_MarshallingArray_A[1] = 0U;
			return FontEngine.GetSingleAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static GlyphAdjustmentRecord[] GetSingleAdjustmentRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetSingleAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static GlyphAdjustmentRecord[] GetSingleAdjustmentRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateSingleAdjustmentRecordMarshallingArray_from_GlyphIndexes(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			GlyphAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphAdjustmentRecord>(ref FontEngine.s_SingleAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetSingleAdjustmentRecordsFromMarshallingArray(FontEngine.s_SingleAdjustmentRecords_MarshallingArray.AsSpan<GlyphAdjustmentRecord>());
				FontEngine.s_SingleAdjustmentRecords_MarshallingArray[num] = default(GlyphAdjustmentRecord);
				array = FontEngine.s_SingleAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateSingleAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateSingleAdjustmentRecordMarshallingArray_from_GlyphIndexes(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateSingleAdjustmentRecordMarshallingArray_from_GlyphIndexes_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetSingleAdjustmentRecordsFromMarshallingArray", IsFreeFunction = true)]
		private unsafe static int GetSingleAdjustmentRecordsFromMarshallingArray(Span<GlyphAdjustmentRecord> singleSubstitutionRecords)
		{
			Span<GlyphAdjustmentRecord> span = singleSubstitutionRecords;
			int singleAdjustmentRecordsFromMarshallingArray_Injected;
			fixed (GlyphAdjustmentRecord* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				singleAdjustmentRecordsFromMarshallingArray_Injected = FontEngine.GetSingleAdjustmentRecordsFromMarshallingArray_Injected(ref managedSpanWrapper);
			}
			return singleAdjustmentRecordsFromMarshallingArray_Injected;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[NativeMethod(Name = "TextCore::FontEngine::GetPairAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		internal static GlyphPairAdjustmentRecord[] GetPairAdjustmentRecords(uint glyphIndex)
		{
			GlyphPairAdjustmentRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetPairAdjustmentRecords_Injected(glyphIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GlyphPairAdjustmentRecord[] array;
				blittableArrayWrapper.Unmarshal<GlyphPairAdjustmentRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetPairAdjustmentRecord", IsThreadSafe = true, IsFreeFunction = true)]
		internal static GlyphPairAdjustmentRecord GetPairAdjustmentRecord(uint firstGlyphIndex, uint secondGlyphIndex)
		{
			GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
			FontEngine.GetPairAdjustmentRecord_Injected(firstGlyphIndex, secondGlyphIndex, out glyphPairAdjustmentRecord);
			return glyphPairAdjustmentRecord;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetAllPairAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static GlyphPairAdjustmentRecord[] GetAllPairAdjustmentRecords()
		{
			GlyphPairAdjustmentRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetAllPairAdjustmentRecords_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GlyphPairAdjustmentRecord[] array;
				blittableArrayWrapper.Unmarshal<GlyphPairAdjustmentRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static GlyphPairAdjustmentRecord[] GetPairAdjustmentRecords(List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetPairAdjustmentRecords(FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static GlyphPairAdjustmentRecord[] GetPairAdjustmentRecords(int lookupIndex, uint glyphIndex)
		{
			FontEngine.GlyphIndexToMarshallingArray(glyphIndex, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetPairAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static GlyphPairAdjustmentRecord[] GetPairAdjustmentRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetPairAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static GlyphPairAdjustmentRecord[] GetPairAdjustmentRecords(uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulatePairAdjustmentRecordMarshallingArray(glyphIndexes, out num);
			bool flag = num == 0;
			GlyphPairAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphPairAdjustmentRecord>(ref FontEngine.s_PairAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetPairAdjustmentRecordsFromMarshallingArray(FontEngine.s_PairAdjustmentRecords_MarshallingArray);
				FontEngine.s_PairAdjustmentRecords_MarshallingArray[num] = default(GlyphPairAdjustmentRecord);
				array = FontEngine.s_PairAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		private static GlyphPairAdjustmentRecord[] GetPairAdjustmentRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulatePairAdjustmentRecordMarshallingArray_for_LookupIndex(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			GlyphPairAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<GlyphPairAdjustmentRecord>(ref FontEngine.s_PairAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetPairAdjustmentRecordsFromMarshallingArray(FontEngine.s_PairAdjustmentRecords_MarshallingArray);
				FontEngine.s_PairAdjustmentRecords_MarshallingArray[num] = default(GlyphPairAdjustmentRecord);
				array = FontEngine.s_PairAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulatePairAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulatePairAdjustmentRecordMarshallingArray(uint[] glyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulatePairAdjustmentRecordMarshallingArray_Injected(ref managedSpanWrapper, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulatePairAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulatePairAdjustmentRecordMarshallingArray_for_LookupIndex(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulatePairAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetGlyphPairAdjustmentRecordsFromMarshallingArray", IsFreeFunction = true)]
		private unsafe static int GetPairAdjustmentRecordsFromMarshallingArray(Span<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords)
		{
			Span<GlyphPairAdjustmentRecord> span = glyphPairAdjustmentRecords;
			int pairAdjustmentRecordsFromMarshallingArray_Injected;
			fixed (GlyphPairAdjustmentRecord* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				pairAdjustmentRecordsFromMarshallingArray_Injected = FontEngine.GetPairAdjustmentRecordsFromMarshallingArray_Injected(ref managedSpanWrapper);
			}
			return pairAdjustmentRecordsFromMarshallingArray_Injected;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetAllMarkToBaseAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static MarkToBaseAdjustmentRecord[] GetAllMarkToBaseAdjustmentRecords()
		{
			MarkToBaseAdjustmentRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetAllMarkToBaseAdjustmentRecords_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				MarkToBaseAdjustmentRecord[] array;
				blittableArrayWrapper.Unmarshal<MarkToBaseAdjustmentRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToBaseAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		internal static MarkToBaseAdjustmentRecord[] GetMarkToBaseAdjustmentRecords(uint baseGlyphIndex)
		{
			MarkToBaseAdjustmentRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetMarkToBaseAdjustmentRecords_Injected(baseGlyphIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				MarkToBaseAdjustmentRecord[] array;
				blittableArrayWrapper.Unmarshal<MarkToBaseAdjustmentRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToBaseAdjustmentRecord", IsFreeFunction = true)]
		internal static MarkToBaseAdjustmentRecord GetMarkToBaseAdjustmentRecord(uint baseGlyphIndex, uint markGlyphIndex)
		{
			MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
			FontEngine.GetMarkToBaseAdjustmentRecord_Injected(baseGlyphIndex, markGlyphIndex, out markToBaseAdjustmentRecord);
			return markToBaseAdjustmentRecord;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static MarkToBaseAdjustmentRecord[] GetMarkToBaseAdjustmentRecords(List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMarkToBaseAdjustmentRecords(FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static MarkToBaseAdjustmentRecord[] GetMarkToBaseAdjustmentRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMarkToBaseAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static MarkToBaseAdjustmentRecord[] GetMarkToBaseAdjustmentRecords(uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMarkToBaseAdjustmentRecordMarshallingArray(glyphIndexes, out num);
			bool flag = num == 0;
			MarkToBaseAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MarkToBaseAdjustmentRecord>(ref FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetMarkToBaseAdjustmentRecordsFromMarshallingArray(FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray);
				FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray[num] = default(MarkToBaseAdjustmentRecord);
				array = FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		private static MarkToBaseAdjustmentRecord[] GetMarkToBaseAdjustmentRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMarkToBaseAdjustmentRecordMarshallingArray_for_LookupIndex(glyphIndexes, lookupIndex, out num);
			bool flag = num == 0;
			MarkToBaseAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MarkToBaseAdjustmentRecord>(ref FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetMarkToBaseAdjustmentRecordsFromMarshallingArray(FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray);
				FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray[num] = default(MarkToBaseAdjustmentRecord);
				array = FontEngine.s_MarkToBaseAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMarkToBaseAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMarkToBaseAdjustmentRecordMarshallingArray(uint[] glyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMarkToBaseAdjustmentRecordMarshallingArray_Injected(ref managedSpanWrapper, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMarkToBaseAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMarkToBaseAdjustmentRecordMarshallingArray_for_LookupIndex(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMarkToBaseAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToBaseAdjustmentRecordsFromMarshallingArray", IsFreeFunction = true)]
		private unsafe static int GetMarkToBaseAdjustmentRecordsFromMarshallingArray(Span<MarkToBaseAdjustmentRecord> adjustmentRecords)
		{
			Span<MarkToBaseAdjustmentRecord> span = adjustmentRecords;
			int markToBaseAdjustmentRecordsFromMarshallingArray_Injected;
			fixed (MarkToBaseAdjustmentRecord* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				markToBaseAdjustmentRecordsFromMarshallingArray_Injected = FontEngine.GetMarkToBaseAdjustmentRecordsFromMarshallingArray_Injected(ref managedSpanWrapper);
			}
			return markToBaseAdjustmentRecordsFromMarshallingArray_Injected;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[NativeMethod(Name = "TextCore::FontEngine::GetAllMarkToMarkAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		internal static MarkToMarkAdjustmentRecord[] GetAllMarkToMarkAdjustmentRecords()
		{
			MarkToMarkAdjustmentRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetAllMarkToMarkAdjustmentRecords_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				MarkToMarkAdjustmentRecord[] array;
				blittableArrayWrapper.Unmarshal<MarkToMarkAdjustmentRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToMarkAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		internal static MarkToMarkAdjustmentRecord[] GetMarkToMarkAdjustmentRecords(uint baseMarkGlyphIndex)
		{
			MarkToMarkAdjustmentRecord[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				FontEngine.GetMarkToMarkAdjustmentRecords_Injected(baseMarkGlyphIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				MarkToMarkAdjustmentRecord[] array;
				blittableArrayWrapper.Unmarshal<MarkToMarkAdjustmentRecord>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToMarkAdjustmentRecord", IsFreeFunction = true)]
		internal static MarkToMarkAdjustmentRecord GetMarkToMarkAdjustmentRecord(uint firstGlyphIndex, uint secondGlyphIndex)
		{
			MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
			FontEngine.GetMarkToMarkAdjustmentRecord_Injected(firstGlyphIndex, secondGlyphIndex, out markToMarkAdjustmentRecord);
			return markToMarkAdjustmentRecord;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal static MarkToMarkAdjustmentRecord[] GetMarkToMarkAdjustmentRecords(List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMarkToMarkAdjustmentRecords(FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static MarkToMarkAdjustmentRecord[] GetMarkToMarkAdjustmentRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMarkToMarkAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static MarkToMarkAdjustmentRecord[] GetMarkToMarkAdjustmentRecords(uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMarkToMarkAdjustmentRecordMarshallingArray(FontEngine.s_GlyphIndexes_MarshallingArray_A, out num);
			bool flag = num == 0;
			MarkToMarkAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MarkToMarkAdjustmentRecord>(ref FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetMarkToMarkAdjustmentRecordsFromMarshallingArray(FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray);
				FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray[num] = default(MarkToMarkAdjustmentRecord);
				array = FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		private static MarkToMarkAdjustmentRecord[] GetMarkToMarkAdjustmentRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMarkToMarkAdjustmentRecordMarshallingArray_for_LookupIndex(FontEngine.s_GlyphIndexes_MarshallingArray_A, lookupIndex, out num);
			bool flag = num == 0;
			MarkToMarkAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MarkToMarkAdjustmentRecord>(ref FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetMarkToMarkAdjustmentRecordsFromMarshallingArray(FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray);
				FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray[num] = default(MarkToMarkAdjustmentRecord);
				array = FontEngine.s_MarkToMarkAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMarkToMarkAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMarkToMarkAdjustmentRecordMarshallingArray(uint[] glyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMarkToMarkAdjustmentRecordMarshallingArray_Injected(ref managedSpanWrapper, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMarkToMarkAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMarkToMarkAdjustmentRecordMarshallingArray_for_LookupIndex(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMarkToMarkAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToMarkAdjustmentRecordsFromMarshallingArray", IsFreeFunction = true)]
		private unsafe static int GetMarkToMarkAdjustmentRecordsFromMarshallingArray(Span<MarkToMarkAdjustmentRecord> adjustmentRecords)
		{
			Span<MarkToMarkAdjustmentRecord> span = adjustmentRecords;
			int markToMarkAdjustmentRecordsFromMarshallingArray_Injected;
			fixed (MarkToMarkAdjustmentRecord* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				markToMarkAdjustmentRecordsFromMarshallingArray_Injected = FontEngine.GetMarkToMarkAdjustmentRecordsFromMarshallingArray_Injected(ref managedSpanWrapper);
			}
			return markToMarkAdjustmentRecordsFromMarshallingArray_Injected;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetAllMarkToLigatureAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MarkToLigatureAdjustmentRecord[] GetAllMarkToLigatureAdjustmentRecords();

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToLigatureAdjustmentRecords", IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MarkToLigatureAdjustmentRecord[] GetMarkToLigatureAdjustmentRecords(uint baseMarkGlyphIndex);

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToLigatureAdjustmentRecord", IsFreeFunction = true)]
		internal static MarkToLigatureAdjustmentRecord GetMarkToLigatureAdjustmentRecord(uint firstGlyphIndex, uint secondGlyphIndex)
		{
			MarkToLigatureAdjustmentRecord markToLigatureAdjustmentRecord;
			FontEngine.GetMarkToLigatureAdjustmentRecord_Injected(firstGlyphIndex, secondGlyphIndex, out markToLigatureAdjustmentRecord);
			return markToLigatureAdjustmentRecord;
		}

		internal static MarkToLigatureAdjustmentRecord[] GetMarkToLigatureAdjustmentRecords(List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMarkToLigatureAdjustmentRecords(FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		internal static MarkToLigatureAdjustmentRecord[] GetMarkToLigatureAdjustmentRecords(int lookupIndex, List<uint> glyphIndexes)
		{
			FontEngine.GenericListToMarshallingArray<uint>(ref glyphIndexes, ref FontEngine.s_GlyphIndexes_MarshallingArray_A);
			return FontEngine.GetMarkToLigatureAdjustmentRecords(lookupIndex, FontEngine.s_GlyphIndexes_MarshallingArray_A);
		}

		private static MarkToLigatureAdjustmentRecord[] GetMarkToLigatureAdjustmentRecords(uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMarkToLigatureAdjustmentRecordMarshallingArray(FontEngine.s_GlyphIndexes_MarshallingArray_A, out num);
			bool flag = num == 0;
			MarkToLigatureAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MarkToLigatureAdjustmentRecord>(ref FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetMarkToLigatureAdjustmentRecordsFromMarshallingArray(FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray);
				FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray[num] = default(MarkToLigatureAdjustmentRecord);
				array = FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		private static MarkToLigatureAdjustmentRecord[] GetMarkToLigatureAdjustmentRecords(int lookupIndex, uint[] glyphIndexes)
		{
			int num;
			FontEngine.PopulateMarkToLigatureAdjustmentRecordMarshallingArray_for_LookupIndex(FontEngine.s_GlyphIndexes_MarshallingArray_A, lookupIndex, out num);
			bool flag = num == 0;
			MarkToLigatureAdjustmentRecord[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				FontEngine.SetMarshallingArraySize<MarkToLigatureAdjustmentRecord>(ref FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray, num);
				FontEngine.GetMarkToLigatureAdjustmentRecordsFromMarshallingArray(FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray);
				FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray[num] = default(MarkToLigatureAdjustmentRecord);
				array = FontEngine.s_MarkToLigatureAdjustmentRecords_MarshallingArray;
			}
			return array;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMarkToLigatureAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMarkToLigatureAdjustmentRecordMarshallingArray(uint[] glyphIndexes, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMarkToLigatureAdjustmentRecordMarshallingArray_Injected(ref managedSpanWrapper, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::PopulateMarkToLigatureAdjustmentRecordMarshallingArray", IsFreeFunction = true)]
		private unsafe static int PopulateMarkToLigatureAdjustmentRecordMarshallingArray_for_LookupIndex(uint[] glyphIndexes, int lookupIndex, out int recordCount)
		{
			Span<uint> span = new Span<uint>(glyphIndexes);
			int num;
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = FontEngine.PopulateMarkToLigatureAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref managedSpanWrapper, lookupIndex, out recordCount);
			}
			return num;
		}

		[NativeMethod(Name = "TextCore::FontEngine::GetMarkToLigatureAdjustmentRecordsFromMarshallingArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMarkToLigatureAdjustmentRecordsFromMarshallingArray([Out] MarkToLigatureAdjustmentRecord[] adjustmentRecords);

		private static void GlyphIndexToMarshallingArray(uint glyphIndex, ref uint[] dstArray)
		{
			bool flag = dstArray == null || dstArray.Length == 1;
			if (flag)
			{
				dstArray = new uint[8];
			}
			dstArray[0] = glyphIndex;
			dstArray[1] = 0U;
		}

		private static void GenericListToMarshallingArray<T>(ref List<T> srcList, ref T[] dstArray)
		{
			int count = srcList.Count;
			bool flag = dstArray == null || dstArray.Length <= count;
			if (flag)
			{
				int num = Mathf.NextPowerOfTwo(count + 1);
				bool flag2 = dstArray == null;
				if (flag2)
				{
					dstArray = new T[num];
				}
				else
				{
					Array.Resize<T>(ref dstArray, num);
				}
			}
			for (int i = 0; i < count; i++)
			{
				dstArray[i] = srcList[i];
			}
			dstArray[count] = default(T);
		}

		private static void SetMarshallingArraySize<T>(ref T[] marshallingArray, int recordCount)
		{
			bool flag = marshallingArray == null || marshallingArray.Length <= recordCount;
			if (flag)
			{
				int num = Mathf.NextPowerOfTwo(recordCount + 1);
				bool flag2 = marshallingArray == null;
				if (flag2)
				{
					marshallingArray = new T[num];
				}
				else
				{
					Array.Resize<T>(ref marshallingArray, num);
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[NativeMethod(Name = "TextCore::FontEngine::ResetAtlasTexture", IsFreeFunction = true)]
		internal static void ResetAtlasTexture(Texture2D texture)
		{
			FontEngine.ResetAtlasTexture_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(texture));
		}

		[NativeMethod(Name = "TextCore::FontEngine::RenderToTexture", IsFreeFunction = true)]
		internal static void RenderBufferToTexture(Texture2D srcTexture, int padding, GlyphRenderMode renderMode, Texture2D dstTexture)
		{
			FontEngine.RenderBufferToTexture_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(srcTexture), padding, renderMode, Object.MarshalledUnityObject.Marshal<Texture2D>(dstTexture));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_Internal_Injected(ref ManagedSpanWrapper filePath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_Internal_Injected(ref ManagedSpanWrapper filePath, int pointSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_And_FaceIndex_Internal_Injected(ref ManagedSpanWrapper filePath, int pointSize, int faceIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_FromSourceFontFile_Internal_Injected(ref ManagedSpanWrapper sourceFontFile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_FromSourceFontFile_Internal_Injected(ref ManagedSpanWrapper sourceFontFile, int pointSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_And_FaceIndex_FromSourceFontFile_Internal_Injected(ref ManagedSpanWrapper sourceFontFile, int pointSize, int faceIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_FromFont_Internal_Injected(IntPtr font);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_FromFont_Internal_Injected(IntPtr font, int pointSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_and_FaceIndex_FromFont_Internal_Injected(IntPtr font, int pointSize, int faceIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_by_FamilyName_and_StyleName_Internal_Injected(ref ManagedSpanWrapper familyName, ref ManagedSpanWrapper styleName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LoadFontFace_With_Size_by_FamilyName_and_StyleName_Internal_Injected(ref ManagedSpanWrapper familyName, ref ManagedSpanWrapper styleName, int pointSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetSystemFontReference_Internal_Injected(ref ManagedSpanWrapper familyName, ref ManagedSpanWrapper styleName, out FontReference fontRef);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFontCharacterMap_Internal_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryPackGlyphInAtlas_Internal_Injected(ref GlyphMarshallingStruct glyph, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, out BlittableArrayWrapper freeGlyphRects, ref int freeGlyphRectCount, out BlittableArrayWrapper usedGlyphRects, ref int usedGlyphRectCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryPackGlyphsInAtlas_Internal_Injected(out BlittableArrayWrapper glyphsToAdd, ref int glyphsToAddCount, out BlittableArrayWrapper glyphsAdded, ref int glyphsAddedCount, int padding, GlyphPackingMode packingMode, GlyphRenderMode renderMode, int width, int height, out BlittableArrayWrapper freeGlyphRects, ref int freeGlyphRectCount, out BlittableArrayWrapper usedGlyphRects, ref int usedGlyphRectCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphToTexture_Internal_Injected([In] ref GlyphMarshallingStruct glyphStruct, int padding, GlyphRenderMode renderMode, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphsToTexture_Internal_Injected(ref ManagedSpanWrapper glyphs, int glyphCount, int padding, GlyphRenderMode renderMode, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphsToTextureBuffer_Internal_Injected(ref ManagedSpanWrapper glyphs, int glyphCount, int padding, GlyphRenderMode renderMode, out BlittableArrayWrapper texBuffer, int texWidth, int texHeight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RenderGlyphsToSharedTexture_Internal_Injected(ref ManagedSpanWrapper glyphs, int glyphCount, int padding, GlyphRenderMode renderMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSharedTexture_Injected(IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryAddGlyphToTexture_Internal_Injected(uint glyphIndex, int padding, GlyphPackingMode packingMode, out BlittableArrayWrapper freeGlyphRects, ref int freeGlyphRectCount, out BlittableArrayWrapper usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, IntPtr texture, out GlyphMarshallingStruct glyph);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryAddGlyphsToTexture_Internal_MultiThread_Injected(out BlittableArrayWrapper glyphsToAdd, ref int glyphsToAddCount, out BlittableArrayWrapper glyphsAdded, ref int glyphsAddedCount, int padding, GlyphPackingMode packingMode, out BlittableArrayWrapper freeGlyphRects, ref int freeGlyphRectCount, out BlittableArrayWrapper usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryAddGlyphsToTexture_Internal_Injected(ref ManagedSpanWrapper glyphIndex, int padding, GlyphPackingMode packingMode, out BlittableArrayWrapper freeGlyphRects, ref int freeGlyphRectCount, out BlittableArrayWrapper usedGlyphRects, ref int usedGlyphRectCount, GlyphRenderMode renderMode, IntPtr texture, out BlittableArrayWrapper glyphs, ref int glyphCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOpenTypeLayoutTable_Injected(OTL_TableType type, out OTL_Table ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOpenTypeLayoutLookups_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAllSingleSubstitutionRecords_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateSingleSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSingleSubstitutionRecordsFromMarshallingArray_Injected(ref ManagedSpanWrapper singleSubstitutionRecords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMultipleSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateAlternateSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateLigatureSubstitutionRecordMarshallingArray_Injected(ref ManagedSpanWrapper glyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateLigatureSubstitutionRecordMarshallingArray_for_LookupIndex_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateChainingContextualSubstitutionRecordMarshallingArray_from_GlyphIndexes_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulatePairAdjustmentRecordMarshallingArray_from_KernTable_Injected(ref ManagedSpanWrapper glyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulatePairAdjustmentRecordMarshallingArray_for_NewlyAddedGlyphIndexes_Injected(ref ManagedSpanWrapper newGlyphIndexes, ref ManagedSpanWrapper allGlyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlyphPairAdjustmentRecord_Injected(uint firstGlyphIndex, uint secondGlyphIndex, out GlyphPairAdjustmentRecord ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateSingleAdjustmentRecordMarshallingArray_from_GlyphIndexes_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSingleAdjustmentRecordsFromMarshallingArray_Injected(ref ManagedSpanWrapper singleSubstitutionRecords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPairAdjustmentRecords_Injected(uint glyphIndex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPairAdjustmentRecord_Injected(uint firstGlyphIndex, uint secondGlyphIndex, out GlyphPairAdjustmentRecord ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAllPairAdjustmentRecords_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulatePairAdjustmentRecordMarshallingArray_Injected(ref ManagedSpanWrapper glyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulatePairAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPairAdjustmentRecordsFromMarshallingArray_Injected(ref ManagedSpanWrapper glyphPairAdjustmentRecords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAllMarkToBaseAdjustmentRecords_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMarkToBaseAdjustmentRecords_Injected(uint baseGlyphIndex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMarkToBaseAdjustmentRecord_Injected(uint baseGlyphIndex, uint markGlyphIndex, out MarkToBaseAdjustmentRecord ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMarkToBaseAdjustmentRecordMarshallingArray_Injected(ref ManagedSpanWrapper glyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMarkToBaseAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMarkToBaseAdjustmentRecordsFromMarshallingArray_Injected(ref ManagedSpanWrapper adjustmentRecords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAllMarkToMarkAdjustmentRecords_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMarkToMarkAdjustmentRecords_Injected(uint baseMarkGlyphIndex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMarkToMarkAdjustmentRecord_Injected(uint firstGlyphIndex, uint secondGlyphIndex, out MarkToMarkAdjustmentRecord ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMarkToMarkAdjustmentRecordMarshallingArray_Injected(ref ManagedSpanWrapper glyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMarkToMarkAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMarkToMarkAdjustmentRecordsFromMarshallingArray_Injected(ref ManagedSpanWrapper adjustmentRecords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMarkToLigatureAdjustmentRecord_Injected(uint firstGlyphIndex, uint secondGlyphIndex, out MarkToLigatureAdjustmentRecord ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMarkToLigatureAdjustmentRecordMarshallingArray_Injected(ref ManagedSpanWrapper glyphIndexes, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopulateMarkToLigatureAdjustmentRecordMarshallingArray_for_LookupIndex_Injected(ref ManagedSpanWrapper glyphIndexes, int lookupIndex, out int recordCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetAtlasTexture_Injected(IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RenderBufferToTexture_Injected(IntPtr srcTexture, int padding, GlyphRenderMode renderMode, IntPtr dstTexture);

		private static Glyph[] s_Glyphs = new Glyph[16];

		private static uint[] s_GlyphIndexes_MarshallingArray_A;

		private static uint[] s_GlyphIndexes_MarshallingArray_B;

		private static GlyphMarshallingStruct[] s_GlyphMarshallingStruct_IN = new GlyphMarshallingStruct[16];

		private static GlyphMarshallingStruct[] s_GlyphMarshallingStruct_OUT = new GlyphMarshallingStruct[16];

		private static GlyphRect[] s_FreeGlyphRects = new GlyphRect[16];

		private static GlyphRect[] s_UsedGlyphRects = new GlyphRect[16];

		private static GlyphAdjustmentRecord[] s_SingleAdjustmentRecords_MarshallingArray;

		private static SingleSubstitutionRecord[] s_SingleSubstitutionRecords_MarshallingArray;

		private static MultipleSubstitutionRecord[] s_MultipleSubstitutionRecords_MarshallingArray;

		private static AlternateSubstitutionRecord[] s_AlternateSubstitutionRecords_MarshallingArray;

		private static LigatureSubstitutionRecord[] s_LigatureSubstitutionRecords_MarshallingArray;

		private static ContextualSubstitutionRecord[] s_ContextualSubstitutionRecords_MarshallingArray;

		private static ChainingContextualSubstitutionRecord[] s_ChainingContextualSubstitutionRecords_MarshallingArray;

		private static GlyphPairAdjustmentRecord[] s_PairAdjustmentRecords_MarshallingArray;

		private static MarkToBaseAdjustmentRecord[] s_MarkToBaseAdjustmentRecords_MarshallingArray;

		private static MarkToMarkAdjustmentRecord[] s_MarkToMarkAdjustmentRecords_MarshallingArray;

		private static MarkToLigatureAdjustmentRecord[] s_MarkToLigatureAdjustmentRecords_MarshallingArray;

		private static Dictionary<uint, Glyph> s_GlyphLookupDictionary = new Dictionary<uint, Glyph>();
	}
}
