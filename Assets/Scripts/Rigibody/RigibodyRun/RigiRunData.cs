using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Run Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class RigiRunData : ScriptableObject
{
    [Header("Run")]
    public float runMaxSpeed; //Target speed we want the player to reach
    public float runAcceleration; //Time (approx.) we want it to take for the player to acceleration from 0 to the runMaxSpeed
    [HideInInspector] public float runAccelAmount; //Actual force (multiplied with speedDiff) applied to the player
    public float runDeceleration; //Time (approx.) we want it to take for the player to acceleration from runMaxSpeed to 0
    [HideInInspector] public float runDecelAmount; //Actual force (multiplied with speedDiff) applied to the player
    [Space(10)]
    [Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborns
    [Range(0.01f, 1)] public float decelInAir;
    public bool doConserveMomentum;

    private void OnValidate()
    {
        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDecelAmount = (50 * runDeceleration) / runMaxSpeed;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDeceleration = Mathf.Clamp(runDeceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
