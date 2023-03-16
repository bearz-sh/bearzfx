using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

using Bearz.Extra.Strings;

namespace Bearz.Dynamic;

public class DynamicMap<TKey, TValue> : Dictionary<TKey, TValue>,
    IDynamicMetaObjectProvider
    where TKey : notnull
{
    [SuppressMessage(
        "Major Code Smell",
        "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
        Justification = "reviewed")]
    [SuppressMessage("ReflectionAnalyzers.SystemReflection", "REFL013:The member is of the wrong type", Justification = "reviewed")]
    private static readonly PropertyInfo s_propertyInfo = typeof(DynamicMap<TKey, TValue>).GetProperty(
        nameof(forwardee),
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

    private ForwardingDynamicMap? forwardee;

    public DynamicMap()
    {
    }

    public DynamicMap(IDictionary<TKey, TValue> dictionary)
        : base(dictionary)
    {
    }

    public DynamicMap(IEqualityComparer<TKey> comparer)
        : base(comparer)
    {
    }

    [MaybeNull]
    public new TValue this[TKey key]
    {
        get
        {
            if (this.TryGetValue(key, out var value))
                return value;

            return default;
        }

        set => base[key] = value;
    }

    public DynamicMetaObject GetMetaObject(Expression parameter)
    {
        this.forwardee ??= new ForwardingDynamicMap(this);
        return new ForwardingMetaObject(
            parameter,
            BindingRestrictions.Empty,
            this,
            this.forwardee,
            exprA => Expression.Property(exprA, s_propertyInfo));
    }

    protected class ForwardingDynamicMap : DynamicObject
    {
        private readonly DynamicMap<TKey, TValue> map;

        private readonly DynamicMap<string, object?>? properties;

        private readonly Type type;

        public ForwardingDynamicMap(DynamicMap<TKey, TValue> map)
        {
            this.type = map.GetType();
            if (map is DynamicMap<string, object?> propertiesMap)
            {
                this.properties = propertiesMap;
            }

            this.map = map;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            try
            {
                result = this.type.InvokeMember(
                    binder.Name,
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    this.map,
                    args);

                return true;
            }
            catch
            {
                Debug.WriteLine($"Calling {binder.Name}() failed.");
            }

            result = null;
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (this.properties?.TryGetValue(binder.Name, out result) == true)
            {
                return true;
            }

            if (binder.Name.EqualsIgnoreCase("Count"))
            {
                result = this.map.Count;
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            if (this.properties != null)
            {
                this.properties[binder.Name] = value;
                return true;
            }

            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
        {
            if (indexes.Length == 1)
            {
                if (indexes[0] is not TKey key)
                {
                    result = null;
                    return false;
                }

                if (this.map.TryGetValue(key, out var value))
                {
                    result = value;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
        {
            if (indexes.Length == 1)
            {
                if (indexes[0] is not TKey key)
                {
                    return false;
                }

                switch (value)
                {
                    case null:
                        this.map[key] = default!;
                        return true;

                    case TValue typedValue:
                        this.map[key] = typedValue;
                        return true;
                }
            }

            return false;
        }
    }
}