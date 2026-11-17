using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suit___Slash
{
    public class ScoredHand
    {
        public List<CardState> Cards { get; set; }
        public HandRank Rank { get; set; }
        public int TotalDamageScore { get; set; }
    }
}
