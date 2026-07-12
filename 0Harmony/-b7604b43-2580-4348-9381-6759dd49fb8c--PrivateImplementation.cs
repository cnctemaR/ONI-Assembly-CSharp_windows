using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[CompilerGenerated]
internal sealed class <b7604b43-2580-4348-9381-6759dd49fb8c><PrivateImplementationDetails>
{
	internal static uint ComputeStringHash(string s)
	{
		uint num;
		if (s != null)
		{
			num = 2166136261U;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((uint)s[i] ^ num) * 16777619U;
			}
		}
		return num;
	}

	// Note: this field is marked with 'hasfieldrva' and has an initial value of '-8511746012302509385'.
	internal static readonly long 1B960802B155541DF3837ADE50790DA7E91762D14B8E011FA8223424FF75ACDB;

	// Note: this field is marked with 'hasfieldrva'.
	internal static readonly <b7604b43-2580-4348-9381-6759dd49fb8c><PrivateImplementationDetails>.__StaticArrayInitTypeSize=1790 2EF0065A03764C27AE8D5DC3002E10F0426E43BDFA7D8ECFFF633E45DD32376B;

	// Note: this field is marked with 'hasfieldrva'.
	internal static readonly <b7604b43-2580-4348-9381-6759dd49fb8c><PrivateImplementationDetails>.__StaticArrayInitTypeSize=160 933598639CBAA1DE502F80D2FD1DB78F13C8D7BB64A5FDC1BC73AC0B5CE4F5CA;

	// Note: this field is marked with 'hasfieldrva' and has an initial value of '4182389475095035824'.
	internal static readonly long 971150DD73DC318E68A98CCE9B91AC7DEA2D43C562B4F5A9A2F4272C7E29477E;

	// Note: this field is marked with 'hasfieldrva'.
	internal static readonly <b7604b43-2580-4348-9381-6759dd49fb8c><PrivateImplementationDetails>.__StaticArrayInitTypeSize=128 BFDF5E72651B4EC588BD5FC6A9F17E9E0972248146BBACC10478F48D72F29B81;

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 128)]
	private struct __StaticArrayInitTypeSize=128
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 160)]
	private struct __StaticArrayInitTypeSize=160
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1790)]
	private struct __StaticArrayInitTypeSize=1790
	{
	}
}
