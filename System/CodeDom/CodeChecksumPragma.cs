using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeChecksumPragma : CodeDirective
	{
		public CodeChecksumPragma()
		{
		}

		public CodeChecksumPragma(string fileName, Guid checksumAlgorithmId, byte[] checksumData)
		{
			this.fileName = fileName;
			this.checksumAlgorithmId = checksumAlgorithmId;
			this.checksumData = checksumData;
		}

		public Guid ChecksumAlgorithmId
		{
			get
			{
				return this.checksumAlgorithmId;
			}
			set
			{
				this.checksumAlgorithmId = value;
			}
		}

		public byte[] ChecksumData
		{
			get
			{
				return this.checksumData;
			}
			set
			{
				this.checksumData = value;
			}
		}

		public string FileName
		{
			get
			{
				if (this.fileName == null)
				{
					return string.Empty;
				}
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		private string fileName;

		private Guid checksumAlgorithmId;

		private byte[] checksumData;
	}
}
