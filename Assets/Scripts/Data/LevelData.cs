using System.Collections.Generic;

namespace Data
{
    public class LevelData
    {
        public List<CargoData> CargoDatas { get; set; }
        
        public List<float> CargoChoosingMasses { get; set; }
        public int UnknownCargoId { get; set; }
    }
}