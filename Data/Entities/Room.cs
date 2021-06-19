using System.Collections.Generic;

namespace BaseCoreAPI.Data.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Surface> Surfaces { get; set; }
    }
}
