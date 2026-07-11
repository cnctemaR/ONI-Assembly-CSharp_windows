using System;
using System.Dynamic.Utils;
using Unity;

namespace System.Linq.Expressions
{
	public class SymbolDocumentInfo
	{
		internal SymbolDocumentInfo(string fileName)
		{
			ContractUtils.RequiresNotNull(fileName, "fileName");
			this.FileName = fileName;
		}

		public string FileName { get; }

		public virtual Guid Language
		{
			get
			{
				return Guid.Empty;
			}
		}

		public virtual Guid LanguageVendor
		{
			get
			{
				return Guid.Empty;
			}
		}

		public virtual Guid DocumentType
		{
			get
			{
				return SymbolDocumentInfo.DocumentType_Text;
			}
		}

		internal SymbolDocumentInfo()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal static readonly Guid DocumentType_Text = new Guid(1518771467, 26129, 4563, 189, 42, 0, 0, 248, 8, 73, 189);
	}
}
