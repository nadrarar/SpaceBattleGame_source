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
    class HUD_Test : TestCase
    {

        [UnitTest]
        public void TestHUDMethods()
        {
            GameHUD HUD = new GameHUD();
			try{
				HUD.Start();
				HUD.OnGUI();
				HUD.Update();
			}catch(Exception e) {
				//Something went wrong
				Assert.True(false,"error "+e.ToString());
			}
			
        }
    }
}
