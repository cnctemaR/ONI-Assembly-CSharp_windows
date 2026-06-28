using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class MetafileHeader
	{
		internal MetafileHeader(IntPtr henhmetafile)
		{
			Marshal.PtrToStructure(henhmetafile, this);
		}

		[MonoTODO("always returns false")]
		public bool IsDisplay()
		{
			return false;
		}

		public bool IsEmf()
		{
			return this.Type == MetafileType.Emf;
		}

		public bool IsEmfOrEmfPlus()
		{
			return this.Type >= MetafileType.Emf;
		}

		public bool IsEmfPlus()
		{
			return this.Type >= MetafileType.EmfPlusOnly;
		}

		public bool IsEmfPlusDual()
		{
			return this.Type == MetafileType.EmfPlusDual;
		}

		public bool IsEmfPlusOnly()
		{
			return this.Type == MetafileType.EmfPlusOnly;
		}

		public bool IsWmf()
		{
			return this.Type <= MetafileType.WmfPlaceable;
		}

		public bool IsWmfPlaceable()
		{
			return this.Type == MetafileType.WmfPlaceable;
		}

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.header.x, this.header.y, this.header.width, this.header.height);
			}
		}

		public float DpiX
		{
			get
			{
				return this.header.dpi_x;
			}
		}

		public float DpiY
		{
			get
			{
				return this.header.dpi_y;
			}
		}

		public int EmfPlusHeaderSize
		{
			get
			{
				return this.header.emfplus_header_size;
			}
		}

		public int LogicalDpiX
		{
			get
			{
				return this.header.logical_dpi_x;
			}
		}

		public int LogicalDpiY
		{
			get
			{
				return this.header.logical_dpi_y;
			}
		}

		public int MetafileSize
		{
			get
			{
				return this.header.size;
			}
		}

		public MetafileType Type
		{
			get
			{
				return this.header.type;
			}
		}

		public int Version
		{
			get
			{
				return this.header.version;
			}
		}

		public MetaHeader WmfHeader
		{
			get
			{
				if (this.IsWmf())
				{
					return new MetaHeader(this.header.wmf_header);
				}
				throw new ArgumentException("WmfHeader only available on WMF files.");
			}
		}

		private MonoMetafileHeader header;
	}
}
