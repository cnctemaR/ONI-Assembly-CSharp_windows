using System;
using System.Collections.Generic;
using System.Security;

namespace System.Runtime.Serialization
{
	internal sealed class GenericParameterDataContract : DataContract
	{
		[SecuritySafeCritical]
		internal GenericParameterDataContract(Type type)
			: base(new GenericParameterDataContract.GenericParameterDataContractCriticalHelper(type))
		{
			this.helper = base.Helper as GenericParameterDataContract.GenericParameterDataContractCriticalHelper;
		}

		internal int ParameterPosition
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ParameterPosition;
			}
		}

		internal override bool IsBuiltInDataContract
		{
			get
			{
				return true;
			}
		}

		internal override DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
		{
			return paramContracts[this.ParameterPosition];
		}

		[SecurityCritical]
		private GenericParameterDataContract.GenericParameterDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class GenericParameterDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal GenericParameterDataContractCriticalHelper(Type type)
				: base(type)
			{
				base.SetDataContractName(DataContract.GetStableName(type));
				this.parameterPosition = type.GenericParameterPosition;
			}

			internal int ParameterPosition
			{
				get
				{
					return this.parameterPosition;
				}
			}

			private int parameterPosition;
		}
	}
}
