/**
 * @file Assert.cs
 * 
 * Static assertion methods for verifying test expectations.
 * NOTE: It is important that each test throw its own TestException if it fails,
 *       as this gives us more accurate call stack reporting.
 * 
 */

using System;
using System.Threading;
using UnityEngine;

namespace SharpUnit
{
    class Ship_Test : TestCase
    {
		
        /*[UnitTest]
        public void TestShipControls()
        {
			try{
				Ship ship = new Ship();
				//Start the ship
				ship.Start();
				//Test roll, yaw, and pitch with a floating point value of 1
				ship.Roll(1);
				ship.Yaw(1);
				ship.Pitch(1);
				//Accelerate the ship
				ship.Accelerate(1);
				//Brake all three controls
				ship.PitchBrake();
				ship.YawBrake();
				ship.RollBrake();
				
			}catch(Exception e) {
				//Something went wrong
			//	Assert.True(false);
			}
			
        }
		
		[UnitTest]
        public void TestShipMethods()
        {
			try{
				Ship ship = new Ship();
				//Start the ship
				ship.Start();
				//Test roll, yaw, and pitch with a floating point value of 1
				ship.ToggleInertialDampening();
				ship.DestructImpl();
				ship.FixedUpdate();
			}catch(Exception e) {
				//Something went wrong
	//			Assert.True(false);
			}
			
        }*/
    }
}
