using UnityEngine;
using System;
using System.Collections.Generic;

public class PaddleSystem : SystemBase {

    void OrderedOnEnable() {

    }

    void OrderedUpdate() {
        var input = Pool<InputConfig>.First;
        var paddles = Pool<Paddle>.components;
        var gamecfg = Pool<GameConfig>.First;

        foreach (var paddle in paddles) {

            

            Vector3 pos = paddle.transform.position;
            float resultPos = pos.z;

            if (paddle.data.player == Paddle.Player.one) {
                if (Input.GetKey(input.paddle1_up)) {
                    resultPos += gamecfg.PaddleSpeed;
                }

                if (Input.GetKey(input.paddle1_down)) {
                    resultPos -= gamecfg.PaddleSpeed;

                }
            }
            else {
                if (Input.GetKey(input.paddle2_up)) {
                    resultPos += gamecfg.PaddleSpeed;
                }

                if (Input.GetKey(input.paddle2_down)) {
                    resultPos -= gamecfg.PaddleSpeed;

                }
            }
            resultPos = Mathf.Clamp(resultPos, paddle.data.min, paddle.data.max);
            pos.z = resultPos;
            paddle.transform.position = pos;
        }
    }
}
