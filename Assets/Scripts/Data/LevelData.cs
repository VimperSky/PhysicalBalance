using System.Collections.Generic;

namespace Data
{
    public class LevelData
    {
        public List<CargoData> CargoDatas { get; set; }
        
        public List<int> CargoChoosingMasses { get; set; }
        public int UnknownCargoId { get; set; }
        public int DefeatMass { get; set; }
        
        public bool IsRotationAvailable { get; set; }
    }
}