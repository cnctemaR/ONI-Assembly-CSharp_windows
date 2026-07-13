using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	public class Image : VisualElement
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[CreateProperty]
		internal Object source
		{
			get
			{
				return this.m_Image;
			}
			set
			{
				Texture texture = value as Texture;
				if (texture == null)
				{
					Sprite sprite = value as Sprite;
					if (sprite == null)
					{
						VectorImage vectorImage = value as VectorImage;
						if (vectorImage == null)
						{
							this.SetInlineProperty<Object>(null, Image.imageProperty);
						}
						else
						{
							this.vectorImage = vectorImage;
						}
					}
					else
					{
						this.sprite = sprite;
					}
				}
				else
				{
					this.image = texture;
				}
				base.NotifyPropertyChanged(in Image.sourceProperty);
			}
		}

		[CreateProperty]
		public Texture image
		{
			get
			{
				return this.m_Image as Texture;
			}
			set
			{
				this.SetInlineProperty<Texture>(value, Image.imageProperty);
			}
		}

		[CreateProperty]
		public Sprite sprite
		{
			get
			{
				return this.m_Image as Sprite;
			}
			set
			{
				this.SetInlineProperty<Sprite>(value, Image.spriteProperty);
			}
		}

		[CreateProperty]
		public VectorImage vectorImage
		{
			get
			{
				return this.m_Image as VectorImage;
			}
			set
			{
				this.SetInlineProperty<VectorImage>(value, Image.vectorImageProperty);
			}
		}

		[CreateProperty]
		public Rect sourceRect
		{
			get
			{
				return this.GetSourceRect();
			}
			set
			{
				bool flag = this.GetSourceRect() == value;
				if (!flag)
				{
					bool flag2 = this.sprite != null;
					if (flag2)
					{
						Debug.LogError("Cannot set sourceRect on a sprite image");
					}
					else
					{
						this.CalculateUV(value);
						base.NotifyPropertyChanged(in Image.sourceRectProperty);
					}
				}
			}
		}

		[CreateProperty]
		public Rect uv
		{
			get
			{
				return this.m_UV;
			}
			set
			{
				bool flag = this.m_UV == value;
				if (!flag)
				{
					this.m_UV = value;
					base.NotifyPropertyChanged(in Image.uvProperty);
				}
			}
		}

		[CreateProperty]
		public ScaleMode scaleMode
		{
			get
			{
				return this.m_ScaleMode;
			}
			set
			{
				bool flag = this.m_ScaleMode == value && this.m_ScaleModeIsInline;
				if (!flag)
				{
					this.m_ScaleModeIsInline = true;
					this.SetScaleMode(value);
				}
			}
		}

		[CreateProperty]
		public Color tintColor
		{
			get
			{
				return this.m_TintColor;
			}
			set
			{
				bool flag = this.m_TintColor == value && this.m_TintColorIsInline;
				if (!flag)
				{
					this.m_TintColorIsInline = true;
					this.SetTintColor(value);
				}
			}
		}

		public Image()
		{
			base.AddToClassList(Image.ussClassName);
			this.m_ScaleMode = ScaleMode.ScaleToFit;
			this.m_TintColor = Color.white;
			this.m_UV = new Rect(0f, 0f, 1f, 1f);
			base.requireMeasureFunction = true;
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
		}

		private Vector2 GetTextureDisplaySize(Texture texture)
		{
			Vector2 zero = Vector2.zero;
			bool flag = texture != null;
			if (flag)
			{
				zero = new Vector2((float)texture.width, (float)texture.height);
			}
			return zero;
		}

		private Vector2 GetTextureDisplaySize(Sprite sprite)
		{
			Vector2 vector = Vector2.zero;
			bool flag = sprite != null;
			if (flag)
			{
				float num = UIElementsUtility.PixelsPerUnitScaleForElement(this, sprite);
				vector = sprite.bounds.size * sprite.pixelsPerUnit * num;
			}
			return vector;
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			bool flag = this.source == null;
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(num, num2);
			}
			else
			{
				Vector2 vector2 = Vector2.zero;
				Object source = this.source;
				Object @object = source;
				Texture texture = @object as Texture;
				if (texture == null)
				{
					Sprite sprite = @object as Sprite;
					if (sprite == null)
					{
						VectorImage vectorImage = @object as VectorImage;
						if (vectorImage != null)
						{
							vector2 = vectorImage.size;
						}
					}
					else
					{
						vector2 = this.GetTextureDisplaySize(sprite);
					}
				}
				else
				{
					vector2 = this.GetTextureDisplaySize(texture);
				}
				Rect sourceRect = this.sourceRect;
				bool flag2 = sourceRect != Rect.zero;
				num = (flag2 ? Mathf.Abs(sourceRect.width) : vector2.x);
				num2 = (flag2 ? Mathf.Abs(sourceRect.height) : vector2.y);
				bool flag3 = widthMode == VisualElement.MeasureMode.AtMost;
				if (flag3)
				{
					num = Mathf.Min(num, desiredWidth);
				}
				bool flag4 = heightMode == VisualElement.MeasureMode.AtMost;
				if (flag4)
				{
					num2 = Mathf.Min(num2, desiredHeight);
				}
				vector = new Vector2(num, num2);
			}
			return vector;
		}

		private void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			bool flag = this.source == null;
			if (!flag)
			{
				Rect rect = GUIUtility.AlignRectToDevice(base.contentRect);
				VisualElement visualElement = mgc.visualElement;
				Color color = ((visualElement != null) ? visualElement.playModeTintColor : Color.white);
				MeshGenerator.RectangleParams rectangleParams = default(MeshGenerator.RectangleParams);
				bool flag2 = this.image != null;
				if (flag2)
				{
					rectangleParams = MeshGenerator.RectangleParams.MakeTextured(rect, this.uv, this.image, this.scaleMode, color);
				}
				else
				{
					bool flag3 = this.sprite != null;
					if (flag3)
					{
						Vector4 zero = Vector4.zero;
						rectangleParams = MeshGenerator.RectangleParams.MakeSprite(rect, this.uv, this.sprite, this.scaleMode, color, false, ref zero, false);
					}
					else
					{
						bool flag4 = this.vectorImage != null;
						if (flag4)
						{
							rectangleParams = MeshGenerator.RectangleParams.MakeVectorTextured(rect, this.uv, this.vectorImage, this.scaleMode, color);
						}
					}
				}
				rectangleParams.color = this.tintColor;
				mgc.meshGenerator.DrawRectangle(rectangleParams);
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			this.ReadCustomProperties(e.customStyle);
		}

		private void ReadCustomProperties(ICustomStyle customStyleProvider)
		{
			bool flag = !this.m_ImageIsInline;
			if (flag)
			{
				Texture2D texture2D;
				bool flag2 = customStyleProvider.TryGetValue(Image.s_ImageProperty, out texture2D);
				if (flag2)
				{
					this.SetCustomProperty(texture2D, Image.imageProperty);
				}
				else
				{
					Sprite sprite;
					bool flag3 = customStyleProvider.TryGetValue(Image.s_SpriteProperty, out sprite);
					if (flag3)
					{
						this.SetCustomProperty(sprite, Image.spriteProperty);
					}
					else
					{
						VectorImage vectorImage;
						bool flag4 = customStyleProvider.TryGetValue(Image.s_VectorImageProperty, out vectorImage);
						if (flag4)
						{
							this.SetCustomProperty(vectorImage, Image.vectorImageProperty);
						}
						else
						{
							this.ClearProperty();
						}
					}
				}
			}
			string text;
			bool flag5 = !this.m_ScaleModeIsInline && customStyleProvider.TryGetValue(Image.s_ScaleModeProperty, out text);
			if (flag5)
			{
				int num;
				StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.ScaleMode, text, out num);
				this.SetScaleMode((ScaleMode)num);
			}
			bool flag6 = !this.m_TintColorIsInline;
			if (flag6)
			{
				Color color;
				bool flag7 = customStyleProvider.TryGetValue(Image.s_TintColorProperty, out color);
				if (flag7)
				{
					this.SetTintColor(color);
				}
				else
				{
					this.SetTintColor(Color.white);
				}
			}
		}

		private void SetInlineProperty<T>(Object value, BindingId binding)
		{
			bool flag = this.source == value && this.m_ImageIsInline;
			if (!flag)
			{
				bool flag2 = !this.m_ImageIsInline;
				if (flag2)
				{
					this.m_Image = null;
				}
				bool flag3 = value != null;
				if (flag3)
				{
					this.m_Image = value;
				}
				else
				{
					bool flag4 = this.m_Image is T;
					if (flag4)
					{
						this.m_Image = null;
					}
				}
				this.m_ImageIsInline = this.m_Image != null;
				bool flag5 = this.m_Image == null;
				if (flag5)
				{
					this.uv = new Rect(0f, 0f, 1f, 1f);
					this.ReadCustomProperties(base.customStyle);
				}
				base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
				base.NotifyPropertyChanged(in binding);
			}
		}

		private void SetCustomProperty(Object value, BindingId binding)
		{
			Debug.Assert(!this.m_ImageIsInline, "Expected image to not be inline when using set custom property");
			bool flag = value == this.source;
			if (!flag)
			{
				this.m_Image = value;
				base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
				base.NotifyPropertyChanged(in binding);
			}
		}

		private void ClearProperty()
		{
			bool imageIsInline = this.m_ImageIsInline;
			if (!imageIsInline)
			{
				this.m_Image = null;
			}
		}

		private void SetScaleMode(ScaleMode mode)
		{
			bool flag = this.m_ScaleMode != mode;
			if (flag)
			{
				this.m_ScaleMode = mode;
				base.IncrementVersion(VersionChangeType.Repaint);
				base.NotifyPropertyChanged(in Image.scaleModeProperty);
			}
		}

		private void SetTintColor(Color color)
		{
			bool flag = this.m_TintColor != color;
			if (flag)
			{
				this.m_TintColor = color;
				base.IncrementVersion(VersionChangeType.Repaint);
				base.NotifyPropertyChanged(in Image.tintColorProperty);
			}
		}

		private void CalculateUV(Rect srcRect)
		{
			this.m_UV = new Rect(0f, 0f, 1f, 1f);
			Vector2 vector = Vector2.zero;
			Texture image = this.image;
			bool flag = image != null;
			if (flag)
			{
				vector = this.GetTextureDisplaySize(image);
			}
			VectorImage vectorImage = this.vectorImage;
			bool flag2 = vectorImage != null;
			if (flag2)
			{
				vector = vectorImage.size;
			}
			bool flag3 = vector != Vector2.zero;
			if (flag3)
			{
				this.m_UV.x = srcRect.x / vector.x;
				this.m_UV.width = srcRect.width / vector.x;
				this.m_UV.height = srcRect.height / vector.y;
				this.m_UV.y = 1f - this.m_UV.height - srcRect.y / vector.y;
			}
		}

		private Rect GetSourceRect()
		{
			Rect zero = Rect.zero;
			Vector2 vector = Vector2.zero;
			Texture image = this.image;
			bool flag = image != null;
			if (flag)
			{
				vector = this.GetTextureDisplaySize(image);
			}
			VectorImage vectorImage = this.vectorImage;
			bool flag2 = vectorImage != null;
			if (flag2)
			{
				vector = vectorImage.size;
			}
			bool flag3 = vector != Vector2.zero;
			if (flag3)
			{
				zero.x = this.uv.x * vector.x;
				zero.width = this.uv.width * vector.x;
				zero.y = (1f - this.uv.y - this.uv.height) * vector.y;
				zero.height = this.uv.height * vector.y;
			}
			return zero;
		}

		internal static readonly BindingId sourceProperty = "source";

		internal static readonly BindingId imageProperty = "image";

		internal static readonly BindingId spriteProperty = "sprite";

		internal static readonly BindingId vectorImageProperty = "vectorImage";

		internal static readonly BindingId sourceRectProperty = "sourceRect";

		internal static readonly BindingId uvProperty = "uv";

		internal static readonly BindingId scaleModeProperty = "scaleMode";

		internal static readonly BindingId tintColorProperty = "tintColor";

		private ScaleMode m_ScaleMode;

		private Object m_Image;

		private Rect m_UV;

		private Color m_TintColor;

		internal bool m_ImageIsInline;

		internal bool m_ScaleModeIsInline;

		internal bool m_TintColorIsInline;

		public static readonly string ussClassName = "unity-image";

		private static CustomStyleProperty<Texture2D> s_ImageProperty = new CustomStyleProperty<Texture2D>("--unity-image");

		private static CustomStyleProperty<Sprite> s_SpriteProperty = new CustomStyleProperty<Sprite>("--unity-image");

		private static CustomStyleProperty<VectorImage> s_VectorImageProperty = new CustomStyleProperty<VectorImage>("--unity-image");

		private static CustomStyleProperty<string> s_ScaleModeProperty = new CustomStyleProperty<string>("--unity-image-size");

		private static CustomStyleProperty<Color> s_TintColorProperty = new CustomStyleProperty<Color>("--unity-image-tint-color");

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Image.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("source", "source", null, Array.Empty<string>()),
					new UxmlAttributeNames("tintColor", "tint-color", null, Array.Empty<string>()),
					new UxmlAttributeNames("scaleMode", "scale-mode", null, Array.Empty<string>()),
					new UxmlAttributeNames("uv", "uv", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Image();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				Image image = (Image)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.source_UxmlAttributeFlags);
				if (flag)
				{
					image.source = this.source;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.tintColor_UxmlAttributeFlags);
				if (flag2)
				{
					image.tintColor = this.tintColor;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.uv_UxmlAttributeFlags);
				if (flag3)
				{
					image.uv = this.uv;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.scaleMode_UxmlAttributeFlags);
				if (flag4)
				{
					image.scaleMode = this.scaleMode;
				}
			}

			[ImageFieldValueDecorator("Source")]
			[SerializeField]
			private Object source;

			[SerializeField]
			private Color tintColor;

			[SerializeField]
			[Tooltip("The base texture coordinates of the Image relative to the bottom left corner.")]
			private Rect uv;

			[SerializeField]
			private ScaleMode scaleMode;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags source_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags tintColor_UxmlAttributeFlags;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags uv_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags scaleMode_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Image, Image.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}
		}
	}
}
