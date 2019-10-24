using RollerSplat.Data;
using UnityEngine;

namespace RollerSplat
{
    public class TeleportTile : LevelBlock
    {
        public Vector2 destination;
        
        public override LevelData.CellType CellType => LevelData.CellType.Teleport;

        private async void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player>();
                if(!player) return;
                //If player has been teleported, don't teleport until it moves again
                if(player.WasTeleported.Value) return;

                //Stop player
                await player.StopMove(transform.position);
                //Play teleport on
                await player.Teleport(true);
                //Teleport player to destination
                player.PlaceOnTile.Execute(destination);
                //Play teleport off
                await player.Teleport(false);
            }
        }
    }
}