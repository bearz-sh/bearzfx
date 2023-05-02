// Copyright © 2017 - 2021 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// source: https://github.com/chocolatey/choco/blob/370e36632670d3e3d25a874ea8175551891bca50/src/chocolatey/infrastructure/guards/Ensure.cs

using System.Linq.Expressions;

using Bearz.Extra.Strings;

namespace Cocoa.Guards;

public static class Ensure
{
    public static EnsureString That(Expression<Func<string>> expression)
    {
        var memberName = expression.GetNameOnRight().Member.Name;
        return new EnsureString(memberName, expression.Compile().Invoke());
    }

    public static Ensure<TypeToEnsure> That<TypeToEnsure>(Expression<Func<TypeToEnsure>> expression)
        where TypeToEnsure : class
    {
        var memberName = expression.GetNameOnRight().Member.Name;
        return new Ensure<TypeToEnsure>(memberName, expression.Compile().Invoke());
    }

    // This method needs a beter name.
    private static MemberExpression GetNameOnRight(this Expression? e)
    {
        if (e is LambdaExpression lambdaExpr)
            return GetNameOnRight(lambdaExpr.Body);

        if (e is MemberExpression memberExpr)
            return memberExpr;

        if (e is MethodCallExpression methodExpr)
        {
            var member = methodExpr.Arguments.Count > 0 ? methodExpr.Arguments[0] : methodExpr.Object;
            return GetNameOnRight(member);
        }

        if (e is UnaryExpression unaryExpr)
        {
            return GetNameOnRight(unaryExpr.Operand);
        }

        throw new InvalidOperationException($"Unable to find member for {e}");
    }
}

public class EnsureString : Ensure<string>
{
    public EnsureString(string name, string value)
        : base(name, value)
    {
    }

    public EnsureString NotNullOrWhitespace()
    {
        this.NotNull();
        if (this.Value.IsNullOrWhiteSpace())
        {
            throw new ArgumentException(
                this.Name,
                $"Value for {this.Name} cannot be empty or only contain whitespace.");
        }

        return this;
    }

    public EnsureString HasExtension(params string[] extensions)
    {
        var actualExtension = Path.GetExtension(this.Value);

        foreach (var extension in extensions)
        {
            if (extension.EqualsInvariant(actualExtension))
            {
                return this;
            }
        }

        throw new ArgumentException(
            this.Name,
            $"Value for {this.Name} must contain one of the following extensions: {string.Join(",", extensions)}");
    }
}

public class Ensure<TEnsurable>
    where TEnsurable : class
{
    public Ensure(string name, TEnsurable value)
    {
        this.Name = name;
        this.Value = value;
    }

    public string Name { get; }

    public TEnsurable? Value { get; }

    public void NotNull()
    {
        if (this.Value == null)
        {
            throw new ArgumentNullException(this.Name, $"Value for {this.Name} cannot be null.");
        }
    }

    public void Meets(Func<TEnsurable?, bool> ensureFunction, Action<string, TEnsurable?> exceptionAction)
    {
        Ensure.That(() => ensureFunction).NotNull();
        Ensure.That(() => exceptionAction).NotNull();

        if (!ensureFunction(this.Value))
        {
            exceptionAction.Invoke(this.Name, this.Value);
        }
    }
}