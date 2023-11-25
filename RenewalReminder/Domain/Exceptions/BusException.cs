using RenewalReminder.Domain;
using System;


namespace   RenewalReminderr.Domain.Exceptions
{
    public class BusException : Exception
    {
        public BusException() : base()
        {
        }

        public BusException(string message) : base(message)
        {
        }

        public BusException(Result result) : base(string.Join(Environment.NewLine, result.Errors))
        {
        }
    }
}
