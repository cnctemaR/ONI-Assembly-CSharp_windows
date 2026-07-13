using System;

public class ExecutableSpecificString
{
	public ExecutableSpecificString(string baseStr, string soStr)
	{
		this.baseString = baseStr;
		this.soString = soStr;
	}

	public static implicit operator string(ExecutableSpecificString dualString)
	{
		if (!DlcManager.IsExpansion1Active())
		{
			return dualString.baseString;
		}
		return dualString.soString;
	}

	public static implicit operator LocString(ExecutableSpecificString dualString)
	{
		return new LocString(dualString);
	}

	private string baseString;

	private string soString;
}
