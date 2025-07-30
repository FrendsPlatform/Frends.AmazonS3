namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Represents error information with message and optional additional details.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// The primary error message describing what went wrong.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional error information such as stack traces, error codes, or other diagnostic data.
        /// </summary>
        public dynamic AdditionalInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the Error class with the specified message.
        /// </summary>
        /// <param name="message">The error message</param>
        public Error(string message)
        {
            Message = message;
            AdditionalInfo = null;
        }

        /// <summary>
        /// Initializes a new instance of the Error class with the specified message and additional information.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="additionalInfo">Additional error information such as exception details or diagnostic data</param>
        public Error(string message, dynamic additionalInfo)
        {
            Message = message;
            AdditionalInfo = additionalInfo;
        }
    }
}
