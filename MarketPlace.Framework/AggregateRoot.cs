using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Framework
{
    public abstract class AggregateRoot<TId> : IInternalEventHandler 
        where TId : Value<TId>
    {
        private readonly List<object> _changes;

        protected AggregateRoot()
        {
            _changes = new List<object>();
        }

        public TId Id { get; protected set; }

        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            EnsureValidState();
            _changes.Add(@event);
        }

        public IEnumerable<object> GetChanges() => _changes.AsEnumerable();

        public void ClearChanges() => _changes.Clear();

        protected abstract void EnsureValidState();

        protected void ApplyToEntity(IInternalEventHandler entity, object @event) => entity?.Handle(@event);

        public void Handle(object @event) => When(@event);
    }
}