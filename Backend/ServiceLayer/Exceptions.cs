using System;

namespace ServiceLayer.Exceptions
{
    public class PasswordPwnedException : Exception
    {
        public PasswordPwnedException() {}

        public PasswordPwnedException(string message) : base(message) {}
    }

    public class PasswordInvalidException : Exception
    {
        public PasswordInvalidException() { }

        public PasswordInvalidException(string message) : base(message) { }
    }

    public class InvalidDobException : Exception
    {
        public InvalidDobException() { }

        public InvalidDobException(string message) : base(message) { }
    }

    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() { }

        public InvalidTokenException(string message) : base(message) { }
    }

    public class InvalidStringException : Exception
    {
        public InvalidStringException() { }

        public InvalidStringException(string message) : base(message) { }
    }

    public class InvalidEmailException : Exception
    {
        public InvalidEmailException() { }

        public InvalidEmailException(string message) : base(message) { }
    }

    public class InvalidUrlException : Exception
    {
        public InvalidUrlException() { }

        public InvalidUrlException(string message) : base(message) { }
    }

    public class InvalidImageException : Exception
    {
        public InvalidImageException() { }

        public InvalidImageException(string message) : base(message) { }
    }

    public class InvalidApiKeyException : Exception
    {
        public InvalidApiKeyException() { }

        public InvalidApiKeyException(string message) : base(message) { }
    }

}
