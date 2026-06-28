using System;
using System.Data;
using System.Data.OleDb;

namespace FileHelpers.DataLink
{
	public sealed class AccessStorage : DatabaseStorage
	{
		public AccessStorage(Type recordType)
			: this(recordType, string.Empty)
		{
		}

		public AccessStorage(Type recordType, string accessFile)
			: base(recordType)
		{
			this.AccessFileName = accessFile;
			base.ConnectionString = DataBaseHelper.GetAccessConnection(this.AccessFileName, this.AccessFilePassword);
		}

		protected sealed override IDbConnection CreateConnection()
		{
			if (this.mAccessFile == null || this.mAccessFile == string.Empty)
			{
				throw new BadUsageException("The AccessFileName can't be null or empty.");
			}
			return new OleDbConnection(base.ConnectionString);
		}

		public string AccessFileName
		{
			get
			{
				return this.mAccessFile;
			}
			set
			{
				this.mAccessFile = value;
				base.ConnectionString = DataBaseHelper.GetAccessConnection(this.AccessFileName, this.AccessFilePassword);
			}
		}

		public string AccessFilePassword
		{
			get
			{
				return this.mAccessPassword;
			}
			set
			{
				this.mAccessPassword = value;
				base.ConnectionString = DataBaseHelper.GetAccessConnection(this.AccessFileName, this.AccessFilePassword);
			}
		}

		private string mAccessFile = string.Empty;

		private string mAccessPassword = string.Empty;
	}
}
