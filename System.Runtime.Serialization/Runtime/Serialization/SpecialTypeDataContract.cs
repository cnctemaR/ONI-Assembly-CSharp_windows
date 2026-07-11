using System;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class SpecialTypeDataContract : DataContract
	{
		[SecuritySafeCritical]
		public SpecialTypeDataContract(Type type)
			: base(new SpecialTypeDataContract.SpecialTypeDataContractCriticalHelper(type))
		{
			this.helper = base.Helper as SpecialTypeDataContract.SpecialTypeDataContractCriticalHelper;
		}

		[SecuritySafeCritical]
		public SpecialTypeDataContract(Type type, XmlDictionaryString name, XmlDictionaryString ns)
			: base(new SpecialTypeDataContract.SpecialTypeDataContractCriticalHelper(type, name, ns))
		{
			this.helper = base.Helper as SpecialTypeDataContract.SpecialTypeDataContractCriticalHelper;
		}

		internal override bool IsBuiltInDataContract
		{
			get
			{
				return true;
			}
		}

		[SecurityCritical]
		private SpecialTypeDataContract.SpecialTypeDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class SpecialTypeDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal SpecialTypeDataContractCriticalHelper(Type type)
				: base(type)
			{
			}

			internal SpecialTypeDataContractCriticalHelper(Type type, XmlDictionaryString name, XmlDictionaryString ns)
				: base(type)
			{
				base.SetDataContractName(name, ns);
			}
		}
	}
}
