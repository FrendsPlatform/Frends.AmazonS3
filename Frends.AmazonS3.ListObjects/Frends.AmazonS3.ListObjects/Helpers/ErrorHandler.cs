using System;
using Frends.AmazonS3.ListObjects.Definitions;

namespace Frends.AmazonS3.ListObjects.Helpers
{
    /// <summary>
    /// Static helper class for handling errors in Amazon S3 operations.
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Handles exceptions according to the provided options.
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="options">Options containing error handling settings</param>
        /// <returns>Result object with error information</returns>
        /// <exception cref="Exception">Throws the original exception if ThrowErrorOnFailure is true</exception>
        public static Result Handle(Exception exception, Options options)
        {
            if (options.ThrowErrorOnFailure)
                throw exception;

            var errorMessage = string.IsNullOrWhiteSpace(options.ErrorMessageOnFailure)
                ? exception.Message
                : options.ErrorMessageOnFailure;

            var error = new Error(errorMessage, new { OriginalException = exception.GetType().Name, StackTrace = exception.StackTrace });
            return new Result(error);
        }

        /// <summary>
        /// Handles credential errors according to the provided options.
        /// </summary>
        /// <param name="credentialsError">The credentials error message</param>
        /// <param name="options">Options containing error handling settings</param>
        /// <returns>Result object with error information</returns>
        /// <exception cref="Exception">Throws an exception if ThrowErrorOnFailure is true</exception>
        public static Result HandleCredentialsError(string credentialsError, Options options)
        {
            if (options.ThrowErrorOnFailure)
                throw new Exception(credentialsError);

            var errorMessage = string.IsNullOrWhiteSpace(options.ErrorMessageOnFailure)
                ? credentialsError
                : options.ErrorMessageOnFailure;

            return new Result(errorMessage);
        }
    }
}
