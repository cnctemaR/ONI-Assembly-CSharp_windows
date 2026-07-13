using System;
using System.Runtime.Serialization;

namespace Mono.Cecil
{
	[Serializable]
	internal sealed class ResolutionException : Exception
	{
		public MemberReference Member
		{
			get
			{
				return this.member;
			}
		}

		public IMetadataScope Scope
		{
			get
			{
				TypeReference typeReference = this.member as TypeReference;
				if (typeReference != null)
				{
					return typeReference.Scope;
				}
				TypeReference declaringType = this.member.DeclaringType;
				if (declaringType != null)
				{
					return declaringType.Scope;
				}
				throw new NotSupportedException();
			}
		}

		public ResolutionException(MemberReference member)
			: base("Failed to resolve " + member.FullName)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			this.member = member;
		}

		public ResolutionException(MemberReference member, Exception innerException)
			: base("Failed to resolve " + member.FullName, innerException)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			this.member = member;
		}

		private ResolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private readonly MemberReference member;
	}
}
