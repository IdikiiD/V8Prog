using System.ComponentModel.DataAnnotations;

namespace v8proj.Web.Model
{
    public class eUseControl
    {
        [Key] 
        public int Id { get; set; } // Добавляем свойство Id в качестве первичного ключа

        public string CartName { get; set; }
        public string CartDescription { get; set; }
        public int CartPrice { get; set; }
        public string CartImage { get; set; }
    }
}