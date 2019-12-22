using System.ComponentModel.DataAnnotations;

namespace BoardGameCollection.Data.Entities
{
    public class Expansion
    {
        public int BoardGameId { get; set; }
        public int ExpansionId { get; set; }

        public BoardGame BoardGame { get; set; }
    }
}
