using System;

namespace System.Linq.Expressions
{
	internal sealed class SymbolDocumentWithGuids : SymbolDocumentInfo
	{
		internal SymbolDocumentWithGuids(string fileName, ref Guid language)
			: base(fileName)
		{
			this.Language = language;
			this.DocumentType = SymbolDocumentInfo.DocumentType_Text;
		}

		internal SymbolDocumentWithGuids(string fileName, ref Guid language, ref Guid vendor)
			: base(fileName)
		{
			this.Language = language;
			this.LanguageVendor = vendor;
			this.DocumentType = SymbolDocumentInfo.DocumentType_Text;
		}

		internal SymbolDocumentWithGuids(string fileName, ref Guid language, ref Guid vendor, ref Guid documentType)
			: base(fileName)
		{
			this.Language = language;
			this.LanguageVendor = vendor;
			this.DocumentType = documentType;
		}

		public override Guid Language { get; }

		public override Guid LanguageVendor { get; }

		public override Guid DocumentType { get; }
	}
}
