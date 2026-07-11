using System;

namespace System.Runtime.Versioning
{
	public static class VersioningHelper
	{
		private static int GetDomainId()
		{
			return AppDomain.CurrentDomain.Id;
		}

		private static int GetProcessId()
		{
			return 0;
		}

		private static string SafeName(string name, bool process, bool appdomain)
		{
			if (process && appdomain)
			{
				return string.Concat(new string[]
				{
					name,
					"_",
					VersioningHelper.GetProcessId().ToString(),
					"_",
					VersioningHelper.GetDomainId().ToString()
				});
			}
			if (process)
			{
				return name + "_" + VersioningHelper.GetProcessId().ToString();
			}
			if (appdomain)
			{
				return name + "_" + VersioningHelper.GetDomainId().ToString();
			}
			return name;
		}

		private static string ConvertFromMachine(string name, ResourceScope to, Type type)
		{
			switch (to)
			{
			case ResourceScope.Machine:
				return VersioningHelper.SafeName(name, false, false);
			case ResourceScope.Process:
				return VersioningHelper.SafeName(name, true, false);
			case ResourceScope.AppDomain:
				return VersioningHelper.SafeName(name, true, true);
			}
			throw new ArgumentException("to");
		}

		private static string ConvertFromProcess(string name, ResourceScope to, Type type)
		{
			if (to < ResourceScope.Process || to >= ResourceScope.Private)
			{
				throw new ArgumentException("to");
			}
			bool flag = (to & ResourceScope.AppDomain) == ResourceScope.AppDomain;
			return VersioningHelper.SafeName(name, false, flag);
		}

		private static string ConvertFromAppDomain(string name, ResourceScope to, Type type)
		{
			if (to < ResourceScope.AppDomain || to >= ResourceScope.Private)
			{
				throw new ArgumentException("to");
			}
			return VersioningHelper.SafeName(name, false, false);
		}

		[MonoTODO("process id is always 0")]
		public static string MakeVersionSafeName(string name, ResourceScope from, ResourceScope to)
		{
			return VersioningHelper.MakeVersionSafeName(name, from, to, null);
		}

		[MonoTODO("type?")]
		public static string MakeVersionSafeName(string name, ResourceScope from, ResourceScope to, Type type)
		{
			if ((from & ResourceScope.Private) != ResourceScope.None)
			{
				to &= ~(ResourceScope.Private | ResourceScope.Assembly);
			}
			else if ((from & ResourceScope.Assembly) != ResourceScope.None)
			{
				to &= ~ResourceScope.Assembly;
			}
			string text = ((name != null) ? name : string.Empty);
			switch (from)
			{
			case ResourceScope.Machine:
				break;
			case ResourceScope.Process:
				goto IL_008F;
			default:
				switch (from)
				{
				case ResourceScope.Machine | ResourceScope.Private:
					break;
				case ResourceScope.Process | ResourceScope.Private:
					goto IL_008F;
				default:
					switch (from)
					{
					case ResourceScope.Machine | ResourceScope.Assembly:
						goto IL_0086;
					case ResourceScope.Process | ResourceScope.Assembly:
						goto IL_008F;
					case ResourceScope.AppDomain | ResourceScope.Assembly:
						goto IL_0098;
					}
					throw new ArgumentException("from");
				case ResourceScope.AppDomain | ResourceScope.Private:
					goto IL_0098;
				}
				break;
			case ResourceScope.AppDomain:
				goto IL_0098;
			}
			IL_0086:
			return VersioningHelper.ConvertFromMachine(text, to, type);
			IL_008F:
			return VersioningHelper.ConvertFromProcess(text, to, type);
			IL_0098:
			return VersioningHelper.ConvertFromAppDomain(text, to, type);
		}
	}
}
