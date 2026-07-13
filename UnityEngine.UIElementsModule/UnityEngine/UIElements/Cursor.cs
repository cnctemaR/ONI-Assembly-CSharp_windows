using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct Cursor : IEquatable<Cursor>
	{
		public Texture2D texture
		{
			get
			{
				return this.m_Texture;
			}
			set
			{
				this.m_Texture = value;
			}
		}

		public Vector2 hotspot
		{
			get
			{
				return this.m_Hotspot;
			}
			set
			{
				this.m_Hotspot = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int defaultCursorId { readonly get; set; }

		public override bool Equals(object obj)
		{
			return obj is Cursor && this.Equals((Cursor)obj);
		}

		public bool Equals(Cursor other)
		{
			return EqualityComparer<Texture2D>.Default.Equals(this.texture, other.texture) && this.hotspot.Equals(other.hotspot) && this.defaultCursorId == other.defaultCursorId;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(this.texture);
			num = num * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.hotspot);
			return num * -1521134295 + this.defaultCursorId.GetHashCode();
		}

		internal static IEnumerable<Type> allowedAssetTypes
		{
			get
			{
				yield return typeof(Texture2D);
				yield break;
			}
		}

		public static bool operator ==(Cursor style1, Cursor style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(Cursor style1, Cursor style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("texture={0}, hotspot={1}", this.texture, this.hotspot);
		}

		[SerializeField]
		private Texture2D m_Texture;

		[SerializeField]
		private Vector2 m_Hotspot;

		internal class PropertyBag : ContainerPropertyBag<Cursor>
		{
			public PropertyBag()
			{
				base.AddProperty<Texture2D>(new Cursor.PropertyBag.TextureProperty());
				base.AddProperty<Vector2>(new Cursor.PropertyBag.HotspotProperty());
				base.AddProperty<int>(new Cursor.PropertyBag.DefaultCursorIdProperty());
			}

			private class TextureProperty : Property<Cursor, Texture2D>
			{
				public override string Name { get; } = "texture";

				public override bool IsReadOnly { get; } = false;

				public override Texture2D GetValue(ref Cursor container)
				{
					return container.texture;
				}

				public override void SetValue(ref Cursor container, Texture2D value)
				{
					container.texture = value;
				}
			}

			private class HotspotProperty : Property<Cursor, Vector2>
			{
				public override string Name { get; } = "hotspot";

				public override bool IsReadOnly { get; } = false;

				public override Vector2 GetValue(ref Cursor container)
				{
					return container.hotspot;
				}

				public override void SetValue(ref Cursor container, Vector2 value)
				{
					container.hotspot = value;
				}
			}

			private class DefaultCursorIdProperty : Property<Cursor, int>
			{
				public override string Name { get; } = "defaultCursorId";

				public override bool IsReadOnly { get; } = false;

				public override int GetValue(ref Cursor container)
				{
					return container.defaultCursorId;
				}

				public override void SetValue(ref Cursor container, int value)
				{
					container.defaultCursorId = value;
				}
			}
		}
	}
}
