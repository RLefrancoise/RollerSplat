using RollerSplat.Data;
using UnityEngine;
using Zenject;

namespace RollerSplat
{
    /// <summary>
    /// Teleport tile
    /// </summary>
    public class TeleportTile : LevelBlock
    {
        /// <summary>
        /// Sound player
        /// </summary>
        private ISoundPlayer _soundPlayer;
        
        /// <summary>
        /// Tile coordinates of the destination
        /// </summary>
        [SerializeField] private Vector2 destination;
        /// <summary>
        /// Teleport sound
        /// </summary>
        [SerializeField] private AudioSource teleportSound;
        /// <summary>
        /// Teleport on sound
        /// </summary>
        [SerializeField] private AudioClip teleportOn;
        /// <summary>
        /// Teleport off sound
        /// </summary>
        [SerializeField] private AudioClip teleportOff;
        
        public override LevelData.CellType CellType => LevelData.CellType.Teleport;

        [Inject]
        public void Construct(ISoundPlayer soundPlayer)
        {
            _soundPlayer = soundPlayer;
        }
        
        private async void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player>();
                if(!player) return;
                //If player has been teleported, don't teleport until it moves again
                if(player.WasTeleported.Value) return;

                //Stop player
                await player.StopMove(transform.position, false);
                //Teleport on sound
                teleportSound.clip = teleportOn;
                _soundPlayer.PlaySound(teleportSound);
                //Play teleport on
                await player.Teleport(true);
                //Teleport player to destination
                player.PlaceOnTile.Execute(destination);
                //Teleport off sound
                teleportSound.clip = teleportOff;
                _soundPlayer.PlaySound(teleportSound);
                //Play teleport off
                await player.Teleport(false);
            }
        }
    }
}