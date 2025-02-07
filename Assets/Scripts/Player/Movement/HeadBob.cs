using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object;
 

public class HeadBob : NetworkBehaviour
{
 
 [SerializeField] private bool _enable = true;
 [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.015f;
 [SerializeField, Range(0, 30)] private float _frequency = 10.0f;

 [SerializeField] private Transform _camera = null;
 [SerializeField] private Transform _cameraHolder = null;

 private float _toggleSpeed = 3.0f;
 private Vector3 _startPos;
private CharacterController _controller;

 private void Awake()
 {
    _controller = GetComponent<CharacterController>();
    _startPos = _camera.localPosition;
 }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<HeadBob>().enabled = false;
        }
    }

 private void CheckMotion()
 {
    float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

    ResetPosition();
    if (speed < _toggleSpeed) return;
    if (!_controller.isGrounded) return;
 
    PlayMotion(FootStepMotion());
 }

 private void PlayMotion(Vector3 motion)
 {
_camera.localPosition += motion; 

}

 private Vector3 FootStepMotion()
 {
     
    Vector3 pos = Vector3.zero;
    pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
    pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
    return pos;
 }

 private void ResetPosition()
 {
    if (_camera.localPosition == _startPos) return;
    _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
 }

 void Update()
 {
    if (!_enable) return;

    CheckMotion();
    
    _camera.LookAt(FocusTarget());

    
 }

 private Vector3 FocusTarget()
 {
    Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
    pos += _cameraHolder.forward * 330.0f;
    return pos;
 }

   public void NormFrequency()
   {
      if (_frequency == 10.0f) return;

      _frequency = 10.0f;
   }

   public void SprintFrequency()
   {
      if (_frequency == 20.0f) return;
      _frequency = 20.0f;
   }

      public void SlowFrequency()
   {
      if (_frequency == 5.0f) return;

      _frequency = 5.0f;
   }

   public void DisableHeadBob()
   {
      _enable = false;
   }
   public void EnableHeadBob()
   {
      _enable = true;
   }
}

