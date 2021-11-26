using System.Threading.Tasks;
using TaApostado.DTOs.OutputModels;

namespace TaApostado.Hubs.Interfaces
{
    public interface IGameHub
    {
        public Task AddToGame(string id_challenge);
        public Task RemoveFromGame(string id_challenge);
    }
}
