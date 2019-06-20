﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

public class KinematicPlayer : MonoBehaviour
{
    public ExampleCharacterController Character;
    public ExampleCharacterCamera CharacterCamera;
    public GameObject CharacterMesh;
    public float MouseSensitivity = 0.1f;

    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";

    private void Start()
    {
        Character = this.gameObject.GetComponent<ExampleCharacterController>();
        if (!Character)
        {
            GameDebug.LogError("Example Character Controller could not be found in the character!/nSetting character inactive.");
            this.gameObject.SetActive(false);
            return;
        }
        // #TODO: Create a camera system like FPS Sample
        Camera cam = Camera.main;
        CharacterCamera = cam.gameObject.AddComponent<ExampleCharacterCamera>();
        if (!CharacterCamera)
        {
            GameDebug.LogError("Couldn't attach Example Character Camera to the camera!/nSetting character inactive.");
            this.gameObject.SetActive(false);
            return;
        }

        // Set camera minDist to 0 to allow first-person perspective
        CharacterCamera.MinDistance = 0.0f;

        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        CharacterCamera.SetFollowCharacter(Character);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCameraInput();
        HandleCharacterInput();
    }

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
        float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
        Vector3 lookInputVector = new Vector3(mouseLookAxisRight * MouseSensitivity, mouseLookAxisUp * MouseSensitivity, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
    scrollInput = 0f;
#endif

        // Apply inputs to the camera
        CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level
        if (Input.GetMouseButtonDown(1))
        {
            CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
        }
        CharacterMesh.SetActive((CharacterCamera.TargetDistance == 0f) ? false : true);

    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
        characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
        characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
        characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
        characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
        characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}