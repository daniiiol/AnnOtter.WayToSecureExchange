namespace AnnOtter.WayToSecureExchange.Exceptions.Exchange
{
    /// <summary>
    /// InputDataInvalidException for the use of a custom error class used in Controller Actions.
    /// </summary>
    public class InputDataInvalidException : Exception
    {
        /// <summary>
        /// Ctor of InputDataInvalidException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public InputDataInvalidException(string message) : base(message) 
        {
            
        }
    }
}
