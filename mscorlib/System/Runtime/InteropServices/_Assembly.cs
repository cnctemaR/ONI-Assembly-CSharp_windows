using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Policy;

namespace System.Runtime.InteropServices
{
	[TypeLibImportClass(typeof(Assembly))]
	[Guid("17156360-2F1A-384A-BC52-FDE93C215C5B")]
	[CLSCompliant(false)]
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface _Assembly
	{
		string ToString();

		bool Equals(object other);

		int GetHashCode();

		Type GetType();

		string CodeBase { get; }

		string EscapedCodeBase { get; }

		AssemblyName GetName();

		AssemblyName GetName(bool copiedName);

		string FullName { get; }

		MethodInfo EntryPoint { get; }

		Type GetType(string name);

		Type GetType(string name, bool throwOnError);

		Type[] GetExportedTypes();

		Type[] GetTypes();

		Stream GetManifestResourceStream(Type type, string name);

		Stream GetManifestResourceStream(string name);

		FileStream GetFile(string name);

		FileStream[] GetFiles();

		FileStream[] GetFiles(bool getResourceModules);

		string[] GetManifestResourceNames();

		ManifestResourceInfo GetManifestResourceInfo(string resourceName);

		string Location { get; }

		Evidence Evidence { get; }

		object[] GetCustomAttributes(Type attributeType, bool inherit);

		object[] GetCustomAttributes(bool inherit);

		bool IsDefined(Type attributeType, bool inherit);

		void GetObjectData(SerializationInfo info, StreamingContext context);

		Type GetType(string name, bool throwOnError, bool ignoreCase);

		Assembly GetSatelliteAssembly(CultureInfo culture);

		Assembly GetSatelliteAssembly(CultureInfo culture, Version version);

		Module LoadModule(string moduleName, byte[] rawModule);

		Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore);

		object CreateInstance(string typeName);

		object CreateInstance(string typeName, bool ignoreCase);

		object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes);

		Module[] GetLoadedModules();

		Module[] GetLoadedModules(bool getResourceModules);

		Module[] GetModules();

		Module[] GetModules(bool getResourceModules);

		Module GetModule(string name);

		AssemblyName[] GetReferencedAssemblies();

		bool GlobalAssemblyCache { get; }

		event ModuleResolveEventHandler ModuleResolve;
	}
}
