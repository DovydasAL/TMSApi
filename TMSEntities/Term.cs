using System.ComponentModel.DataAnnotations;

namespace TMSEntities
{
    public class Term
    {
        [Key]
        public long TermID { get; set; }
        public string TermName { get; set; }
        public string LookupLabel { get; set; }
    }
}
