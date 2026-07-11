using System;
using System.CodeDom;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	internal class ContractCodeDomInfo
	{
		internal string ClrNamespace
		{
			get
			{
				if (!this.ReferencedTypeExists)
				{
					return this.clrNamespace;
				}
				return null;
			}
			set
			{
				if (this.ReferencedTypeExists)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Cannot set namespace for already referenced type. Base type is '{0}'.", new object[] { this.TypeReference.BaseType })));
				}
				this.clrNamespace = value;
			}
		}

		internal Dictionary<string, object> GetMemberNames()
		{
			if (this.ReferencedTypeExists)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Cannot set members for already referenced type. Base type is '{0}'.", new object[] { this.TypeReference.BaseType })));
			}
			if (this.memberNames == null)
			{
				this.memberNames = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			}
			return this.memberNames;
		}

		internal bool IsProcessed;

		internal CodeTypeDeclaration TypeDeclaration;

		internal CodeTypeReference TypeReference;

		internal CodeNamespace CodeNamespace;

		internal bool ReferencedTypeExists;

		internal bool UsesWildcardNamespace;

		private string clrNamespace;

		private Dictionary<string, object> memberNames;
	}
}
