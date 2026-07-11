using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class TextElement : VisualElement, ITextElement
	{
		public TextElement()
		{
			base.requireMeasureFunction = true;
			base.AddToClassList("textElement");
		}

		public virtual string text
		{
			get
			{
				return this.m_Text ?? string.Empty;
			}
			set
			{
				if (!(this.m_Text == value))
				{
					this.m_Text = value;
					base.IncrementVersion(VersionChangeType.Layout);
					if (!string.IsNullOrEmpty(base.persistenceKey))
					{
						base.SavePersistentData();
					}
				}
			}
		}

		protected override void DoRepaint(IStylePainter painter)
		{
			IStylePainterInternal stylePainterInternal = (IStylePainterInternal)painter;
			stylePainterInternal.DrawText(this.text);
		}

		public Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextElement.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode);
		}

		internal static Vector2 MeasureVisualElementTextSize(VisualElement ve, string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			Font font = ve.style.font;
			Vector2 vector;
			if (textToMeasure == null || font == null)
			{
				vector = new Vector2(num, num2);
			}
			else
			{
				Vector3 vector2 = ve.ComputeGlobalScale();
				float num3 = ((ve.elementPanel == null) ? GUIUtility.pixelsPerPoint : ve.elementPanel.currentPixelsPerPoint);
				float num4 = (vector2.x + vector2.y) * 0.5f * num3;
				if (widthMode == VisualElement.MeasureMode.Exactly)
				{
					num = width;
				}
				else
				{
					TextStylePainterParameters @default = TextStylePainterParameters.GetDefault(ve, textToMeasure);
					@default.text = textToMeasure;
					@default.font = font;
					@default.wordWrapWidth = 0f;
					@default.wordWrap = false;
					@default.richText = true;
					num = Mathf.Ceil(TextNative.ComputeTextWidth(@default.GetTextNativeSettings(num4)));
					if (widthMode == VisualElement.MeasureMode.AtMost)
					{
						num = Mathf.Min(num, width);
					}
				}
				if (heightMode == VisualElement.MeasureMode.Exactly)
				{
					num2 = height;
				}
				else
				{
					TextStylePainterParameters default2 = TextStylePainterParameters.GetDefault(ve, textToMeasure);
					default2.text = textToMeasure;
					default2.font = font;
					default2.wordWrapWidth = num;
					default2.richText = true;
					num2 = Mathf.Ceil(TextNative.ComputeTextHeight(default2.GetTextNativeSettings(num4)));
					if (heightMode == VisualElement.MeasureMode.AtMost)
					{
						num2 = Mathf.Min(num2, height);
					}
				}
				vector = new Vector2(num, num2);
			}
			return vector;
		}

		protected internal override Vector2 DoMeasure(float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return this.MeasureTextSize(this.text, width, widthMode, height, heightMode);
		}

		internal const string k_TextElementClass = "textElement";

		[SerializeField]
		private string m_Text;

		public new class UxmlFactory : UxmlFactory<TextElement, TextElement.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((ITextElement)ve).text = this.m_Text.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};
		}
	}
}
