using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[Editor("System.Drawing.Design.MetafileEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[MonoTODO("Metafiles, both WMF and EMF formats, are only partially supported.")]
	[Serializable]
	public sealed class Metafile : Image
	{
		internal Metafile(IntPtr ptr)
		{
			this.nativeObject = ptr;
		}

		internal Metafile(IntPtr ptr, Stream stream)
		{
			if (GDIPlus.RunningOnWindows())
			{
				this.stream = stream;
			}
			this.nativeObject = ptr;
		}

		public Metafile(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentException("stream");
			}
			Status status;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, false);
				status = GDIPlus.GdipCreateMetafileFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, out this.nativeObject);
			}
			else
			{
				status = GDIPlus.GdipCreateMetafileFromStream(new ComIStreamWrapper(stream), out this.nativeObject);
			}
			GDIPlus.CheckStatus(status);
		}

		public Metafile(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (filename.Length == 0)
			{
				throw new ArgumentException("filename");
			}
			Status status = GDIPlus.GdipCreateMetafileFromFile(filename, out this.nativeObject);
			if (status == Status.GenericError)
			{
				throw new ExternalException("Couldn't load specified file.");
			}
			GDIPlus.CheckStatus(status);
		}

		public Metafile(IntPtr henhmetafile, bool deleteEmf)
		{
			Status status = GDIPlus.GdipCreateMetafileFromEmf(henhmetafile, deleteEmf, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Metafile(IntPtr referenceHdc, EmfType emfType)
			: this(referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, emfType, null)
		{
		}

		public Metafile(IntPtr referenceHdc, Rectangle frameRect)
			: this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr referenceHdc, RectangleF frameRect)
			: this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
		{
			Status status = GDIPlus.GdipCreateMetafileFromEmf(hmetafile, false, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Metafile(Stream stream, IntPtr referenceHdc)
			: this(stream, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc)
			: this(fileName, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr referenceHdc, EmfType emfType, string description)
			: this(referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, emfType, description)
		{
		}

		public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit)
			: this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
			: this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader, bool deleteWmf)
		{
			Status status = GDIPlus.GdipCreateMetafileFromEmf(hmetafile, deleteWmf, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Metafile(Stream stream, IntPtr referenceHdc, EmfType type)
			: this(stream, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, null)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect)
			: this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect)
			: this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, EmfType type)
			: this(fileName, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect)
			: this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect)
			: this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type)
			: this(referenceHdc, frameRect, frameUnit, type, null)
		{
		}

		public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type)
			: this(referenceHdc, frameRect, frameUnit, type, null)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, EmfType type, string description)
			: this(stream, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, description)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit)
			: this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
			: this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, EmfType type, string description)
			: this(fileName, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, description)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit)
			: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
			: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
		{
		}

		public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
		{
			Status status = GDIPlus.GdipRecordMetafileI(referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
		{
			Status status = GDIPlus.GdipRecordMetafile(referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type)
			: this(stream, referenceHdc, frameRect, frameUnit, type, null)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type)
			: this(stream, referenceHdc, frameRect, frameUnit, type, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type)
			: this(fileName, referenceHdc, frameRect, frameUnit, type, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, string description)
			: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, description)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type)
			: this(fileName, referenceHdc, frameRect, frameUnit, type, null)
		{
		}

		public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, string description)
			: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, description)
		{
		}

		public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
		{
			if (stream == null)
			{
				throw new NullReferenceException("stream");
			}
			Status status;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, false);
				status = GDIPlus.GdipRecordMetafileFromDelegateI_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			}
			else
			{
				status = GDIPlus.GdipRecordMetafileStreamI(new ComIStreamWrapper(stream), referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			}
			GDIPlus.CheckStatus(status);
		}

		public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
		{
			if (stream == null)
			{
				throw new NullReferenceException("stream");
			}
			Status status;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, false);
				status = GDIPlus.GdipRecordMetafileFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			}
			else
			{
				status = GDIPlus.GdipRecordMetafileStream(new ComIStreamWrapper(stream), referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			}
			GDIPlus.CheckStatus(status);
		}

		public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
		{
			Status status = GDIPlus.GdipRecordMetafileFileNameI(fileName, referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
		{
			Status status = GDIPlus.GdipRecordMetafileFileName(fileName, referenceHdc, type, ref frameRect, frameUnit, description, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public IntPtr GetHenhmetafile()
		{
			return this.nativeObject;
		}

		[MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
		public MetafileHeader GetMetafileHeader()
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
			MetafileHeader metafileHeader;
			try
			{
				Status status = GDIPlus.GdipGetMetafileHeaderFromMetafile(this.nativeObject, intPtr);
				GDIPlus.CheckStatus(status);
				metafileHeader = new MetafileHeader(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return metafileHeader;
		}

		[MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
			MetafileHeader metafileHeader;
			try
			{
				Status status = GDIPlus.GdipGetMetafileHeaderFromEmf(henhmetafile, intPtr);
				GDIPlus.CheckStatus(status);
				metafileHeader = new MetafileHeader(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return metafileHeader;
		}

		[MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader(Stream stream)
		{
			if (stream == null)
			{
				throw new NullReferenceException("stream");
			}
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
			MetafileHeader metafileHeader;
			try
			{
				Status status;
				if (GDIPlus.RunningOnUnix())
				{
					GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, false);
					status = GDIPlus.GdipGetMetafileHeaderFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, intPtr);
				}
				else
				{
					status = GDIPlus.GdipGetMetafileHeaderFromStream(new ComIStreamWrapper(stream), intPtr);
				}
				GDIPlus.CheckStatus(status);
				metafileHeader = new MetafileHeader(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return metafileHeader;
		}

		[MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
			MetafileHeader metafileHeader;
			try
			{
				Status status = GDIPlus.GdipGetMetafileHeaderFromFile(fileName, intPtr);
				GDIPlus.CheckStatus(status);
				metafileHeader = new MetafileHeader(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return metafileHeader;
		}

		[MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile, WmfPlaceableFileHeader wmfHeader)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
			MetafileHeader metafileHeader;
			try
			{
				Status status = GDIPlus.GdipGetMetafileHeaderFromEmf(henhmetafile, intPtr);
				GDIPlus.CheckStatus(status);
				metafileHeader = new MetafileHeader(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return metafileHeader;
		}

		[MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
		public void PlayRecord(EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
		{
			Status status = GDIPlus.GdipPlayMetafileRecord(this.nativeObject, recordType, flags, dataSize, data);
			GDIPlus.CheckStatus(status);
		}
	}
}
