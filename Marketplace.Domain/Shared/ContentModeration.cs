namespace Marketplace.Domain.Shared
{
    public class ContentModeration
    {
        public delegate bool CheckTextForProfanity(string text);
    }
}