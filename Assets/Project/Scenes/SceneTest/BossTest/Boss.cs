using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    enum State
    {
        doNothing,
        generateMob,
        generateObstacle,
        attackLaser,
        specialLaser
    }

    //ƒpƒ‰ƒ[ƒ^Ý’è
    float generateMobTime = 0;
    float generateMobUpSpeed = 5f;
    float throwMobSpeed = 3f;
    
    float generateObstacleTime = 0;
    float generateObstacleUpSpeed = 10f;
    float throwObstacleSpeed = 3f;

    float attackLaserTime = 0;
    float attackLaserUpSpeed = 15f;


    State currentState = State.doNothing;
    bool stateEnter = true;

    void ChangeState(State newState)
    {
        currentState = newState;
        stateEnter = true;
    }

    private void Update()
    {
        if(currentState != State.generateMob)
        {
            generateMobTime += Time.deltaTime / generateMobUpSpeed;
        }

        if(currentState != State.generateObstacle)
        {
            generateObstacleTime += Time.deltaTime / generateObstacleUpSpeed;
        }

        switch (currentState)
        {
            case State.doNothing:
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("Boss‚Í”÷Î‚ñ‚Å‚¢‚é");
                }

                if(generateMobTime >= 1)
                {
                    ChangeState(State.generateMob);
                    return;
                }
                #endregion
                break;

            case State.generateMob:
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("Boss‚ÍMob‚ð“Š‚°‚½");
                }

                generateMobTime -= Time.deltaTime / throwMobSpeed;

                if(generateMobTime <= 0)
                {
                    ChangeState(State.doNothing);
                }
                #endregion
                break;

            case State.generateObstacle:
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("Boss‚ÍObstacle‚ð“Š‚°‚½");
                }

                generateObstacleTime -= Time.deltaTime / throwObstacleSpeed;

                if(generateObstacleTime <= 0)
                {
                    ChangeState(State.doNothing);
                }
                #endregion
                break;
        }
    }
}
