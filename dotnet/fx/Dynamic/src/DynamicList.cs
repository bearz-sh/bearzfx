using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bearz.Dynamic;

public class DynamicList<T> : List<T>, IDynamicMetaObjectProvider
{
    [SuppressMessage(
        "ReflectionAnalyzers.SystemReflection",
        "REFL013:The member is of the wrong type",
        Justification = "reviewed")]
    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Reviewed")]
    private static readonly PropertyInfo s_propertyInfo = typeof(DynamicList<T>).GetProperty(
        nameof(forwardee),
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

    private ForwardingDynamicList? forwardee;

    public DynamicList()
    {
    }

    public DynamicList(IEnumerable<T> collection)
        : base(collection)
    {
    }

    public new T this[int index]
    {
        get
        {
            if (index >= this.Count)
            {
                return default!;
            }

            if (index < 0 && index > -this.Count)
            {
                var i = this.Count - index;
                return base[i];
            }

            return base[index];
        }

        set => base[index] = value;
    }

    public bool MatchAll(Predicate<T> predicate)
    {
        foreach (var item in this)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    public bool MatchOne(Predicate<T> predicate)
    {
        foreach (var item in this)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    public new DynamicList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        => new(base.ConvertAll(converter));

    public void Fill(T value)
        => this.Fill(value, 0, this.Count);

    public void Fill(T value, int startIndex, int count)
    {
        var c = Math.Min(this.Count, count);

        for (; startIndex < c; startIndex++)
        {
            this[startIndex] = value;
        }
    }

    public T Pop()
    {
        if (this.Count == 0)
            return default!;

        var index = this.Count - 1;
        var item = base[index];
        this.RemoveAt(index);
        return item;
    }

    public T Reduce(Func<T, T, T> reducer, T initial)
    {
        var result = initial;
        foreach (var item in this)
        {
            result = reducer(result, item);
        }

        return result;
    }

    public T Reduce(Func<T, T, T> reducer)
    {
        if (this.Count == 0)
            return default!;

        var result = base[0];
        for (var i = 1; i < this.Count; i++)
        {
            result = reducer(result, base[i]);
        }

        return result;
    }

    public bool RemoveRange(IEnumerable<T> items)
    {
        var atLeastOneRemoved = false;
        foreach (var c in items)
        {
            if (this.Remove(c))
                atLeastOneRemoved = true;
        }

        return atLeastOneRemoved;
    }

    public DynamicMetaObject GetMetaObject(Expression parameter)
    {
        this.forwardee ??= new ForwardingDynamicList(this);
        return new ForwardingMetaObject(
            parameter,
            BindingRestrictions.Empty,
            this,
            this.forwardee,
            exprA => Expression.Property(exprA, s_propertyInfo));
    }

    protected class ForwardingDynamicList : DynamicObject
    {
        private readonly DynamicList<T> list;

        private readonly bool nullable;

        public ForwardingDynamicList(DynamicList<T> list)
        {
            var type = typeof(T);
            if (Nullable.GetUnderlyingType(typeof(T)) != null || type.IsClass)
            {
                this.nullable = true;
            }

            this.list = list;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var name = binder.Name.ToLowerInvariant();
            switch (name)
            {
                case "count":
                    result = this.list.Count;
                    return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            var name = binder.Name;
            switch (name)
            {
                case "add":
                    {
                        result = null;
                        if (args?.Length == 1)
                        {
                            var next = args[0];
                            if (next is T item)
                            {
                                this.list.Add(item);
                                return true;
                            }

                            if (this.nullable && next is null)
                            {
                                this.list.Add(default!);
                                return true;
                            }
                        }

                        return false;
                    }

                case "addrange":
                    {
                        result = null;
                        if (args?.Length == 1 && args[0] is IEnumerable<T> items)
                        {
                            this.list.AddRange(items);
                            return true;
                        }

                        return false;
                    }

                case "clear":
                    {
                        result = null;
                        if (args?.Length > 0)
                        {
                            return false;
                        }

                        this.list.Clear();
                        return true;
                    }

                case "contains":
                    {
                        result = false;
                        if (args?.Length == 1)
                        {
                            var next = args[0];
                            if (next is T item)
                            {
                                result = this.list.Contains(item);
                                return true;
                            }

                            if (this.nullable && next is null)
                            {
                                result = this.list.Contains(default!);
                                return true;
                            }
                        }

                        return false;
                    }

                default:
                    try
                    {
                        result = this.list.GetType()
                            .InvokeMember(
                                name,
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase,
                                null,
                                this.list,
                                args);

                        return true;
                    }
                    catch
                    {
                        result = null;
                        return false;
                    }
            }
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
        {
            if (indexes.Length == 1 && indexes[0] is int key && value is T item)
            {
                this.list[key] = item;
                return true;
            }

            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
        {
            if (indexes.Length == 1 && indexes[0] is int key && key >= 0 && key < this.list.Count)
            {
                result = this.list[key];
                return true;
            }

            result = default(T);
            return false;
        }
    }
}