using System.Net;

namespace PartnersHub.Synergy.Application.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; private set; } = HttpStatusCode.OK;
        public object CustomData { get; private set; }
        public string Placeholder { get; private set; }
        public UserFriendlyException(string message, string messageParam = "") : base(message)
        {
            Placeholder = messageParam;
        }

        public UserFriendlyException(string message, HttpStatusCode code, string messageParam = "") : base(message)
        {
            HttpStatusCode = code;
            Placeholder = messageParam;
        }

        public UserFriendlyException(string message, object customData, string messageParam = "")
            : base(message)
        {
            CustomData = customData;
            Placeholder = messageParam;
        }
    }
}
