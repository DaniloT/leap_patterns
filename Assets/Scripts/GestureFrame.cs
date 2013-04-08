using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Gesture frame object with all relevant frame information.
/// </summary>
public class GestureFrame : Object {
	
	public Vector position;
	public Vector velocity;
	public float timestamp;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GestureFrame"/> class.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	/// <param name='velocity'>
	/// Velocity.
	/// </param>
	/// <param name='timestamp'>
	/// Timestamp.
	/// </param>
	public GestureFrame(Vector position, Vector velocity, float timestamp) {
		this.position = position;
		this.velocity = velocity;
		this.timestamp = timestamp;
	}
}
