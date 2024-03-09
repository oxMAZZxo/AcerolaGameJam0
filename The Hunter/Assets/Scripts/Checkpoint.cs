using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour
{
    public Transform transformToSave;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && GameManager.Instance.IsFinishedLoading())
        {
            Debug.Log("Checkpoint Reached");
            GameData.Instance.SetLastSavedPlayerLocationX(transformToSave.position.x);
            GameData.Instance.SetLastSavedPlayerLocationY(transformToSave.position.y);
            GameData.Instance.SetPortalDestroyed(GameManager.Instance.GetPortals());
            GameData.Instance.SetCurrentHealth(PlayerCombat.Instance.GetCurrentHealth());
            GameData.Instance.SetCookedBunnies(Inventory.Instance.GetNoOfCookedBunnies());
            GameData.Instance.SetRawBunnies(Inventory.Instance.GetNoOfRawBunnies());
            GameData.Instance.SetInOverworld(GameManager.Instance.IsInOverworld());
            GameData.Instance.SaveGame();
        }
    }

}
