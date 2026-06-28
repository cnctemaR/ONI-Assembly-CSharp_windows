using System;
using System.IO;

namespace System.Drawing
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ToolboxBitmapAttribute : Attribute
	{
		private ToolboxBitmapAttribute()
		{
		}

		public ToolboxBitmapAttribute(string imageFile)
		{
		}

		public ToolboxBitmapAttribute(Type t)
		{
			this.smallImage = ToolboxBitmapAttribute.GetImageFromResource(t, null, false);
		}

		public ToolboxBitmapAttribute(Type t, string name)
		{
			this.smallImage = ToolboxBitmapAttribute.GetImageFromResource(t, name, false);
		}

		public override bool Equals(object value)
		{
			return value is ToolboxBitmapAttribute && (value == this || ((ToolboxBitmapAttribute)value).smallImage == this.smallImage);
		}

		public override int GetHashCode()
		{
			return this.smallImage.GetHashCode() ^ this.bigImage.GetHashCode();
		}

		public Image GetImage(object component)
		{
			return this.GetImage(component.GetType(), null, false);
		}

		public Image GetImage(object component, bool large)
		{
			return this.GetImage(component.GetType(), null, large);
		}

		public Image GetImage(Type type)
		{
			return this.GetImage(type, null, false);
		}

		public Image GetImage(Type type, bool large)
		{
			return this.GetImage(type, null, large);
		}

		public Image GetImage(Type type, string imgName, bool large)
		{
			if (this.smallImage == null)
			{
				this.smallImage = ToolboxBitmapAttribute.GetImageFromResource(type, imgName, false);
			}
			if (large)
			{
				if (this.bigImage == null)
				{
					this.bigImage = new Bitmap(this.smallImage, 32, 32);
				}
				return this.bigImage;
			}
			return this.smallImage;
		}

		public static Image GetImageFromResource(Type t, string imageName, bool large)
		{
			if (imageName == null)
			{
				imageName = t.Name + ".bmp";
			}
			Image image;
			try
			{
				Bitmap bitmap;
				using (Stream manifestResourceStream = t.Assembly.GetManifestResourceStream(t.Namespace + "." + imageName))
				{
					if (manifestResourceStream == null)
					{
						return null;
					}
					bitmap = new Bitmap(manifestResourceStream, false);
				}
				if (large)
				{
					image = new Bitmap(bitmap, 32, 32);
				}
				else
				{
					image = bitmap;
				}
			}
			catch
			{
				image = null;
			}
			return image;
		}

		private Image smallImage;

		private Image bigImage;

		public static readonly ToolboxBitmapAttribute Default = new ToolboxBitmapAttribute();
	}
}
