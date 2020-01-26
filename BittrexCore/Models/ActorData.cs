using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class ActorData
    {
        public Guid Guid;
        public List<BalancedRule> Rules;
        public List<Prediction> Predictions;

        public List<Transaction> Transactions;
        public double HesitationToBuy;
        public double HesitationToSell;
        public double ChangeCoefficient;


    }
}
