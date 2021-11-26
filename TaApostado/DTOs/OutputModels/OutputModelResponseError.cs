
namespace TaApostado.DTOs.OutputModels
{
    public class OutputModelResponseError
    {
        public int Status { get;  set; }
        public string Error { get;  set; }

        public OutputModelResponseError(int status, string error)
        {
            Status = status;
            Error = error;
        }
    }
}
