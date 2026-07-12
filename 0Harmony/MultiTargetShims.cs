using System;
using Mono.Cecil;

internal static class MultiTargetShims
{
	public static string Replace(this string self, string oldValue, string newValue, StringComparison comparison)
	{
		return self.Replace(oldValue, newValue);
	}

	public static bool Contains(this string self, string value, StringComparison comparison)
	{
		return self.Contains(value);
	}

	public static int GetHashCode(this string self, StringComparison comparison)
	{
		return self.GetHashCode();
	}

	public static int IndexOf(this string self, char value, StringComparison comparison)
	{
		return self.IndexOf(value);
	}

	public static int IndexOf(this string self, string value, StringComparison comparison)
	{
		return self.IndexOf(value);
	}

	public static TypeReference GetConstraintType(this GenericParameterConstraint constraint)
	{
		return constraint.ConstraintType;
	}

	private static readonly object[] _NoArgs = new object[0];
}
