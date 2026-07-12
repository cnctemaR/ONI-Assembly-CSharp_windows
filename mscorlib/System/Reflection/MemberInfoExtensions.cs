using System;

namespace System.Reflection
{
	public static class MemberInfoExtensions
	{
		public static bool HasMetadataToken(this MemberInfo member)
		{
			Requires.NotNull(member, "member");
			bool flag;
			try
			{
				flag = MemberInfoExtensions.GetMetadataTokenOrZeroOrThrow(member) != 0;
			}
			catch (InvalidOperationException)
			{
				flag = false;
			}
			return flag;
		}

		public static int GetMetadataToken(this MemberInfo member)
		{
			Requires.NotNull(member, "member");
			int metadataTokenOrZeroOrThrow = MemberInfoExtensions.GetMetadataTokenOrZeroOrThrow(member);
			if (metadataTokenOrZeroOrThrow == 0)
			{
				throw new InvalidOperationException("There is no metadata token available for the given member.");
			}
			return metadataTokenOrZeroOrThrow;
		}

		private static int GetMetadataTokenOrZeroOrThrow(MemberInfo member)
		{
			int metadataToken = member.MetadataToken;
			if ((metadataToken & 16777215) == 0)
			{
				return 0;
			}
			return metadataToken;
		}
	}
}
