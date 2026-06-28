using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Drawing
{
	public sealed class TextureBrush : Brush
	{
		internal TextureBrush(IntPtr ptr)
			: base(ptr)
		{
		}

		public TextureBrush(Image bitmap)
			: this(bitmap, WrapMode.Tile)
		{
		}

		public TextureBrush(Image image, Rectangle dstRect)
			: this(image, WrapMode.Tile, dstRect)
		{
		}

		public TextureBrush(Image image, RectangleF dstRect)
			: this(image, WrapMode.Tile, dstRect)
		{
		}

		public TextureBrush(Image image, WrapMode wrapMode)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipCreateTexture(image.nativeObject, wrapMode, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		[MonoLimitation("ImageAttributes are ignored when using libgdiplus")]
		public TextureBrush(Image image, Rectangle dstRect, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			IntPtr intPtr = ((imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero);
			Status status = GDIPlus.GdipCreateTextureIAI(image.nativeObject, intPtr, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		[MonoLimitation("ImageAttributes are ignored when using libgdiplus")]
		public TextureBrush(Image image, RectangleF dstRect, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			IntPtr intPtr = ((imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero);
			Status status = GDIPlus.GdipCreateTextureIA(image.nativeObject, intPtr, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public TextureBrush(Image image, WrapMode wrapMode, Rectangle dstRect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipCreateTexture2I(image.nativeObject, wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipCreateTexture2(image.nativeObject, wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Image Image
		{
			get
			{
				if (this.nativeObject == IntPtr.Zero)
				{
					throw new ArgumentException("Object was disposed");
				}
				IntPtr intPtr;
				Status status = GDIPlus.GdipGetTextureImage(this.nativeObject, out intPtr);
				GDIPlus.CheckStatus(status);
				return new Bitmap(intPtr);
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				Status status = GDIPlus.GdipGetTextureTransform(this.nativeObject, matrix.nativeMatrix);
				GDIPlus.CheckStatus(status);
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Transform");
				}
				Status status = GDIPlus.GdipSetTextureTransform(this.nativeObject, value.nativeMatrix);
				GDIPlus.CheckStatus(status);
			}
		}

		public WrapMode WrapMode
		{
			get
			{
				WrapMode wrapMode;
				Status status = GDIPlus.GdipGetTextureWrapMode(this.nativeObject, out wrapMode);
				GDIPlus.CheckStatus(status);
				return wrapMode;
			}
			set
			{
				if (value < WrapMode.Tile || value > WrapMode.Clamp)
				{
					throw new InvalidEnumArgumentException("WrapMode");
				}
				Status status = GDIPlus.GdipSetTextureWrapMode(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public override object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBrush(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new TextureBrush(intPtr);
		}

		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			Status status = GDIPlus.GdipMultiplyTextureTransform(this.nativeObject, matrix.nativeMatrix, order);
			GDIPlus.CheckStatus(status);
		}

		public void ResetTransform()
		{
			Status status = GDIPlus.GdipResetTextureTransform(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			Status status = GDIPlus.GdipRotateTextureTransform(this.nativeObject, angle, order);
			GDIPlus.CheckStatus(status);
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipScaleTextureTransform(this.nativeObject, sx, sy, order);
			GDIPlus.CheckStatus(status);
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipTranslateTextureTransform(this.nativeObject, dx, dy, order);
			GDIPlus.CheckStatus(status);
		}
	}
}
