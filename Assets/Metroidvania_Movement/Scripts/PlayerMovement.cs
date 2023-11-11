using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameInput _input;
    private CharacterController _controller;

    [Header("Movement")]
    [SerializeField]
    private float _leftRightMoveSpeed;
    [SerializeField]
    private float _upDownMoveSpeed;
    [SerializeField]
    private float _gravity;
    [SerializeField]
    private bool _canMove = true;

    [Header("Jump")]
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _wallJumpForce;
    [SerializeField]
    private bool _grounded = false;
    [SerializeField]
    private bool _jumped = false;
    [SerializeField]
    private bool _canDoubleJump = false;
    [SerializeField]
    private float doubleJumpMultiplier;
    [SerializeField]
    private bool _canJump = true;
    [SerializeField]
    private bool _onWall = false;

    [Header("Ledge Grab")]
    [SerializeField]
    private Ledge activeLedge;
    [SerializeField]
    private bool _onLedge;

    [Header("Stomp")]
    [SerializeField]
    private bool _stomping = false;

    [Header("Climb")]
    [SerializeField]
    private Climbable activeClimbable;
    [SerializeField]
    private bool _climbing = false;


    Vector3 direction; // direction of movment

    [SerializeField]
    private BoxCollider wallDetectorCollider;
    private Vector3 originalSize; // box collider's original size
    private Vector3 wallSurfaceNormal; // obtained in OnControllerColliderHit()

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        originalSize = wallDetectorCollider.size;
        _input = new GameInput();
        _input.Player.Enable();
        _input.Player.Jump.performed += Jump_performed;
        _input.Player.PullUp.performed += PullUp_performed;
        _input.Player.Stomp.performed += Stomp_performed;
        direction = new Vector3(0, 0, 0); 
    }

    //use this to get the surface normal of the wall. 
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Wall"))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.yellow);
            wallSurfaceNormal = hit.normal;
            //use hit normal to push player off the wall in the direction of normal 
        }
    }

    void Update()
    {
        if (GroundChecker() == true && _jumped == false && _climbing == true) // if on the ground and not jumping
        {
            direction.y = 0; // dont constantly drop the y value. 
        }

        LeftRightUpDownMovement();

        if (_canMove == true) // this is just to avoid the error message
                              // about move being called on an inactive controller.(when climbing ledge)
        {
            _controller.Move(direction * _leftRightMoveSpeed * Time.deltaTime);
        }

        direction.y -= _gravity * Time.deltaTime; // constantly apply gravity
    }
    //new input system 
    #region PERFORMED METHODS
    private void Stomp_performed(InputAction.CallbackContext obj)
    {
        if(GroundChecker() == false) { Stomp(); }
    }

    private void PullUp_performed(InputAction.CallbackContext obj)
    {
        StandUp(); // move to onanimation exit when that exists. 
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if(_canJump == false) return;
        Jump();
    }
    #endregion
    //handles movement on horizontal axis
    #region Movement
    private void LeftRightUpDownMovement()
    {
        //poll the read value from the LeftRightMove action
        var moveLR = _input.Player.LeftRightMove.ReadValue<Vector2>();
        var moveUD = _input.Player.UpDownMove.ReadValue<Vector2>();

        if (_onWall == false)
        {
            direction.z = moveLR.x;
        }
        if(_climbing == true)
        {
            direction.y = moveUD.y;
        }

        if (direction.z != 0) // rotate facing direction
        {
            Vector3 facingDirection = transform.localEulerAngles;
            facingDirection.y = direction.z > 0 ? 0 : 180;
            transform.localEulerAngles = facingDirection;
            //set animation here. 
        }
        else if (direction.z == 0) //not moving
        {
            //change to idle 
        }
    }
    #endregion
    //Jump
    #region JUMP
    private void Jump() // called on Jump_performed
    {
        ///DOUBLE JUMP
        if (_jumped == true && _canDoubleJump == true) // if you just pressed jump while having already jumped
        {
            direction.y = 0;
            direction.y += _jumpHeight;
            _canJump = false; // no more jumping
        }
        ///NORMAL JUMP
        if (GroundChecker() == true) // if on the ground
        {
            _jumped = true; // you jumped
            _canDoubleJump = true;
            direction.y = _jumpHeight;
        }
        //WALL JUMP
        if (WallChecker() == true)
        {
            direction = new Vector3(0, _jumpHeight, wallSurfaceNormal.z * _wallJumpForce);
            _jumped = true; // you jumped
            _canDoubleJump = true; // since you just jumped from the wall, double jumping is possible
            _gravity = 6.81f; // makes it a little easier to jump up walls
            wallDetectorCollider.size = new Vector3(0, 0, 0); // sets wall checker to 0 for 0.1 seconds so it will retrigger the OnWall()
            StartCoroutine(SetSize_WallChecker());
        }
    }
    #endregion
    //handles ledge grab
    #region Ledge Grab
    public void GrabLedge(Ledge currentLedge)
    {
        _canMove = false;
        _controller.enabled = false;
        _onLedge = true;
        activeLedge = currentLedge;
    }

    public void StandUp() // called when exiting the climb up animation?
    {
        _canMove = true;
        transform.position = activeLedge.standPosition.transform.position;
        _controller.enabled = true;
        activeLedge = null;
        _onLedge = false;
    }
    #endregion

    #region Climbable
    public void OnClimbable(Climbable currentClimbable)
    {
        _canDoubleJump = false;
        _canJump = false;

        _climbing = true;
        activeClimbable = currentClimbable;
        //set climb animation idle    
        direction.y = 0;
        _gravity = 0;
        _input.Player.LeftRightMove.Disable();
        _input.Player.UpDownMove.Enable();
    }

    public void LeftClimbable()
    {
        _climbing = false;
        _canJump = true;
        activeClimbable = null;
        _gravity = 9.81f;
    }

    public void ClimbUp()
    {
        _controller.enabled = false;
        transform.position = activeClimbable.standPoint.transform.position;
        wallDetectorCollider.size = new Vector3(0, 0, 0);
        StartCoroutine(SetSize_WallChecker());
        _climbing = false;
        _canJump = true;
        _gravity = 9.81f;
        _controller.enabled = true;
    }
    #endregion
    //handles stomping
    #region Stomp
    private void Stomp()
    {
        _stomping = true;
        _gravity = 50f;
    }
    public void BounceOff()
    {
        _gravity = 9.81f;
        _jumped = true; // TREAT THE BOUNCE UP AS A NORMAL JUMP SO YOU can DOUBLE JUMP AFTERWARD
        _canDoubleJump = true;
        direction.y = _jumpHeight;
    } // current known issues: double jumping then stomping causes the player to not be able to double jump after bouncing off
    #endregion
    //handles wall jumping
    #region Wall Jump
    public void OnWall()
    {
        _onWall = true;
        _canDoubleJump = false;
        _canJump = true;

        if(_jumped == true)
        {
            _jumped = false;
        }
      
        if (_grounded == true) // if touching the ground and the wall.. you're grounded. Normal gravity
        {
            _gravity = 9.81f;
        }
        else if(_grounded == false) // otherwise
        {
            _input.Player.LeftRightMove.Disable(); // disable the ability to move left and right
           
            if (_jumped == true) /// can probably get rid of this line
            {
                _gravity = 9.81f;
            }
            else
            {
                _gravity = 2.0f;//slowly slide down
            }
            
            direction.y = 0; // stop moving up 
        } 
    }
    public void NotOnWall() // leave wall
    {
        StartCoroutine(EnableControls());
        _onWall = false;
    }
    IEnumerator EnableControls()
    {
        yield return new WaitForSeconds(0.2f);
        _input.Player.LeftRightMove.Enable();
    }
    IEnumerator SetSize_WallChecker()
    {
        yield return new WaitForSeconds(0.1f);
        wallDetectorCollider.size = originalSize;
    }

    #endregion
    // used to limit direct access to variables within this script from outside scripts.
    #region RETURN TYPE METHODS
    private bool GroundChecker()
    {
        return _grounded;
    }
    private bool WallChecker()
    {
        return _onWall;
    }
    public bool Stomping()
    {
        return _stomping;
    }
    #endregion

    #region GROUND CHECK
    public void IsGrounded()
    {
        //re-enable left right movement.
        if(_input.Player.LeftRightMove.enabled == false)
        {
            _input.Player.LeftRightMove.Enable();
        }
        if(_input.Player.UpDownMove.enabled == true)
        {
            _input.Player.UpDownMove.Disable();
        }
        _grounded = true;
        _jumped = false;
        _canJump = true;
        _canDoubleJump = false;
        _onWall = false;
        _gravity = 9.81f;
        _stomping = false;
        _climbing = false;
    }

    public void NotGrounded()
    {
        _grounded = false;
    }
    #endregion
}
