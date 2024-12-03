using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedItem : MonoBehaviour
{
    private bool isItemPicked = false;
    private GameObject playerObject = null;

    public bool IsItemPicked(GameObject player)
    {
        if(this.playerObject == player)
        {
            return false;
        }
        else
        {
            return isItemPicked;
        }
    }

    public GameObject GetPlayerAssignedToItem()
    {
        return this.playerObject;
    }

    public void SetPlayer(GameObject playerObject)
    {
        if(playerObject != null)
        {
            isItemPicked = true;
            this.playerObject = playerObject;
        }
        else
        {
            isItemPicked = false;
            this.playerObject = null;
        }
    }


}
