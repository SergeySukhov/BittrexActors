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

		public int Generation;
		public bool IsAlive;

		public DateTime LastActionTime;
		public DateTime CurrentTime;

		public ActorType ActorType;

        public Account Account; 
        public List<BalancedRule> Rules;
        public List<Prediction> Predictions;

        public List<Transaction> Transactions;

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
