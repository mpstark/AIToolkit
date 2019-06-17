using System;
using System.Linq;
using BattleTech;

namespace AIToolkit.Selectors.Combat
{
    public class PlanetBiome : Selector<CombatGameState>
    {
        public override bool Select(string selectString, CombatGameState combat)
        {
            if (combat.ActiveContract == null)
                return false;

            var biomes = selectString.Split(' ');
            return biomes.Contains(Enum.GetName(typeof(Biome.BIOMESKIN), combat.ActiveContract.ContractBiome));
        }
    }
}
