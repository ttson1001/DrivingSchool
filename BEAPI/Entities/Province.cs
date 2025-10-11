using System.ComponentModel.DataAnnotations;

namespace BEAPI.Entities
{
    public class Province: IEntity
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
