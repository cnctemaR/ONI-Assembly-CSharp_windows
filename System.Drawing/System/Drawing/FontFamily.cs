using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	public sealed class FontFamily : MarshalByRefObject, IDisposable
	{
		internal FontFamily(IntPtr fntfamily)
		{
			this.nativeFontFamily = fntfamily;
		}

		internal unsafe void refreshName()
		{
			if (this.nativeFontFamily == IntPtr.Zero)
			{
				return;
			}
			char* ptr = stackalloc char[(UIntPtr)64];
			GDIPlus.CheckStatus(GDIPlus.GdipGetFamilyName(this.nativeFontFamily, (IntPtr)((void*)ptr), 0));
			this.name = Marshal.PtrToStringUni((IntPtr)((void*)ptr));
		}

		~FontFamily()
		{
			this.Dispose();
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeFontFamily;
			}
		}

		internal IntPtr NativeFamily
		{
			get
			{
				return this.nativeFontFamily;
			}
		}

		public FontFamily(GenericFontFamilies genericFamily)
		{
			Status status;
			switch (genericFamily)
			{
			case GenericFontFamilies.Serif:
				status = GDIPlus.GdipGetGenericFontFamilySerif(out this.nativeFontFamily);
				goto IL_004D;
			case GenericFontFamilies.SansSerif:
				status = GDIPlus.GdipGetGenericFontFamilySansSerif(out this.nativeFontFamily);
				goto IL_004D;
			}
			status = GDIPlus.GdipGetGenericFontFamilyMonospace(out this.nativeFontFamily);
			IL_004D:
			GDIPlus.CheckStatus(status);
		}

		public FontFamily(string name)
			: this(name, null)
		{
		}

		public FontFamily(string name, FontCollection fontCollection)
		{
			IntPtr intPtr = ((fontCollection == null) ? IntPtr.Zero : fontCollection._nativeFontCollection);
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFamilyFromName(name, intPtr, out this.nativeFontFamily));
		}

		public string Name
		{
			get
			{
				if (this.nativeFontFamily == IntPtr.Zero)
				{
					throw new ArgumentException("Name", Locale.GetText("Object was disposed."));
				}
				if (this.name == null)
				{
					this.refreshName();
				}
				return this.name;
			}
		}

		public static FontFamily GenericMonospace
		{
			get
			{
				return new FontFamily(GenericFontFamilies.Monospace);
			}
		}

		public static FontFamily GenericSansSerif
		{
			get
			{
				return new FontFamily(GenericFontFamilies.SansSerif);
			}
		}

		public static FontFamily GenericSerif
		{
			get
			{
				return new FontFamily(GenericFontFamilies.Serif);
			}
		}

		public int GetCellAscent(FontStyle style)
		{
			short num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetCellAscent(this.nativeFontFamily, (int)style, out num));
			return (int)num;
		}

		public int GetCellDescent(FontStyle style)
		{
			short num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetCellDescent(this.nativeFontFamily, (int)style, out num));
			return (int)num;
		}

		public int GetEmHeight(FontStyle style)
		{
			short num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetEmHeight(this.nativeFontFamily, (int)style, out num));
			return (int)num;
		}

		public int GetLineSpacing(FontStyle style)
		{
			short num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetLineSpacing(this.nativeFontFamily, (int)style, out num));
			return (int)num;
		}

		[MonoDocumentationNote("When used with libgdiplus this method always return true (styles are created on demand).")]
		public bool IsStyleAvailable(FontStyle style)
		{
			bool flag;
			GDIPlus.CheckStatus(GDIPlus.GdipIsStyleAvailable(this.nativeFontFamily, (int)style, out flag));
			return flag;
		}

		public void Dispose()
		{
			if (this.nativeFontFamily != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteFontFamily(this.nativeFontFamily);
				this.nativeFontFamily = IntPtr.Zero;
				GC.SuppressFinalize(this);
				GDIPlus.CheckStatus(status);
			}
		}

		public override bool Equals(object obj)
		{
			FontFamily fontFamily = obj as FontFamily;
			return fontFamily != null && this.Name == fontFamily.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public static FontFamily[] Families
		{
			get
			{
				return new InstalledFontCollection().Families;
			}
		}

		public static FontFamily[] GetFamilies(Graphics graphics)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			return new InstalledFontCollection().Families;
		}

		[MonoLimitation("The language parameter is ignored. We always return the name using the default system language.")]
		public string GetName(int language)
		{
			return this.Name;
		}

		public override string ToString()
		{
			return "[FontFamily: Name=" + this.Name + "]";
		}

		private string name;

		private IntPtr nativeFontFamily = IntPtr.Zero;
	}
}
