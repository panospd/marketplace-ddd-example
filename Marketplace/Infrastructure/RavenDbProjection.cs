using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marketplace.Framework;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure
{
    public abstract class RavenDbProjection<T> : IProjection
    {
        protected Func<IAsyncDocumentSession> GetSession { get; }

        protected RavenDbProjection(Func<IAsyncDocumentSession> getSession)
        {
            GetSession = getSession;
        }

        public abstract Task Project(object @event);

        protected Task Create(Func<Task<T>> model)
        {
            return UsingSession(async session => await session.StoreAsync(await model()));
        }

        protected Task UpdateOne(Guid id, Action<T> update)
        {
            return UsingSession(session => UpdateItem(session, id, update));
        }

        protected Task UpdateWhere(Expression<Func<T, bool>> where, Action<T> update)
        {
            return UsingSession(session => UpdateMultipleItems(session, where, update));
        }

        private async Task UsingSession(Func<IAsyncDocumentSession, Task> operation)
        {
            using var session = GetSession();

            await operation(session);
            await session.SaveChangesAsync();
        }

        private async Task UpdateMultipleItems(
            IAsyncDocumentSession session, 
            Expression<Func<T, bool>> query,
            Action<T> update)
        {
            var items = await session
                .Query<T>()
                .Where(query)
                .ToListAsync();

            foreach (var item in items)
                update(item);
        }

        private static async Task UpdateItem(IAsyncDocumentSession session, Guid id, Action<T> update)
        {
            var identifier = id.ToString();
            
            var item = await session.LoadAsync<T>(identifier);
            
            if(item == null) return;

            update(item);
        }
    }
}