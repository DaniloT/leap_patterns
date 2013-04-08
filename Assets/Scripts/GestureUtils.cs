using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Leap;

/// <summary>
/// Class with utilities to detect patterns drawed
/// </summary>
public class GestureUtils : UnityEngine.Object {
	
	/// <summary>
	/// Enum for directions.
	/// </summary>
	private enum EnumVectorDirection {None = 0, Up, Down, Left, Right, UpRight, UpLeft, DownRight, DownLeft};
	/// <summary>
	/// Enum for patterns possible.
	/// </summary>
	public enum EnumGestures {Nothing = 0, DownZigZag, RightZigZag, Square};
	
	Texture2D canvas = null;
	
	float canvasSize = 0.0f;
	
	float precisionLineAprox;
	float precisionSquareAlignment;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GestureUtils"/> class
	/// with the precisions passed, if any. Otherwise uses the defaults.
	/// </summary>
	/// <param name='precisionLineAprox'>
	/// Precision to use in line straightening algorithm.
	/// </param>
	/// <param name='precisionSquareAlignment'>
	/// Precision to use when checking alignment of the points in square gesture.
	/// </param>
	public GestureUtils(float precisionLineAprox = 30.0f,
						float precisionSquareAlignment = 170.0f) {
		this.precisionLineAprox = precisionLineAprox;
		this.precisionSquareAlignment = precisionSquareAlignment;	
	}
	
	/// <summary>
	/// Returns which gesture was made from a list of frames. If a canvas
	/// and it's size is passed, draw the points considered for gesture
	/// detection on the canvas.
	/// </summary>
	/// <returns>
	/// Gesture from EnumGestures detected or EnumGestures.Nothing (== 0) if no gesture is detected.
	/// </returns>
	/// <param name='frameList'>
	/// List of frames to use in gesture detection
	/// </param>
	/// <param name='canvas'>
	/// Canvas to draw the points on.
	/// </param>
	/// <param name='canvasSize'>
	/// Size of the canvas to draw on.
	/// </param>
	public EnumGestures GestureChecker(List<Vector2> frameList, Texture2D canvas, float canvasSize) {
		this.canvas = canvas;
		this.canvasSize = canvasSize;
		return GestureChecker(frameList);
	}
	
	/// <summary>
	/// Returns which gesture was made from a list of frames.
	/// </summary>
	/// <returns>
	/// Gesture from EnumGestures detected, or EnumGestures.Nothing (== 0) if no gesture is detected.
	/// </returns>
	/// <param name='frameList'>
	/// List of frames to use in gesture detection
	/// </param>
	public EnumGestures GestureChecker(List<Vector2> frameList) {
		List<Vector2> keyPointsList = null;
		EnumGestures result = EnumGestures.Nothing;
		
		const int strokeSize = 5;
		
		keyPointsList = PointSimplifier.RamerDouglasPeucker(frameList, precisionLineAprox);
		GC.Collect();
		
		if (canvas != null) {
			for (int i = 0; i < keyPointsList.Count; i++) {
				Vector2 currentFrame = keyPointsList[i];
				
				for (int j = -strokeSize; j <= strokeSize; j++) {
					for (int k = -strokeSize; k <= strokeSize; k++) {
						if ((((j+(int)currentFrame.x) > 0) &&
							((k+canvas.height-(int)currentFrame.y) > 0)) &&
							(((j+currentFrame.x) < canvasSize) &&
							((k+canvas.height-(int)currentFrame.y) < canvasSize))) {
							
							canvas.SetPixel(j+(int)currentFrame.x, k+canvas.height-(int)currentFrame.y, Color.black);
							
						}
					}
				}
			}
			
			canvas.Apply();
		}
		
		result = OpenPass(keyPointsList);
		if (result == EnumGestures.Nothing) {
			
			keyPointsList = PointSimplifier.RamerDouglasPeucker(keyPointsList, precisionLineAprox*4);
			GC.Collect();
			
			if (canvas != null) {
				for (int i = 0; i < keyPointsList.Count; i++) {
					Vector2 currentFrame = keyPointsList[i];
					
					for (int j = -strokeSize*2; j <= strokeSize*2; j++) {
						for (int k = -strokeSize*2; k <= strokeSize*2; k++) {
							if ((((j+(int)currentFrame.x) > 0) &&
								((k+canvas.height-(int)currentFrame.y) > 0)) &&
								(((j+currentFrame.x) < canvasSize) &&
								((k+canvas.height-(int)currentFrame.y) < canvasSize))) {
								
								canvas.SetPixel(j+(int)currentFrame.x, k+canvas.height-(int)currentFrame.y, Color.black);
								
							}
						}
					}
				}
				
				canvas.Apply();
			}
			
			result = ClosedPass(keyPointsList);
		}
		
		return result;
	}
	
	/// <summary>
	/// Returns if one of the open gestures was made, either lightning or rain.
	/// </summary>
	/// <returns>
	/// EnumGestures.DownZigZag or EnumGestures.RightZigZag if detected, or EnumGestures.Nothing (== 0) if no gesture is detected.
	/// </returns>
	/// <param name='keyPointsList'>
	/// List of frames to use in gesture detection, must only contain the edges of the gesture made.
	/// </param>
	private EnumGestures OpenPass(List<Vector2> keyPointsList) {
		
		const int downZigEdgesTotal = 4;
		const int rightZigEdgesTotal = 4;
		
		int i = 0;
		int j = 0;
		
		bool hasFound = false;
		
		bool isDownZig = true;
		bool isRightZig = true;
		
		while ((!hasFound) && (i < keyPointsList.Count)) {
			
			int totalPoints = keyPointsList.Count - i;
			EnumVectorDirection currentDirection;
			
			Vector2 previousFrame = keyPointsList[i];
				
			isDownZig = true;
			isRightZig = true;
			
			if (totalPoints < downZigEdgesTotal) {
				isDownZig = false;
			}
			
			if (totalPoints < rightZigEdgesTotal) {
				isRightZig = false;
			}
			
			j = i+1;
			
			currentDirection = EnumVectorDirection.None;
			
			while ((!hasFound) && (isDownZig || isRightZig) && (j < keyPointsList.Count)) {
				Vector2 currentFrame = keyPointsList[j];
				float difX = currentFrame.x - previousFrame.x;
				float difY = currentFrame.y - previousFrame.y;

				if ((difX < 0) && (difY > 0)) {
					currentDirection = EnumVectorDirection.DownLeft;
				} else if ((difX > 0) && (difY > 0)) {
					currentDirection = EnumVectorDirection.DownRight;
				} else if ((difX < 0) && (difY < 0)) {
					currentDirection = EnumVectorDirection.UpLeft;
				} else if ((difX > 0) && (difY < 0)) {
					currentDirection = EnumVectorDirection.UpRight;
				} else if ((difX < 0) && (difY == 0)) {
					currentDirection = EnumVectorDirection.Left;
				} else if ((difX > 0) && (difY == 0)) {
					currentDirection = EnumVectorDirection.Right;
				} else if ((difX == 0) && (difY < 0)) {
					currentDirection = EnumVectorDirection.Up;
				} else {
					currentDirection = EnumVectorDirection.Down;
				}
				
				j++;

				if (isDownZig) {
					if (j - i > downZigEdgesTotal+1) {
						isDownZig = false;
					} else {
						if ((j - i) % 2 == 0) {
							if (currentDirection == EnumVectorDirection.DownLeft) {
								if (!(j < i+downZigEdgesTotal)) {
									hasFound = true;
									
									isRightZig = false;
								}
							} else {
								isDownZig = false;
							}
						} else {
							if (!((currentDirection == EnumVectorDirection.DownRight) ||
								(currentDirection == EnumVectorDirection.UpRight) ||
								(currentDirection == EnumVectorDirection.Right))) {
								isDownZig = false;
							}
						}
					}
				}
				
				if (isRightZig) {
					if (j - i > rightZigEdgesTotal+1) {
						isRightZig = false;
					} else {
						if ((j - i) % 2 == 0) {
							if ((currentDirection == EnumVectorDirection.DownRight) ||
								(currentDirection == EnumVectorDirection.Down)) {
								if (!(j < i+rightZigEdgesTotal)) {
									hasFound = true;
									
									isDownZig = false;
								}
							} else {
								isRightZig = false;
							}
						} else {
							if (!((currentDirection == EnumVectorDirection.Up) ||
								(currentDirection == EnumVectorDirection.UpRight))) {
								isRightZig = false;
							}
						}
					}
				}
				
				previousFrame = currentFrame;
			}
			
			i++;
		}
		
		if (hasFound) {
			if (isDownZig) {
				return EnumGestures.DownZigZag;
			} else {
				return EnumGestures.RightZigZag;
			}
		}
		
		return EnumGestures.Nothing;
	}
	
	/// <summary>
	/// Returns if the square closed form was made.
	/// </summary>
	/// <returns>
	/// EnumGestures.Square if detected, or EnumGestures.Nothing (== 0) if no gesture is detected.
	/// </returns>
	/// <param name='keyPointsList'>
	/// List of frames to use in gesture detection, must only contain the edges of the gesture made.
	/// </param>
	private EnumGestures ClosedPass(List<Vector2> keyPointsList) {
		
		const int squareEdgesTotal = 4;
		
		int i = 0;
		int j = 0;
		
		bool hasFound = false;
		
		bool isSquare = true;
		
		i = 0;
		while ((!hasFound) && (i < keyPointsList.Count)) {
			
			int totalPoints = keyPointsList.Count - i;
			List<EnumVectorDirection> directionList = new List<EnumVectorDirection>();
			EnumVectorDirection currentDirection;
			
			Vector2 startingFrame = keyPointsList[i];
			Vector2 previousFrame = startingFrame;

			isSquare = true;
			
			if (totalPoints < squareEdgesTotal) {
				isSquare = false;
			}
			
			j = i+1;
			
			currentDirection = EnumVectorDirection.None;
			
			directionList.Add(currentDirection);
			
			while ((!hasFound) && (isSquare) && (j < keyPointsList.Count)) {
				Vector2 currentFrame = keyPointsList[j];
				float difX = currentFrame.x - previousFrame.x;
				float difY = currentFrame.y - previousFrame.y;

				if ((difX < 0) && (difY > 0)) {
					currentDirection = EnumVectorDirection.DownLeft;
				} else if ((difX > 0) && (difY > 0)) {
					currentDirection = EnumVectorDirection.DownRight;
				} else if ((difX < 0) && (difY < 0)) {
					currentDirection = EnumVectorDirection.UpLeft;
				} else if ((difX > 0) && (difY < 0)) {
					currentDirection = EnumVectorDirection.UpRight;
				} else if ((difX < 0) && (difY == 0)) {
					currentDirection = EnumVectorDirection.Left;
				} else if ((difX > 0) && (difY == 0)) {
					currentDirection = EnumVectorDirection.Right;
				} else if ((difX == 0) && (difY < 0)) {
					currentDirection = EnumVectorDirection.Up;
				} else {
					currentDirection = EnumVectorDirection.Down;
				}
				
				directionList.Add(currentDirection);
				
				j++;
				
				if (isSquare) {
					if (j - i > squareEdgesTotal+1) {
						isSquare = false;
					} else {
						if (j - i == 5) {
							float difStartX = Math.Abs(currentFrame.x - startingFrame.x);
							float difStartY = Math.Abs(currentFrame.y - startingFrame.y);
							
							if ((difStartX < precisionSquareAlignment) && (difStartY < precisionSquareAlignment)) {
								if ((((directionList[1] == EnumVectorDirection.Right) || (directionList[1] == EnumVectorDirection.DownRight) || (directionList[1] == EnumVectorDirection.UpRight)) &&
									((directionList[2] == EnumVectorDirection.Up) || (directionList[2] == EnumVectorDirection.UpLeft) || (directionList[2] == EnumVectorDirection.UpRight)) &&
									((directionList[3] == EnumVectorDirection.Left) || (directionList[3] == EnumVectorDirection.DownLeft) || (directionList[3] == EnumVectorDirection.UpLeft)) &&
									((directionList[4] == EnumVectorDirection.Down) || (directionList[4] == EnumVectorDirection.DownLeft) || (directionList[4] == EnumVectorDirection.DownRight))) ||
									
									(((directionList[1] == EnumVectorDirection.Right) || (directionList[1] == EnumVectorDirection.DownRight) || (directionList[1] == EnumVectorDirection.UpRight)) &&
									((directionList[2] == EnumVectorDirection.Down) || (directionList[2] == EnumVectorDirection.DownLeft) || (directionList[2] == EnumVectorDirection.DownRight)) &&
									((directionList[3] == EnumVectorDirection.Left) || (directionList[3] == EnumVectorDirection.DownLeft) || (directionList[3] == EnumVectorDirection.UpLeft)) &&
									((directionList[4] == EnumVectorDirection.Up) || (directionList[4] == EnumVectorDirection.UpLeft) || (directionList[4] == EnumVectorDirection.UpRight))) ||
									
									(((directionList[1] == EnumVectorDirection.Left) || (directionList[1] == EnumVectorDirection.DownLeft) || (directionList[1] == EnumVectorDirection.UpLeft)) &&
									((directionList[2] == EnumVectorDirection.Up) || (directionList[2] == EnumVectorDirection.UpLeft) || (directionList[2] == EnumVectorDirection.UpRight)) &&
									((directionList[3] == EnumVectorDirection.Right) || (directionList[3] == EnumVectorDirection.DownRight) || (directionList[3] == EnumVectorDirection.UpRight)) &&
									((directionList[4] == EnumVectorDirection.Down) || (directionList[4] == EnumVectorDirection.DownLeft) || (directionList[4] == EnumVectorDirection.DownRight))) ||
									
									(((directionList[1] == EnumVectorDirection.Left) || (directionList[1] == EnumVectorDirection.DownLeft) || (directionList[1] == EnumVectorDirection.UpLeft)) &&
									((directionList[2] == EnumVectorDirection.Down) || (directionList[2] == EnumVectorDirection.DownLeft) || (directionList[2] == EnumVectorDirection.DownRight)) &&
									((directionList[3] == EnumVectorDirection.Right) || (directionList[3] == EnumVectorDirection.DownRight) || (directionList[3] == EnumVectorDirection.UpRight)) &&
									((directionList[4] == EnumVectorDirection.Up) || (directionList[4] == EnumVectorDirection.UpLeft) || (directionList[4] == EnumVectorDirection.UpRight)))) {
									
									float horLineInclination1 = Math.Abs(keyPointsList[j-5].y - keyPointsList[j-4].y);
									float horLineInclination2 = Math.Abs(keyPointsList[j-3].y - keyPointsList[j-2].y);
									float verLineInclination1 = Math.Abs(keyPointsList[j-4].x - keyPointsList[j-3].x);
									float verLineInclination2 = Math.Abs(keyPointsList[j-2].x - keyPointsList[j-5].x);
									
									if ((horLineInclination1 < precisionSquareAlignment*4) &&
										(horLineInclination2 < precisionSquareAlignment*4) &&
										(verLineInclination1 < precisionSquareAlignment*4) &&
										(verLineInclination2 < precisionSquareAlignment*4)) {
										
										hasFound = true;
									}
								
								} else if ((((directionList[1] == EnumVectorDirection.Up) || (directionList[1] == EnumVectorDirection.UpLeft) || (directionList[1] == EnumVectorDirection.UpRight)) &&
									((directionList[2] == EnumVectorDirection.Left) || (directionList[2] == EnumVectorDirection.DownLeft) || (directionList[2] == EnumVectorDirection.UpLeft)) &&
									((directionList[3] == EnumVectorDirection.Down) || (directionList[3] == EnumVectorDirection.DownLeft) || (directionList[3] == EnumVectorDirection.DownRight)) &&
									((directionList[4] == EnumVectorDirection.Right) || (directionList[4] == EnumVectorDirection.DownRight) || (directionList[4] == EnumVectorDirection.UpRight))) ||
								
									(((directionList[1] == EnumVectorDirection.Up) || (directionList[1] == EnumVectorDirection.UpLeft) || (directionList[1] == EnumVectorDirection.UpRight)) &&
									((directionList[2] == EnumVectorDirection.Right) || (directionList[2] == EnumVectorDirection.DownRight) || (directionList[2] == EnumVectorDirection.UpRight)) &&
									((directionList[3] == EnumVectorDirection.Down) || (directionList[3] == EnumVectorDirection.DownLeft) || (directionList[3] == EnumVectorDirection.DownRight)) &&
									((directionList[4] == EnumVectorDirection.Left) || (directionList[4] == EnumVectorDirection.DownLeft) || (directionList[4] == EnumVectorDirection.UpLeft))) ||
								
									(((directionList[1] == EnumVectorDirection.Down) || (directionList[1] == EnumVectorDirection.DownLeft) || (directionList[1] == EnumVectorDirection.DownRight)) &&
									((directionList[2] == EnumVectorDirection.Left) || (directionList[2] == EnumVectorDirection.DownLeft) || (directionList[2] == EnumVectorDirection.UpLeft)) &&
									((directionList[3] == EnumVectorDirection.Up) || (directionList[3] == EnumVectorDirection.UpLeft) || (directionList[3] == EnumVectorDirection.UpRight)) &&
									((directionList[4] == EnumVectorDirection.Right) || (directionList[4] == EnumVectorDirection.DownRight) || (directionList[4] == EnumVectorDirection.UpRight))) ||
								
									(((directionList[1] == EnumVectorDirection.Down) || (directionList[1] == EnumVectorDirection.DownLeft) || (directionList[1] == EnumVectorDirection.DownRight)) &&
									((directionList[2] == EnumVectorDirection.Right) || (directionList[2] == EnumVectorDirection.DownRight) || (directionList[2] == EnumVectorDirection.UpRight)) &&
									((directionList[3] == EnumVectorDirection.Up) || (directionList[3] == EnumVectorDirection.UpLeft) || (directionList[3] == EnumVectorDirection.UpRight)) &&
									((directionList[4] == EnumVectorDirection.Left) || (directionList[4] == EnumVectorDirection.DownLeft) || (directionList[4] == EnumVectorDirection.UpLeft)))) {
									
									float horLineInclination1 = Math.Abs(keyPointsList[j-4].x - keyPointsList[j-3].x);
									float horLineInclination2 = Math.Abs(keyPointsList[j-2].x - keyPointsList[j-5].x);
									float verLineInclination1 = Math.Abs(keyPointsList[j-5].y - keyPointsList[j-4].y);
									float verLineInclination2 = Math.Abs(keyPointsList[j-3].y - keyPointsList[j-2].y);
									
									if ((horLineInclination1 < precisionSquareAlignment*4) &&
										(horLineInclination2 < precisionSquareAlignment*4) &&
										(verLineInclination1 < precisionSquareAlignment*4) &&
										(verLineInclination2 < precisionSquareAlignment*4)) {
										
										hasFound = true;
									}
								}
							} else {
								isSquare = false;
							}
						}
					}
				}
				
				previousFrame = currentFrame;
			}
			
			i++;
		}
		
		if (hasFound) {
			return EnumGestures.Square;
		}
		
		return EnumGestures.Nothing;
	}
}
