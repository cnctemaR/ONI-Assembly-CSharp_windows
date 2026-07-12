using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Threading;

[assembly: AssemblyVersion("4.0.0.0")]
[assembly: DefaultDependency(LoadHint.Always)]
[assembly: AssemblyKeyFile("../ecma.pub")]
[assembly: AssemblyDelaySign(true)]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyFileVersion("4.6.57.0")]
[assembly: AssemblyInformationalVersion("4.6.57.0")]
[assembly: SatelliteContractVersion("4.0.0.0")]
[assembly: AssemblyCopyright("(c) Various Mono authors")]
[assembly: AssemblyProduct("Mono Common Language Infrastructure")]
[assembly: AssemblyCompany("Mono development team")]
[assembly: AssemblyDefaultAlias("System.Core.dll")]
[assembly: AssemblyDescription("System.Core.dll")]
[assembly: AssemblyTitle("System.Core.dll")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: StringFreezing]
[assembly: ComVisible(false)]
[assembly: SecurityCritical]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
// mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
[assembly: TypeForwardedTo(typeof(Action))]
[assembly: TypeForwardedTo(typeof(Action<, >))]
[assembly: TypeForwardedTo(typeof(Action<, , >))]
[assembly: TypeForwardedTo(typeof(Action<, , , >))]
[assembly: TypeForwardedTo(typeof(TimeZoneInfo.AdjustmentRule))]
[assembly: TypeForwardedTo(typeof(Aes))]
[assembly: TypeForwardedTo(typeof(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION))]
[assembly: TypeForwardedTo(typeof(ExtensionAttribute))]
[assembly: TypeForwardedTo(typeof(Func<>))]
[assembly: TypeForwardedTo(typeof(Func<, >))]
[assembly: TypeForwardedTo(typeof(Func<, , >))]
[assembly: TypeForwardedTo(typeof(Func<, , , >))]
[assembly: TypeForwardedTo(typeof(Func<, , , , >))]
[assembly: TypeForwardedTo(typeof(InvalidTimeZoneException))]
[assembly: TypeForwardedTo(typeof(LazyThreadSafetyMode))]
[assembly: TypeForwardedTo(typeof(Lazy<>))]
[assembly: TypeForwardedTo(typeof(LockRecursionException))]
[assembly: TypeForwardedTo(typeof(TimeZoneInfo))]
[assembly: TypeForwardedTo(typeof(TimeZoneNotFoundException))]
[assembly: TypeForwardedTo(typeof(TimeZoneInfo.TransitionTime))]
