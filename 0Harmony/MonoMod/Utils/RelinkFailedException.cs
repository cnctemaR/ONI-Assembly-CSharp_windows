using System;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	[Serializable]
	internal class RelinkFailedException : Exception
	{
		public IMetadataTokenProvider MTP { get; }

		[Nullable(2)]
		public IMetadataTokenProvider Context
		{
			[NullableContext(2)]
			get;
		}

		public RelinkFailedException(IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context = null)
			: this(RelinkFailedException.Format("MonoMod failed relinking", mtp, context), mtp, context)
		{
		}

		public RelinkFailedException(string message, IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context = null)
			: base(message)
		{
			this.MTP = mtp;
			this.Context = context;
		}

		public RelinkFailedException(string message, Exception innerException, IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context = null)
			: base(message ?? RelinkFailedException.Format("MonoMod failed relinking", mtp, context), innerException)
		{
			this.MTP = mtp;
			this.Context = context;
		}

		protected static string Format(string message, IMetadataTokenProvider mtp, [Nullable(2)] IMetadataTokenProvider context)
		{
			if (mtp == null && context == null)
			{
				return message;
			}
			StringBuilder stringBuilder = new StringBuilder(message);
			stringBuilder.Append(' ');
			if (mtp != null)
			{
				stringBuilder.Append(mtp.ToString());
			}
			if (context != null)
			{
				stringBuilder.Append(' ');
			}
			if (context != null)
			{
				stringBuilder.Append("(context: ").Append(context.ToString()).Append(')');
			}
			return stringBuilder.ToString();
		}

		public const string DefaultMessage = "MonoMod failed relinking";
	}
}
