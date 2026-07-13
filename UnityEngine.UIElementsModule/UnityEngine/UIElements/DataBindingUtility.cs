using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class DataBindingUtility
	{
		public static void GetBoundElements(IPanel panel, List<VisualElement> boundElements)
		{
			BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
			bool flag = baseVisualElementPanel != null;
			if (flag)
			{
				boundElements.AddRange(baseVisualElementPanel.dataBindingManager.GetUnorderedBoundElements());
			}
		}

		public static void GetBindingsForElement(VisualElement element, List<BindingInfo> result)
		{
			HashSet<PropertyPath> hashSet;
			using (CollectionPool<HashSet<PropertyPath>, PropertyPath>.Get(out hashSet))
			{
				List<ValueTuple<Binding, BindingId>> list;
				using (CollectionPool<List<ValueTuple<Binding, BindingId>>, ValueTuple<Binding, BindingId>>.Get(out list))
				{
					DataBindingManager.GetBindingRequests(element, list);
					foreach (ValueTuple<Binding, BindingId> valueTuple in list)
					{
						bool flag = hashSet.Add(in valueTuple.Item2) && valueTuple.Item1 != null;
						if (flag)
						{
							PropertyPath propertyPath = in valueTuple.Item2;
							result.Add(BindingInfo.FromRequest(element, in propertyPath, valueTuple.Item1));
						}
					}
					bool flag2 = element.elementPanel == null;
					if (!flag2)
					{
						List<DataBindingManager.BindingData> bindingData = element.elementPanel.dataBindingManager.GetBindingData(element);
						foreach (DataBindingManager.BindingData bindingData2 in bindingData)
						{
							bool flag3 = hashSet.Add(in bindingData2.target.bindingId);
							if (flag3)
							{
								result.Add(BindingInfo.FromBindingData(in bindingData2));
							}
						}
					}
				}
			}
		}

		public static bool TryGetBinding(VisualElement element, in BindingId bindingId, out BindingInfo bindingInfo)
		{
			Binding binding;
			bool flag = DataBindingManager.TryGetBindingRequest(element, in bindingId, out binding);
			bool flag3;
			if (flag)
			{
				bool flag2 = binding == null;
				if (flag2)
				{
					bindingInfo = default(BindingInfo);
					flag3 = false;
				}
				else
				{
					PropertyPath propertyPath = in bindingId;
					bindingInfo = BindingInfo.FromRequest(element, in propertyPath, binding);
					flag3 = true;
				}
			}
			else
			{
				bool flag4 = element.elementPanel != null;
				if (flag4)
				{
					DataBindingManager.BindingData bindingData;
					bool flag5 = element.elementPanel.dataBindingManager.TryGetBindingData(element, in bindingId, out bindingData);
					if (flag5)
					{
						bindingInfo = BindingInfo.FromBindingData(in bindingData);
						return true;
					}
				}
				bindingInfo = default(BindingInfo);
				flag3 = false;
			}
			return flag3;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static bool TryGetDataSourceOrDataSourceTypeFromHierarchy(VisualElement element, out object dataSourceObject, out Type dataSourceType, out PropertyPath fullPath)
		{
			VisualElement visualElement = element;
			dataSourceObject = null;
			dataSourceType = null;
			fullPath = default(PropertyPath);
			while (visualElement != null)
			{
				bool flag = !visualElement.isDataSourcePathEmpty;
				if (flag)
				{
					bool isEmpty = fullPath.IsEmpty;
					if (isEmpty)
					{
						fullPath = visualElement.dataSourcePath;
					}
					else
					{
						PropertyPath dataSourcePath = visualElement.dataSourcePath;
						fullPath = PropertyPath.Combine(in dataSourcePath, in fullPath);
					}
				}
				dataSourceObject = visualElement.dataSource;
				bool flag2 = visualElement.dataSource != null;
				if (flag2)
				{
					return true;
				}
				visualElement = visualElement.hierarchy.parent;
			}
			return !fullPath.IsEmpty;
		}

		public static bool TryGetRelativeDataSourceFromHierarchy(VisualElement element, out object dataSource)
		{
			DataSourceContext hierarchicalDataSourceContext = element.GetHierarchicalDataSourceContext();
			dataSource = hierarchicalDataSourceContext.dataSource;
			PropertyPath propertyPath = hierarchicalDataSourceContext.dataSourcePath;
			bool isEmpty = propertyPath.IsEmpty;
			bool flag;
			if (isEmpty)
			{
				flag = dataSource != null;
			}
			else
			{
				bool flag2 = hierarchicalDataSourceContext.dataSource == null;
				if (flag2)
				{
					flag = false;
				}
				else
				{
					propertyPath = hierarchicalDataSourceContext.dataSourcePath;
					object obj;
					bool flag3 = !PropertyContainer.TryGetValue<object, object>(ref dataSource, in propertyPath, out obj);
					if (flag3)
					{
						flag = true;
					}
					else
					{
						dataSource = obj;
						flag = true;
					}
				}
			}
			return flag;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static string ReplaceAllIndicesInPath(string path, string newText)
		{
			return path.Contains('[') ? DataBindingUtility.s_ReplaceIndices.Replace(path, "[" + newText + "]") : path;
		}

		public static bool TryGetLastUIBindingResult(in BindingId bindingId, VisualElement element, out BindingResult result)
		{
			result = default(BindingResult);
			DataBindingManager.BindingData activeBindingData = DataBindingUtility.GetActiveBindingData(in bindingId, element);
			bool flag = ((activeBindingData != null) ? activeBindingData.binding : null) == null || element.elementPanel == null;
			return !flag && element.elementPanel.dataBindingManager.TryGetLastUIBindingResult(activeBindingData, out result);
		}

		public static bool TryGetLastSourceBindingResult(in BindingId bindingId, VisualElement element, out BindingResult result)
		{
			result = default(BindingResult);
			DataBindingManager.BindingData activeBindingData = DataBindingUtility.GetActiveBindingData(in bindingId, element);
			bool flag = ((activeBindingData != null) ? activeBindingData.binding : null) == null;
			return !flag && element.elementPanel.dataBindingManager.TryGetLastSourceBindingResult(activeBindingData, out result);
		}

		public static void GetMatchingConverterGroups(Type sourceType, Type destinationType, List<string> result)
		{
			List<ConverterGroup> list;
			using (CollectionPool<List<ConverterGroup>, ConverterGroup>.Get(out list))
			{
				ConverterGroups.GetAllConverterGroups(list);
				foreach (ConverterGroup converterGroup in list)
				{
					Delegate @delegate;
					bool flag = converterGroup.registry.TryGetConverter(sourceType, destinationType, out @delegate);
					if (flag)
					{
						result.Add(converterGroup.id);
					}
				}
			}
		}

		public static void GetMatchingConverterGroupsFromType(Type sourceType, List<string> result)
		{
			List<ConverterGroup> list;
			using (CollectionPool<List<ConverterGroup>, ConverterGroup>.Get(out list))
			{
				List<Type> list2;
				using (CollectionPool<List<Type>, Type>.Get(out list2))
				{
					ConverterGroups.GetAllConverterGroups(list);
					foreach (ConverterGroup converterGroup in list)
					{
						converterGroup.registry.GetAllTypesConvertingFromType(sourceType, list2);
						bool flag = list2.Count > 0;
						if (flag)
						{
							result.Add(converterGroup.id);
						}
						list2.Clear();
					}
				}
			}
		}

		public static void GetMatchingConverterGroupsToType(Type destinationType, List<string> result)
		{
			List<ConverterGroup> list;
			using (CollectionPool<List<ConverterGroup>, ConverterGroup>.Get(out list))
			{
				List<Type> list2;
				using (CollectionPool<List<Type>, Type>.Get(out list2))
				{
					ConverterGroups.GetAllConverterGroups(list);
					foreach (ConverterGroup converterGroup in list)
					{
						converterGroup.registry.GetAllTypesConvertingToType(destinationType, list2);
						bool flag = list2.Count > 0;
						if (flag)
						{
							result.Add(converterGroup.id);
						}
						list2.Clear();
					}
				}
			}
		}

		public static void GetAllConversionsFromSourceToUI(DataBinding binding, Type destinationType, List<Type> result)
		{
			result.Add(destinationType);
			if (binding != null)
			{
				binding.sourceToUiConverters.registry.GetAllTypesConvertingToType(destinationType, result);
			}
			ConverterGroups.globalConverters.registry.GetAllTypesConvertingToType(destinationType, result);
			ConverterGroups.primitivesConverters.registry.GetAllTypesConvertingToType(destinationType, result);
		}

		public static void GetAllConversionsFromUIToSource(DataBinding binding, Type sourceType, List<Type> result)
		{
			result.Add(sourceType);
			if (binding != null)
			{
				binding.uiToSourceConverters.registry.GetAllTypesConvertingToType(sourceType, result);
			}
			ConverterGroups.globalConverters.registry.GetAllTypesConvertingToType(sourceType, result);
			ConverterGroups.primitivesConverters.registry.GetAllTypesConvertingToType(sourceType, result);
		}

		public static BindingTypeResult IsPathValid(object dataSource, string path)
		{
			return DataBindingUtility.IsPathValid(dataSource, (dataSource != null) ? dataSource.GetType() : null, path);
		}

		public static BindingTypeResult IsPathValid(Type type, string path)
		{
			return DataBindingUtility.IsPathValid(null, type, path);
		}

		private static BindingTypeResult IsPathValid(object dataSource, Type type, string path)
		{
			bool flag = type == null;
			BindingTypeResult bindingTypeResult;
			if (flag)
			{
				VisitReturnCode visitReturnCode = VisitReturnCode.NullContainer;
				int num = 0;
				PropertyPath propertyPath = default(PropertyPath);
				bindingTypeResult = new BindingTypeResult(visitReturnCode, num, in propertyPath);
			}
			else
			{
				TypePathVisitor typePathVisitor = DataBindingUtility.k_TypeVisitors.Get();
				IPropertyBag propertyBag = PropertyBag.GetPropertyBag(type);
				BindingTypeResult bindingTypeResult2;
				try
				{
					typePathVisitor.Path = new PropertyPath(path);
					bool flag2 = propertyBag == null;
					if (flag2)
					{
						typePathVisitor.ReturnCode = VisitReturnCode.MissingPropertyBag;
					}
					bool flag3 = dataSource == null;
					if (flag3)
					{
						if (propertyBag != null)
						{
							propertyBag.Accept(typePathVisitor);
						}
					}
					else if (propertyBag != null)
					{
						propertyBag.Accept(typePathVisitor, ref dataSource);
					}
					bool flag4 = typePathVisitor.ReturnCode == VisitReturnCode.Ok;
					if (flag4)
					{
						Type resolvedType = typePathVisitor.resolvedType;
						PropertyPath propertyPath = typePathVisitor.Path;
						bindingTypeResult2 = new BindingTypeResult(resolvedType, in propertyPath);
					}
					else
					{
						PropertyPath propertyPath = typePathVisitor.Path;
						PropertyPath propertyPath2 = PropertyPath.SubPath(in propertyPath, 0, typePathVisitor.PathIndex);
						bindingTypeResult2 = new BindingTypeResult(typePathVisitor.ReturnCode, typePathVisitor.PathIndex, in propertyPath2);
					}
				}
				finally
				{
					DataBindingUtility.k_TypeVisitors.Release(typePathVisitor);
				}
				bindingTypeResult = bindingTypeResult2;
			}
			return bindingTypeResult;
		}

		public static void GetPropertyPaths(object dataSource, int depth, List<PropertyPathInfo> listResult)
		{
			DataBindingUtility.GetPropertyPaths(dataSource, (dataSource != null) ? dataSource.GetType() : null, depth, listResult);
		}

		public static void GetPropertyPaths(Type type, int depth, List<PropertyPathInfo> listResult)
		{
			DataBindingUtility.GetPropertyPaths(null, type, depth, listResult);
		}

		private static void GetPropertyPaths(object dataSource, Type type, int depth, List<PropertyPathInfo> resultList)
		{
			bool flag = type == null;
			if (!flag)
			{
				IPropertyBag propertyBag = PropertyBag.GetPropertyBag(type);
				bool flag2 = propertyBag == null;
				if (!flag2)
				{
					AutoCompletePathVisitor autoCompletePathVisitor = DataBindingUtility.k_AutoCompleteVisitors.Get();
					try
					{
						autoCompletePathVisitor.propertyPathList = resultList;
						autoCompletePathVisitor.maxDepth = depth;
						bool flag3 = dataSource == null;
						if (flag3)
						{
							propertyBag.Accept(autoCompletePathVisitor);
						}
						else
						{
							propertyBag.Accept(autoCompletePathVisitor, ref dataSource);
						}
					}
					finally
					{
						DataBindingUtility.k_AutoCompleteVisitors.Release(autoCompletePathVisitor);
					}
				}
			}
		}

		private static DataBindingManager.BindingData GetActiveBindingData(in BindingId bindingId, VisualElement element)
		{
			DataBindingManager.BindingData bindingData;
			bool flag = element.elementPanel != null && element.elementPanel.dataBindingManager.TryGetBindingData(element, in bindingId, out bindingData);
			DataBindingManager.BindingData bindingData2;
			if (flag)
			{
				bindingData2 = bindingData;
			}
			else
			{
				bindingData2 = null;
			}
			return bindingData2;
		}

		private static readonly ObjectPool<TypePathVisitor> k_TypeVisitors = new ObjectPool<TypePathVisitor>(() => new TypePathVisitor(), delegate(TypePathVisitor v)
		{
			v.Reset();
		}, null, null, true, 1, 10000);

		private static readonly ObjectPool<AutoCompletePathVisitor> k_AutoCompleteVisitors = new ObjectPool<AutoCompletePathVisitor>(() => new AutoCompletePathVisitor(), delegate(AutoCompletePathVisitor v)
		{
			v.Reset();
		}, null, null, true, 1, 10000);

		private static readonly Regex s_ReplaceIndices = new Regex("\\[[0-9]+\\]", RegexOptions.Compiled);
	}
}
