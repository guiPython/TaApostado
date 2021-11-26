
namespace TaApostado.DTOs.OutputModels
{
    public class OutputModelLogin
    {
        public OutputModelUser user { get; set; }
        public string token { get; set; }

        public OutputModelLogin(OutputModelUser user, string token)
        {
            this.user = user;
            this.token = token;
        }
    }
}
