using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	public sealed class ImageCodecInfo
	{
		internal ImageCodecInfo()
		{
		}

		public static ImageCodecInfo[] GetImageDecoders()
		{
			GdipImageCodecInfo gdipImageCodecInfo = default(GdipImageCodecInfo);
			int num;
			int num2;
			Status status = GDIPlus.GdipGetImageDecodersSize(out num, out num2);
			GDIPlus.CheckStatus(status);
			ImageCodecInfo[] array = new ImageCodecInfo[num];
			if (num == 0)
			{
				return array;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num2);
			try
			{
				status = GDIPlus.GdipGetImageDecoders(num, num2, intPtr);
				GDIPlus.CheckStatus(status);
				int num3 = Marshal.SizeOf(gdipImageCodecInfo);
				IntPtr intPtr2 = intPtr;
				int i = 0;
				while (i < num)
				{
					gdipImageCodecInfo = (GdipImageCodecInfo)Marshal.PtrToStructure(intPtr2, typeof(GdipImageCodecInfo));
					array[i] = new ImageCodecInfo();
					GdipImageCodecInfo.MarshalTo(gdipImageCodecInfo, array[i]);
					i++;
					intPtr2 = new IntPtr(intPtr2.ToInt64() + (long)num3);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return array;
		}

		public static ImageCodecInfo[] GetImageEncoders()
		{
			GdipImageCodecInfo gdipImageCodecInfo = default(GdipImageCodecInfo);
			int num;
			int num2;
			Status status = GDIPlus.GdipGetImageEncodersSize(out num, out num2);
			GDIPlus.CheckStatus(status);
			ImageCodecInfo[] array = new ImageCodecInfo[num];
			if (num == 0)
			{
				return array;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num2);
			try
			{
				status = GDIPlus.GdipGetImageEncoders(num, num2, intPtr);
				GDIPlus.CheckStatus(status);
				int num3 = Marshal.SizeOf(gdipImageCodecInfo);
				IntPtr intPtr2 = intPtr;
				int i = 0;
				while (i < num)
				{
					gdipImageCodecInfo = (GdipImageCodecInfo)Marshal.PtrToStructure(intPtr2, typeof(GdipImageCodecInfo));
					array[i] = new ImageCodecInfo();
					GdipImageCodecInfo.MarshalTo(gdipImageCodecInfo, array[i]);
					i++;
					intPtr2 = new IntPtr(intPtr2.ToInt64() + (long)num3);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return array;
		}

		public Guid Clsid
		{
			get
			{
				return this.clsid;
			}
			set
			{
				this.clsid = value;
			}
		}

		public string CodecName
		{
			get
			{
				return this.codecName;
			}
			set
			{
				this.codecName = value;
			}
		}

		public string DllName
		{
			get
			{
				return this.dllName;
			}
			set
			{
				this.dllName = value;
			}
		}

		public string FilenameExtension
		{
			get
			{
				return this.filenameExtension;
			}
			set
			{
				this.filenameExtension = value;
			}
		}

		public ImageCodecFlags Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public string FormatDescription
		{
			get
			{
				return this.formatDescription;
			}
			set
			{
				this.formatDescription = value;
			}
		}

		public Guid FormatID
		{
			get
			{
				return this.formatID;
			}
			set
			{
				this.formatID = value;
			}
		}

		public string MimeType
		{
			get
			{
				return this.mimeType;
			}
			set
			{
				this.mimeType = value;
			}
		}

		[CLSCompliant(false)]
		public byte[][] SignatureMasks
		{
			get
			{
				return this.signatureMasks;
			}
			set
			{
				this.signatureMasks = value;
			}
		}

		[CLSCompliant(false)]
		public byte[][] SignaturePatterns
		{
			get
			{
				return this.signaturePatterns;
			}
			set
			{
				this.signaturePatterns = value;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		private Guid clsid;

		private string codecName;

		private string dllName;

		private string filenameExtension;

		private ImageCodecFlags flags;

		private string formatDescription;

		private Guid formatID;

		private string mimeType;

		private byte[][] signatureMasks;

		private byte[][] signaturePatterns;

		private int version;
	}
}
