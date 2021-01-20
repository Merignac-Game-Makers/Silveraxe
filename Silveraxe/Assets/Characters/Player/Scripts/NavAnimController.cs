//Copyright Filmstorm (C) 2018 - Movement Controller for Root Motion and built in IK solver
using System;
using UnityEngine;
using UnityEngine.AI;

using static App;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
//[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(NavMeshAgent))]
public class NavAnimController : MonoBehaviour
{
	#region Variables
	public float inputX { get; set; }       //Left and Right Inputs
	public float inputZ { get; set; }       //Forward and Back Inputs

	public Animator anim { get; private set; }                  //Animator
	private NavMeshAgent agent;                                 //Navigation agent

	private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
	private Quaternion leftFootIkRotation, rightFootIkRotation;
	private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

	MovementInput movementInput;

	[Header("Feet Grounder")]
	public bool enableFeetIk = true;
	[Range(0, 9)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
	[Range(0, 10)] [SerializeField] private float raycastDownDistance = 1.5f;
	//[SerializeField] private LayerMask environmentLayer;
	public LayerMask environmentLayer;
	[SerializeField] private float pelvisOffset = 0f;
	[Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
	[Range(0, 1)] [SerializeField] private float feetToIkPositionSpeed = 0.5f;

	public bool showSolverDebug = true;

	//public string leftFootAnimVariableName = "LeftFootCurve";
	//public string rightFootAnimVariableName = "RightFootCurve";
	//public bool useProIkFeature = false;



	[Header("Animation Smoothing")]
	[Range(0, 1f)]
	public float animSmoothTime = 0.2f; //velocity dampening

	Animator[] anims;
	#endregion

	#region Initialization

	// Initialization of variables
	void Start() {
		anims = GetComponentsInChildren<Animator>(true);

		anim = GetComponentInChildren<Animator>();
		agent = GetComponent<NavMeshAgent>();
		movementInput = GetComponentInParent<MovementInput>();

		if (anim == null)
			Debug.LogError("We require " + transform.name + " game object to have at least an animator. This will allow for Foot IK to function");
	}
	#endregion


	#region PlayerMovement
	Vector3 tmp;
	void InputMagnitude() {
		if (agent) {
			tmp = transform.InverseTransformVector(agent.velocity);
		} else {
			tmp = movementInput.velocity;
		}

		//Calculate Input Vectors
		inputX = tmp.x*2;			// lat�ral
		inputZ = tmp.z*10;			// avant/arri�re

		//var v = Mathf.Abs(anim.GetFloat("InputX") + anim.GetFloat("InputZ"));
		foreach (Animator anim in anims) {
			if (anim.enabled) {
				anim.SetFloat("InputX", inputX, animSmoothTime, Time.deltaTime);
				anim.SetFloat("InputZ", inputZ, animSmoothTime, Time.deltaTime);
				//anim.SetFloat("velocity", v, animSmoothTime, Time.deltaTime);
			}
		}
	}

	#endregion


	#region FeetGrounding
	void Update() {
		InputMagnitude();

		if (enableFeetIk == false) { return; }
		if (anim == null) { return; }

		AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
		AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

		//find and raycast to the ground to find positions
		FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation); // handle the solver for right foot
		FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation); //handle the solver for the left foot
	}

	private void OnAnimatorIK(int layerIndex) {
		if (enableFeetIk == false) { return; }
		if (anim == null) { return; }

		//MovePelvisHeight();

		anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);		//right foot ik position 
		MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

		anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);			//left foot ik position 
		MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
	}

	#endregion



	#region FeetGroundingMethods

	/// <summary>
	/// Moves the feet to ik point.
	/// </summary>
	/// <param name="foot">Foot.</param>
	/// <param name="positionIkHolder">Position ik holder.</param>
	/// <param name="rotationIkHolder">Rotation ik holder.</param>
	/// <param name="lastFootPositionY">Last foot position y.</param>
	void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY) {
		Vector3 targetIkPosition = anim.GetIKPosition(foot);

		if (positionIkHolder != Vector3.zero) {
			targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
			positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

			float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
			targetIkPosition.y += yVariable;

			lastFootPositionY = yVariable;

			targetIkPosition = transform.TransformPoint(targetIkPosition);

			anim.SetIKRotation(foot, rotationIkHolder);
		}

		anim.SetIKPosition(foot, targetIkPosition);
	}
	/// <summary>
	/// Moves the height of the pelvis.
	/// </summary>
	private void MovePelvisHeight() {

		if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0) {
			lastPelvisPositionY = anim.bodyPosition.y;
			return;
		}

		float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
		float rOffsetPosition = rightFootIkPosition.y - transform.position.y;

		float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

		Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

		newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

		anim.bodyPosition = newPelvisPosition;

		lastPelvisPositionY = anim.bodyPosition.y;

	}

	/// <summary>
	/// We are locating the Feet position via a Raycast and then Solving
	/// </summary>
	/// <param name="fromSkyPosition">From sky position.</param>
	/// <param name="feetIkPositions">Feet ik positions.</param>
	/// <param name="feetIkRotations">Feet ik rotations.</param>
	private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations) {
		//raycast handling section 
		RaycastHit feetOutHit;

		if (showSolverDebug)
			Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * raycastDownDistance, Color.yellow);

		if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer)) {
			//finding our feet ik positions from the sky position
			feetIkPositions = fromSkyPosition;
			feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
			feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

			return;
		}

		feetIkPositions = Vector3.zero; //it didn't work :(

	}
	/// <summary>
	/// Adjusts the feet target.
	/// </summary>
	/// <param name="feetPositions">Feet positions.</param>
	/// <param name="foot">Foot.</param>
	private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot) {
		feetPositions = anim.GetBoneTransform(foot).position;               // r�cup�rer la postion du pied
		feetPositions.y = transform.position.y + heightFromGroundRaycast;   // remonter la position pour la source du raycast
	}

	#endregion
}





