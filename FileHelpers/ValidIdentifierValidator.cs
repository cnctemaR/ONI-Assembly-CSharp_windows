using System;

namespace FileHelpers
{
	internal static class ValidIdentifierValidator
	{
		internal static bool ValidIdentifier(string id)
		{
			return ValidIdentifierValidator.ValidIdentifier(id, false);
		}

		internal static bool ValidIdentifier(string id, bool isType)
		{
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			if (!char.IsLetter(id[0]) && id[0] != '_')
			{
				return false;
			}
			for (int i = 1; i < id.Length; i++)
			{
				if ((!isType || (id[i] != '.' && id[i] != '<' && id[i] != '>' && id[i] != '?' && id[i] != ',')) && id[i] != '_' && !char.IsLetterOrDigit(id[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
