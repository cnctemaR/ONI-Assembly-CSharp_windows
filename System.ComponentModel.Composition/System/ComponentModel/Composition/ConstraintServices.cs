using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal static class ConstraintServices
	{
		public static Expression<Func<ExportDefinition, bool>> CreateConstraint(string contractName, string requiredTypeIdentity, IEnumerable<KeyValuePair<string, Type>> requiredMetadata, CreationPolicy requiredCreationPolicy)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(ExportDefinition), "exportDefinition");
			Expression expression = ConstraintServices.CreateContractConstraintBody(contractName, parameterExpression);
			if (!string.IsNullOrEmpty(requiredTypeIdentity))
			{
				Expression expression2 = ConstraintServices.CreateTypeIdentityContraint(requiredTypeIdentity, parameterExpression);
				expression = Expression.AndAlso(expression, expression2);
			}
			if (requiredMetadata != null)
			{
				Expression expression3 = ConstraintServices.CreateMetadataConstraintBody(requiredMetadata, parameterExpression);
				if (expression3 != null)
				{
					expression = Expression.AndAlso(expression, expression3);
				}
			}
			if (requiredCreationPolicy != CreationPolicy.Any)
			{
				Expression expression4 = ConstraintServices.CreateCreationPolicyContraint(requiredCreationPolicy, parameterExpression);
				expression = Expression.AndAlso(expression, expression4);
			}
			return Expression.Lambda<Func<ExportDefinition, bool>>(expression, new ParameterExpression[] { parameterExpression });
		}

		private static Expression CreateContractConstraintBody(string contractName, ParameterExpression parameter)
		{
			Assumes.NotNull<ParameterExpression>(parameter);
			return Expression.Equal(Expression.Property(parameter, ConstraintServices._exportDefinitionContractNameProperty), Expression.Constant(contractName ?? string.Empty, typeof(string)));
		}

		private static Expression CreateMetadataConstraintBody(IEnumerable<KeyValuePair<string, Type>> requiredMetadata, ParameterExpression parameter)
		{
			Assumes.NotNull<IEnumerable<KeyValuePair<string, Type>>>(requiredMetadata);
			Assumes.NotNull<ParameterExpression>(parameter);
			Expression expression = null;
			foreach (KeyValuePair<string, Type> keyValuePair in requiredMetadata)
			{
				Expression expression2 = ConstraintServices.CreateMetadataContainsKeyExpression(parameter, keyValuePair.Key);
				expression = ((expression != null) ? Expression.AndAlso(expression, expression2) : expression2);
				expression = Expression.AndAlso(expression, ConstraintServices.CreateMetadataOfTypeExpression(parameter, keyValuePair.Key, keyValuePair.Value));
			}
			return expression;
		}

		private static Expression CreateCreationPolicyContraint(CreationPolicy policy, ParameterExpression parameter)
		{
			Assumes.IsTrue(policy > CreationPolicy.Any);
			Assumes.NotNull<ParameterExpression>(parameter);
			return Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(ExpressionType.OrElse, Expression.Not(ConstraintServices.CreateMetadataContainsKeyExpression(parameter, "System.ComponentModel.Composition.CreationPolicy")), ConstraintServices.CreateMetadataValueEqualsExpression(parameter, CreationPolicy.Any, "System.ComponentModel.Composition.CreationPolicy")), ConstraintServices.CreateMetadataValueEqualsExpression(parameter, policy, "System.ComponentModel.Composition.CreationPolicy"));
		}

		private static Expression CreateTypeIdentityContraint(string requiredTypeIdentity, ParameterExpression parameter)
		{
			Assumes.NotNull<string>(requiredTypeIdentity);
			Assumes.NotNull<ParameterExpression>(parameter);
			return Expression.MakeBinary(ExpressionType.AndAlso, ConstraintServices.CreateMetadataContainsKeyExpression(parameter, "ExportTypeIdentity"), ConstraintServices.CreateMetadataValueEqualsExpression(parameter, requiredTypeIdentity, "ExportTypeIdentity"));
		}

		private static Expression CreateMetadataContainsKeyExpression(ParameterExpression parameter, string constantKey)
		{
			Assumes.NotNull<ParameterExpression, string>(parameter, constantKey);
			return Expression.Call(Expression.Property(parameter, ConstraintServices._exportDefinitionMetadataProperty), ConstraintServices._metadataContainsKeyMethod, new Expression[] { Expression.Constant(constantKey) });
		}

		private static Expression CreateMetadataOfTypeExpression(ParameterExpression parameter, string constantKey, Type constantType)
		{
			Assumes.NotNull<ParameterExpression, string>(parameter, constantKey);
			Assumes.NotNull<ParameterExpression, Type>(parameter, constantType);
			return Expression.Call(Expression.Constant(constantType, typeof(Type)), ConstraintServices._typeIsInstanceOfTypeMethod, new Expression[] { Expression.Call(Expression.Property(parameter, ConstraintServices._exportDefinitionMetadataProperty), ConstraintServices._metadataItemMethod, new Expression[] { Expression.Constant(constantKey) }) });
		}

		private static Expression CreateMetadataValueEqualsExpression(ParameterExpression parameter, object constantValue, string metadataName)
		{
			Assumes.NotNull<ParameterExpression, object>(parameter, constantValue);
			return Expression.Call(Expression.Constant(constantValue), ConstraintServices._metadataEqualsMethod, new Expression[] { Expression.Call(Expression.Property(parameter, ConstraintServices._exportDefinitionMetadataProperty), ConstraintServices._metadataItemMethod, new Expression[] { Expression.Constant(metadataName) }) });
		}

		public static Expression<Func<ExportDefinition, bool>> CreatePartCreatorConstraint(Expression<Func<ExportDefinition, bool>> baseConstraint, ImportDefinition productImportDefinition)
		{
			ParameterExpression parameterExpression = baseConstraint.Parameters[0];
			Expression expression = Expression.Property(parameterExpression, ConstraintServices._exportDefinitionMetadataProperty);
			Expression expression2 = Expression.Call(expression, ConstraintServices._metadataContainsKeyMethod, new Expression[] { Expression.Constant("ProductDefinition") });
			Expression expression3 = Expression.Call(expression, ConstraintServices._metadataItemMethod, new Expression[] { Expression.Constant("ProductDefinition") });
			Expression expression4 = Expression.Invoke(productImportDefinition.Constraint, new Expression[] { Expression.Convert(expression3, typeof(ExportDefinition)) });
			return Expression.Lambda<Func<ExportDefinition, bool>>(Expression.AndAlso(baseConstraint.Body, Expression.AndAlso(expression2, expression4)), new ParameterExpression[] { parameterExpression });
		}

		private static readonly PropertyInfo _exportDefinitionContractNameProperty = typeof(ExportDefinition).GetProperty("ContractName");

		private static readonly PropertyInfo _exportDefinitionMetadataProperty = typeof(ExportDefinition).GetProperty("Metadata");

		private static readonly MethodInfo _metadataContainsKeyMethod = typeof(IDictionary<string, object>).GetMethod("ContainsKey");

		private static readonly MethodInfo _metadataItemMethod = typeof(IDictionary<string, object>).GetMethod("get_Item");

		private static readonly MethodInfo _metadataEqualsMethod = typeof(object).GetMethod("Equals", new Type[] { typeof(object) });

		private static readonly MethodInfo _typeIsInstanceOfTypeMethod = typeof(Type).GetMethod("IsInstanceOfType");
	}
}
