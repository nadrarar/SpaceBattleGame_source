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
    class Weapons_Test : TestCase
    {
		//The current number of secondary weapons we currently have available, will need to switch some tests when we add more weapons
		const int MAX_WEAPONS = 2;
		const int MAX_AMMUNITION = 10;
		
        [UnitTest]
        public void TestFireSecondayMissile()
        {
			
            FireSecondary fs = new FireSecondary();
			fs.shoot();
			//Check if ammunition was decremented
			Assert.Equals(fs.ammo(), 9);
			
			//Check if ammunition can be decremented to zero and no greater
			for (int i = 0 ; i < MAX_AMMUNITION + 1; i ++) 
			{
				fs.shoot();
				Thread.Sleep(1000);
			}
			Assert.Equals (fs.ammo(), 0);
			
			//Check that the correct weapon is currently equipped
			Assert.Equals(fs.currentWeaponEquipped(), 0);
        }
		
		[UnitTest]
		public void TestFireSecondayScattershot()
        {
            FireSecondary fs = new FireSecondary();
			fs.switchWeaponUp();
			fs.shoot();
			//Check if ammunition was decremented
			Assert.Equals(fs.ammo(), 9);
			
			//Check if ammunition can be decremented to zero and no greater
			for (int i = 0 ; i < MAX_AMMUNITION + 1; i ++) 
			{
				fs.shoot();
				Thread.Sleep(1000);
			}
			Assert.Equals (fs.ammo(), 0);
			
			//Check that the correct weapon is currently equipped
			Assert.Equals(fs.currentWeaponEquipped(), 1);
        }
		
		[UnitTest]
		public void TestFireSecondaySwitchWeapons()
        {
			
            FireSecondary fs = new FireSecondary();
			//Make sure correct starting weapon
			Assert.Equals(fs.currentWeaponEquipped(), 0);
			//Make sure it correctly switches weapons
			fs.switchWeaponUp();
			Assert.Equals(fs.currentWeaponEquipped(), 1);
			//Make sure it can't go above weapon limit
			for (int i = 1; i <= MAX_WEAPONS; i++){
				fs.switchWeaponUp();
			}
			//Should be the maximum weapon
			Assert.Equals(fs.currentWeaponEquipped(), MAX_WEAPONS - 1);
			for (int i = 0; i <= MAX_WEAPONS; i++){
				fs.switchWeaponDown();
			}
			//Should be the missile
			Assert.Equals(fs.currentWeaponEquipped(), 0);
        }
		
		[UnitTest]
		 public void TestFirePrimary()
        {
            FirePrimary fp = new FirePrimary();
			bool first = fp.shoot();
			Assert.False(first);	
        }
		
		[UnitTest]
		 public void TestBullerController()
        {
            BulletController bc = new BulletController();
			try {
				bc.ExplodeBullet(bc.transform.position);
			}catch (Exception e) {
				//Explode bullet doesn't work
				Assert.True(false,"error "+e.ToString());
		//		Assert.True(false);
			}
        }
		
		[UnitTest]
		 public void TestMissileController()
        {
            MissileController bc = new MissileController();		
			try {
				bc.ExplodeBullet(bc.transform.position);
			}catch (Exception e) {
				//Explode bullet doesn't work
				Assert.True(false,"error "+e.ToString());
			//	Assert.True(false);
			}
        }
    }
}
