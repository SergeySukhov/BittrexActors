using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BittrexCore.Models;
using BittrexData.Interfaces;

namespace BittrexCore
{
    public class ActorFactory
    {
        private IActorProvider actorProvider;
        

        public ActorFactory(IActorProvider actorProvider)
        {
            this.actorProvider = actorProvider;
        }

        public Actor CreateActor(ICurrencyProvider currencyProvider, RulesLibrary.RuleLibrary ruleLibrary)
        {
            var actor = new Actor(currencyProvider, ruleLibrary);

            // ...

            return actor;
        }

        public List<Actor> LoadAliveActors()
        {
            var aliveActors = new List<Actor>();
            //actorProvider

            return null;
        }



        private ActorData DbActorToModel(BittrexData.Models.ActorData actorData)
        {
            var actorDataModel = new ActorData();

            return actorDataModel;
        }

        private BittrexData.Models.ActorData ActorToDbModel(Actor actor)
        {
            return null;
        }
    }
}
