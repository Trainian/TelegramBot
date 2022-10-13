using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Base
{
    public class BaseViewModel
    {
        [Key]
        public int Id { get; set; }
    }
}
