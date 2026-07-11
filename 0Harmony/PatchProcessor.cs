using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public class PatchProcessor
	{
		public PatchProcessor(HarmonyInstance instance, Type type, HarmonyMethod attributes)
		{
			this.instance = instance;
			this.container = type;
			this.containerAttributes = attributes ?? new HarmonyMethod(null);
			this.prefix = this.containerAttributes.Clone();
			this.postfix = this.containerAttributes.Clone();
			this.transpiler = this.containerAttributes.Clone();
			this.PrepareType();
		}

		public PatchProcessor(HarmonyInstance instance, List<MethodBase> originals, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			this.instance = instance;
			this.originals = originals;
			this.prefix = prefix ?? new HarmonyMethod(null);
			this.postfix = postfix ?? new HarmonyMethod(null);
			this.transpiler = transpiler ?? new HarmonyMethod(null);
		}

		public static Patches GetPatchInfo(MethodBase method)
		{
			object obj = PatchProcessor.locker;
			Patches patches;
			lock (obj)
			{
				PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(method);
				bool flag = patchInfo == null;
				if (flag)
				{
					patches = null;
				}
				else
				{
					patches = new Patches(patchInfo.prefixes, patchInfo.postfixes, patchInfo.transpilers);
				}
			}
			return patches;
		}

		public static IEnumerable<MethodBase> AllPatchedMethods()
		{
			object obj = PatchProcessor.locker;
			IEnumerable<MethodBase> patchedMethods;
			lock (obj)
			{
				patchedMethods = HarmonySharedState.GetPatchedMethods();
			}
			return patchedMethods;
		}

		public List<DynamicMethod> Patch()
		{
			object obj = PatchProcessor.locker;
			List<DynamicMethod> list2;
			lock (obj)
			{
				List<DynamicMethod> list = new List<DynamicMethod>();
				foreach (MethodBase methodBase in this.originals)
				{
					bool flag = methodBase == null;
					if (flag)
					{
						throw new NullReferenceException("original");
					}
					bool flag2 = this.RunMethod<HarmonyPrepare, bool>(true, new object[] { methodBase });
					bool flag3 = flag2;
					if (flag3)
					{
						PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(methodBase);
						bool flag4 = patchInfo == null;
						if (flag4)
						{
							patchInfo = new PatchInfo();
						}
						PatchFunctions.AddPrefix(patchInfo, this.instance.Id, this.prefix);
						PatchFunctions.AddPostfix(patchInfo, this.instance.Id, this.postfix);
						PatchFunctions.AddTranspiler(patchInfo, this.instance.Id, this.transpiler);
						list.Add(PatchFunctions.UpdateWrapper(methodBase, patchInfo, this.instance.Id));
						HarmonySharedState.UpdatePatchInfo(methodBase, patchInfo);
						this.RunMethod<HarmonyCleanup>(new object[] { methodBase });
					}
				}
				list2 = list;
			}
			return list2;
		}

		public void Unpatch(HarmonyPatchType type, string harmonyID)
		{
			object obj = PatchProcessor.locker;
			lock (obj)
			{
				foreach (MethodBase methodBase in this.originals)
				{
					PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(methodBase);
					bool flag = patchInfo == null;
					if (flag)
					{
						patchInfo = new PatchInfo();
					}
					bool flag2 = type == HarmonyPatchType.All || type == HarmonyPatchType.Prefix;
					if (flag2)
					{
						PatchFunctions.RemovePrefix(patchInfo, harmonyID);
					}
					bool flag3 = type == HarmonyPatchType.All || type == HarmonyPatchType.Postfix;
					if (flag3)
					{
						PatchFunctions.RemovePostfix(patchInfo, harmonyID);
					}
					bool flag4 = type == HarmonyPatchType.All || type == HarmonyPatchType.Transpiler;
					if (flag4)
					{
						PatchFunctions.RemoveTranspiler(patchInfo, harmonyID);
					}
					PatchFunctions.UpdateWrapper(methodBase, patchInfo, this.instance.Id);
					HarmonySharedState.UpdatePatchInfo(methodBase, patchInfo);
				}
			}
		}

		public void Unpatch(MethodInfo patch)
		{
			object obj = PatchProcessor.locker;
			lock (obj)
			{
				foreach (MethodBase methodBase in this.originals)
				{
					PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(methodBase);
					bool flag = patchInfo == null;
					if (flag)
					{
						patchInfo = new PatchInfo();
					}
					PatchFunctions.RemovePatch(patchInfo, patch);
					PatchFunctions.UpdateWrapper(methodBase, patchInfo, this.instance.Id);
					HarmonySharedState.UpdatePatchInfo(methodBase, patchInfo);
				}
			}
		}

		private void PrepareType()
		{
			bool flag = this.RunMethod<HarmonyPrepare, bool>(true, new object[0]);
			bool flag2 = !flag;
			if (!flag2)
			{
				IEnumerable<MethodBase> enumerable = this.RunMethod<HarmonyTargetMethods, IEnumerable<MethodBase>>(null, new object[0]);
				bool flag3 = enumerable != null;
				if (flag3)
				{
					this.originals = enumerable.ToList<MethodBase>();
				}
				else
				{
					MethodType? methodType = this.containerAttributes.methodType;
					bool flag4 = this.containerAttributes.methodType == null;
					if (flag4)
					{
						this.containerAttributes.methodType = new MethodType?(MethodType.Normal);
					}
					bool flag5 = Attribute.GetCustomAttribute(this.container, typeof(HarmonyPatchAll)) != null;
					bool flag6 = flag5;
					if (flag6)
					{
						Type declaringType = this.containerAttributes.declaringType;
						this.originals.AddRange(AccessTools.GetDeclaredConstructors(declaringType).Cast<MethodBase>());
						this.originals.AddRange(AccessTools.GetDeclaredMethods(declaringType).Cast<MethodBase>());
					}
					else
					{
						MethodBase methodBase = this.RunMethod<HarmonyTargetMethod, MethodBase>(null, new object[0]);
						bool flag7 = methodBase == null;
						if (flag7)
						{
							methodBase = this.GetOriginalMethod();
						}
						bool flag8 = methodBase == null;
						if (flag8)
						{
							string text = "(";
							text = string.Concat(new object[]
							{
								text,
								"declaringType=",
								this.containerAttributes.declaringType,
								", "
							});
							text = text + "methodName =" + this.containerAttributes.methodName + ", ";
							text = string.Concat(new object[] { text, "methodType=", methodType, ", " });
							text = text + "argumentTypes=" + this.containerAttributes.argumentTypes.Description();
							text += ")";
							throw new ArgumentException("No target method specified for class " + this.container.FullName + " " + text);
						}
						this.originals.Add(methodBase);
					}
				}
				PatchTools.GetPatches(this.container, out this.prefix.method, out this.postfix.method, out this.transpiler.method);
				bool flag9 = this.prefix.method != null;
				if (flag9)
				{
					bool flag10 = !this.prefix.method.IsStatic;
					if (flag10)
					{
						throw new ArgumentException("Patch method " + this.prefix.method.FullDescription() + " must be static");
					}
					List<HarmonyMethod> harmonyMethods = this.prefix.method.GetHarmonyMethods();
					this.containerAttributes.Merge(HarmonyMethod.Merge(harmonyMethods)).CopyTo(this.prefix);
				}
				bool flag11 = this.postfix.method != null;
				if (flag11)
				{
					bool flag12 = !this.postfix.method.IsStatic;
					if (flag12)
					{
						throw new ArgumentException("Patch method " + this.postfix.method.FullDescription() + " must be static");
					}
					List<HarmonyMethod> harmonyMethods2 = this.postfix.method.GetHarmonyMethods();
					this.containerAttributes.Merge(HarmonyMethod.Merge(harmonyMethods2)).CopyTo(this.postfix);
				}
				bool flag13 = this.transpiler.method != null;
				if (flag13)
				{
					bool flag14 = !this.transpiler.method.IsStatic;
					if (flag14)
					{
						throw new ArgumentException("Patch method " + this.transpiler.method.FullDescription() + " must be static");
					}
					List<HarmonyMethod> harmonyMethods3 = this.transpiler.method.GetHarmonyMethods();
					this.containerAttributes.Merge(HarmonyMethod.Merge(harmonyMethods3)).CopyTo(this.transpiler);
				}
			}
		}

		private MethodBase GetOriginalMethod()
		{
			HarmonyMethod harmonyMethod = this.containerAttributes;
			bool flag = harmonyMethod.declaringType == null;
			MethodBase methodBase;
			if (flag)
			{
				methodBase = null;
			}
			else
			{
				MethodType? methodType = harmonyMethod.methodType;
				MethodType? methodType2 = methodType;
				if (methodType2 != null)
				{
					switch (methodType2.GetValueOrDefault())
					{
					case MethodType.Normal:
					{
						bool flag2 = harmonyMethod.methodName == null;
						if (flag2)
						{
							return null;
						}
						return AccessTools.DeclaredMethod(harmonyMethod.declaringType, harmonyMethod.methodName, harmonyMethod.argumentTypes, null);
					}
					case MethodType.Getter:
					{
						bool flag3 = harmonyMethod.methodName == null;
						if (flag3)
						{
							return null;
						}
						return AccessTools.DeclaredProperty(harmonyMethod.declaringType, harmonyMethod.methodName).GetGetMethod(true);
					}
					case MethodType.Setter:
					{
						bool flag4 = harmonyMethod.methodName == null;
						if (flag4)
						{
							return null;
						}
						return AccessTools.DeclaredProperty(harmonyMethod.declaringType, harmonyMethod.methodName).GetSetMethod(true);
					}
					case MethodType.Constructor:
						return AccessTools.DeclaredConstructor(harmonyMethod.declaringType, harmonyMethod.argumentTypes);
					case MethodType.StaticConstructor:
						return (from c in AccessTools.GetDeclaredConstructors(harmonyMethod.declaringType)
							where c.IsStatic
							select c).FirstOrDefault<ConstructorInfo>();
					}
				}
				methodBase = null;
			}
			return methodBase;
		}

		private T RunMethod<S, T>(T defaultIfNotExisting, params object[] parameters)
		{
			bool flag = this.container == null;
			T t;
			if (flag)
			{
				t = defaultIfNotExisting;
			}
			else
			{
				string text = typeof(S).Name.Replace("Harmony", "");
				List<object> list = new List<object> { this.instance };
				list.AddRange(parameters);
				Type[] types = AccessTools.GetTypes(list.ToArray());
				MethodInfo methodInfo = PatchTools.GetPatchMethod<S>(this.container, text, types);
				bool flag2 = methodInfo != null && typeof(T).IsAssignableFrom(methodInfo.ReturnType);
				if (flag2)
				{
					t = (T)((object)methodInfo.Invoke(null, list.ToArray()));
				}
				else
				{
					methodInfo = PatchTools.GetPatchMethod<S>(this.container, text, new Type[] { typeof(HarmonyInstance) });
					bool flag3 = methodInfo != null && typeof(T).IsAssignableFrom(methodInfo.ReturnType);
					if (flag3)
					{
						t = (T)((object)methodInfo.Invoke(null, new object[] { this.instance }));
					}
					else
					{
						methodInfo = PatchTools.GetPatchMethod<S>(this.container, text, Type.EmptyTypes);
						bool flag4 = methodInfo != null;
						if (flag4)
						{
							bool flag5 = typeof(T).IsAssignableFrom(methodInfo.ReturnType);
							if (flag5)
							{
								MethodBase methodBase = methodInfo;
								object obj = null;
								object[] array = Type.EmptyTypes;
								t = (T)((object)methodBase.Invoke(obj, array));
							}
							else
							{
								MethodBase methodBase2 = methodInfo;
								object obj2 = null;
								object[] array = Type.EmptyTypes;
								methodBase2.Invoke(obj2, array);
								t = defaultIfNotExisting;
							}
						}
						else
						{
							t = defaultIfNotExisting;
						}
					}
				}
			}
			return t;
		}

		private void RunMethod<S>(params object[] parameters)
		{
			bool flag = this.container == null;
			if (!flag)
			{
				string text = typeof(S).Name.Replace("Harmony", "");
				List<object> list = new List<object> { this.instance };
				list.AddRange(parameters);
				Type[] types = AccessTools.GetTypes(list.ToArray());
				MethodInfo methodInfo = PatchTools.GetPatchMethod<S>(this.container, text, types);
				bool flag2 = methodInfo != null;
				if (flag2)
				{
					methodInfo.Invoke(null, list.ToArray());
				}
				else
				{
					methodInfo = PatchTools.GetPatchMethod<S>(this.container, text, new Type[] { typeof(HarmonyInstance) });
					bool flag3 = methodInfo != null;
					if (flag3)
					{
						methodInfo.Invoke(null, new object[] { this.instance });
					}
					else
					{
						methodInfo = PatchTools.GetPatchMethod<S>(this.container, text, Type.EmptyTypes);
						bool flag4 = methodInfo != null;
						if (flag4)
						{
							MethodBase methodBase = methodInfo;
							object obj = null;
							object[] emptyTypes = Type.EmptyTypes;
							methodBase.Invoke(obj, emptyTypes);
						}
					}
				}
			}
		}

		private static object locker = new object();

		private readonly HarmonyInstance instance;

		private readonly Type container;

		private readonly HarmonyMethod containerAttributes;

		private List<MethodBase> originals = new List<MethodBase>();

		private HarmonyMethod prefix;

		private HarmonyMethod postfix;

		private HarmonyMethod transpiler;
	}
}
