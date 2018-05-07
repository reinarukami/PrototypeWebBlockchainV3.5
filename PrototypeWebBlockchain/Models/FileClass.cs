using System.Web;
using System.ComponentModel.DataAnnotations;


namespace PrototypeWebBlockchain.Models
{
    public class FileClass
    {
        [Key]
        public int id { get; set; }

        public HttpPostedFileBase image { get; set; }

        public string hash { get; set; }
    }
}