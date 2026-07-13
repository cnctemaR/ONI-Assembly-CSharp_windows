using System;

public static class DlcRestrictionsUtil
{
	public static bool HasAnyRestrictions(IHasDlcRestrictions restrictions)
	{
		return (restrictions.GetRequiredDlcIds() != null && restrictions.GetRequiredDlcIds().Length != 0) || (restrictions.GetAnyRequiredDlcIds() != null && restrictions.GetAnyRequiredDlcIds().Length != 0) || (restrictions.GetForbiddenDlcIds() != null && restrictions.GetForbiddenDlcIds().Length != 0);
	}

	public static string[] GetRequiredDlcsOrNull(IHasDlcRestrictions hasDlcRestrictions)
	{
		if (hasDlcRestrictions != null)
		{
			return hasDlcRestrictions.GetRequiredDlcIds();
		}
		return null;
	}

	public static string[] GetForbiddenDlcIdsOrNull(IHasDlcRestrictions hasDlcRestrictions)
	{
		if (hasDlcRestrictions != null)
		{
			return hasDlcRestrictions.GetForbiddenDlcIds();
		}
		return null;
	}

	public static DlcRestrictionsUtil.TemporaryHelperObject GetTransientHelperObject(string[] requiredDlcIds, string[] forbiddenDlcIds)
	{
		DlcRestrictionsUtil._sharedRestrictionTemporaryHelper.requiredDlcIds = requiredDlcIds;
		DlcRestrictionsUtil._sharedRestrictionTemporaryHelper.forbiddenDlcIds = forbiddenDlcIds;
		return DlcRestrictionsUtil._sharedRestrictionTemporaryHelper;
	}

	public static DlcRestrictionsUtil.TemporaryHelperObject GetTransientHelperObjectFromAllowList(string[] allowList)
	{
		string[] array;
		string[] array2;
		DlcManager.ConvertAvailableToRequireAndForbidden(allowList, out array, out array2);
		DlcRestrictionsUtil._sharedRestrictionTemporaryHelper.requiredDlcIds = array;
		DlcRestrictionsUtil._sharedRestrictionTemporaryHelper.forbiddenDlcIds = array2;
		return DlcRestrictionsUtil._sharedRestrictionTemporaryHelper;
	}

	private static DlcRestrictionsUtil.TemporaryHelperObject _sharedRestrictionTemporaryHelper = new DlcRestrictionsUtil.TemporaryHelperObject(null, null);

	public class TemporaryHelperObject : IHasDlcRestrictions
	{
		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public TemporaryHelperObject(string[] requiredDlcIds, string[] forbiddenDlcIds)
		{
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
		}

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;
	}
}
