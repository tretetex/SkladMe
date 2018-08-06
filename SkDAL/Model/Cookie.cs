using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkDAL.Model
{
    public class Cookie
    {
        [Column("host_key")]
        public string Host { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("creation_utc")]
        public long CreationUtc { get; set; }
        [Column("expires_utc")]
        public long ExpiresUtc { get; set; }
    }
}
