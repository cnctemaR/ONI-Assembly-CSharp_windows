using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Abstract base class for VisualElement containing text.</para>
	/// </summary>
	public abstract class BaseTextElement : VisualElement
	{
		public BaseTextElement()
		{
			base.requireMeasureFunction = true;
			base.AddToClassList("textElement");
		}

		/// <summary>
		///   <para>The text associated with the element.</para>
		/// </summary>
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
					base.Dirty(ChangeType.Layout);
					if (!string.IsNullOrEmpty(base.persistenceKey))
					{
						base.SavePersistentData();
					}
				}
			}
		}

		public override void DoRepaint()
		{
			IStylePainter stylePainter = base.elementPanel.stylePainter;
			stylePainter.DrawBackground(this);
			stylePainter.DrawBorder(this);
			stylePainter.DrawText(this);
		}

		protected internal override Vector2 DoMeasure(float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			Font font = base.style.font;
			Vector2 vector;
			if (this.text == null || font == null)
			{
				vector = new Vector2(num, num2);
			}
			else
			{
				IStylePainter stylePainter = base.elementPanel.stylePainter;
				if (widthMode == VisualElement.MeasureMode.Exactly)
				{
					num = width;
				}
				else
				{
					TextStylePainterParameters defaultTextParameters = stylePainter.GetDefaultTextParameters(this);
					defaultTextParameters.text = this.text;
					defaultTextParameters.font = font;
					defaultTextParameters.wordWrapWidth = 0f;
					defaultTextParameters.wordWrap = false;
					defaultTextParameters.richText = true;
					num = stylePainter.ComputeTextWidth(defaultTextParameters);
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
					TextStylePainterParameters defaultTextParameters2 = stylePainter.GetDefaultTextParameters(this);
					defaultTextParameters2.text = this.text;
					defaultTextParameters2.font = font;
					defaultTextParameters2.wordWrapWidth = num;
					defaultTextParameters2.richText = true;
					num2 = stylePainter.ComputeTextHeight(defaultTextParameters2);
					if (heightMode == VisualElement.MeasureMode.AtMost)
					{
						num2 = Mathf.Min(num2, height);
					}
				}
				vector = new Vector2(num, num2);
			}
			return vector;
		}

		internal const string k_TextElementClass = "textElement";

		[SerializeField]
		private string m_Text;

		/// <summary>
		///   <para>UxmlTraits for the BaseTextElement.</para>
		/// </summary>
		public class BaseTextElementUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public BaseTextElementUxmlTraits()
			{
				this.m_Text = new UxmlStringAttributeDescription
				{
					name = "text"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for BasetextElement properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_Text;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an empty enumerable, as text elements generally do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize BaseTextElement properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((BaseTextElement)ve).text = this.m_Text.GetValueFromBag(bag);
			}

			private UxmlStringAttributeDescription m_Text;
		}
	}
}
