using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace Bearz.Dynamic;

/// <summary>
///  Based on
///  <see href="https://matousek.wordpress.com/2009/11/07/forwarding-meta-object/">Tomáš Matoušek’s</see>
///  Forwarding MetaObj.
/// </summary>
public sealed class ForwardingMetaObject : DynamicMetaObject
{
    private readonly DynamicMetaObject metaForwardee;

    public ForwardingMetaObject(
        Expression expression,
        BindingRestrictions restrictions,
        object forwarder,
        IDynamicMetaObjectProvider forwardee,
        Func<Expression, Expression> forwardeeGetter)
        : base(expression, restrictions, forwarder)
    {
        // We'll use forwardee's meta-object to bind dynamic operations.
        this.metaForwardee = forwardee.GetMetaObject(
            forwardeeGetter(
                Expression.Convert(expression, forwarder.GetType())));
    }

    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
        return this.AddRestrictions(this.metaForwardee.BindGetMember(binder));
    }

    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
        return this.AddRestrictions(this.metaForwardee.BindSetMember(binder, value));
    }

    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
        return this.AddRestrictions(this.metaForwardee.BindDeleteMember(binder));
    }

    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
    {
        return this.AddRestrictions(this.metaForwardee.BindGetIndex(binder, indexes));
    }

    public override DynamicMetaObject BindSetIndex(
        SetIndexBinder binder,
        DynamicMetaObject[] indexes,
        DynamicMetaObject value)
    {
        return this.AddRestrictions(this.metaForwardee.BindSetIndex(binder, indexes, value));
    }

    public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
    {
        return this.AddRestrictions(this.metaForwardee.BindDeleteIndex(binder, indexes));
    }

    public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
    {
        return this.AddRestrictions(this.metaForwardee.BindInvokeMember(binder, args));
    }

    public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
    {
        return this.AddRestrictions(this.metaForwardee.BindInvoke(binder, args));
    }

    public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
    {
        return this.AddRestrictions(this.metaForwardee.BindCreateInstance(binder, args));
    }

    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
    {
        return this.AddRestrictions(this.metaForwardee.BindUnaryOperation(binder));
    }

    public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
    {
        return this.AddRestrictions(this.metaForwardee.BindBinaryOperation(binder, arg));
    }

    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    {
        return this.AddRestrictions(this.metaForwardee.BindConvert(binder));
    }

    // Restricts the target object's type to TForwarder.
    // The meta-object we are forwarding to assumes that it gets an instance of TForwarder (see [1]).
    // We need to ensure that the assumption holds.
    private DynamicMetaObject AddRestrictions(DynamicMetaObject result)
    {
#pragma warning disable S112, CS8604 // General exceptions should never be thrown
        if (this.Value is null)
            throw new NullReferenceException($"Value property should not be null for {nameof(ForwardingMetaObject)}");

        var restricted = new DynamicMetaObject(
            result.Expression,
            BindingRestrictions.GetTypeRestriction(this.Expression, this.Value.GetType())
                .Merge(result.Restrictions),
            this.metaForwardee.Value);

        return restricted;
    }
}