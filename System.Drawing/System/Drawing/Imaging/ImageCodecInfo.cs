using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	public sealed class ImageCodecInfo
	{
		internal ImageCodecInfo()
		{
		}

		public Guid Clsid
		{
			get
			{
				return this._clsid;
			}
			set
			{
				this._clsid = value;
			}
		}

		public Guid FormatID
		{
			get
			{
				return this._formatID;
			}
			set
			{
				this._formatID = value;
			}
		}

		public string CodecName
		{
			get
			{
				return this._codecName;
			}
			set
			{
				this._codecName = value;
			}
		}

		public string DllName
		{
			get
			{
				return this._dllName;
			}
			set
			{
				this._dllName = value;
			}
		}

		public string FormatDescription
		{
			get
			{
				return this._formatDescription;
			}
			set
			{
				this._formatDescription = value;
			}
		}

		public string FilenameExtension
		{
			get
			{
				return this._filenameExtension;
			}
			set
			{
				this._filenameExtension = value;
			}
		}

		public string MimeType
		{
			get
			{
				return this._mimeType;
			}
			set
			{
				this._mimeType = value;
			}
		}

		public ImageCodecFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				this._flags = value;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		[CLSCompliant(false)]
		public byte[][] SignaturePatterns
		{
			get
			{
				return this._signaturePatterns;
			}
			set
			{
				this._signaturePatterns = value;
			}
		}

		[CLSCompliant(false)]
		public byte[][] SignatureMasks
		{
			get
			{
				return this._signatureMasks;
			}
			set
			{
				this._signatureMasks = value;
			}
		}

		public static ImageCodecInfo[] GetImageDecoders()
		{
			int num2;
			int num3;
			int num = GDIPlus.GdipGetImageDecodersSize(out num2, out num3);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num3);
			ImageCodecInfo[] array;
			try
			{
				num = GDIPlus.GdipGetImageDecoders(num2, num3, intPtr);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				array = ImageCodecInfo.ConvertFromMemory(intPtr, num2);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return array;
		}

		public static ImageCodecInfo[] GetImageEncoders()
		{
			int num2;
			int num3;
			int num = GDIPlus.GdipGetImageEncodersSize(out num2, out num3);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num3);
			ImageCodecInfo[] array;
			try
			{
				num = GDIPlus.GdipGetImageEncoders(num2, num3, intPtr);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				array = ImageCodecInfo.ConvertFromMemory(intPtr, num2);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return array;
		}

		private static ImageCodecInfo[] ConvertFromMemory(IntPtr memoryStart, int numCodecs)
		{
			ImageCodecInfo[] array = new ImageCodecInfo[numCodecs];
			for (int i = 0; i < numCodecs; i++)
			{
				IntPtr intPtr = (IntPtr)((long)memoryStart + (long)(Marshal.SizeOf(typeof(ImageCodecInfoPrivate)) * i));
				ImageCodecInfoPrivate imageCodecInfoPrivate = new ImageCodecInfoPrivate();
				Marshal.PtrToStructure<ImageCodecInfoPrivate>(intPtr, imageCodecInfoPrivate);
				array[i] = new ImageCodecInfo();
				array[i].Clsid = imageCodecInfoPrivate.Clsid;
				array[i].FormatID = imageCodecInfoPrivate.FormatID;
				array[i].CodecName = Marshal.PtrToStringUni(imageCodecInfoPrivate.CodecName);
				array[i].DllName = Marshal.PtrToStringUni(imageCodecInfoPrivate.DllName);
				array[i].FormatDescription = Marshal.PtrToStringUni(imageCodecInfoPrivate.FormatDescription);
				array[i].FilenameExtension = Marshal.PtrToStringUni(imageCodecInfoPrivate.FilenameExtension);
				array[i].MimeType = Marshal.PtrToStringUni(imageCodecInfoPrivate.MimeType);
				array[i].Flags = (ImageCodecFlags)imageCodecInfoPrivate.Flags;
				array[i].Version = imageCodecInfoPrivate.Version;
				array[i].SignaturePatterns = new byte[imageCodecInfoPrivate.SigCount][];
				array[i].SignatureMasks = new byte[imageCodecInfoPrivate.SigCount][];
				for (int j = 0; j < imageCodecInfoPrivate.SigCount; j++)
				{
					array[i].SignaturePatterns[j] = new byte[imageCodecInfoPrivate.SigSize];
					array[i].SignatureMasks[j] = new byte[imageCodecInfoPrivate.SigSize];
					Marshal.Copy((IntPtr)((long)imageCodecInfoPrivate.SigMask + (long)(j * imageCodecInfoPrivate.SigSize)), array[i].SignatureMasks[j], 0, imageCodecInfoPrivate.SigSize);
					Marshal.Copy((IntPtr)((long)imageCodecInfoPrivate.SigPattern + (long)(j * imageCodecInfoPrivate.SigSize)), array[i].SignaturePatterns[j], 0, imageCodecInfoPrivate.SigSize);
				}
			}
			return array;
		}

		private Guid _clsid;

		private Guid _formatID;

		private string _codecName;

		private string _dllName;

		private string _formatDescription;

		private string _filenameExtension;

		private string _mimeType;

		private ImageCodecFlags _flags;

		private int _version;

		private byte[][] _signaturePatterns;

		private byte[][] _signatureMasks;
	}
}
