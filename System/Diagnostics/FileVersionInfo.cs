using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;

namespace System.Diagnostics
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class FileVersionInfo
	{
		private FileVersionInfo()
		{
			this.comments = null;
			this.companyname = null;
			this.filedescription = null;
			this.filename = null;
			this.fileversion = null;
			this.internalname = null;
			this.language = null;
			this.legalcopyright = null;
			this.legaltrademarks = null;
			this.originalfilename = null;
			this.privatebuild = null;
			this.productname = null;
			this.productversion = null;
			this.specialbuild = null;
			this.isdebug = false;
			this.ispatched = false;
			this.isprerelease = false;
			this.isprivatebuild = false;
			this.isspecialbuild = false;
			this.filemajorpart = 0;
			this.fileminorpart = 0;
			this.filebuildpart = 0;
			this.fileprivatepart = 0;
			this.productmajorpart = 0;
			this.productminorpart = 0;
			this.productbuildpart = 0;
			this.productprivatepart = 0;
		}

		public string Comments
		{
			get
			{
				return this.comments;
			}
		}

		public string CompanyName
		{
			get
			{
				return this.companyname;
			}
		}

		public int FileBuildPart
		{
			get
			{
				return this.filebuildpart;
			}
		}

		public string FileDescription
		{
			get
			{
				return this.filedescription;
			}
		}

		public int FileMajorPart
		{
			get
			{
				return this.filemajorpart;
			}
		}

		public int FileMinorPart
		{
			get
			{
				return this.fileminorpart;
			}
		}

		public string FileName
		{
			get
			{
				return this.filename;
			}
		}

		public int FilePrivatePart
		{
			get
			{
				return this.fileprivatepart;
			}
		}

		public string FileVersion
		{
			get
			{
				return this.fileversion;
			}
		}

		public string InternalName
		{
			get
			{
				return this.internalname;
			}
		}

		public bool IsDebug
		{
			get
			{
				return this.isdebug;
			}
		}

		public bool IsPatched
		{
			get
			{
				return this.ispatched;
			}
		}

		public bool IsPreRelease
		{
			get
			{
				return this.isprerelease;
			}
		}

		public bool IsPrivateBuild
		{
			get
			{
				return this.isprivatebuild;
			}
		}

		public bool IsSpecialBuild
		{
			get
			{
				return this.isspecialbuild;
			}
		}

		public string Language
		{
			get
			{
				return this.language;
			}
		}

		public string LegalCopyright
		{
			get
			{
				return this.legalcopyright;
			}
		}

		public string LegalTrademarks
		{
			get
			{
				return this.legaltrademarks;
			}
		}

		public string OriginalFilename
		{
			get
			{
				return this.originalfilename;
			}
		}

		public string PrivateBuild
		{
			get
			{
				return this.privatebuild;
			}
		}

		public int ProductBuildPart
		{
			get
			{
				return this.productbuildpart;
			}
		}

		public int ProductMajorPart
		{
			get
			{
				return this.productmajorpart;
			}
		}

		public int ProductMinorPart
		{
			get
			{
				return this.productminorpart;
			}
		}

		public string ProductName
		{
			get
			{
				return this.productname;
			}
		}

		public int ProductPrivatePart
		{
			get
			{
				return this.productprivatepart;
			}
		}

		public string ProductVersion
		{
			get
			{
				return this.productversion;
			}
		}

		public string SpecialBuild
		{
			get
			{
				return this.specialbuild;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void GetVersionInfo_icall(char* fileName, int fileName_length);

		private unsafe void GetVersionInfo_internal(string fileName)
		{
			fixed (string text = fileName)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.GetVersionInfo_icall(ptr, (fileName != null) ? fileName.Length : 0);
			}
		}

		public static FileVersionInfo GetVersionInfo(string fileName)
		{
			if (!File.Exists(Path.GetFullPath(fileName)))
			{
				throw new FileNotFoundException(fileName);
			}
			FileVersionInfo fileVersionInfo = new FileVersionInfo();
			fileVersionInfo.GetVersionInfo_internal(fileName);
			return fileVersionInfo;
		}

		private static void AppendFormat(StringBuilder sb, string format, params object[] args)
		{
			sb.AppendFormat(format, args);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			FileVersionInfo.AppendFormat(stringBuilder, "File:             {0}{1}", new object[]
			{
				this.FileName,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "InternalName:     {0}{1}", new object[]
			{
				this.internalname,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "OriginalFilename: {0}{1}", new object[]
			{
				this.originalfilename,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "FileVersion:      {0}{1}", new object[]
			{
				this.fileversion,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "FileDescription:  {0}{1}", new object[]
			{
				this.filedescription,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Product:          {0}{1}", new object[]
			{
				this.productname,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "ProductVersion:   {0}{1}", new object[]
			{
				this.productversion,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Debug:            {0}{1}", new object[]
			{
				this.isdebug,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Patched:          {0}{1}", new object[]
			{
				this.ispatched,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "PreRelease:       {0}{1}", new object[]
			{
				this.isprerelease,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "PrivateBuild:     {0}{1}", new object[]
			{
				this.isprivatebuild,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "SpecialBuild:     {0}{1}", new object[]
			{
				this.isspecialbuild,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Language          {0}{1}", new object[]
			{
				this.language,
				Environment.NewLine
			});
			return stringBuilder.ToString();
		}

		private string comments;

		private string companyname;

		private string filedescription;

		private string filename;

		private string fileversion;

		private string internalname;

		private string language;

		private string legalcopyright;

		private string legaltrademarks;

		private string originalfilename;

		private string privatebuild;

		private string productname;

		private string productversion;

		private string specialbuild;

		private bool isdebug;

		private bool ispatched;

		private bool isprerelease;

		private bool isprivatebuild;

		private bool isspecialbuild;

		private int filemajorpart;

		private int fileminorpart;

		private int filebuildpart;

		private int fileprivatepart;

		private int productmajorpart;

		private int productminorpart;

		private int productbuildpart;

		private int productprivatepart;
	}
}
