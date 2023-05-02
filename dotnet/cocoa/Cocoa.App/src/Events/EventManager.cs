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

using Cocoa.Services;

namespace Cocoa.Events;

public class EventManager
{
    private readonly IEventSubscriptionManager manager;

    public EventManager(IEventSubscriptionManager manager)
    {
        this.manager = manager;
    }

    public static EventManager Instance { get; } = new EventManager(new EventSubscriptionManager());

    /// <summary>
    ///   Gets the manager service.
    /// </summary>
    /// <value>
    ///   The manager service.
    /// </value>
    public IEventSubscriptionManager ManagerService => this.manager;

    /// <summary>
    ///   Publishes the specified message.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="message">The message.</param>
    public void Publish<TEvent>(TEvent message)
        where TEvent : class, IMessage
    {
        this.manager.Publish(message);
    }

    /// <summary>
    ///   Subscribes to the specified message.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="handleEvent">The handle message.</param>
    /// <param name="handleError">The handle error.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>The subscription so that a service could unsubscribe.</returns>
    public IDisposable Subscribe<TEvent>(Action<TEvent> handleEvent, Action<Exception>? handleError, Func<TEvent, bool> filter)
        where TEvent : class, IMessage
    {
        return this.manager.Subscribe(handleEvent, handleError, filter);
    }
}