using System;
using System.Runtime.CompilerServices;

namespace System.Dynamic
{
	internal static class UpdateDelegates
	{
		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute1<T0, TRet>(CallSite site, T0 arg0)
		{
			CallSite<Func<CallSite, T0, TRet>> callSite = (CallSite<Func<CallSite, T0, TRet>>)site;
			Func<CallSite, T0, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, TRet>[] array;
			Func<CallSite, T0, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch1<T0, TRet>(CallSite site, T0 arg0)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute2<T0, T1, TRet>(CallSite site, T0 arg0, T1 arg1)
		{
			CallSite<Func<CallSite, T0, T1, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, TRet>>)site;
			Func<CallSite, T0, T1, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, TRet>[] array;
			Func<CallSite, T0, T1, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch2<T0, T1, TRet>(CallSite site, T0 arg0, T1 arg1)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute3<T0, T1, T2, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2)
		{
			CallSite<Func<CallSite, T0, T1, T2, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, TRet>>)site;
			Func<CallSite, T0, T1, T2, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, TRet>[] array;
			Func<CallSite, T0, T1, T2, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch3<T0, T1, T2, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute4<T0, T1, T2, T3, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch4<T0, T1, T2, T3, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute5<T0, T1, T2, T3, T4, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, T4, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, T4, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, T4, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, T4, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, T4, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3, arg4);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, T4, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch5<T0, T1, T2, T3, T4, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute6<T0, T1, T2, T3, T4, T5, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, T4, T5, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, T4, T5, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch6<T0, T1, T2, T3, T4, T5, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute7<T0, T1, T2, T3, T4, T5, T6, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch7<T0, T1, T2, T3, T4, T5, T6, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute8<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch8<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute9<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch9<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet UpdateAndExecute10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>> callSite = (CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>)site;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>[] array;
			Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> func;
			if ((array = CallSiteOps.GetRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					func = array[i];
					if (func != target)
					{
						callSite.Target = func;
						TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return tret;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>> ruleCache = CallSiteOps.GetRuleCache<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				func = array[j];
				callSite.Target = func;
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(callSite, func);
						CallSiteOps.MoveRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(ruleCache, func, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			func = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 };
			for (;;)
			{
				callSite.Target = target;
				func = (callSite.Target = callSite.Binder.BindCore<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(callSite, array2));
				try
				{
					TRet tret = func(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return tret;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>>(callSite, func);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static TRet NoMatch10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			site._match = false;
			return default(TRet);
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid1<T0>(CallSite site, T0 arg0)
		{
			CallSite<Action<CallSite, T0>> callSite = (CallSite<Action<CallSite, T0>>)site;
			Action<CallSite, T0> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0>[] array;
			Action<CallSite, T0> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0>>(callSite, array2));
				try
				{
					action(site, arg0);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid1<T0>(CallSite site, T0 arg0)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid2<T0, T1>(CallSite site, T0 arg0, T1 arg1)
		{
			CallSite<Action<CallSite, T0, T1>> callSite = (CallSite<Action<CallSite, T0, T1>>)site;
			Action<CallSite, T0, T1> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1>[] array;
			Action<CallSite, T0, T1> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1>>(callSite, array2));
				try
				{
					action(site, arg0, arg1);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid2<T0, T1>(CallSite site, T0 arg0, T1 arg1)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid3<T0, T1, T2>(CallSite site, T0 arg0, T1 arg1, T2 arg2)
		{
			CallSite<Action<CallSite, T0, T1, T2>> callSite = (CallSite<Action<CallSite, T0, T1, T2>>)site;
			Action<CallSite, T0, T1, T2> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2>[] array;
			Action<CallSite, T0, T1, T2> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid3<T0, T1, T2>(CallSite site, T0 arg0, T1 arg1, T2 arg2)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid4<T0, T1, T2, T3>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3>>)site;
			Action<CallSite, T0, T1, T2, T3> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3>[] array;
			Action<CallSite, T0, T1, T2, T3> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid4<T0, T1, T2, T3>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid5<T0, T1, T2, T3, T4>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3, T4>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3, T4>>)site;
			Action<CallSite, T0, T1, T2, T3, T4> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3, T4>[] array;
			Action<CallSite, T0, T1, T2, T3, T4> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3, T4>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3, arg4);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3, T4>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3, T4>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3, T4>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3, T4>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3, T4>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid5<T0, T1, T2, T3, T4>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid6<T0, T1, T2, T3, T4, T5>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5>>)site;
			Action<CallSite, T0, T1, T2, T3, T4, T5> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3, T4, T5>[] array;
			Action<CallSite, T0, T1, T2, T3, T4, T5> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3, T4, T5>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3, arg4, arg5);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3, T4, T5>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3, T4, T5>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3, T4, T5>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid6<T0, T1, T2, T3, T4, T5>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid7<T0, T1, T2, T3, T4, T5, T6>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>)site;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6>[] array;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid7<T0, T1, T2, T3, T4, T5, T6>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid8<T0, T1, T2, T3, T4, T5, T6, T7>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>)site;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>[] array;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid8<T0, T1, T2, T3, T4, T5, T6, T7>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>)site;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>[] array;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			site._match = false;
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void UpdateAndExecuteVoid10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> callSite = (CallSite<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>)site;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> target = callSite.Target;
			site = callSite.GetMatchmaker();
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>[] array;
			Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action;
			if ((array = CallSiteOps.GetRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(callSite)) != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					action = array[i];
					if (action != target)
					{
						callSite.Target = action;
						action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
						if (CallSiteOps.GetMatch(site))
						{
							CallSiteOps.UpdateRules<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(callSite, i);
							callSite.ReleaseMatchmaker(site);
							return;
						}
						CallSiteOps.ClearMatch(site);
					}
				}
			}
			RuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> ruleCache = CallSiteOps.GetRuleCache<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(callSite);
			array = ruleCache.GetRules();
			for (int j = 0; j < array.Length; j++)
			{
				action = array[j];
				callSite.Target = action;
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(callSite, action);
						CallSiteOps.MoveRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(ruleCache, action, j);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
			action = null;
			object[] array2 = new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 };
			for (;;)
			{
				callSite.Target = target;
				action = (callSite.Target = callSite.Binder.BindCore<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(callSite, array2));
				try
				{
					action(site, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
					if (CallSiteOps.GetMatch(site))
					{
						callSite.ReleaseMatchmaker(site);
						return;
					}
				}
				finally
				{
					if (CallSiteOps.GetMatch(site))
					{
						CallSiteOps.AddRule<Action<CallSite, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(callSite, action);
					}
				}
				CallSiteOps.ClearMatch(site);
			}
		}

		[Obsolete("pregenerated CallSite<T>.Update delegate", true)]
		internal static void NoMatchVoid10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			site._match = false;
		}
	}
}
