namespace Stoquei.Domain.ViewModels
{
    public class MensagemBase<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Object { get; set; }

        public MensagemBase() {}

        public MensagemBase(int statusCode, string message, T @object)
        {
            StatusCode = statusCode;
            Message = message;
            Object = @object;
        }

        public MensagemBase(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
