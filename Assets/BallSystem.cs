using UnityEngine;
using System;
using System.Collections.Generic;

public class BallSystem : SystemBase {

	void OrderedOnEnable(){
        var ball = Pool<Ball>.First;
        var gamecfg = Pool<GameConfig>.First;

        var rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(UnityEngine.Random.insideUnitSphere * gamecfg.BallInitialImpulse , ForceMode.Impulse);
	}

	void OrderedUpdate(){
	
	}
}
