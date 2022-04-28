﻿using System.Collections;
using UnityEngine;

public class ObjectiveReachTargets : Objective
{

    public bool mustCollectAllPickups = true;
    
    public int pickupsToCompleteObjective = 5;
    
    public int notificationPickupsRemainingThreshold = 1;
    
    

    IEnumerator Start()
    {
   
        TimeManager.OnSetTime(totalTimeInSecs, isTimed, gameMode);
        
        yield return new WaitForEndOfFrame();

        title = "Collect " +
                (mustCollectAllPickups ? "all the" : pickupsToCompleteObjective.ToString()) + " " +
                targetName + "s";
        
        if (mustCollectAllPickups)
            pickupsToCompleteObjective = NumberOfPickupsTotal;
        
        Register();
    }

    protected override void ReachCheckpoint(int remaining)
    {

        if (isCompleted)
            return;
        
        if (mustCollectAllPickups) 
            pickupsToCompleteObjective = NumberOfPickupsTotal;

        m_PickupTotal = NumberOfPickupsTotal - remaining;
        int targetRemaining = mustCollectAllPickups ? remaining : pickupsToCompleteObjective - m_PickupTotal;

        if (targetRemaining == 0)
        {
            CompleteObjective(string.Empty, GetUpdatedCounterAmount(),
                "Objective complete: " + title);
        }
        else if (targetRemaining == 1)
        {
            string notificationText = notificationPickupsRemainingThreshold >= targetRemaining
                ? "One " + targetName + " left"
                : string.Empty;
            UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
        }
        else if (targetRemaining > 1)
        {
            string notificationText = notificationPickupsRemainingThreshold >= targetRemaining
                ? targetRemaining + " " + targetName + "s to collect left"
                : string.Empty;

            UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
        }

    }

    public override string GetUpdatedCounterAmount()
    {
        return m_PickupTotal + " / " + pickupsToCompleteObjective;
    }
    
}
