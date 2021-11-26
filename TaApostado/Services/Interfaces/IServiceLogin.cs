using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;

namespace TaApostado.Services.Interfaces
{
    public interface IServiceLogin
    {
        public Task<OutputModelLogin> SignIn(InputModelLogin login);
        public Task<OutputModelLogin> SignUp(InputModelUser user);
    }
}
