/*
 * The MIT License (MIT)
 * Copyright (c) 2013 Danilo Gaby Andersen Trindade
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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
