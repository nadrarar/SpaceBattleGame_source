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
    class Radar_Test : TestCase
    {
		
        [UnitTest]
        public void TestRadarController()
        {
			try{
				RadarController rc = new RadarController();
				rc.Start();
				Collider c1 = new Collider();
				rc.OnTriggerEnter(c1);
				Collider c2 = new Collider();
				rc.OnTriggerExit(c2);
			}catch(Exception e) {
				//Something went wrong
				Assert.True(false,"error "+e.ToString());
				//Assert.True(false);
			}
			
        }
		
		[UnitTest]
        public void TestShipMethods()
        {
			try{
				RadarIconController RIC = new RadarIconController();
				RIC.Start();
				RIC.Update();
			}catch(Exception e) {
				//Something went wrong
				Assert.True(false,"error "+e.ToString());
				//Assert.True(false);
			}
			
        }
    }
}
