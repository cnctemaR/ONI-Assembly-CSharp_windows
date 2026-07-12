using System;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal class RelinkTargetNotFoundException : RelinkFailedException
	{
		public RelinkTargetNotFoundException(IMetadataTokenProvider mtp, IMetadataTokenProvider context = null)
			: base(RelinkFailedException._Format("MonoMod relinker failed finding", mtp, context), mtp, context)
		{
		}

		public RelinkTargetNotFoundException(string message, IMetadataTokenProvider mtp, IMetadataTokenProvider context = null)
			: base(message ?? "MonoMod relinker failed finding", mtp, context)
		{
		}

		public RelinkTargetNotFoundException(string message, Exception innerException, IMetadataTokenProvider mtp, IMetadataTokenProvider context = null)
			: base(message ?? "MonoMod relinker failed finding", innerException, mtp, context)
		{
		}

		public new const string DefaultMessage = "MonoMod relinker failed finding";
	}
}
