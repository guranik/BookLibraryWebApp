namespace BookLibraryDataAccessClassLibrary.Exceptions
{
    public class NoAvailableBooksException : Exception
    {
        public NoAvailableBooksException() { }

        public NoAvailableBooksException(string message)
            : base(message) { }

        public NoAvailableBooksException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class BookIsAlreadyIssuedException : Exception
    {
        public BookIsAlreadyIssuedException() { }

        public BookIsAlreadyIssuedException(string message)
            : base(message) { }

        public BookIsAlreadyIssuedException(string message, Exception inner)
            : base(message, inner) { }
    }
}
