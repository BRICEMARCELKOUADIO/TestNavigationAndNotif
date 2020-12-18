using Kola.Infrastructure.Navigation;

namespace Kola.Starting.Models
{
    public class PikerItem
    {
        public string Id { get; set; }
        public string Name_1 { get; set; }
        public string Name_2 { get; set; }
        public string Name_3 { get; set; }
        public string Name_4 { get; set; }
        public Field<bool> IsSelected { get; set; } = new Field<bool>();
    }
}
