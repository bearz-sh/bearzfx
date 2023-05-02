// Copyright © 2023-Present Chocolatey Software, Inc
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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/rules/ImmutableRule.cs

namespace Cocoa.Nuget.Rules;

public readonly struct ImmutableRule
{
    public ImmutableRule(RuleType severity, string id, string summary, string? helpUrl = null)
    {
        this.Severity = severity;
        this.Id = id;
        this.Summary = summary;
        this.HelpUrl = helpUrl;
    }

    public RuleType Severity { get; }

    public string Id { get; }

    public string Summary { get; }

    public string? HelpUrl { get; }
}