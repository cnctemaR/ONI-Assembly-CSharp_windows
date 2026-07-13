using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	internal class BindingUpdater
	{
		public bool ShouldProcessBindingAtStage(Binding bindingObject, BindingUpdateStage stage, bool versionChanged, bool dirty)
		{
			if (!true)
			{
			}
			DataBinding dataBinding = bindingObject as DataBinding;
			bool flag;
			if (dataBinding == null)
			{
				CustomBinding customBinding = bindingObject as CustomBinding;
				if (customBinding == null)
				{
					throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
				}
				flag = this.ShouldProcessBindingAtStage(customBinding, stage, versionChanged, dirty);
			}
			else
			{
				flag = BindingUpdater.ShouldProcessBindingAtStage(dataBinding, stage, versionChanged, dirty);
			}
			if (!true)
			{
			}
			return flag;
		}

		private static bool ShouldProcessBindingAtStage(DataBinding dataBinding, BindingUpdateStage stage, bool versionChanged, bool dirty)
		{
			bool flag2;
			if (stage != BindingUpdateStage.UpdateUI)
			{
				if (stage != BindingUpdateStage.UpdateSource)
				{
					throw new ArgumentOutOfRangeException("stage", stage, null);
				}
				BindingMode bindingMode = dataBinding.bindingMode;
				bool flag = bindingMode == BindingMode.ToTarget || bindingMode == BindingMode.ToTargetOnce;
				flag2 = !flag;
			}
			else
			{
				bool flag3 = dataBinding.bindingMode == BindingMode.ToSource;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = dataBinding.updateTrigger == BindingUpdateTrigger.EveryUpdate || dirty;
					if (flag4)
					{
						flag2 = true;
					}
					else
					{
						bool flag5 = dataBinding.bindingMode == BindingMode.ToTargetOnce;
						flag2 = !flag5 && (dataBinding.updateTrigger == BindingUpdateTrigger.OnSourceChanged && versionChanged);
					}
				}
			}
			return flag2;
		}

		private bool ShouldProcessBindingAtStage(CustomBinding customBinding, BindingUpdateStage stage, bool versionChanged, bool dirty)
		{
			bool flag;
			if (stage != BindingUpdateStage.UpdateUI)
			{
				if (stage != BindingUpdateStage.UpdateSource)
				{
					throw new ArgumentOutOfRangeException("stage", stage, null);
				}
				flag = false;
			}
			else
			{
				BindingUpdateTrigger updateTrigger = customBinding.updateTrigger;
				if (!true)
				{
				}
				bool flag2;
				if (updateTrigger != BindingUpdateTrigger.OnSourceChanged)
				{
					if (updateTrigger == BindingUpdateTrigger.EveryUpdate)
					{
						flag2 = true;
						goto IL_003D;
					}
				}
				else if (versionChanged || dirty)
				{
					flag2 = true;
					goto IL_003D;
				}
				flag2 = dirty;
				IL_003D:
				if (!true)
				{
				}
				flag = flag2;
			}
			return flag;
		}

		public BindingResult UpdateUI(in BindingContext context, Binding bindingObject)
		{
			if (!true)
			{
			}
			DataBinding dataBinding = bindingObject as DataBinding;
			BindingResult bindingResult;
			if (dataBinding == null)
			{
				CustomBinding customBinding = bindingObject as CustomBinding;
				if (customBinding == null)
				{
					throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
				}
				bindingResult = this.UpdateUI(in context, customBinding);
			}
			else
			{
				bindingResult = this.UpdateUI(in context, dataBinding);
			}
			if (!true)
			{
			}
			return bindingResult;
		}

		public BindingResult UpdateSource(in BindingContext context, Binding bindingObject)
		{
			if (!true)
			{
			}
			DataBinding dataBinding = bindingObject as DataBinding;
			BindingResult bindingResult;
			if (dataBinding == null)
			{
				CustomBinding customBinding = bindingObject as CustomBinding;
				if (customBinding == null)
				{
					throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
				}
				bindingResult = this.UpdateDataSource(in context, customBinding);
			}
			else
			{
				bindingResult = this.UpdateDataSource(in context, dataBinding);
			}
			if (!true)
			{
			}
			return bindingResult;
		}

		private BindingResult UpdateUI(in BindingContext context, DataBinding dataBinding)
		{
			VisualElement targetElement = context.targetElement;
			object dataSource = context.dataSource;
			bool flag = dataSource == null;
			BindingResult bindingResult;
			if (flag)
			{
				string text = (string.IsNullOrEmpty(targetElement.name) ? TypeUtility.GetTypeDisplayName(targetElement.GetType()) : targetElement.name);
				string text2 = "[UI Toolkit] Could not bind '" + text + "' because there is no data source.";
				bindingResult = new BindingResult(BindingStatus.Pending, text2);
			}
			else
			{
				PropertyPath propertyPath = context.dataSourcePath;
				bool isEmpty = propertyPath.IsEmpty;
				if (isEmpty)
				{
					bool flag2 = !TypeTraits.IsContainer(dataSource.GetType());
					if (flag2)
					{
						bindingResult = BindingUpdater.TryUpdateUIWithNonContainer(in context, dataBinding, dataSource);
					}
					else
					{
						ValueTuple<bool, VisitReturnCode, BindingResult> valueTuple = BindingUpdater.VisitRoot(dataBinding, ref dataSource, in context);
						bool flag3 = !valueTuple.Item1;
						if (flag3)
						{
							string visitationErrorString = BindingUpdater.GetVisitationErrorString(valueTuple.Item2, in context);
							bindingResult = new BindingResult(BindingStatus.Failure, visitationErrorString);
						}
						else
						{
							bindingResult = BindingUpdater.s_VisitDataSourceAsRootVisitor.result;
						}
					}
				}
				else
				{
					BindingUpdateStage bindingUpdateStage = BindingUpdateStage.UpdateUI;
					propertyPath = context.dataSourcePath;
					ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult> valueTuple2 = BindingUpdater.VisitAtPath<object>(dataBinding, bindingUpdateStage, ref dataSource, in propertyPath, in context);
					bool flag4 = !valueTuple2.Item1;
					if (flag4)
					{
						string visitationErrorString2 = BindingUpdater.GetVisitationErrorString(valueTuple2.Item2, in context);
						bindingResult = new BindingResult(BindingStatus.Failure, visitationErrorString2);
					}
					else
					{
						bool flag5 = valueTuple2.Item3 > VisitReturnCode.Ok;
						if (flag5)
						{
							VisitReturnCode item = valueTuple2.Item3;
							object dataSource2 = context.dataSource;
							propertyPath = context.dataSourcePath;
							string extractValueErrorString = BindingUpdater.GetExtractValueErrorString(item, dataSource2, in propertyPath);
							bindingResult = new BindingResult(BindingStatus.Failure, extractValueErrorString);
						}
						else
						{
							bindingResult = valueTuple2.Item4;
						}
					}
				}
			}
			return bindingResult;
		}

		private BindingResult UpdateUI(in BindingContext context, CustomBinding customBinding)
		{
			return customBinding.Update(in context);
		}

		private BindingResult UpdateDataSource(in BindingContext context, DataBinding dataBinding)
		{
			VisualElement targetElement = context.targetElement;
			object dataSource = context.dataSource;
			PropertyPath dataSourcePath = context.dataSourcePath;
			bool flag = dataSource == null;
			BindingResult bindingResult;
			if (flag)
			{
				string text = (string.IsNullOrEmpty(targetElement.name) ? TypeUtility.GetTypeDisplayName(targetElement.GetType()) : targetElement.name);
				string text2 = "[UI Toolkit] Could not set value on '" + text + "' because there is no data source.";
				bindingResult = new BindingResult(BindingStatus.Pending, text2);
			}
			else
			{
				bool isEmpty = dataSourcePath.IsEmpty;
				if (isEmpty)
				{
					string rootDataSourceError = BindingUpdater.GetRootDataSourceError(dataSource);
					bindingResult = new BindingResult(BindingStatus.Failure, rootDataSourceError);
				}
				else
				{
					BindingUpdateStage bindingUpdateStage = BindingUpdateStage.UpdateSource;
					BindingId bindingId = context.bindingId;
					PropertyPath propertyPath = in bindingId;
					ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult> valueTuple = BindingUpdater.VisitAtPath<VisualElement>(dataBinding, bindingUpdateStage, ref targetElement, in propertyPath, in context);
					bool flag2 = !valueTuple.Item1;
					if (flag2)
					{
						string visitationErrorString = BindingUpdater.GetVisitationErrorString(valueTuple.Item2, in context);
						bindingResult = new BindingResult(BindingStatus.Failure, visitationErrorString);
					}
					else
					{
						bool flag3 = valueTuple.Item3 > VisitReturnCode.Ok;
						if (flag3)
						{
							VisitReturnCode item = valueTuple.Item3;
							object obj = targetElement;
							bindingId = context.bindingId;
							propertyPath = in bindingId;
							string extractValueErrorString = BindingUpdater.GetExtractValueErrorString(item, obj, in propertyPath);
							bindingResult = new BindingResult(BindingStatus.Failure, extractValueErrorString);
						}
						else
						{
							bindingResult = valueTuple.Item4;
						}
					}
				}
			}
			return bindingResult;
		}

		private BindingResult UpdateDataSource(in BindingContext context, CustomBinding customBinding)
		{
			return new BindingResult(BindingStatus.Pending, null);
		}

		private static BindingResult TryUpdateUIWithNonContainer(in BindingContext context, DataBinding binding, object value)
		{
			Type type = value.GetType();
			bool isEnum = type.IsEnum;
			BindingResult bindingResult;
			if (isEnum)
			{
				MethodInfo methodInfo = DataBinding.updateUIMethod.MakeGenericMethod(new Type[] { type });
				bindingResult = (BindingResult)methodInfo.Invoke(binding, new object[] { context, value });
			}
			else
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
				{
					bool flag = (bool)value;
					return binding.UpdateUI<bool>(in context, ref flag);
				}
				case TypeCode.Char:
				{
					char c = (char)value;
					return binding.UpdateUI<char>(in context, ref c);
				}
				case TypeCode.SByte:
				{
					sbyte b = (sbyte)value;
					return binding.UpdateUI<sbyte>(in context, ref b);
				}
				case TypeCode.Byte:
				{
					byte b2 = (byte)value;
					return binding.UpdateUI<byte>(in context, ref b2);
				}
				case TypeCode.Int16:
				{
					short num = (short)value;
					return binding.UpdateUI<short>(in context, ref num);
				}
				case TypeCode.UInt16:
				{
					ushort num2 = (ushort)value;
					return binding.UpdateUI<ushort>(in context, ref num2);
				}
				case TypeCode.Int32:
				{
					int num3 = (int)value;
					return binding.UpdateUI<int>(in context, ref num3);
				}
				case TypeCode.UInt32:
				{
					uint num4 = (uint)value;
					return binding.UpdateUI<uint>(in context, ref num4);
				}
				case TypeCode.Int64:
				{
					long num5 = (long)value;
					return binding.UpdateUI<long>(in context, ref num5);
				}
				case TypeCode.UInt64:
				{
					ulong num6 = (ulong)value;
					return binding.UpdateUI<ulong>(in context, ref num6);
				}
				case TypeCode.Single:
				{
					float num7 = (float)value;
					return binding.UpdateUI<float>(in context, ref num7);
				}
				case TypeCode.Double:
				{
					double num8 = (double)value;
					return binding.UpdateUI<double>(in context, ref num8);
				}
				case TypeCode.String:
				{
					string text = (string)value;
					return binding.UpdateUI<string>(in context, ref text);
				}
				}
				bindingResult = new BindingResult(BindingStatus.Failure, "[UI Toolkit] Unsupported primitive type");
			}
			return bindingResult;
		}

		[return: TupleElementNames(new string[] { "succeeded", "visitationReturnCode", "bindingResult" })]
		private static ValueTuple<bool, VisitReturnCode, BindingResult> VisitRoot(DataBinding dataBinding, ref object container, in BindingContext context)
		{
			BindingUpdater.s_VisitDataSourceAsRootVisitor.Reset();
			BindingUpdater.s_VisitDataSourceAsRootVisitor.Binding = dataBinding;
			BindingUpdater.s_VisitDataSourceAsRootVisitor.bindingContext = context;
			VisitReturnCode visitReturnCode;
			bool flag = PropertyContainer.TryAccept<object>(BindingUpdater.s_VisitDataSourceAsRootVisitor, ref container, out visitReturnCode, default(VisitParameters));
			return new ValueTuple<bool, VisitReturnCode, BindingResult>(flag, visitReturnCode, BindingUpdater.s_VisitDataSourceAsRootVisitor.result);
		}

		[return: TupleElementNames(new string[] { "succeeded", "visitationReturnCode", "atPathReturnCode", "bindingResult" })]
		private static ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult> VisitAtPath<TContainer>(DataBinding dataBinding, BindingUpdateStage direction, ref TContainer container, in PropertyPath path, in BindingContext context)
		{
			BindingUpdater.s_VisitDataSourceAtPathVisitor.Reset();
			BindingUpdater.s_VisitDataSourceAtPathVisitor.binding = dataBinding;
			BindingUpdater.s_VisitDataSourceAtPathVisitor.direction = direction;
			BindingUpdater.s_VisitDataSourceAtPathVisitor.Path = path;
			BindingUpdater.s_VisitDataSourceAtPathVisitor.bindingContext = context;
			VisitReturnCode visitReturnCode;
			bool flag = PropertyContainer.TryAccept<TContainer>(BindingUpdater.s_VisitDataSourceAtPathVisitor, ref container, out visitReturnCode, default(VisitParameters));
			return new ValueTuple<bool, VisitReturnCode, VisitReturnCode, BindingResult>(flag, visitReturnCode, BindingUpdater.s_VisitDataSourceAtPathVisitor.ReturnCode, BindingUpdater.s_VisitDataSourceAtPathVisitor.result);
		}

		internal static string GetVisitationErrorString(VisitReturnCode returnCode, in BindingContext context)
		{
			string text = string.Format("[UI Toolkit] Could not bind target of type '<b>{0}</b>' at path '<b>{1}</b>':", context.targetElement.GetType().Name, context.bindingId);
			string text2;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidCast:
			case VisitReturnCode.AccessViolation:
				throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.InvalidContainerType:
				text2 = text + " the data source cannot be a primitive, a string or an enum.";
				break;
			case VisitReturnCode.MissingPropertyBag:
				text2 = text + " the data source is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				text2 = text + " the path from the data source to the target is either invalid or contains a null value.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return text2;
		}

		internal static string GetExtractValueErrorString(VisitReturnCode returnCode, object target, in PropertyPath path)
		{
			string text = string.Format("[UI Toolkit] Could not retrieve the value at path '<b>{0}</b>' for source of type '<b>{1}</b>':", path, (target != null) ? target.GetType().Name : null);
			string text2;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidCast:
			case VisitReturnCode.AccessViolation:
				throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.InvalidContainerType:
				text2 = text + " the source cannot be a primitive, a string or an enum.";
				break;
			case VisitReturnCode.MissingPropertyBag:
				text2 = text + " the source is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				text2 = text + " the path from the source to the target is either invalid or contains a null value.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return text2;
		}

		internal static string GetSetValueErrorString(VisitReturnCode returnCode, object source, in PropertyPath sourcePath, object target, in PropertyPath targetPath, object extractedValueFromSource)
		{
			string text = string.Format("[UI Toolkit] Could not set value for target of type '<b>{0}</b>' at path '<b>{1}</b>':", target.GetType().Name, targetPath);
			string text2;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidContainerType:
				throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.MissingPropertyBag:
				text2 = text + " the type '" + target.GetType().Name + "' is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				text2 = text + " the path is either invalid or contains a null value.";
				break;
			case VisitReturnCode.InvalidCast:
			{
				bool isEmpty = sourcePath.IsEmpty;
				if (isEmpty)
				{
					object obj;
					bool flag = PropertyContainer.TryGetValue<object, object>(ref target, in targetPath, out obj) && obj != null;
					if (flag)
					{
						text2 = ((extractedValueFromSource == null) ? (text + " could not convert from '<b>null</b>' to '<b>" + obj.GetType().Name + "</b>'.") : string.Concat(new string[]
						{
							text,
							" could not convert from type '<b>",
							extractedValueFromSource.GetType().Name,
							"</b>' to type '<b>",
							obj.GetType().Name,
							"</b>'."
						}));
						break;
					}
				}
				IProperty property;
				bool flag2 = PropertyContainer.TryGetProperty<object>(ref source, in sourcePath, out property);
				if (flag2)
				{
					object obj2;
					bool flag3 = PropertyContainer.TryGetValue<object, object>(ref target, in targetPath, out obj2) && obj2 != null;
					if (flag3)
					{
						text2 = ((extractedValueFromSource == null) ? string.Concat(new string[]
						{
							text,
							" could not convert from '<b>null (",
							property.DeclaredValueType().Name,
							")</b>' to '<b>",
							obj2.GetType().Name,
							"</b>'."
						}) : string.Concat(new string[]
						{
							text,
							" could not convert from type '<b>",
							extractedValueFromSource.GetType().Name,
							"</b>' to type '<b>",
							obj2.GetType().Name,
							"</b>'."
						}));
						break;
					}
				}
				text2 = text + " conversion failed.";
				break;
			}
			case VisitReturnCode.AccessViolation:
				text2 = text + " the path is read-only.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return text2;
		}

		internal static string GetRootDataSourceError(object target)
		{
			return "[UI Toolkit] Could not set value for target of type '<b>" + target.GetType().Name + "</b>': no path was provided.";
		}

		private static readonly BindingUpdater.CastDataSourceVisitor s_VisitDataSourceAsRootVisitor = new BindingUpdater.CastDataSourceVisitor();

		private static readonly BindingUpdater.UIPathVisitor s_VisitDataSourceAtPathVisitor = new BindingUpdater.UIPathVisitor();

		private sealed class CastDataSourceVisitor : ConcreteTypeVisitor
		{
			public DataBinding Binding { get; set; }

			public BindingContext bindingContext { get; set; }

			public BindingResult result { get; set; }

			public void Reset()
			{
				this.Binding = null;
				this.bindingContext = default(BindingContext);
				this.result = default(BindingResult);
			}

			protected override void VisitContainer<TContainer>(ref TContainer container)
			{
				DataBinding binding = this.Binding;
				BindingContext bindingContext = this.bindingContext;
				this.result = binding.UpdateUI<TContainer>(in bindingContext, ref container);
			}
		}

		private sealed class UIPathVisitor : PathVisitor
		{
			public DataBinding binding { get; set; }

			public BindingUpdateStage direction { get; set; }

			public BindingContext bindingContext { get; set; }

			public BindingResult result { get; set; }

			public override void Reset()
			{
				base.Reset();
				this.binding = null;
				this.direction = BindingUpdateStage.UpdateUI;
				this.bindingContext = default(BindingContext);
				this.result = default(BindingResult);
				base.ReadonlyVisit = true;
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				BindingUpdateStage direction = this.direction;
				if (!true)
				{
				}
				BindingResult bindingResult;
				if (direction != BindingUpdateStage.UpdateUI)
				{
					if (direction != BindingUpdateStage.UpdateSource)
					{
						throw new ArgumentOutOfRangeException();
					}
					DataBinding binding = this.binding;
					BindingContext bindingContext = this.bindingContext;
					bindingResult = binding.UpdateSource<TValue>(in bindingContext, ref value);
				}
				else
				{
					DataBinding binding2 = this.binding;
					BindingContext bindingContext = this.bindingContext;
					bindingResult = binding2.UpdateUI<TValue>(in bindingContext, ref value);
				}
				if (!true)
				{
				}
				this.result = bindingResult;
			}
		}
	}
}
