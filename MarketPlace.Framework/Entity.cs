using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Framework
{
    public abstract class Entity<TId> : IInternalEventHandler where TId : Value<TId>
    {
        private readonly Action<object> _applier;

        protected Entity(Action<object> applier)
        {
            _applier = applier;
        }
        
        public TId Id { get; protected set; }

        protected void Apply(object @event)
        {
            When(@event);
            _applier(@event);
        }

        protected abstract void When(object @event);

        public void Handle(object @event) => When(@event);
    }
}