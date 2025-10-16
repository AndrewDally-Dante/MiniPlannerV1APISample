using System;

namespace DanteAPI.Entities.References
{
    public class File
    {
        public int ID { get; set; }
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}