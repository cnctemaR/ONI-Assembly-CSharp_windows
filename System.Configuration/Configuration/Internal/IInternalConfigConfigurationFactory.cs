using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	[ComVisible(false)]
	public interface IInternalConfigConfigurationFactory
	{
		Configuration Create(Type typeConfigHost, params object[] hostInitConfigurationParams);

		string NormalizeLocationSubPath(string subPath, IConfigErrorInfo errorInfo);
	}
}
