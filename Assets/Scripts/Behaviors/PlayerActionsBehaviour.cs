using System;
using System.Collections;
using Photon.Pun;
using Rewired;
using UnityEngine;
using System.Linq;

namespace Behaviors
{
    public class PlayerActionsBehaviour : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        public int playerId = 0; // rewired player Id of this character
        [SerializeField] 
        public float moveSpeed = 15.0f;
        [SerializeField]
        public GameObject beltPrefab;
        [SerializeField]
        public GameObject itemsEquippedUI;
        [SerializeField]
        public Camera camera;
        [SerializeField]
        public Camera miniCamera;
        [SerializeField]
        public GameObject canvas;

        private Player _player;
        private Vector3 _moveVector;
        private Rigidbody _playerRigidBody;
        private PhotonView _viewPlayer;
        private Animator _animator;
        private GameObject _itemToPickUp;

        /*Animations assigned as const*/
        private const string WALKLEFT = "walkLeft";
        private const string WALKRIGHT = "walkRight";
        private const string WALKUP = "walkUp";
        private const string WALKDOWN = "walkDown";
        private const string ISIDDLE = "isIddle";
        
        /*regular consts*/
        private const string PICK_UP_ITEM_TAG = "PickUpItem";
        private const float CUT_JUMP = 0.5f;
        private const string ACTION_BUTTON = "Action";
        

        #region MonoBehaviour Functions from unity
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _player = ReInput.players.GetPlayer(playerId);
            _playerRigidBody = GetComponent<Rigidbody>();
            _viewPlayer = GetComponent<PhotonView>();

            if (!_viewPlayer.IsMine)
            {
                camera.enabled = false;
                miniCamera.enabled = false;
            }

            var spawnPlayers = GameObject.Find("SpawnPlayers");
            if (spawnPlayers)
            {
                transform.parent = spawnPlayers.transform;
            }
            
        }

        private void FixedUpdate()
        {
            if (_viewPlayer && _viewPlayer.IsMine)
            {
                if (_moveVector != Vector3.zero)
                {
                    ProcessMovementInput();
                }
            }
        }

        private void Update()
        {
            if (_viewPlayer && _viewPlayer.IsMine)
            {
                CheckInputMovement();
                //CheckJump();
                CheckToGrabItem();
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag(PICK_UP_ITEM_TAG))
            {
                _itemToPickUp = other.gameObject;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(PICK_UP_ITEM_TAG))
            {
                _itemToPickUp = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(PICK_UP_ITEM_TAG))
            {
                _itemToPickUp = null;
            }
        }

        #endregion


        #region PUN FUNCTIONS
        [PunRPC]
        private void RPC_PlayerGrabItemBroadCast(string itemToShowUIName, int itemToPickUpID, int playerID)
        {
            var player = PhotonView.Find(playerID).gameObject;
            var itemToShowUI = player.transform.Find("Canvas").GetChild(0).GetChild(0).GetChild(0);
            var itemToPickUp = PhotonView.Find(itemToPickUpID).gameObject;
 
            if (player.transform.GetChild(0).transform)
            {
                itemToPickUp.transform.SetParent(player.transform.GetChild(0).transform, true);
                itemToPickUp.transform.position = new Vector3(itemToPickUp.transform.position.x, 2.5f,transform.position.z+1.5f);   
            }
            
            itemToShowUI.GetComponent<SpriteRenderer>().sprite = itemToPickUp.GetComponent<SpriteRenderer>().sprite;
            itemToPickUp.GetComponent<SpriteRenderer>().sprite = null;
            _itemToPickUp = null;
        }

        [PunRPC]
        private void RPC_PlayerLeaveItemBroadCast(int playerName)
        {
            var player = PhotonView.Find(playerName).gameObject;
            ChangeItemSpriteForPlayer(player);
            //ChangeItemSpriteForPlayer(gameObject, itemEquipped);
        }

        private void ChangeItemSpriteForPlayer(GameObject player)
        {
            var itemEquipped = player.transform.Find("Canvas").GetChild(0).GetChild(0).GetChild(0);
            var itemsInBelt = player.transform.Find("ItemsBelt");

            if (itemsInBelt.childCount > 0)
            {
                var itemEquippedSpriteRenderComponent = itemEquipped.GetComponent<SpriteRenderer>();
                var itemToDrop = itemsInBelt.GetChild(0).transform;
                ApplyBehaviourFromItem(itemToDrop.gameObject);
                itemToDrop.SetParent(null);
                itemToDrop.localPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

                itemToDrop.GetComponent<SpriteRenderer>().sprite =
                    itemEquippedSpriteRenderComponent.sprite;
                itemEquippedSpriteRenderComponent.sprite = null;
            }
        }

        [PunRPC]
        private void OnPlayerFinishMove(int playerName)
        {
            var player = PhotonView.Find(playerName).gameObject;
            var playerAnimator = player.transform.GetComponent<Animator>();
            playerAnimator.SetBool(ISIDDLE, true);

            // Call an RPC to notify other players about the animation change
            photonView.RPC("SyncAnimationState", RpcTarget.Others, playerName, ISIDDLE, true);
            photonView.RPC("SyncAnimationSpeed", RpcTarget.Others, playerName, 0);

            // Call SyncAnimationState for the local player
            SyncAnimationState(playerName, ISIDDLE, true);
        }

        [PunRPC]
        private void SyncAnimationState(int playerName, string paramName, bool paramValue)
        {
            var player = PhotonView.Find(playerName).gameObject;
            var playerAnimator = player.transform.GetComponent<Animator>();
            playerAnimator.SetBool(paramName, paramValue);
        }

        [PunRPC]
        private void SyncAnimationSpeed(int playerName, int speed)
        {
            var player = PhotonView.Find(playerName).gameObject;
            var playerAnimator = player.transform.GetComponent<Animator>();
            playerAnimator.speed = speed;
        }


        #endregion

        #region Functions created for the actions of the player

        // Called when the player finishes their move
        public void FinishMove()
        {
            // Call an RPC to notify other players that this player has finished their move
            photonView.RPC("OnPlayerFinishMove", RpcTarget.AllBufferedViaServer, _viewPlayer.ViewID);
        }

        private void LeaveItemBehind()
        {
            if(_player.GetButtonDown(ACTION_BUTTON))
            {
                if (beltPrefab.transform.childCount > 0)
                {
                    photonView.RPC("RPC_PlayerLeaveItemBroadCast", RpcTarget.AllBufferedViaServer, _viewPlayer.ViewID);
                }
            }
        }
        
        private void ApplyBehaviourFromItem(GameObject itemObject)
        {
            if (_viewPlayer && _viewPlayer.IsMine)
            {
                if (itemObject.name.Equals("MapRadar(Clone)"))
                {
                    var radarCanvas = canvas.transform.Find("Border");
                    if (radarCanvas)
                    {
                        radarCanvas.gameObject.SetActive(!radarCanvas.gameObject.activeSelf);
                    }
                }
            }
        }
        private void PickItemAction(GameObject itemToPickUp)
        {
            if(_player.GetButtonDown(ACTION_BUTTON))
            {
                var photonViewComponentItemPickUp = itemToPickUp.GetComponent<PhotonView>();
                ApplyBehaviourFromItem(itemToPickUp);
                foreach (Transform itemToShowUI in itemsEquippedUI.transform)
                {
                    
                    var spriteItemUIComponent = itemToShowUI.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteItemUIComponent.sprite is null || spriteItemUIComponent.sprite == null)
                    {
                        try
                        {
                            //Due to size limitations and staying optimized for networking traffic, Photon RPCs don't allow you to pass something like an entire GameObject ): 
                            photonView.RPC("RPC_PlayerGrabItemBroadCast", RpcTarget.AllBufferedViaServer,
                                itemToShowUI.name,
                                photonViewComponentItemPickUp.ViewID,
                               _viewPlayer.ViewID);
                            break;
                        }
                        catch(Exception e)
                        {
                           Debug.LogException(e);   
                        }
                       
                    }

                }
            }
        }

        private void CheckToGrabItem()
        {
            var checkButton = _player.GetButtonDown("Jump");
            if (checkButton && _itemToPickUp != null)
            {
                PickItemAction(_itemToPickUp);
            }
            else if(_itemToPickUp == null)
            {
                LeaveItemBehind();
            }

        }

        private void CheckInputMovement()
        {

            _moveVector = Vector3.zero;
            _moveVector.x = Input.GetAxis("Horizontal");
            _moveVector.z = Input.GetAxis("Vertical");

            if (_moveVector.x == 0 && _moveVector.z == 0)
            {
                FinishMove();
                _animator.SetBool(ISIDDLE, true);
            }
            else
            {
                _animator.SetBool(ISIDDLE, false);

                // Call an RPC to notify other players about the animation speed
                photonView.RPC("SyncAnimationSpeed", RpcTarget.Others, _viewPlayer.ViewID, 1);
            }

            switch (_moveVector.x)
            {
                case > 0 when _moveVector.z is 0: // movimiento horizontal
                    _animator.speed = 1;
                    _animator.SetBool(WALKRIGHT, true);
                    _animator.SetBool(WALKDOWN, false);
                    _animator.SetBool(WALKLEFT, false);
                    _animator.SetBool(WALKUP, false);
                    //ChangeAnimationState(WALKRIGHT);
                    break;
                case < 0 when _moveVector.z is 0: // movimiento horizontal
                    _animator.speed = 1;
                    _animator.SetBool(WALKLEFT, true);
                    _animator.SetBool(WALKDOWN, false);
                    _animator.SetBool(WALKRIGHT, false);
                    _animator.SetBool(WALKUP, false);
                    //ChangeAnimationState(WALKLEFT);
                    break;
                case > 0 when _moveVector.z is > 0: // movimiento vertical/ horizontal derecha direccion arriba
                    _animator.speed = 1;
                    _animator.SetBool(WALKUP, true);
                    _animator.SetBool(WALKDOWN, false);
                    _animator.SetBool(WALKRIGHT, false);
                    _animator.SetBool(WALKLEFT, false);
                    // ChangeAnimationState(WALKUP);
                    break;
                case < 0 when _moveVector.z is > 0: // movimiento vertical/ horizontal izquierda direccion arriba
                    _animator.speed = 1;
                    _animator.SetBool(WALKUP, true);
                    _animator.SetBool(WALKDOWN, false);
                    _animator.SetBool(WALKRIGHT, false);
                    _animator.SetBool(WALKLEFT, false);
                    // ChangeAnimationState(WALKUP);
                    break;
                case > 0 when _moveVector.z is < 0: // movimiento vertical/ horizontal derecha direccion abajo
                    _animator.speed = 1;
                    _animator.SetBool(WALKRIGHT, true);
                    _animator.SetBool(WALKDOWN, false);
                    _animator.SetBool(WALKUP, false);
                    _animator.SetBool(WALKLEFT, false);
                    // ChangeAnimationState(WALKRIGHT);
                    break;
                case < 0 when _moveVector.z is < 0: // movimiento vertical/ horizontal derecha direccion abajo
                    _animator.speed = 1;
                    _animator.SetBool(WALKLEFT, true);
                    _animator.SetBool(WALKDOWN, false);
                    _animator.SetBool(WALKUP, false);
                    _animator.SetBool(WALKRIGHT, false);
                    //ChangeAnimationState(WALKLEFT);
                    break;
                default:
                    switch (_moveVector.z)
                    {
                        case > 0 when _moveVector.x is 0: // movimiento arriba
                            _animator.speed = 1;
                            _animator.SetBool(WALKUP, true);
                            _animator.SetBool(WALKDOWN, false);
                            _animator.SetBool(WALKLEFT, false);
                            _animator.SetBool(WALKRIGHT, false);
                            // ChangeAnimationState(WALKUP);
                            break;
                        case < 0 when _moveVector.x is 0: // movimiento abajo
                            _animator.speed = 1;
                            _animator.SetBool(WALKDOWN, true);
                            _animator.SetBool(WALKUP, false);
                            _animator.SetBool(WALKLEFT, false);
                            _animator.SetBool(WALKRIGHT, false);
                            //ChangeAnimationState(WALKDOWN);
                            break;
                        default:
                            _animator.speed = 0;
                            break;
                    }

                    break;
            }
      
        }

        private void ProcessMovementInput()
        {
            _playerRigidBody.MovePosition(
                transform.position + _moveVector * (moveSpeed * Time.deltaTime));
        }

        private void CheckJump()
        {
            //if player release de jump button
            if(_player.GetButtonUp(PICK_UP_ITEM_TAG))
            {
                if (_playerRigidBody.velocity.y > 0) // and is still going up
                {
                    // we cut the force by half to not jump so high
                    var velocity = _playerRigidBody.velocity;
                    velocity = new Vector2(velocity.x, velocity.y * CUT_JUMP);
                    _playerRigidBody.velocity = velocity;
                }
            }
        }
        
        #endregion
    }
}
