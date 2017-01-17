using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

[Config("Pong config", "")]
public class GameConfig : ConfigBase {

    [ConsoleVar("game.ballImpulse")]
    public r_float BallInitialImpulse = new r_float(10f);

    [ConsoleVar("game.paddleSpeed")]
    public r_float PaddleSpeed = new r_float(.05f);

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
