using System;

public interface IHasDlcRestrictions
{
	string[] GetRequiredDlcIds();

	string[] GetForbiddenDlcIds();

	string[] GetAnyRequiredDlcIds()
	{
		return null;
	}
}
