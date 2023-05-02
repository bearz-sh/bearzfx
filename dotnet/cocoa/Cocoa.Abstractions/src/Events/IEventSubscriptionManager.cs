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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/services/IEventSubscriptionManagerService.cs

using Cocoa.Events;

namespace Cocoa.Services;

/// <summary>
///   Interface for EventSubscriptionManagerService.
/// </summary>
public interface IEventSubscriptionManager
{
    /// <summary>
    ///   Publishes the specified event message.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventMessage">The message to publish.</param>
    void Publish<TEvent>(TEvent eventMessage)
        where TEvent : class, IMessage;

    /// <summary>
    ///   Subscribes to the specified event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="handleEvent">The message handler.</param>
    /// <param name="handleError">The error handler.</param>
    /// <param name="filter">The message filter.</param>
    /// <returns>The subscription as Disposable.</returns>
    IDisposable Subscribe<TEvent>(Action<TEvent> handleEvent, Action<Exception>? handleError = null, Func<TEvent, bool>? filter = null)
        where TEvent : class, IMessage;
}