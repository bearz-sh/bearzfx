using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

using Bearz.Reflection;

namespace Bearz.Dynamic;

public class DynamicObject<T> : IReadOnlyDictionary<string, object?>,
    IDictionary<string, object?>,
    IDynamicMetaObjectProvider
{
    [SuppressMessage("ReflectionAnalyzers.SystemReflection", "REFL013:The member is of the wrong type", Justification = "reviewed")]
    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "reviewed")]
    private static readonly PropertyInfo s_propertyInfo = typeof(DynamicObject<T>).GetProperty(
        nameof(forwardee),
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

    private readonly IDictionary<string, object?> properties;

    [MaybeNull]
    private readonly T value;

    private ForwardingDynamicObject? forwardee;

    public DynamicObject()
    {
        this.value = default!;
        this.properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    public DynamicObject(T value)
    {
        this.properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        this.value = value;
    }

    public DynamicObject(IDictionary<string, object?> properties)
    {
        this.value = default!;
        this.properties = properties;
    }

    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => this.properties.Keys;

    ICollection<object?> IDictionary<string, object?>.Values => this.properties.Values;

    ICollection<string> IDictionary<string, object?>.Keys => this.properties.Keys;

    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => this.properties.Values;

    int ICollection<KeyValuePair<string, object?>>.Count => this.properties.Count;

    bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => this.properties.IsReadOnly;

    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => this.properties.Count;

    public bool HasValue => this.value != null;

    public object? this[string key]
    {
        get
        {
            if (this.TryGetValue(key, out var value))
                return value;

            return default;
        }

        set => this.properties[key] = value;
    }

    void IDictionary<string, object?>.Add(string key, object? value)
        => this.properties.Add(key, value);

    bool IDictionary<string, object?>.ContainsKey(string key)
        => this.properties.ContainsKey(key);

    bool IDictionary<string, object?>.Remove(string key)
        => this.properties.Remove(key);

    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key)
        => this.properties.ContainsKey(key);

    public bool TryGetValue(string key, out object? value)
    {
        return this.properties.TryGetValue(key, out value);
    }

    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator()
        => this.properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.properties.GetEnumerator();

    void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item)
        => this.properties.Add(item);

    void ICollection<KeyValuePair<string, object?>>.Clear()
        => this.properties.Clear();

    bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
        => this.properties.Contains(item);

    void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        => this.properties.CopyTo(array, arrayIndex);

    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
        => this.properties.Remove(item);

    public DynamicMetaObject GetMetaObject(Expression parameter)
    {
        this.forwardee ??= new ForwardingDynamicObject(this);
        return new ForwardingMetaObject(
            parameter,
            BindingRestrictions.Empty,
            this,
            this.forwardee,
            exprA => Expression.Property(exprA, s_propertyInfo));
    }

    protected class ForwardingDynamicObject : DynamicObject
    {
        private readonly DynamicObject<T> instance;

        private readonly Type type = typeof(T);

        public ForwardingDynamicObject(DynamicObject<T> instance)
        {
            this.instance = instance;
        }

        /*
        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            DelegateFactory.Create(binder.)
            return this.type.TryInvokeMember(
                this.instance,
                binder.Name,
                true,
                args,
                out result);
        }
        */

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (this.instance.TryGetValue(binder.Name, out result))
            {
                return true;
            }

            var property = this.type.GetProperty(binder.Name);
            if (property is not null)
            {
                result = property.GetValue(this.instance);
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            var overriden = this.instance.properties.ContainsKey(binder.Name);
            if (!overriden)
            {
                var property = this.type.GetProperty(binder.Name);
                if (property is not null)
                {
                    property.SetValue(this.instance, value);
                    return true;
                }
            }

            this.instance.properties[binder.Name] = value;
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
        {
            if (indexes.Length == 1 && indexes[0] is string key)
            {
                if (this.instance.TryGetValue(key, out result))
                {
                    return true;
                }

                var property = this.type.GetProperty(key);
                if (property is not null)
                {
                    result = property.GetValue(this.instance);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
        {
            if (indexes.Length == 1 && indexes[0] is string key)
            {
                var overriden = this.instance.properties.ContainsKey(key);
                if (!overriden)
                {
                    var property = this.type.GetProperty(key);
                    if (property is not null)
                    {
                        property.SetValue(this.instance, value);
                        return true;
                    }
                }

                this.instance.properties[key] = value;
                return true;
            }

            return false;
        }
    }
}