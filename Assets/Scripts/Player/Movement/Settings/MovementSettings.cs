using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Movement/Settings")]
public class MovemnentSettings : ScriptableObject
{
    public float speed { get {return _speed; } private set { _speed = value; } }
    [SerializeField] private float _speed = 5.0f;

    public float sprint { get {return _sprint; } private set { _sprint = value; } }
    [SerializeField] private float _sprint = 10.0f;

    public float slow { get {return _slow; } private set { _slow = value; } }
    [SerializeField] private float _slow = 3.0f;

    public float jumpForce { get {return _jumpForce; } private set { _jumpForce = value; } }
    [SerializeField] private float _jumpForce = 13f;

    public float antiBump { get {return _antiBump; } private set { _antiBump = value; } }
    [SerializeField] private float _antiBump = 4.5f;

    public float gravity { get {return _gravity; } private set { _gravity = value; } }
    [SerializeField] private float _gravity = 30f;
}