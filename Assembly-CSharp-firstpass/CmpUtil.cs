using System;
using System.Collections.Generic;

public class CmpUtil
{
	public static CmpFns GetCmpFns(string type_name)
	{
		return CmpUtil.GetCmpFns(Type.GetType(type_name));
	}

	public static CmpFns GetCmpFns(Type type)
	{
		CmpFns cmpFns = null;
		if (!CmpUtil.sCmpFns.TryGetValue(type, out cmpFns))
		{
			cmpFns = new CmpFns(type);
			CmpUtil.sCmpFns[type] = cmpFns;
		}
		return cmpFns;
	}

	private static Dictionary<Type, CmpFns> sCmpFns = new Dictionary<Type, CmpFns>();
}
