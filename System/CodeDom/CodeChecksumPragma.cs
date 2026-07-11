using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeChecksumPragma : CodeDirective
	{
		public CodeChecksumPragma()
		{
		}

		public CodeChecksumPragma(string fileName, Guid checksumAlgorithmId, byte[] checksumData)
		{
			this._fileName = fileName;
			this.ChecksumAlgorithmId = checksumAlgorithmId;
			this.ChecksumData = checksumData;
		}

		public string FileName
		{
			get
			{
				return this._fileName ?? string.Empty;
			}
			set
			{
				this._fileName = value;
			}
		}

		public Guid ChecksumAlgorithmId { get; set; }

		public byte[] ChecksumData { get; set; }

		private string _fileName;
	}
}
