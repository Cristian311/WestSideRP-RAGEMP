using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Interfaces;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.Logger;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSide.Server.Houses
{
    internal class HouseManager : Script
    {
        public static readonly uint HouseDimension = 10000;
        private static IdManager<House> houseIdManager = new IdManager<House>();
        private static List<IHouse> houses = new List<IHouse>();
        private static List<ColShape> housesColshapes = new List<ColShape>();
        private static List<ColShape> exitHousesColshapes = new List<ColShape>();

        [ServerEvent(Event.PlayerEnterColshape)]
        private void OnPlayerEnterHouseColshape(ColShape colShape, Player player)
        {
            if(housesColshapes.Contains(colShape))
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                player.SendChatMessage($"{Colors.INFO}Apasa Y sa intrii in casa.");
                character.currentColshape = colShape;
            }
            if (exitHousesColshapes.Contains(colShape))
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                player.SendChatMessage($"{Colors.INFO}Apasa Y sa iesi din incapere.");
                character.currentColshape = colShape;
            }
        }
        [RemoteEvent("CLIENT:SERVER:FallDetected")]
        private void FallDetected(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if(character.isInInterior)
            {
                character.SendChatMessage($"{Colors.INFO}[!]{Colors.DARK_BLUE}Cadere pe interior detectata, te repozitionam...");
                player.Position = character.InteriorPosition;
            }
        }
        [RemoteEvent("CLIENT:SERVER:TryEnterDoor")]
        private void EnterDoor(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            //player.SendChatMessage("hatz");
            if(housesColshapes.Contains(character.currentColshape))
            {
                House house = houses.FirstOrDefault(h => h.ColShape == character.currentColshape) as House;
                if(house.LockStatus == false)
                {
                    player.Position = house.InteriorPosition;
                    player.Dimension = house.Dimension;
                    character.InteriorPosition = house.InteriorPosition;
                    character.isInInterior = true;
                    //player.SendChatMessage(player.Dimension.ToString());
                }
                else
                {
                    player.SendChatMessage($"{Colors.ERROR}Aceasta usa este incuiata.");
                }
            }
            if(exitHousesColshapes.Contains(character.currentColshape))
            {
                var house = houses.FirstOrDefault(h => h.ExitColShape ==  character.currentColshape) as House;
                if(house.LockStatus == false)
                {
                    player.Position = house.Position;
                    player.Dimension = NAPI.GlobalDimension;
                    character.InteriorPosition = null;
                    character.isInInterior = false;
                }
            }
        }
        [RequiresAdmin]
        [Command("createhouse", GreedyArg = true)]
        private void CreateHouseCommand(Player player, string Name)
        {
            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(player.Position, 1f, 1f, NAPI.GlobalDimension);
            housesColshapes.Add(colShape);

            House house = new House(Name, string.Empty, "state", player.Position, colShape);
            house.Id = houseIdManager.AssignNewId(house);
            house.Dimension = HouseDimension + uint.Parse(house.Id.ToString());
            house.InteriorPosition = HouseInteriors.Interiors[Name];

            ColShape interiorColShape = NAPI.ColShape.CreateCylinderColShape(house.InteriorPosition, 1f, 1f, house.Dimension);
            house.ExitColShape = interiorColShape;
            exitHousesColshapes.Add(interiorColShape);
            houses.Add(house);
            ServerLogger.LogWithAccount(player, " a creat casa ID " + house.Id + " tip:" + house.Name);
        }
    }
}
