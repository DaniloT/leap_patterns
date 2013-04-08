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
using System.Collections.Generic;
using System;
using System.IO;
using Leap;
using System.Runtime.InteropServices;

public class LeapController : MonoBehaviour {
	
	private const float CANVAS_SIZE = 512.0f;
	
	/// <summary>
	/// Constant indicating accepted frame variation to cancel out noises.
	/// </summary>
	private const float NORMAL_FRAME_VARIATION = 250.0f;
	
	Listener listener;
	Controller controller;
	
	GestureInterpreter gi = null;
	GestureUtils gu = null;
	
	public Renderer canvasRenderer;
	Texture2D canvas;
	string status = "Nothing.";
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		listener = new Listener();
		controller = new Controller(listener);
		
		//This constructor can be called with parameters to change default precision values.
		gu = new GestureUtils();
		
		gi = new GestureInterpreter();
		gi.callbackInstance = this;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		Frame frame = controller.Frame();
		gi.DetectMovement(frame);
	}
	
	/// <summary>
	/// Raises the GU event.
	/// </summary>
	public void OnGUI()	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 40;
		GUI.Label(new Rect(CANVAS_SIZE/2.0f - 75.0f, 30.0f, 150.0f, 50.0f), status, style);
	}
	
	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	public void OnApplicationQuit() {
		Debug.Log("Quit!");
		controller.Dispose();
	}
	
	/// <summary>
	/// Signals the start of a gesture movement.
	/// </summary>
	public void beginOfGestureCallback() {
		Debug.Log("Gesture started! Clearing canvas...");
		canvas = new Texture2D((int)CANVAS_SIZE, (int)CANVAS_SIZE, TextureFormat.ARGB32, false);
		for (int i = 0; i < canvas.width; i++) {
			for (int j = 0; j < canvas.width; j++) {
				canvas.SetPixel(i, j, Color.white);
			}
		}
		canvasRenderer.material.mainTexture = canvas;
		canvas.Apply();
		
		GC.Collect();
	}
	
	/// <summary>
	/// End of gesture movement callback.
	/// </summary>
	/// <param name='data'>
	/// Data of the gesture gathered.
	/// </param>
	public void endOfGestureCallback(GestureData data) {
		GestureUtils.EnumGestures gResult;
		
		Debug.Log("Gesture ended!");
		List<Vector2> pointList = new List<Vector2>();
		GestureData normalizedData = data.getNormalizedGestureData(CANVAS_SIZE);
		
		GestureFrame previousFrame = (GestureFrame)normalizedData.frames[0];
		pointList.Add(new Vector2(previousFrame.position.x, previousFrame.position.y));
		for (int i = 1; i < normalizedData.frames.Count; i++) {
			GestureFrame currentFrame = (GestureFrame)normalizedData.frames[i];
			
			if ((Math.Abs(currentFrame.position.x - previousFrame.position.x) < NORMAL_FRAME_VARIATION) &&
				(Math.Abs(currentFrame.position.y - previousFrame.position.y) < NORMAL_FRAME_VARIATION)) {
				
				pointList.Add(new Vector2(currentFrame.position.x, currentFrame.position.y));
				previousFrame = currentFrame;
			}
			
		}
		
		//At gesture end, draw the normalized data.
		Vector2 previousPoint = pointList[0];
		for (int i = 1; i < pointList.Count; i++) {
			Vector2 currentPoint = pointList[i];
			DrawLine(canvas, previousPoint.x, previousPoint.y, currentPoint.x, currentPoint.y, Color.black);
			previousPoint = currentPoint;
		}
		
		canvas.Apply();
		
		//The checker method only receives canvas if you want the points used drawn on the screen,
		//can just receive a points list otherwise.
		gResult = gu.GestureChecker(pointList, canvas, CANVAS_SIZE);
		
		switch(gResult) {
		case GestureUtils.EnumGestures.DownZigZag:
			Debug.Log("Downwards Zig Zag!");
			status = "Downwards Zig Zag!";
			break;
		case GestureUtils.EnumGestures.RightZigZag:
			Debug.Log("Rightwards Zig Zag!");
			status = "Rightwards Zig Zag!";
			break;
		case GestureUtils.EnumGestures.Square:
			Debug.Log("Square!");
			status = "Square!";
			break;
		default:
			Debug.Log("No Gesture found.");
			status = "Nothing.";
			break;
		}

	}
	
	/// <summary>
	/// Draws a line between the points passed.
	/// </summary>
	/// <param name='a_Texture'>
	/// Texture to draw on.
	/// </param>
	/// <param name='x1'>
	/// X coordinate of point 1.
	/// </param>
	/// <param name='y1'>
	/// Y coordinate of point 1.
	/// </param>
	/// <param name='x2'>
	/// X coordinate of point 2.
	/// </param>
	/// <param name='y2'>
	/// Y coordinate of point 2.
	/// </param>
	/// <param name='a_Color'>
	/// Color to use.
	/// </param>
	void DrawLine(Texture2D a_Texture, float x1, float y1, float x2, float y2, Color a_Color) {
 
		float b = x2 - x1;
		float h = y2 - y1;
		float l = Mathf.Abs(b);
		if (Mathf.Abs (h) > l) l = Mathf.Abs(h);
		int il = (int)l;
		float dx = b / (float)l;
		float dy = h / (float)l;

		for ( int i = 0; i <= il; i++ )
		{
			a_Texture.SetPixel((int)x1, a_Texture.height-(int)y1, a_Color);
			
			x1 += dx;
			y1 += dy;
		}
	}
	 
}
