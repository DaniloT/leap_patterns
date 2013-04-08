using UnityEngine;
using System.Collections;
using Leap;

public class GestureInterpreter : Object {
	
	ArrayList detectionHeap = null;
	int maximumHeapSize = 2;
	
	float lastEndMovementTime = 0.0f;
	float lastStoppedTime = 0.0f;
	
	GestureData gestureData = null;
	
	bool performingMovement = false;
	bool performingGesture = false;
	
	Vector3 movementDirection = Vector3.zero;
	Vector3 movementTotalWork = Vector3.zero;
	
	float minimumRelevantWorkThreshold = 0.000002f;
	float minimumStartMovementTotalWork = 0.02f;
	float minimumEndMovementKinectEnergy = 2.0e-3f;
	
	public LeapController callbackInstance;
	
	public GestureInterpreter()
	{
		detectionHeap = new ArrayList();
		detectionHeap.Capacity = this.maximumHeapSize;
	}
	
	/// <summary>
	/// Detects the movement and builds the frame list when movement is detected.
	/// </summary>
	/// <param name='frame'>
	/// Leap frame.
	/// </param>
	public void DetectMovement(Frame frame)
	{	
		GestureFrame gestureFrame = new GestureFrame(frame.Pointables[0].TipPosition, frame.Pointables[0].TipVelocity, frame.Timestamp);
		this.detectionHeap.Add(gestureFrame);
		
		Vector3 acceleration = Vector3.zero;
		Vector3 delta = Vector3.zero;
		Vector3 movementWork = Vector3.zero;
		Vector3 kinecticEnergy = Vector3.zero;
		
		float movementTotalWork;
		float kinecticEnergyTotal;
		
		if (this.detectionHeap.Count > this.maximumHeapSize) {
			this.detectionHeap.RemoveAt(0);
			
			GestureFrame firstFrame = (GestureFrame)this.detectionHeap[0];
			GestureFrame lastFrame = (GestureFrame)this.detectionHeap[this.maximumHeapSize - 1];
			
			float deltaTime = lastFrame.timestamp - firstFrame.timestamp;
			
			acceleration.x = (lastFrame.velocity.x - firstFrame.velocity.x) / deltaTime;
			acceleration.y = (lastFrame.velocity.y - firstFrame.velocity.y) / deltaTime;
			acceleration.z = (lastFrame.velocity.z - firstFrame.velocity.z) / deltaTime;
			
			this.movementDirection.x = acceleration.x > 0 ? 1 : -1;
			this.movementDirection.y = acceleration.y > 0 ? 1 : -1;
			this.movementDirection.z = acceleration.z > 0 ? 1 : -1;
			
			delta.x = (lastFrame.position.x - firstFrame.position.x);
			delta.y = (lastFrame.position.y - firstFrame.position.y);
			delta.z = (lastFrame.position.z - firstFrame.position.z);
			
			/*
			 * http://en.wikipedia.org/wiki/Work_(physics)
			 * Work calculation, using the acceleration previously calculated as the force to move the finger from one point to another
			 */
			
			movementWork.x = delta.x * acceleration.x;
			movementWork.y = delta.y * acceleration.y;
			movementWork.z = delta.z * acceleration.z;
			
			/* 
			 * http://en.wikipedia.org/wiki/Kinetic_energy
			 * Measures the pointable kinectic energy to detect when gesture have ended.
			 */
			
			kinecticEnergy.x = Mathf.Pow(lastFrame.velocity.x, 2) * 0.5f * 1.0e-6f;
			kinecticEnergy.y = Mathf.Pow(lastFrame.velocity.y, 2) * 0.5f * 1.0e-6f;
			kinecticEnergy.z = Mathf.Pow(lastFrame.velocity.z, 2) * 0.5f * 1.0e-6f;
			
			kinecticEnergyTotal = kinecticEnergy.x + kinecticEnergy.y;
			
			if (Mathf.Abs(movementWork.x) > this.minimumRelevantWorkThreshold) {
				this.movementTotalWork.x += movementWork.x;
			}
			
			if (Mathf.Abs(movementWork.y) > this.minimumRelevantWorkThreshold) {
				this.movementTotalWork.y += movementWork.y;
			}
			
			if (Mathf.Abs(movementWork.z) > this.minimumRelevantWorkThreshold) {
				this.movementTotalWork.z += movementWork.z;
			}
			
			if (!this.performingMovement && lastFrame.timestamp - this.lastStoppedTime > 50000.0f) {
				this.movementTotalWork.x = 0.0f;
				this.movementTotalWork.y = 0.0f;
				this.movementTotalWork.z = 0.0f;
				this.lastStoppedTime = lastFrame.timestamp;
			}
			
			movementTotalWork = this.movementTotalWork.x + this.movementTotalWork.y;
			
			if (movementTotalWork > this.minimumStartMovementTotalWork && !this.performingMovement) {
				this.movementTotalWork.x = 0;
				this.movementTotalWork.y = 0;
				this.movementTotalWork.z = 0;
				
				this.performingMovement = true;
				
				if (!this.performingGesture) {
					this.performingGesture = true;
					this.gestureData = new GestureData();
					
					if (this.callbackInstance != null)
					{
						this.callbackInstance.beginOfGestureCallback();
					}
				}
				
			} else if (this.performingMovement && kinecticEnergyTotal < this.minimumEndMovementKinectEnergy) {
				
				this.movementTotalWork.x = 0.0f;
				this.movementTotalWork.y = 0.0f;
				this.movementTotalWork.z = 0.0f;
				
				this.performingMovement = false;
				this.lastStoppedTime = this.lastEndMovementTime = lastFrame.timestamp;
			}
			
			if (this.performingGesture) {
				this.gestureData.appendFrame(gestureFrame);
			}
			
			float timeSinceEndMovement = lastFrame.timestamp - this.lastEndMovementTime;
			
			if (!this.performingMovement && this.performingGesture && timeSinceEndMovement > 500000) {
				if (this.callbackInstance != null) {
					this.callbackInstance.endOfGestureCallback(this.gestureData);
				}
				
				this.performingGesture = false;
			} 
		}
	}
}
