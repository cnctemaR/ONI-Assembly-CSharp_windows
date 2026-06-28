using System;
using UnityEngine;

namespace TMPro
{
	public static class TMP_TextUtilities
	{
		public static CaretInfo GetCursorInsertionIndex(TMP_Text textComponent, Vector3 position, Camera camera)
		{
			int num = TMP_TextUtilities.FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tmp_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				return new CaretInfo(num, CaretPosition.Left);
			}
			return new CaretInfo(num, CaretPosition.Right);
		}

		public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera)
		{
			int num = TMP_TextUtilities.FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tmp_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				return num;
			}
			return num + 1;
		}

		public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera, out CaretPosition cursor)
		{
			int num = TMP_TextUtilities.FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tmp_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				cursor = CaretPosition.Left;
				return num;
			}
			cursor = CaretPosition.Right;
			return num + 1;
		}

		public static bool IsIntersectingRectTransform(RectTransform rectTransform, Vector3 position, Camera camera)
		{
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			rectTransform.GetWorldCorners(TMP_TextUtilities.m_rectWorldCorners);
			return TMP_TextUtilities.PointIntersectRectangle(position, TMP_TextUtilities.m_rectWorldCorners[0], TMP_TextUtilities.m_rectWorldCorners[1], TMP_TextUtilities.m_rectWorldCorners[2], TMP_TextUtilities.m_rectWorldCorners[3]);
		}

		public static int FindIntersectingCharacter(TextMeshProUGUI text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tmp_CharacterInfo.isVisible)
				{
					Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int FindIntersectingCharacter(TextMeshPro text, Vector3 position, Camera camera, bool visibleOnly)
		{
			Transform transform = text.transform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tmp_CharacterInfo.isVisible)
				{
					Vector3 vector = transform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 vector2 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = transform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int FindNearestCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int num2 = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tmp_CharacterInfo.isVisible)
				{
					Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num3 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
					float num4 = TMP_TextUtilities.DistanceToLine(vector2, vector3, position);
					float num5 = TMP_TextUtilities.DistanceToLine(vector3, vector4, position);
					float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector, position);
					float num7 = ((num3 >= num4) ? num4 : num3);
					num7 = ((num7 >= num5) ? num5 : num7);
					num7 = ((num7 >= num6) ? num6 : num7);
					if (num > num7)
					{
						num = num7;
						num2 = i;
					}
				}
			}
			return num2;
		}

		public static int FindNearestCharacter(TextMeshProUGUI text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int num2 = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tmp_CharacterInfo.isVisible)
				{
					Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num3 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
					float num4 = TMP_TextUtilities.DistanceToLine(vector2, vector3, position);
					float num5 = TMP_TextUtilities.DistanceToLine(vector3, vector4, position);
					float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector, position);
					float num7 = ((num3 >= num4) ? num4 : num3);
					num7 = ((num7 >= num5) ? num5 : num7);
					num7 = ((num7 >= num6) ? num6 : num7);
					if (num > num7)
					{
						num = num7;
						num2 = i;
					}
				}
			}
			return num2;
		}

		public static int FindNearestCharacter(TextMeshPro text, Vector3 position, Camera camera, bool visibleOnly)
		{
			Transform transform = text.transform;
			float num = float.PositiveInfinity;
			int num2 = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tmp_CharacterInfo.isVisible)
				{
					Vector3 vector = transform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 vector2 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = transform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num3 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
					float num4 = TMP_TextUtilities.DistanceToLine(vector2, vector3, position);
					float num5 = TMP_TextUtilities.DistanceToLine(vector3, vector4, position);
					float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector, position);
					float num7 = ((num3 >= num4) ? num4 : num3);
					num7 = ((num7 >= num5) ? num5 : num7);
					num7 = ((num7 >= num6) ? num6 : num7);
					if (num > num7)
					{
						num = num7;
						num2 = i;
					}
				}
			}
			return num2;
		}

		public static int FindIntersectingWord(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tmp_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				float num = float.NegativeInfinity;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < tmp_WordInfo.characterCount; j++)
				{
					int num3 = tmp_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					bool flag2 = num3 <= text.maxVisibleCharacters && (int)tmp_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay);
					num = Mathf.Max(num, tmp_CharacterInfo.ascender);
					num2 = Mathf.Min(num2, tmp_CharacterInfo.descender);
					if (!flag && flag2)
					{
						flag = true;
						vector = new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f);
						vector2 = new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f);
						if (tmp_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
							vector4 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
							vector = rectTransform.TransformPoint(new Vector3(vector.x, num2, 0f));
							vector2 = rectTransform.TransformPoint(new Vector3(vector2.x, num, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(vector4.x, num, 0f));
							vector3 = rectTransform.TransformPoint(new Vector3(vector3.x, num2, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
						vector4 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
						vector = rectTransform.TransformPoint(new Vector3(vector.x, num2, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(vector2.x, num, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(vector4.x, num, 0f));
						vector3 = rectTransform.TransformPoint(new Vector3(vector3.x, num2, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						vector3 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
						vector4 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
						vector = rectTransform.TransformPoint(new Vector3(vector.x, num2, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(vector2.x, num, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(vector4.x, num, 0f));
						vector3 = rectTransform.TransformPoint(new Vector3(vector3.x, num2, 0f));
						num = float.NegativeInfinity;
						num2 = float.PositiveInfinity;
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindIntersectingWord(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tmp_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				float num = float.NegativeInfinity;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < tmp_WordInfo.characterCount; j++)
				{
					int num3 = tmp_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					bool flag2 = num3 <= text.maxVisibleCharacters && (int)tmp_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay);
					num = Mathf.Max(num, tmp_CharacterInfo.ascender);
					num2 = Mathf.Min(num2, tmp_CharacterInfo.descender);
					if (!flag && flag2)
					{
						flag = true;
						vector = new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f);
						vector2 = new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f);
						if (tmp_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
							vector4 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
							vector = transform.TransformPoint(new Vector3(vector.x, num2, 0f));
							vector2 = transform.TransformPoint(new Vector3(vector2.x, num, 0f));
							vector4 = transform.TransformPoint(new Vector3(vector4.x, num, 0f));
							vector3 = transform.TransformPoint(new Vector3(vector3.x, num2, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
						vector4 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
						vector = transform.TransformPoint(new Vector3(vector.x, num2, 0f));
						vector2 = transform.TransformPoint(new Vector3(vector2.x, num, 0f));
						vector4 = transform.TransformPoint(new Vector3(vector4.x, num, 0f));
						vector3 = transform.TransformPoint(new Vector3(vector3.x, num2, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						vector3 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
						vector4 = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
						vector = transform.TransformPoint(new Vector3(vector.x, num2, 0f));
						vector2 = transform.TransformPoint(new Vector3(vector2.x, num, 0f));
						vector4 = transform.TransformPoint(new Vector3(vector4.x, num, 0f));
						vector3 = transform.TransformPoint(new Vector3(vector3.x, num2, 0f));
						num = float.NegativeInfinity;
						num2 = float.PositiveInfinity;
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindNearestWord(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int num2 = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tmp_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_WordInfo.characterCount; j++)
				{
					int num3 = tmp_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					bool flag2 = num3 <= text.maxVisibleCharacters && (int)tmp_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay);
					if (!flag && flag2)
					{
						flag = true;
						vector = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num4 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
				float num5 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
				float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
				float num7 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
				float num8 = ((num4 >= num5) ? num5 : num4);
				num8 = ((num8 >= num6) ? num6 : num8);
				num8 = ((num8 >= num7) ? num7 : num8);
				if (num > num8)
				{
					num = num8;
					num2 = i;
				}
			}
			return num2;
		}

		public static int FindNearestWord(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			float num = float.PositiveInfinity;
			int num2 = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tmp_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_WordInfo.characterCount; j++)
				{
					int num3 = tmp_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					bool flag2 = num3 <= text.maxVisibleCharacters && (int)tmp_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay);
					if (!flag && flag2)
					{
						flag = true;
						vector = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num4 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
				float num5 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
				float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
				float num7 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
				float num8 = ((num4 >= num5) ? num5 : num4);
				num8 = ((num8 >= num6) ? num6 : num8);
				num8 = ((num8 >= num7) ? num7 : num8);
				if (num > num8)
				{
					num = num8;
					num2 = i;
				}
			}
			return num2;
		}

		public static int FindIntersectingLink(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tmp_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_LinkInfo.linkTextLength; j++)
				{
					int num = tmp_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						vector = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num + 1].lineNumber)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindIntersectingLink(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tmp_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_LinkInfo.linkTextLength; j++)
				{
					int num = tmp_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						vector = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num + 1].lineNumber)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindNearestLink(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tmp_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_LinkInfo.linkTextLength; j++)
				{
					int num3 = tmp_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						vector = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num4 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
				float num5 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
				float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
				float num7 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
				float num8 = ((num4 >= num5) ? num5 : num4);
				num8 = ((num8 >= num6) ? num6 : num8);
				num8 = ((num8 >= num7) ? num7 : num8);
				if (num > num8)
				{
					num = num8;
					num2 = i;
				}
			}
			return num2;
		}

		public static int FindNearestLink(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tmp_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_LinkInfo.linkTextLength; j++)
				{
					int num3 = tmp_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						vector = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num4 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
				float num5 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
				float num6 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
				float num7 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
				float num8 = ((num4 >= num5) ? num5 : num4);
				num8 = ((num8 >= num6) ? num6 : num8);
				num8 = ((num8 >= num7) ? num7 : num8);
				if (num > num8)
				{
					num = num8;
					num2 = i;
				}
			}
			return num2;
		}

		private static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			Vector3 vector = b - a;
			Vector3 vector2 = m - a;
			Vector3 vector3 = c - b;
			Vector3 vector4 = m - b;
			float num = Vector3.Dot(vector, vector2);
			float num2 = Vector3.Dot(vector3, vector4);
			return 0f <= num && num <= Vector3.Dot(vector, vector) && 0f <= num2 && num2 <= Vector3.Dot(vector3, vector3);
		}

		public static bool ScreenPointToWorldPointInRectangle(Transform transform, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
		{
			worldPoint = Vector2.zero;
			Ray ray = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
			Plane plane = new Plane(transform.rotation * Vector3.back, transform.position);
			float num;
			if (!plane.Raycast(ray, out num))
			{
				return false;
			}
			worldPoint = ray.GetPoint(num);
			return true;
		}

		private static bool IntersectLinePlane(TMP_TextUtilities.LineSegment line, Vector3 point, Vector3 normal, out Vector3 intersectingPoint)
		{
			intersectingPoint = Vector3.zero;
			Vector3 vector = line.Point2 - line.Point1;
			Vector3 vector2 = line.Point1 - point;
			float num = Vector3.Dot(normal, vector);
			float num2 = -Vector3.Dot(normal, vector2);
			if (Mathf.Abs(num) < Mathf.Epsilon)
			{
				return num2 == 0f;
			}
			float num3 = num2 / num;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			intersectingPoint = line.Point1 + num3 * vector;
			return true;
		}

		public static float DistanceToLine(Vector3 a, Vector3 b, Vector3 point)
		{
			Vector3 vector = b - a;
			Vector3 vector2 = a - point;
			float num = Vector3.Dot(vector, vector2);
			if (num > 0f)
			{
				return Vector3.Dot(vector2, vector2);
			}
			Vector3 vector3 = point - b;
			if (Vector3.Dot(vector, vector3) > 0f)
			{
				return Vector3.Dot(vector3, vector3);
			}
			Vector3 vector4 = vector2 - vector * (num / Vector3.Dot(vector, vector));
			return Vector3.Dot(vector4, vector4);
		}

		public static char ToLowerFast(char c)
		{
			return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-"[(int)c];
		}

		public static char ToUpperFast(char c)
		{
			return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[(int)c];
		}

		public static int GetSimpleHashCode(string s)
		{
			int num = 0;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((num << 5) + num) ^ (int)s[i];
			}
			return num;
		}

		public static uint GetSimpleHashCodeLowercase(string s)
		{
			uint num = 5381U;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((num << 5) + num) ^ (uint)TMP_TextUtilities.ToLowerFast(s[i]);
			}
			return num;
		}

		private static Vector3[] m_rectWorldCorners = new Vector3[4];

		private const string k_lookupStringL = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-";

		private const string k_lookupStringU = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-";

		private struct LineSegment
		{
			public LineSegment(Vector3 p1, Vector3 p2)
			{
				this.Point1 = p1;
				this.Point2 = p2;
			}

			public Vector3 Point1;

			public Vector3 Point2;
		}
	}
}
