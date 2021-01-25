using System.Threading.Tasks;

namespace Marketplace.Domain.Shared
{
    public class ContentModeration
    {
        public delegate Task<bool> CheckTextForProfanity(string text);
    }
}