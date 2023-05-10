using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

using Bearz.Extra.Arrays;

#pragma warning disable CS8601
namespace Bearz.Extra.Object;

[SuppressMessage("ReflectionAnalyzers.SystemReflection", "REFL029:Specify types in case an overload is added in the future")]
[SuppressMessage("ReflectionAnalyzers.SystemReflection", "REFL008:Specify binding flags for better performance and less fragile code")]
[SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields")]
[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
public static class ObjectExtensions
{
    private static readonly MethodInfo CloneMethod = typeof(object).GetMethod(
        nameof(MemberwiseClone),
        BindingFlags.NonPublic | BindingFlags.Instance);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSafeString(this object? value)
    {
        return value?.ToString() ?? string.Empty;
    }

    public static bool IsPrimitive(this Type type)
    {
        if (type == typeof(string))
            return true;
        return type is { IsValueType: true, IsPrimitive: true };
    }

    // https://github.com/Burtsev-Alexey/net-object-deep-copy/tree/master
    public static T DeepCopy<T>(this T? original)
    {
        var copy = DeepCopy((object)original!);
        return copy == null ? default! : (T)copy;
    }

    public static object? DeepCopy(this object? originalObject)
    {
        return InternalCopy(originalObject, new Dictionary<object, object?>(new ReferenceEqualityComparer()));
    }

    private static object? InternalCopy(object? originalObject, IDictionary<object, object?> visited)
    {
        if (originalObject == null)
            return null;
        var typeToReflect = originalObject.GetType();
        if (IsPrimitive(typeToReflect))
            return originalObject;
        if (visited.ContainsKey(originalObject))
            return visited[originalObject];

        if (originalObject is Delegate)
            return null;

        var cloneObject = CloneMethod.Invoke(originalObject, null);
        if (cloneObject is not null && typeToReflect.IsArray)
        {
            var arrayType = typeToReflect.GetElementType();
            if (arrayType is not null && !IsPrimitive(arrayType))
            {
                Array clonedArray = (Array)cloneObject;

                clonedArray.ForEach((array, indices) =>
                    array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
            }
        }

        visited.Add(originalObject, cloneObject);
        CopyFields(originalObject, visited, cloneObject, typeToReflect);
        RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
        return cloneObject;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object?> visited, object? cloneObject, Type typeToReflect)
    {
        if (typeToReflect.BaseType != null)
        {
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(
                originalObject,
                visited,
                cloneObject,
                typeToReflect.BaseType,
                BindingFlags.Instance | BindingFlags.NonPublic,
                info => info.IsPrivate);
        }
    }

    private static void CopyFields(
        object originalObject,
        IDictionary<object, object?> visited,
        object? cloneObject,
        Type typeToReflect,
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
        Func<FieldInfo, bool>? filter = null)
    {
        foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && !filter(fieldInfo))
                continue;

            if (IsPrimitive(fieldInfo.FieldType))
                continue;
            var originalFieldValue = fieldInfo.GetValue(originalObject);
            var clonedFieldValue = InternalCopy(originalFieldValue, visited);
            fieldInfo.SetValue(cloneObject, clonedFieldValue);
        }
    }
}

internal sealed class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object? x, object? y)
    {
        return ReferenceEquals(x, y);
    }

    public override int GetHashCode(object? obj)
    {
        if (obj == null)
            return 0;

        return obj.GetHashCode();
    }
}