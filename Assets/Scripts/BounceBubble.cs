
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BounceBubble : MonoBehaviour
{
    private Rigidbody2D _bubbleBody;
    public Vector2 initialForce;

    public float forceFactor = 6f;

    private void Awake()
    {
        _bubbleBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(Vector2 launchVector)
    {
        _bubbleBody.velocity = launchVector.normalized * forceFactor;
    }
}
