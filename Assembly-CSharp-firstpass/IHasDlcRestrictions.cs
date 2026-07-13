using System;

public interface IHasDlcRestrictions
{
	string[] GetRequiredDlcIds();

	string[] GetForbiddenDlcIds();
}
