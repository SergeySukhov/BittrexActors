using BittrexData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexData.Interfaces
{
    public interface IActorProvider
    {
        Task<List<ActorDataDto>> LoadAliveActors();

        Task SaveOrUpdateActor(ActorDataDto actorData);

        Task ClearOldData();
    }
}
