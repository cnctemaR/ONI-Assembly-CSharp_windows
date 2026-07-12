using System;

namespace System.Linq.Expressions
{
	internal static class Strings
	{
		internal static string ReducibleMustOverrideReduce
		{
			get
			{
				return "reducible nodes must override Expression.Reduce()";
			}
		}

		internal static string MustReduceToDifferent
		{
			get
			{
				return "node cannot reduce to itself or null";
			}
		}

		internal static string ReducedNotCompatible
		{
			get
			{
				return "cannot assign from the reduced node type to the original node type";
			}
		}

		internal static string SetterHasNoParams
		{
			get
			{
				return "Setter must have parameters.";
			}
		}

		internal static string PropertyCannotHaveRefType
		{
			get
			{
				return "Property cannot have a managed pointer type.";
			}
		}

		internal static string IndexesOfSetGetMustMatch
		{
			get
			{
				return "Indexing parameters of getter and setter must match.";
			}
		}

		internal static string AccessorsCannotHaveVarArgs
		{
			get
			{
				return "Accessor method should not have VarArgs.";
			}
		}

		internal static string AccessorsCannotHaveByRefArgs
		{
			get
			{
				return "Accessor indexes cannot be passed ByRef.";
			}
		}

		internal static string BoundsCannotBeLessThanOne
		{
			get
			{
				return "Bounds count cannot be less than 1";
			}
		}

		internal static string TypeMustNotBeByRef
		{
			get
			{
				return "Type must not be ByRef";
			}
		}

		internal static string TypeMustNotBePointer
		{
			get
			{
				return "Type must not be a pointer type";
			}
		}

		internal static string SetterMustBeVoid
		{
			get
			{
				return "Setter should have void type.";
			}
		}

		internal static string PropertyTypeMustMatchGetter
		{
			get
			{
				return "Property type must match the value type of getter";
			}
		}

		internal static string PropertyTypeMustMatchSetter
		{
			get
			{
				return "Property type must match the value type of setter";
			}
		}

		internal static string BothAccessorsMustBeStatic
		{
			get
			{
				return "Both accessors must be static.";
			}
		}

		internal static string OnlyStaticFieldsHaveNullInstance
		{
			get
			{
				return "Static field requires null instance, non-static field requires non-null instance.";
			}
		}

		internal static string OnlyStaticPropertiesHaveNullInstance
		{
			get
			{
				return "Static property requires null instance, non-static property requires non-null instance.";
			}
		}

		internal static string OnlyStaticMethodsHaveNullInstance
		{
			get
			{
				return "Static method requires null instance, non-static method requires non-null instance.";
			}
		}

		internal static string PropertyTypeCannotBeVoid
		{
			get
			{
				return "Property cannot have a void type.";
			}
		}

		internal static string InvalidUnboxType
		{
			get
			{
				return "Can only unbox from an object or interface type to a value type.";
			}
		}

		internal static string ExpressionMustBeWriteable
		{
			get
			{
				return "Expression must be writeable";
			}
		}

		internal static string ArgumentMustNotHaveValueType
		{
			get
			{
				return "Argument must not have a value type.";
			}
		}

		internal static string MustBeReducible
		{
			get
			{
				return "must be reducible node";
			}
		}

		internal static string AllTestValuesMustHaveSameType
		{
			get
			{
				return "All test values must have the same type.";
			}
		}

		internal static string AllCaseBodiesMustHaveSameType
		{
			get
			{
				return "All case bodies and the default body must have the same type.";
			}
		}

		internal static string DefaultBodyMustBeSupplied
		{
			get
			{
				return "Default body must be supplied if case bodies are not System.Void.";
			}
		}

		internal static string LabelMustBeVoidOrHaveExpression
		{
			get
			{
				return "Label type must be System.Void if an expression is not supplied";
			}
		}

		internal static string LabelTypeMustBeVoid
		{
			get
			{
				return "Type must be System.Void for this label argument";
			}
		}

		internal static string QuotedExpressionMustBeLambda
		{
			get
			{
				return "Quoted expression must be a lambda";
			}
		}

		internal static string CollectionModifiedWhileEnumerating
		{
			get
			{
				return "Collection was modified; enumeration operation may not execute.";
			}
		}

		internal static string VariableMustNotBeByRef(object p0, object p1)
		{
			return global::SR.Format("Variable '{0}' uses unsupported type '{1}'. Reference types are not supported for variables.", p0, p1);
		}

		internal static string CollectionReadOnly
		{
			get
			{
				return "Collection is read-only.";
			}
		}

		internal static string AmbiguousMatchInExpandoObject(object p0)
		{
			return global::SR.Format("More than one key matching '{0}' was found in the ExpandoObject.", p0);
		}

		internal static string SameKeyExistsInExpando(object p0)
		{
			return global::SR.Format("An element with the same key '{0}' already exists in the ExpandoObject.", p0);
		}

		internal static string KeyDoesNotExistInExpando(object p0)
		{
			return global::SR.Format("The specified key '{0}' does not exist in the ExpandoObject.", p0);
		}

		internal static string ArgCntMustBeGreaterThanNameCnt
		{
			get
			{
				return "Argument count must be greater than number of named arguments.";
			}
		}

		internal static string InvalidMetaObjectCreated(object p0)
		{
			return global::SR.Format("An IDynamicMetaObjectProvider {0} created an invalid DynamicMetaObject instance.", p0);
		}

		internal static string BinderNotCompatibleWithCallSite(object p0, object p1, object p2)
		{
			return global::SR.Format("The result type '{0}' of the binder '{1}' is not compatible with the result type '{2}' expected by the call site.", p0, p1, p2);
		}

		internal static string DynamicBindingNeedsRestrictions(object p0, object p1)
		{
			return global::SR.Format("The result of the dynamic binding produced by the object with type '{0}' for the binder '{1}' needs at least one restriction.", p0, p1);
		}

		internal static string DynamicObjectResultNotAssignable(object p0, object p1, object p2, object p3)
		{
			return global::SR.Format("The result type '{0}' of the dynamic binding produced by the object with type '{1}' for the binder '{2}' is not compatible with the result type '{3}' expected by the call site.", new object[] { p0, p1, p2, p3 });
		}

		internal static string DynamicBinderResultNotAssignable(object p0, object p1, object p2)
		{
			return global::SR.Format("The result type '{0}' of the dynamic binding produced by binder '{1}' is not compatible with the result type '{2}' expected by the call site.", p0, p1, p2);
		}

		internal static string BindingCannotBeNull
		{
			get
			{
				return "Bind cannot return null.";
			}
		}

		internal static string DuplicateVariable(object p0)
		{
			return global::SR.Format("Found duplicate parameter '{0}'. Each ParameterExpression in the list must be a unique object.", p0);
		}

		internal static string ArgumentTypeCannotBeVoid
		{
			get
			{
				return "Argument type cannot be void";
			}
		}

		internal static string TypeParameterIsNotDelegate(object p0)
		{
			return global::SR.Format("Type parameter is {0}. Expected a delegate.", p0);
		}

		internal static string NoOrInvalidRuleProduced
		{
			get
			{
				return "No or Invalid rule produced";
			}
		}

		internal static string TypeMustBeDerivedFromSystemDelegate
		{
			get
			{
				return "Type must be derived from System.Delegate";
			}
		}

		internal static string FirstArgumentMustBeCallSite
		{
			get
			{
				return "First argument of delegate must be CallSite";
			}
		}

		internal static string StartEndMustBeOrdered
		{
			get
			{
				return "Start and End must be well ordered";
			}
		}

		internal static string FaultCannotHaveCatchOrFinally
		{
			get
			{
				return "fault cannot be used with catch or finally clauses";
			}
		}

		internal static string TryMustHaveCatchFinallyOrFault
		{
			get
			{
				return "try must have at least one catch, finally, or fault clause";
			}
		}

		internal static string BodyOfCatchMustHaveSameTypeAsBodyOfTry
		{
			get
			{
				return "Body of catch must have the same type as body of try.";
			}
		}

		internal static string ExtensionNodeMustOverrideProperty(object p0)
		{
			return global::SR.Format("Extension node must override the property {0}.", p0);
		}

		internal static string UserDefinedOperatorMustBeStatic(object p0)
		{
			return global::SR.Format("User-defined operator method '{0}' must be static.", p0);
		}

		internal static string UserDefinedOperatorMustNotBeVoid(object p0)
		{
			return global::SR.Format("User-defined operator method '{0}' must not be void.", p0);
		}

		internal static string CoercionOperatorNotDefined(object p0, object p1)
		{
			return global::SR.Format("No coercion operator is defined between types '{0}' and '{1}'.", p0, p1);
		}

		internal static string UnaryOperatorNotDefined(object p0, object p1)
		{
			return global::SR.Format("The unary operator {0} is not defined for the type '{1}'.", p0, p1);
		}

		internal static string BinaryOperatorNotDefined(object p0, object p1, object p2)
		{
			return global::SR.Format("The binary operator {0} is not defined for the types '{1}' and '{2}'.", p0, p1, p2);
		}

		internal static string ReferenceEqualityNotDefined(object p0, object p1)
		{
			return global::SR.Format("Reference equality is not defined for the types '{0}' and '{1}'.", p0, p1);
		}

		internal static string OperandTypesDoNotMatchParameters(object p0, object p1)
		{
			return global::SR.Format("The operands for operator '{0}' do not match the parameters of method '{1}'.", p0, p1);
		}

		internal static string OverloadOperatorTypeDoesNotMatchConversionType(object p0, object p1)
		{
			return global::SR.Format("The return type of overload method for operator '{0}' does not match the parameter type of conversion method '{1}'.", p0, p1);
		}

		internal static string ConversionIsNotSupportedForArithmeticTypes
		{
			get
			{
				return "Conversion is not supported for arithmetic types without operator overloading.";
			}
		}

		internal static string ArgumentMustBeArray
		{
			get
			{
				return "Argument must be array";
			}
		}

		internal static string ArgumentMustBeBoolean
		{
			get
			{
				return "Argument must be boolean";
			}
		}

		internal static string EqualityMustReturnBoolean(object p0)
		{
			return global::SR.Format("The user-defined equality method '{0}' must return a boolean value.", p0);
		}

		internal static string ArgumentMustBeFieldInfoOrPropertyInfo
		{
			get
			{
				return "Argument must be either a FieldInfo or PropertyInfo";
			}
		}

		internal static string ArgumentMustBeFieldInfoOrPropertyInfoOrMethod
		{
			get
			{
				return "Argument must be either a FieldInfo, PropertyInfo or MethodInfo";
			}
		}

		internal static string ArgumentMustBeInstanceMember
		{
			get
			{
				return "Argument must be an instance member";
			}
		}

		internal static string ArgumentMustBeInteger
		{
			get
			{
				return "Argument must be of an integer type";
			}
		}

		internal static string ArgumentMustBeArrayIndexType
		{
			get
			{
				return "Argument for array index must be of type Int32";
			}
		}

		internal static string ArgumentMustBeSingleDimensionalArrayType
		{
			get
			{
				return "Argument must be single-dimensional, zero-based array type";
			}
		}

		internal static string ArgumentTypesMustMatch
		{
			get
			{
				return "Argument types do not match";
			}
		}

		internal static string CannotAutoInitializeValueTypeElementThroughProperty(object p0)
		{
			return global::SR.Format("Cannot auto initialize elements of value type through property '{0}', use assignment instead", p0);
		}

		internal static string CannotAutoInitializeValueTypeMemberThroughProperty(object p0)
		{
			return global::SR.Format("Cannot auto initialize members of value type through property '{0}', use assignment instead", p0);
		}

		internal static string IncorrectTypeForTypeAs(object p0)
		{
			return global::SR.Format("The type used in TypeAs Expression must be of reference or nullable type, {0} is neither", p0);
		}

		internal static string CoalesceUsedOnNonNullType
		{
			get
			{
				return "Coalesce used with type that cannot be null";
			}
		}

		internal static string ExpressionTypeCannotInitializeArrayType(object p0, object p1)
		{
			return global::SR.Format("An expression of type '{0}' cannot be used to initialize an array of type '{1}'", p0, p1);
		}

		internal static string ArgumentTypeDoesNotMatchMember(object p0, object p1)
		{
			return global::SR.Format(" Argument type '{0}' does not match the corresponding member type '{1}'", p0, p1);
		}

		internal static string ArgumentMemberNotDeclOnType(object p0, object p1)
		{
			return global::SR.Format(" The member '{0}' is not declared on type '{1}' being created", p0, p1);
		}

		internal static string ExpressionTypeDoesNotMatchReturn(object p0, object p1)
		{
			return global::SR.Format("Expression of type '{0}' cannot be used for return type '{1}'", p0, p1);
		}

		internal static string ExpressionTypeDoesNotMatchAssignment(object p0, object p1)
		{
			return global::SR.Format("Expression of type '{0}' cannot be used for assignment to type '{1}'", p0, p1);
		}

		internal static string ExpressionTypeDoesNotMatchLabel(object p0, object p1)
		{
			return global::SR.Format("Expression of type '{0}' cannot be used for label of type '{1}'", p0, p1);
		}

		internal static string ExpressionTypeNotInvocable(object p0)
		{
			return global::SR.Format("Expression of type '{0}' cannot be invoked", p0);
		}

		internal static string FieldNotDefinedForType(object p0, object p1)
		{
			return global::SR.Format("Field '{0}' is not defined for type '{1}'", p0, p1);
		}

		internal static string InstanceFieldNotDefinedForType(object p0, object p1)
		{
			return global::SR.Format("Instance field '{0}' is not defined for type '{1}'", p0, p1);
		}

		internal static string FieldInfoNotDefinedForType(object p0, object p1, object p2)
		{
			return global::SR.Format("Field '{0}.{1}' is not defined for type '{2}'", p0, p1, p2);
		}

		internal static string IncorrectNumberOfIndexes
		{
			get
			{
				return "Incorrect number of indexes";
			}
		}

		internal static string IncorrectNumberOfLambdaDeclarationParameters
		{
			get
			{
				return "Incorrect number of parameters supplied for lambda declaration";
			}
		}

		internal static string IncorrectNumberOfMembersForGivenConstructor
		{
			get
			{
				return " Incorrect number of members for constructor";
			}
		}

		internal static string IncorrectNumberOfArgumentsForMembers
		{
			get
			{
				return "Incorrect number of arguments for the given members ";
			}
		}

		internal static string LambdaTypeMustBeDerivedFromSystemDelegate
		{
			get
			{
				return "Lambda type parameter must be derived from System.MulticastDelegate";
			}
		}

		internal static string MemberNotFieldOrProperty(object p0)
		{
			return global::SR.Format("Member '{0}' not field or property", p0);
		}

		internal static string MethodContainsGenericParameters(object p0)
		{
			return global::SR.Format("Method {0} contains generic parameters", p0);
		}

		internal static string MethodIsGeneric(object p0)
		{
			return global::SR.Format("Method {0} is a generic method definition", p0);
		}

		internal static string MethodNotPropertyAccessor(object p0, object p1)
		{
			return global::SR.Format("The method '{0}.{1}' is not a property accessor", p0, p1);
		}

		internal static string PropertyDoesNotHaveGetter(object p0)
		{
			return global::SR.Format("The property '{0}' has no 'get' accessor", p0);
		}

		internal static string PropertyDoesNotHaveSetter(object p0)
		{
			return global::SR.Format("The property '{0}' has no 'set' accessor", p0);
		}

		internal static string PropertyDoesNotHaveAccessor(object p0)
		{
			return global::SR.Format("The property '{0}' has no 'get' or 'set' accessors", p0);
		}

		internal static string NotAMemberOfType(object p0, object p1)
		{
			return global::SR.Format("'{0}' is not a member of type '{1}'", p0, p1);
		}

		internal static string NotAMemberOfAnyType(object p0)
		{
			return global::SR.Format("'{0}' is not a member of any type", p0);
		}

		internal static string ParameterExpressionNotValidAsDelegate(object p0, object p1)
		{
			return global::SR.Format("ParameterExpression of type '{0}' cannot be used for delegate parameter of type '{1}'", p0, p1);
		}

		internal static string PropertyNotDefinedForType(object p0, object p1)
		{
			return global::SR.Format("Property '{0}' is not defined for type '{1}'", p0, p1);
		}

		internal static string InstancePropertyNotDefinedForType(object p0, object p1)
		{
			return global::SR.Format("Instance property '{0}' is not defined for type '{1}'", p0, p1);
		}

		internal static string InstancePropertyWithoutParameterNotDefinedForType(object p0, object p1)
		{
			return global::SR.Format("Instance property '{0}' that takes no argument is not defined for type '{1}'", p0, p1);
		}

		internal static string InstancePropertyWithSpecifiedParametersNotDefinedForType(object p0, object p1, object p2)
		{
			return global::SR.Format("Instance property '{0}{1}' is not defined for type '{2}'", p0, p1, p2);
		}

		internal static string InstanceAndMethodTypeMismatch(object p0, object p1, object p2)
		{
			return global::SR.Format("Method '{0}' declared on type '{1}' cannot be called with instance of type '{2}'", p0, p1, p2);
		}

		internal static string TypeMissingDefaultConstructor(object p0)
		{
			return global::SR.Format("Type '{0}' does not have a default constructor", p0);
		}

		internal static string ElementInitializerMethodNotAdd
		{
			get
			{
				return "Element initializer method must be named 'Add'";
			}
		}

		internal static string ElementInitializerMethodNoRefOutParam(object p0, object p1)
		{
			return global::SR.Format("Parameter '{0}' of element initializer method '{1}' must not be a pass by reference parameter", p0, p1);
		}

		internal static string ElementInitializerMethodWithZeroArgs
		{
			get
			{
				return "Element initializer method must have at least 1 parameter";
			}
		}

		internal static string ElementInitializerMethodStatic
		{
			get
			{
				return "Element initializer method must be an instance method";
			}
		}

		internal static string TypeNotIEnumerable(object p0)
		{
			return global::SR.Format("Type '{0}' is not IEnumerable", p0);
		}

		internal static string UnhandledBinary(object p0)
		{
			return global::SR.Format("Unhandled binary: {0}", p0);
		}

		internal static string UnhandledBinding
		{
			get
			{
				return "Unhandled binding ";
			}
		}

		internal static string UnhandledBindingType(object p0)
		{
			return global::SR.Format("Unhandled Binding Type: {0}", p0);
		}

		internal static string UnhandledUnary(object p0)
		{
			return global::SR.Format("Unhandled unary: {0}", p0);
		}

		internal static string UnknownBindingType
		{
			get
			{
				return "Unknown binding type";
			}
		}

		internal static string UserDefinedOpMustHaveConsistentTypes(object p0, object p1)
		{
			return global::SR.Format("The user-defined operator method '{1}' for operator '{0}' must have identical parameter and return types.", p0, p1);
		}

		internal static string UserDefinedOpMustHaveValidReturnType(object p0, object p1)
		{
			return global::SR.Format("The user-defined operator method '{1}' for operator '{0}' must return the same type as its parameter or a derived type.", p0, p1);
		}

		internal static string LogicalOperatorMustHaveBooleanOperators(object p0, object p1)
		{
			return global::SR.Format("The user-defined operator method '{1}' for operator '{0}' must have associated boolean True and False operators.", p0, p1);
		}

		internal static string MethodWithArgsDoesNotExistOnType(object p0, object p1)
		{
			return global::SR.Format("No method '{0}' on type '{1}' is compatible with the supplied arguments.", p0, p1);
		}

		internal static string GenericMethodWithArgsDoesNotExistOnType(object p0, object p1)
		{
			return global::SR.Format("No generic method '{0}' on type '{1}' is compatible with the supplied type arguments and arguments. No type arguments should be provided if the method is non-generic. ", p0, p1);
		}

		internal static string MethodWithMoreThanOneMatch(object p0, object p1)
		{
			return global::SR.Format("More than one method '{0}' on type '{1}' is compatible with the supplied arguments.", p0, p1);
		}

		internal static string PropertyWithMoreThanOneMatch(object p0, object p1)
		{
			return global::SR.Format("More than one property '{0}' on type '{1}' is compatible with the supplied arguments.", p0, p1);
		}

		internal static string IncorrectNumberOfTypeArgsForFunc
		{
			get
			{
				return "An incorrect number of type arguments were specified for the declaration of a Func type.";
			}
		}

		internal static string IncorrectNumberOfTypeArgsForAction
		{
			get
			{
				return "An incorrect number of type arguments were specified for the declaration of an Action type.";
			}
		}

		internal static string ArgumentCannotBeOfTypeVoid
		{
			get
			{
				return "Argument type cannot be System.Void.";
			}
		}

		internal static string OutOfRange(object p0, object p1)
		{
			return global::SR.Format("{0} must be greater than or equal to {1}", p0, p1);
		}

		internal static string LabelTargetAlreadyDefined(object p0)
		{
			return global::SR.Format("Cannot redefine label '{0}' in an inner block.", p0);
		}

		internal static string LabelTargetUndefined(object p0)
		{
			return global::SR.Format("Cannot jump to undefined label '{0}'.", p0);
		}

		internal static string ControlCannotLeaveFinally
		{
			get
			{
				return "Control cannot leave a finally block.";
			}
		}

		internal static string ControlCannotLeaveFilterTest
		{
			get
			{
				return "Control cannot leave a filter test.";
			}
		}

		internal static string AmbiguousJump(object p0)
		{
			return global::SR.Format("Cannot jump to ambiguous label '{0}'.", p0);
		}

		internal static string ControlCannotEnterTry
		{
			get
			{
				return "Control cannot enter a try block.";
			}
		}

		internal static string ControlCannotEnterExpression
		{
			get
			{
				return "Control cannot enter an expression--only statements can be jumped into.";
			}
		}

		internal static string NonLocalJumpWithValue(object p0)
		{
			return global::SR.Format("Cannot jump to non-local label '{0}' with a value. Only jumps to labels defined in outer blocks can pass values.", p0);
		}

		internal static string CannotCompileConstant(object p0)
		{
			return global::SR.Format("CompileToMethod cannot compile constant '{0}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.", p0);
		}

		internal static string CannotCompileDynamic
		{
			get
			{
				return "Dynamic expressions are not supported by CompileToMethod. Instead, create an expression tree that uses System.Runtime.CompilerServices.CallSite.";
			}
		}

		internal static string MethodBuilderDoesNotHaveTypeBuilder
		{
			get
			{
				return "MethodBuilder does not have a valid TypeBuilder";
			}
		}

		internal static string InvalidLvalue(object p0)
		{
			return global::SR.Format("Invalid lvalue for assignment: {0}.", p0);
		}

		internal static string UndefinedVariable(object p0, object p1, object p2)
		{
			return global::SR.Format("variable '{0}' of type '{1}' referenced from scope '{2}', but it is not defined", p0, p1, p2);
		}

		internal static string CannotCloseOverByRef(object p0, object p1)
		{
			return global::SR.Format("Cannot close over byref parameter '{0}' referenced in lambda '{1}'", p0, p1);
		}

		internal static string UnexpectedVarArgsCall(object p0)
		{
			return global::SR.Format("Unexpected VarArgs call to method '{0}'", p0);
		}

		internal static string RethrowRequiresCatch
		{
			get
			{
				return "Rethrow statement is valid only inside a Catch block.";
			}
		}

		internal static string TryNotAllowedInFilter
		{
			get
			{
				return "Try expression is not allowed inside a filter body.";
			}
		}

		internal static string MustRewriteToSameNode(object p0, object p1, object p2)
		{
			return global::SR.Format("When called from '{0}', rewriting a node of type '{1}' must return a non-null value of the same type. Alternatively, override '{2}' and change it to not visit children of this type.", p0, p1, p2);
		}

		internal static string MustRewriteChildToSameType(object p0, object p1, object p2)
		{
			return global::SR.Format("Rewriting child expression from type '{0}' to type '{1}' is not allowed, because it would change the meaning of the operation. If this is intentional, override '{2}' and change it to allow this rewrite.", p0, p1, p2);
		}

		internal static string MustRewriteWithoutMethod(object p0, object p1)
		{
			return global::SR.Format("Rewritten expression calls operator method '{0}', but the original node had no operator method. If this is intentional, override '{1}' and change it to allow this rewrite.", p0, p1);
		}

		internal static string TryNotSupportedForMethodsWithRefArgs(object p0)
		{
			return global::SR.Format("TryExpression is not supported as an argument to method '{0}' because it has an argument with by-ref type. Construct the tree so the TryExpression is not nested inside of this expression.", p0);
		}

		internal static string TryNotSupportedForValueTypeInstances(object p0)
		{
			return global::SR.Format("TryExpression is not supported as a child expression when accessing a member on type '{0}' because it is a value type. Construct the tree so the TryExpression is not nested inside of this expression.", p0);
		}

		internal static string TestValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
		{
			return global::SR.Format("Test value of type '{0}' cannot be used for the comparison method parameter of type '{1}'", p0, p1);
		}

		internal static string SwitchValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
		{
			return global::SR.Format("Switch value of type '{0}' cannot be used for the comparison method parameter of type '{1}'", p0, p1);
		}

		internal static string NonStaticConstructorRequired
		{
			get
			{
				return "The constructor should not be static";
			}
		}

		internal static string NonAbstractConstructorRequired
		{
			get
			{
				return "Can't compile a NewExpression with a constructor declared on an abstract class";
			}
		}

		internal static string ExpressionMustBeReadable
		{
			get
			{
				return "Expression must be readable";
			}
		}

		internal static string ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1)
		{
			return global::SR.Format("Expression of type '{0}' cannot be used for constructor parameter of type '{1}'", p0, p1);
		}

		internal static string EnumerationIsDone
		{
			get
			{
				return "Enumeration has either not started or has already finished.";
			}
		}

		internal static string TypeContainsGenericParameters(object p0)
		{
			return global::SR.Format("Type {0} contains generic parameters", p0);
		}

		internal static string TypeIsGeneric(object p0)
		{
			return global::SR.Format("Type {0} is a generic type definition", p0);
		}

		internal static string InvalidArgumentValue
		{
			get
			{
				return "Invalid argument value";
			}
		}

		internal static string NonEmptyCollectionRequired
		{
			get
			{
				return "Non-empty collection required";
			}
		}

		internal static string InvalidNullValue(object p0)
		{
			return global::SR.Format("The value null is not of type '{0}' and cannot be used in this collection.", p0);
		}

		internal static string InvalidObjectType(object p0, object p1)
		{
			return global::SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this collection.", p0, p1);
		}

		internal static string ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2)
		{
			return global::SR.Format("Expression of type '{0}' cannot be used for parameter of type '{1}' of method '{2}'", p0, p1, p2);
		}

		internal static string ExpressionTypeDoesNotMatchParameter(object p0, object p1)
		{
			return global::SR.Format("Expression of type '{0}' cannot be used for parameter of type '{1}'", p0, p1);
		}

		internal static string IncorrectNumberOfMethodCallArguments(object p0)
		{
			return global::SR.Format("Incorrect number of arguments supplied for call to method '{0}'", p0);
		}

		internal static string IncorrectNumberOfLambdaArguments
		{
			get
			{
				return "Incorrect number of arguments supplied for lambda invocation";
			}
		}

		internal static string IncorrectNumberOfConstructorArguments
		{
			get
			{
				return "Incorrect number of arguments for constructor";
			}
		}
	}
}
