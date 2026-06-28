using System;
using System.Collections;

namespace System.EnterpriseServices
{
	internal interface IConfigurationAttribute
	{
		bool AfterSaveChanges(Hashtable info);

		bool Apply(Hashtable info);

		bool IsValidTarget(string s);
	}
}
