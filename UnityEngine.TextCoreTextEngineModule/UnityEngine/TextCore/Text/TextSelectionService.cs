using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextSelectionService.h")]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "Unity.UIElements.PlayModeTests" })]
	internal class TextSelectionService
	{
		[NativeMethod(Name = "TextSelectionService::Substring")]
		internal static string Substring(IntPtr textGenerationInfo, int startIndex, int endIndex)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				TextSelectionService.Substring_Injected(textGenerationInfo, startIndex, endIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeMethod(Name = "TextSelectionService::SelectCurrentWord")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SelectCurrentWord(IntPtr textGenerationInfo, int currentIndex, ref int startIndex, ref int endIndex);

		[NativeMethod(Name = "TextSelectionService::PreviousCodePointIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int PreviousCodePointIndex(IntPtr textGenerationInfo, int currentIndex);

		[NativeMethod(Name = "TextSelectionService::NextCodePointIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int NextCodePointIndex(IntPtr textGenerationInfo, int currentIndex);

		[NativeMethod(Name = "TextSelectionService::GetCursorLogicalIndexFromPosition")]
		internal static int GetCursorLogicalIndexFromPosition(IntPtr textGenerationInfo, Vector2 position)
		{
			return TextSelectionService.GetCursorLogicalIndexFromPosition_Injected(textGenerationInfo, ref position);
		}

		[NativeMethod(Name = "TextSelectionService::GetCursorPositionFromLogicalIndex")]
		internal static Vector2 GetCursorPositionFromLogicalIndex(IntPtr textGenerationInfo, int logicalIndex)
		{
			Vector2 vector;
			TextSelectionService.GetCursorPositionFromLogicalIndex_Injected(textGenerationInfo, logicalIndex, out vector);
			return vector;
		}

		[NativeMethod(Name = "TextSelectionService::LineUpCharacterPosition")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int LineUpCharacterPosition(IntPtr textGenerationInfo, int originalPos);

		[NativeMethod(Name = "TextSelectionService::LineDownCharacterPosition")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int LineDownCharacterPosition(IntPtr textGenerationInfo, int originalPos);

		[NativeMethod(Name = "TextSelectionService::GetHighlightRectangles")]
		internal static Rect[] GetHighlightRectangles(IntPtr textGenerationInfo, int cursorIndex, int selectIndex)
		{
			Rect[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				TextSelectionService.GetHighlightRectangles_Injected(textGenerationInfo, cursorIndex, selectIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Rect[] array;
				blittableArrayWrapper.Unmarshal<Rect>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "TextSelectionService::GetCharacterHeightFromIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetCharacterHeightFromIndex(IntPtr textGenerationInfo, int index);

		[NativeMethod(Name = "TextSelectionService::GetStartOfNextWord")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetStartOfNextWord(IntPtr textGenerationInfo, int currentIndex);

		[NativeMethod(Name = "TextSelectionService::GetEndOfPreviousWord")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetEndOfPreviousWord(IntPtr textGenerationInfo, int currentIndex);

		[NativeMethod(Name = "TextSelectionService::GetFirstCharacterIndexOnLine")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetFirstCharacterIndexOnLine(IntPtr textGenerationInfo, int currentIndex);

		[NativeMethod(Name = "TextSelectionService::GetLastCharacterIndexOnLine")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLastCharacterIndexOnLine(IntPtr textGenerationInfo, int currentIndex);

		[NativeMethod(Name = "TextSelectionService::GetLineHeight")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetLineHeight(IntPtr textGenerationInfo, int lineIndex);

		[NativeMethod(Name = "TextSelectionService::GetLineNumberFromLogicalIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLineNumber(IntPtr textGenerationInfo, int logicalIndex);

		[NativeMethod(Name = "TextSelectionService::SelectToPreviousParagraph")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SelectToPreviousParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

		[NativeMethod(Name = "TextSelectionService::SelectToStartOfParagraph")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SelectToStartOfParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

		[NativeMethod(Name = "TextSelectionService::SelectToEndOfParagraph")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SelectToEndOfParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

		[NativeMethod(Name = "TextSelectionService::SelectToNextParagraph")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SelectToNextParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

		[NativeMethod(Name = "TextSelectionService::SelectCurrentParagraph")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SelectCurrentParagraph(IntPtr textGenerationInfo, ref int cursorIndex, ref int selectIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Substring_Injected(IntPtr textGenerationInfo, int startIndex, int endIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCursorLogicalIndexFromPosition_Injected(IntPtr textGenerationInfo, [In] ref Vector2 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCursorPositionFromLogicalIndex_Injected(IntPtr textGenerationInfo, int logicalIndex, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHighlightRectangles_Injected(IntPtr textGenerationInfo, int cursorIndex, int selectIndex, out BlittableArrayWrapper ret);
	}
}
