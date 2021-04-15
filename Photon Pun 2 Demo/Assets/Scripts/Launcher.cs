using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The Maximum bumber of players per room. When a room is full, it cant be joined by the players , and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanal;

        [Tooltip("The UI Label to inform the user that the connection is in proggress")]
        [SerializeField]
        private GameObject progressLabel;



        #endregion

        #region Private Fields

        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).

        string gameVersion = "1";
        bool isConnecting;

        #endregion

        #region MonoBehaviuor CallBacks

        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.

        private void Awake()
        {
            // >>>>>>!!!!Important!!!!<<<<<<
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically

            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// MonoBehaviour method called on GameObject by Unity during initialization phase.

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanal.SetActive(true);
        }

        #endregion


        #region Public Methods



        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanal.SetActive(false);


            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // >>>>>>!!!!Important!!!!<<<<<<
                // we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.

                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // >>>>>>!!!!Important!!!!<<<<<<
               //////// we must first and foremost connect to Photon Online Server.
              ////////  PhotonNetwork.ConnectUsingSettings();

                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        #endregion

        #region MonoBehaviourPunCallbacks CallBacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
             // >>>>>>!!!!Important!!!!<<<<<<
                //The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            Debug.Log("PUN Basics Tutorial/Launcher : OnConnectedToMaster() was called bu PUN");
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanal.SetActive(true);

            isConnecting = false;

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher : OnDisconnected() was called by PUN with reason {0}", cause);

        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // >>>>>>!!!!Important!!!!<<<<<<
            //we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            //Important!!
            if (PhotonNetwork.CurrentRoom.PlayerCount ==1)
            {
                Debug.Log("We load the 'Room for 1' ");
                //Important!!
                //Load the room level
                PhotonNetwork.LoadLevel("Room for 1");
            }

        }



        #endregion
    }
}
