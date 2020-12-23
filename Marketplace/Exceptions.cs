using System;

namespace Marketplace
{
    public static class Exceptions
    {
        public class DuplicateEntityIdException : Exception
        {
            public DuplicateEntityIdException(string message)
                : base(message)
            {
            }
        }
    }
}