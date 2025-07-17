namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Error information.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional error information.
        /// </summary>
        public dynamic AdditionalInfo { get; set; }

        /// <summary>
        /// Initialize Error with message.
        /// </summary>
        /// <param name="message">Error message</param>
        public Error(string message)
        {
            Message = message;
            AdditionalInfo = null;
        }

        /// <summary>
        /// Initialize Error with message and additional info.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="additionalInfo">Additional error information</param>
        public Error(string message, dynamic additionalInfo)
        {
            Message = message;
            AdditionalInfo = additionalInfo;
        }
    }
}
