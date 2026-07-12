using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Text
{
	public abstract class FontCollection : IDisposable
	{
		internal FontCollection()
		{
			this._nativeFontCollection = IntPtr.Zero;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public FontFamily[] Families
		{
			get
			{
				int num = 0;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetFontCollectionFamilyCount(new HandleRef(this, this._nativeFontCollection), out num));
				IntPtr[] array = new IntPtr[num];
				int num2 = 0;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetFontCollectionFamilyList(new HandleRef(this, this._nativeFontCollection), num, array, out num2));
				FontFamily[] array2 = new FontFamily[num2];
				for (int i = 0; i < num2; i++)
				{
					IntPtr intPtr;
					GDIPlus.GdipCloneFontFamily(new HandleRef(null, array[i]), out intPtr);
					array2[i] = new FontFamily(intPtr);
				}
				return array2;
			}
		}

		~FontCollection()
		{
			this.Dispose(false);
		}

		internal IntPtr _nativeFontCollection;
	}
}
