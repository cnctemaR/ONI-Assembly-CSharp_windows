using System;

namespace Mono.Cecil.Pdb
{
	internal class SymDocumentWriter
	{
		public ISymUnmanagedDocumentWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		public SymDocumentWriter(ISymUnmanagedDocumentWriter writer)
		{
			this.writer = writer;
		}

		public void SetSource(byte[] source)
		{
			this.writer.SetSource((uint)source.Length, source);
		}

		public void SetCheckSum(Guid hashAlgo, byte[] checkSum)
		{
			this.writer.SetCheckSum(hashAlgo, (uint)checkSum.Length, checkSum);
		}

		private readonly ISymUnmanagedDocumentWriter writer;
	}
}
