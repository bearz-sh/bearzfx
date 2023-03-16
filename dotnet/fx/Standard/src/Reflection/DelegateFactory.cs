using System.Linq.Expressions;
using System.Reflection;

namespace Bearz.Reflection;

public static class DelegateFactory
{
    public static Delegate Create(PropertyInfo propertyInfo, bool createGetter)
    {
        return createGetter
            ? CreateGetter(propertyInfo)
            : CreateSetter(propertyInfo);
    }

    public static Delegate Create(FieldInfo fieldInfo, bool createGetter)
    {
        return createGetter
            ? CreateGetter(fieldInfo)
            : CreateSetter(fieldInfo);
    }

    public static Delegate Create(ConstructorInfo constructorInfo)
        => Create(constructorInfo, null);

    public static Delegate Create(ConstructorInfo constructorInfo, IEnumerable<ParameterInfo>? parameters)
    {
        var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
        var argumentExpressions = new List<Expression>();

        parameters ??= constructorInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            argumentExpressions.Add(
                Expression.Convert(
                    Expression.ArrayIndex(
                        argumentsExpression,
                        Expression.Constant(parameter.Position)),
                    parameter.ParameterType));
        }

        NewExpression newExpression;
        if (argumentExpressions.Count == 0)
            newExpression = Expression.New(constructorInfo);
        else
            newExpression = Expression.New(constructorInfo, argumentExpressions.ToArray());

        return Expression.Lambda<Func<object[], object>>(
                Expression.Convert(newExpression, typeof(object)),
                argumentsExpression)
            .Compile();
    }

    public static Delegate Create(MethodInfo methodInfo)
        => Create(methodInfo, null);

    public static Delegate Create(MethodInfo methodInfo, IEnumerable<ParameterInfo>? parameters)
    {
        var instanceExpression = Expression.Parameter(typeof(object), "obj");
        var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
        var argumentExpressions = new List<Expression>();

        parameters ??= methodInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            argumentExpressions.Add(
                Expression.Convert(
                    Expression.ArrayIndex(
                        argumentsExpression,
                        Expression.Constant(parameter.Position)),
                    parameter.ParameterType));
        }

        UnaryExpression? unary = null;
        if (!methodInfo.IsStatic && methodInfo.ReflectedType != null)
        {
            unary = Expression.Convert(instanceExpression, methodInfo.ReflectedType);
        }

        var callExpression = Expression.Call(
            unary,
            methodInfo,
            argumentExpressions);

        if (callExpression.Type == typeof(void))
        {
            var voidDelegate = Expression.Lambda<Action<object, object[]>>(
                    callExpression,
                    instanceExpression,
                    argumentsExpression)
                .Compile();

            Func<object, object[], object?> action = (obj, arguments) =>
            {
                voidDelegate(obj, arguments);
                return null;
            };

            return action;
        }
        else
        {
            return Expression.Lambda<Func<object, object[], object?>>(
                    Expression.Convert(
                        callExpression,
                        typeof(object)),
                    instanceExpression,
                    argumentsExpression)
                .Compile();
        }
    }

    public static Delegate CreateGetter(FieldInfo fieldInfo)
    {
        if (fieldInfo.IsStatic)
        {
            var invokeGet = Expression.Field(null, fieldInfo);
            return Expression
                .Lambda(Expression.Block(invokeGet))
                .Compile();
        }
        else
        {
            if (fieldInfo.DeclaringType is null)
                throw new InvalidOperationException($"Nonstatic field {fieldInfo.Name} is missing DeclaringType");

            var oVariable = Expression.Parameter(fieldInfo.DeclaringType, "o");
            var invokeGet = Expression.Field(oVariable, fieldInfo);
            return Expression
                .Lambda(Expression.Block(invokeGet), oVariable)
                .Compile();
        }
    }

    public static Delegate CreateGetter(PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanRead)
            throw new InvalidOperationException($"Property {propertyInfo.Name} prohibits reading the value.");

        var isStatic = propertyInfo.GetMethod?.IsStatic == true;

        if (isStatic)
        {
            var invokeGet = Expression.Property(null, propertyInfo);
            return Expression
                .Lambda(Expression.Block(invokeGet))
                .Compile();
        }
        else
        {
            if (propertyInfo.DeclaringType is null)
            {
                throw new InvalidOperationException(
                    $"Nonstatic property {propertyInfo.Name} is missing DeclaringType");
            }

            var indexExpressions = new List<Expression>();
            var argumentsExpression = Expression.Parameter(typeof(object?[]), "arguments");

            foreach (var parameter in propertyInfo.GetIndexParameters())
            {
                indexExpressions.Add(
                    Expression.Convert(
                        Expression.ArrayIndex(
                            argumentsExpression,
                            Expression.Constant(parameter.Position)),
                        parameter.ParameterType));
            }

            var oVariable = Expression.Parameter(propertyInfo.DeclaringType, "o");
            Expression invokeGet = indexExpressions.Count > 0
                ? Expression.Property(oVariable, propertyInfo, indexExpressions)
                : Expression.Property(oVariable, propertyInfo);
            var b = Expression.Block(invokeGet);
            return Expression
                .Lambda(b, oVariable)
                .Compile();
        }
    }

    public static Delegate CreateSetter(FieldInfo fieldInfo)
    {
        if (fieldInfo.IsStatic)
        {
            var invokeSet = Expression.Field(null, fieldInfo);
            var valueVariable = Expression.Variable(fieldInfo.FieldType, "value");
            var b = Expression.Block(
                Expression.Assign(invokeSet, valueVariable));

            return Expression.Lambda(b, valueVariable)
                .Compile();
        }
        else
        {
            if (fieldInfo.DeclaringType is null)
                throw new InvalidOperationException($"Nonstatic field {fieldInfo.Name} is missing DeclaringType");

            var oVariable = Expression.Parameter(fieldInfo.DeclaringType, "o");
            var invokeSet = Expression.Field(oVariable, fieldInfo);
            var valueVariable = Expression.Variable(fieldInfo.FieldType, "value");
            var b = Expression.Block(
                Expression.Assign(invokeSet, valueVariable));

            return Expression.Lambda(b, oVariable, valueVariable)
                .Compile();
        }
    }

    public static Delegate CreateSetter(PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanWrite)
            throw new InvalidOperationException($"Property {propertyInfo.Name} prohibits writing the value.");

        var isStatic = propertyInfo.SetMethod?.IsStatic == true;

        if (isStatic)
        {
            var invokeSet = Expression.Property(null, propertyInfo);
            var valueVariable = Expression.Variable(propertyInfo.PropertyType, "value");
            var b = Expression.Block(Expression.Assign(invokeSet, valueVariable));
            return Expression
                .Lambda(b, valueVariable)
                .Compile();
        }
        else
        {
            if (propertyInfo.DeclaringType is null)
            {
                throw new InvalidOperationException(
                    $"Nonstatic property {propertyInfo.Name} is missing DeclaringType");
            }

            var indexExpressions = new List<Expression>();
            var argumentsExpression = Expression.Parameter(typeof(object?[]), "arguments");

            foreach (var parameter in propertyInfo.GetIndexParameters())
            {
                indexExpressions.Add(
                    Expression.Convert(
                        Expression.ArrayIndex(
                            argumentsExpression,
                            Expression.Constant(parameter.Position)),
                        parameter.ParameterType));
            }

            var oVariable = Expression.Parameter(propertyInfo.DeclaringType, "o");
            var valueVariable = Expression.Variable(propertyInfo.PropertyType, "value");

            if (indexExpressions.Count > 0)
            {
                var invokeSet = Expression.Property(oVariable, propertyInfo, indexExpressions);
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));

                return Expression
                    .Lambda(
                        b,
                        oVariable,
                        valueVariable,
                        argumentsExpression)
                    .Compile();
            }
            else
            {
                var invokeSet = Expression.Property(oVariable, propertyInfo);
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));
                return Expression
                    .Lambda(b, oVariable, valueVariable)
                    .Compile();
            }
        }
    }
}