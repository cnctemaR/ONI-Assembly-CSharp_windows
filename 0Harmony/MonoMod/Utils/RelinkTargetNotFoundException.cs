using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	[Serializable]
	internal class RelinkTargetNotFoundException : RelinkFailedException
	{
		public RelinkTargetNotFoundException(IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context = null)
			: base(RelinkFailedException.Format("MonoMod relinker failed finding", mtp, context), mtp, context)
		{
		}

		public RelinkTargetNotFoundException(string message, IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context = null)
			: base(message ?? "MonoMod relinker failed finding", mtp, context)
		{
		}

		public RelinkTargetNotFoundException(string message, Exception innerException, IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context = null)
			: base(message ?? "MonoMod relinker failed finding", innerException, mtp, context)
		{
		}

		public new const string DefaultMessage = "MonoMod relinker failed finding";
	}
}
