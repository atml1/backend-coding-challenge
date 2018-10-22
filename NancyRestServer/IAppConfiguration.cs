namespace NancyRestServer
{
    /// <summary>
    /// Configuration parameters to run the application with.
    /// </summary>
    public interface IAppConfiguration
    {
        /// <summary>
        /// The port to host the service.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The default maximum number of responses to return if it is not specified in the request.
        /// </summary>
        int DefaultMaxResponseCount { get; }
    }
}
