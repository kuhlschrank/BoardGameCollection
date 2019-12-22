namespace BoardGameCollection.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Unknown")]
    public class Unknown
    {
        [Key]
        public int Id { get; set; }
    }
}
