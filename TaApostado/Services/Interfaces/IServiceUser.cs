using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using System.Threading.Tasks;
using System;

namespace TaApostado.Services.Interfaces
{
    public interface IServiceUser
    {
        public Task<OutputModelUser> Find(Guid id);
        public Task Update(Guid id, InputModelUser user);
        public Task Update(Guid id, string name, string lastName);
        public Task Update(Guid id, string password);
        public Task Delete(Guid id);
    }
}
