using BittrexData;
using System;
using System.Collections.Generic;

namespace BittrexCore.Models
{
    public class ActorData
    {
        public Guid Guid;

        public double HesitationToBuy;
        public double HesitationToSell;
        public double ChangeCoefficient;
        public bool IsAlive;

        public ActorType ActorType;

        public Account Account; 
        public readonly List<BalancedRule> Rules;
        public readonly List<Prediction> Predictions;

        public readonly List<Transaction> Transactions;

        public ActorData()
        {
            Guid = Guid.NewGuid();
            Account = new Account();
            Rules = new List<BalancedRule>();
            Predictions = new List<Prediction>();
            Transactions = new List<Transaction>();
        }
    }
}
