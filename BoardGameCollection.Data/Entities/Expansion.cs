using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameCollection.Data.Entities
{
    [Table("Expansion")]
    public class Expansion
    {
        public int BoardGameId { get; set; }
        public int ExpansionId { get; set; }

        public BoardGame BoardGame { get; set; }
    }
}
