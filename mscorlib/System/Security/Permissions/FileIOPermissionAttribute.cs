using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class FileIOPermissionAttribute : CodeAccessSecurityAttribute
	{
		public FileIOPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		[Obsolete("use newer properties")]
		public string All
		{
			get
			{
				throw new NotSupportedException("All");
			}
			set
			{
				this.append = value;
				this.path = value;
				this.read = value;
				this.write = value;
			}
		}

		public string Append
		{
			get
			{
				return this.append;
			}
			set
			{
				this.append = value;
			}
		}

		public string PathDiscovery
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		public string Read
		{
			get
			{
				return this.read;
			}
			set
			{
				this.read = value;
			}
		}

		public string Write
		{
			get
			{
				return this.write;
			}
			set
			{
				this.write = value;
			}
		}

		public FileIOPermissionAccess AllFiles
		{
			get
			{
				return this.allFiles;
			}
			set
			{
				this.allFiles = value;
			}
		}

		public FileIOPermissionAccess AllLocalFiles
		{
			get
			{
				return this.allLocalFiles;
			}
			set
			{
				this.allLocalFiles = value;
			}
		}

		public string ChangeAccessControl
		{
			get
			{
				return this.changeAccessControl;
			}
			set
			{
				this.changeAccessControl = value;
			}
		}

		public string ViewAccessControl
		{
			get
			{
				return this.viewAccessControl;
			}
			set
			{
				this.viewAccessControl = value;
			}
		}

		public string ViewAndModify
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				this.append = value;
				this.path = value;
				this.read = value;
				this.write = value;
			}
		}

		public override IPermission CreatePermission()
		{
			FileIOPermission fileIOPermission;
			if (base.Unrestricted)
			{
				fileIOPermission = new FileIOPermission(PermissionState.Unrestricted);
			}
			else
			{
				fileIOPermission = new FileIOPermission(PermissionState.None);
				if (this.append != null)
				{
					fileIOPermission.AddPathList(FileIOPermissionAccess.Append, this.append);
				}
				if (this.path != null)
				{
					fileIOPermission.AddPathList(FileIOPermissionAccess.PathDiscovery, this.path);
				}
				if (this.read != null)
				{
					fileIOPermission.AddPathList(FileIOPermissionAccess.Read, this.read);
				}
				if (this.write != null)
				{
					fileIOPermission.AddPathList(FileIOPermissionAccess.Write, this.write);
				}
			}
			return fileIOPermission;
		}

		private string append;

		private string path;

		private string read;

		private string write;

		private FileIOPermissionAccess allFiles;

		private FileIOPermissionAccess allLocalFiles;

		private string changeAccessControl;

		private string viewAccessControl;
	}
}
