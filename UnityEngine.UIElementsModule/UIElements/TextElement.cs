using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class TextElement : BindableElement, ITextElement, INotifyValueChanged<string>
	{
		public TextElement()
		{
			base.requireMeasureFunction = true;
			base.AddToClassList(TextElement.ussClassName);
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
			base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
		}

		internal TextHandle textHandle
		{
			get
			{
				return this.m_TextHandle;
			}
		}

		private void OnAttachToPanel(AttachToPanelEvent e)
		{
			this.m_TextHandle.useLegacy = e.destinationPanel.contextType == ContextType.Editor;
		}

		public virtual string text
		{
			get
			{
				return ((INotifyValueChanged<string>)this).value;
			}
			set
			{
				((INotifyValueChanged<string>)this).value = value;
			}
		}

		private void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			mgc.Text(MeshGenerationContextUtils.TextParams.MakeStyleBased(this, this.text), this.m_TextHandle, base.scaledPixelsPerPoint);
		}

		public Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextElement.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode, this.m_TextHandle);
		}

		internal static Vector2 MeasureVisualElementTextSize(VisualElement ve, string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, TextHandle textHandle)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			Font value = ve.computedStyle.unityFont.value;
			bool flag = textToMeasure == null || value == null;
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(num, num2);
			}
			else
			{
				Vector3 vector2 = ve.ComputeGlobalScale();
				float num3 = (vector2.x + vector2.y) * 0.5f * ve.scaledPixelsPerPoint;
				bool flag2 = num3 <= 0f;
				if (flag2)
				{
					vector = Vector2.zero;
				}
				else
				{
					bool flag3 = widthMode == VisualElement.MeasureMode.Exactly;
					if (flag3)
					{
						num = width;
					}
					else
					{
						MeshGenerationContextUtils.TextParams textSettings = TextElement.GetTextSettings(ve, textToMeasure);
						textSettings.wordWrap = false;
						textSettings.richText = false;
						num = Mathf.Ceil(textHandle.ComputeTextWidth(textSettings, num3));
						bool flag4 = widthMode == VisualElement.MeasureMode.AtMost;
						if (flag4)
						{
							num = Mathf.Min(num, width);
						}
					}
					bool flag5 = heightMode == VisualElement.MeasureMode.Exactly;
					if (flag5)
					{
						num2 = height;
					}
					else
					{
						MeshGenerationContextUtils.TextParams textSettings2 = TextElement.GetTextSettings(ve, textToMeasure);
						textSettings2.wordWrapWidth = num;
						textSettings2.richText = false;
						num2 = Mathf.Ceil(textHandle.ComputeTextHeight(textSettings2, num3));
						bool flag6 = heightMode == VisualElement.MeasureMode.AtMost;
						if (flag6)
						{
							num2 = Mathf.Min(num2, height);
						}
					}
					vector = new Vector2(num, num2);
				}
			}
			return vector;
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			return this.MeasureTextSize(this.text, desiredWidth, widthMode, desiredHeight, heightMode);
		}

		private static MeshGenerationContextUtils.TextParams GetTextSettings(VisualElement ve, string text)
		{
			ComputedStyle computedStyle = ve.computedStyle;
			return new MeshGenerationContextUtils.TextParams
			{
				rect = ve.contentRect,
				text = text,
				font = computedStyle.unityFont.value,
				fontSize = (int)computedStyle.fontSize.value.value,
				fontStyle = computedStyle.unityFontStyleAndWeight.value,
				fontColor = computedStyle.color.value,
				anchor = computedStyle.unityTextAlign.value,
				wordWrap = (computedStyle.whiteSpace.value == WhiteSpace.Normal),
				wordWrapWidth = ((computedStyle.whiteSpace.value == WhiteSpace.Normal) ? ve.contentRect.width : 0f),
				richText = true
			};
		}

		string INotifyValueChanged<string>.value
		{
			get
			{
				return this.m_Text ?? string.Empty;
			}
			set
			{
				bool flag = this.m_Text != value;
				if (flag)
				{
					bool flag2 = base.panel != null;
					if (flag2)
					{
						using (ChangeEvent<string> pooled = ChangeEvent<string>.GetPooled(this.text, value))
						{
							pooled.target = this;
							((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
							this.SendEvent(pooled);
						}
					}
					else
					{
						((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
					}
				}
			}
		}

		void INotifyValueChanged<string>.SetValueWithoutNotify(string newValue)
		{
			bool flag = this.m_Text != newValue;
			if (flag)
			{
				this.m_Text = newValue;
				base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
				bool flag2 = !string.IsNullOrEmpty(base.viewDataKey);
				if (flag2)
				{
					base.SaveViewData();
				}
			}
		}

		public static readonly string ussClassName = "unity-text-element";

		private TextHandle m_TextHandle = TextHandle.New();

		[SerializeField]
		private string m_Text;

		public new class UxmlFactory : UxmlFactory<TextElement, TextElement.UxmlTraits>
		{
		}

		public new class UxmlTraits : BindableElement.UxmlTraits
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
