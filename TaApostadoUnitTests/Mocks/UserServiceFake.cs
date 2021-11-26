using System;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using TaApostado.Entities;
using System.Collections.Generic;
using TaApostado.Utils;
using System.Linq;

namespace TaApostadoUnitTests.Mocks
{
    class ServiceUserFake : IServiceUser
    {
        private readonly List<User> _users;
        public ServiceUserFake()
        {
            _users = new List<User>()
            {
                new User(Guid.NewGuid(),"Guilherme","Rocha",MD5Utils.Generate("430.164.198-07"),"gui@gmail.com",MD5Utils.Generate("teste1223")),
                new User(Guid.NewGuid(),"Rafael","Silva",MD5Utils.Generate("430.164.198-07"),"rafa@gmail.com",MD5Utils.Generate("teste1223")),
                new User(Guid.NewGuid(),"Marcos","Castro",MD5Utils.Generate("430.164.198-07"),"marcos@gmail.com",MD5Utils.Generate("teste1223")),
            };
        }
        public async Task Delete(Guid id)
        {
            await new Task(() =>
            {
                var user = _users.Where(u => u.Id == id).FirstOrDefault();
                user.SetAsDeactivated();
            });
        }

        public Task<OutputModelUser> Find(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, InputModelUser user)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, string name, string lastName)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, string password)
        {
            throw new NotImplementedException();
        }
    }
}
