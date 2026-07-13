using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct Background : IEquatable<Background>
	{
		public Texture2D texture
		{
			get
			{
				return this.m_Texture;
			}
			set
			{
				bool flag = this.m_Texture == value;
				if (!flag)
				{
					this.m_Texture = value;
					this.m_Sprite = null;
					this.m_RenderTexture = null;
					this.m_VectorImage = null;
				}
			}
		}

		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				bool flag = this.m_Sprite == value;
				if (!flag)
				{
					this.m_Texture = null;
					this.m_Sprite = value;
					this.m_RenderTexture = null;
					this.m_VectorImage = null;
				}
			}
		}

		public RenderTexture renderTexture
		{
			get
			{
				return this.m_RenderTexture;
			}
			set
			{
				bool flag = this.m_RenderTexture == value;
				if (!flag)
				{
					this.m_Texture = null;
					this.m_Sprite = null;
					this.m_RenderTexture = value;
					this.m_VectorImage = null;
				}
			}
		}

		public VectorImage vectorImage
		{
			get
			{
				return this.m_VectorImage;
			}
			set
			{
				bool flag = this.vectorImage == value;
				if (!flag)
				{
					this.m_Texture = null;
					this.m_Sprite = null;
					this.m_RenderTexture = null;
					this.m_VectorImage = value;
				}
			}
		}

		[Obsolete("Use Background.FromTexture2D instead")]
		public Background(Texture2D t)
		{
			this.m_Texture = t;
			this.m_Sprite = null;
			this.m_RenderTexture = null;
			this.m_VectorImage = null;
		}

		public static Background FromTexture2D(Texture2D t)
		{
			return new Background
			{
				texture = t
			};
		}

		public static Background FromRenderTexture(RenderTexture rt)
		{
			return new Background
			{
				renderTexture = rt
			};
		}

		public static Background FromSprite(Sprite s)
		{
			return new Background
			{
				sprite = s
			};
		}

		public static Background FromVectorImage(VectorImage vi)
		{
			return new Background
			{
				vectorImage = vi
			};
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static Background FromObject(object obj)
		{
			Texture2D texture2D = obj as Texture2D;
			bool flag = texture2D != null;
			Background background;
			if (flag)
			{
				background = Background.FromTexture2D(texture2D);
			}
			else
			{
				RenderTexture renderTexture = obj as RenderTexture;
				bool flag2 = renderTexture != null;
				if (flag2)
				{
					background = Background.FromRenderTexture(renderTexture);
				}
				else
				{
					Sprite sprite = obj as Sprite;
					bool flag3 = sprite != null;
					if (flag3)
					{
						background = Background.FromSprite(sprite);
					}
					else
					{
						VectorImage vectorImage = obj as VectorImage;
						bool flag4 = vectorImage != null;
						if (flag4)
						{
							background = Background.FromVectorImage(vectorImage);
						}
						else
						{
							background = default(Background);
						}
					}
				}
			}
			return background;
		}

		internal static IEnumerable<Type> allowedAssetTypes
		{
			get
			{
				yield return typeof(Texture2D);
				yield return typeof(RenderTexture);
				yield return typeof(Sprite);
				yield return typeof(VectorImage);
				yield break;
			}
		}

		public Object GetSelectedImage()
		{
			bool flag = this.texture != null;
			Object @object;
			if (flag)
			{
				@object = this.texture;
			}
			else
			{
				bool flag2 = this.sprite != null;
				if (flag2)
				{
					@object = this.sprite;
				}
				else
				{
					bool flag3 = this.renderTexture != null;
					if (flag3)
					{
						@object = this.renderTexture;
					}
					else
					{
						bool flag4 = this.vectorImage != null;
						if (flag4)
						{
							@object = this.vectorImage;
						}
						else
						{
							@object = null;
						}
					}
				}
			}
			return @object;
		}

		public bool IsEmpty()
		{
			return this.texture == null && this.sprite == null && this.vectorImage == null && this.renderTexture == null;
		}

		public static bool operator ==(Background lhs, Background rhs)
		{
			return lhs.texture == rhs.texture && lhs.sprite == rhs.sprite && lhs.renderTexture == rhs.renderTexture && lhs.vectorImage == rhs.vectorImage;
		}

		public static bool operator !=(Background lhs, Background rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator Background(Texture2D v)
		{
			return Background.FromTexture2D(v);
		}

		public bool Equals(Background other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is Background);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Background background = (Background)obj;
				flag2 = background == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = 851985039;
			bool flag = this.texture != null;
			if (flag)
			{
				num = num * -1521134295 + this.texture.GetHashCode();
			}
			bool flag2 = this.sprite != null;
			if (flag2)
			{
				num = num * -1521134295 + this.sprite.GetHashCode();
			}
			bool flag3 = this.renderTexture != null;
			if (flag3)
			{
				num = num * -1521134295 + this.renderTexture.GetHashCode();
			}
			bool flag4 = this.vectorImage != null;
			if (flag4)
			{
				num = num * -1521134295 + this.vectorImage.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			bool flag = this.texture != null;
			string text;
			if (flag)
			{
				text = this.texture.ToString();
			}
			else
			{
				bool flag2 = this.sprite != null;
				if (flag2)
				{
					text = this.sprite.ToString();
				}
				else
				{
					bool flag3 = this.renderTexture != null;
					if (flag3)
					{
						text = this.renderTexture.ToString();
					}
					else
					{
						bool flag4 = this.vectorImage != null;
						if (flag4)
						{
							text = this.vectorImage.ToString();
						}
						else
						{
							text = "";
						}
					}
				}
			}
			return text;
		}

		[SerializeField]
		private Texture2D m_Texture;

		[SerializeField]
		private Sprite m_Sprite;

		[SerializeField]
		private RenderTexture m_RenderTexture;

		[SerializeField]
		private VectorImage m_VectorImage;

		internal class PropertyBag : ContainerPropertyBag<Background>
		{
			public PropertyBag()
			{
				base.AddProperty<Texture2D>(new Background.PropertyBag.TextureProperty());
				base.AddProperty<Sprite>(new Background.PropertyBag.SpriteProperty());
				base.AddProperty<RenderTexture>(new Background.PropertyBag.RenderTextureProperty());
				base.AddProperty<VectorImage>(new Background.PropertyBag.VectorImageProperty());
			}

			private class TextureProperty : Property<Background, Texture2D>
			{
				public override string Name { get; } = "texture";

				public override bool IsReadOnly { get; } = false;

				public override Texture2D GetValue(ref Background container)
				{
					return container.texture;
				}

				public override void SetValue(ref Background container, Texture2D value)
				{
					container.texture = value;
				}
			}

			private class SpriteProperty : Property<Background, Sprite>
			{
				public override string Name { get; } = "sprite";

				public override bool IsReadOnly { get; } = false;

				public override Sprite GetValue(ref Background container)
				{
					return container.sprite;
				}

				public override void SetValue(ref Background container, Sprite value)
				{
					container.sprite = value;
				}
			}

			private class RenderTextureProperty : Property<Background, RenderTexture>
			{
				public override string Name { get; } = "renderTexture";

				public override bool IsReadOnly { get; } = false;

				public override RenderTexture GetValue(ref Background container)
				{
					return container.renderTexture;
				}

				public override void SetValue(ref Background container, RenderTexture value)
				{
					container.renderTexture = value;
				}
			}

			private class VectorImageProperty : Property<Background, VectorImage>
			{
				public override string Name { get; } = "vectorImage";

				public override bool IsReadOnly { get; } = false;

				public override VectorImage GetValue(ref Background container)
				{
					return container.vectorImage;
				}

				public override void SetValue(ref Background container, VectorImage value)
				{
					container.vectorImage = value;
				}
			}
		}
	}
}
