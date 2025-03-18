namespace BookLibraryAPI.Middleware
{


    public class NoAvailableBooksException : Exception
    {
        public NoAvailableBooksException() { }

        public NoAvailableBooksException(string message)
            : base(message) { }

        public NoAvailableBooksException(string message, Exception inner)
            : base(message, inner) { }
    }
}
