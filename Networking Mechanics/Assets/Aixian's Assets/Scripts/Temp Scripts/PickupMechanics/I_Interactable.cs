using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Interface for all interactable objects
//Contains base functions
public interface I_Interactable
{
 
   

    //functions

    //pickup object, takes in pickedUpObject, held object, and an object icon
    void PickUpObject();

    //drop object, takes in pickedUpObject, held object, and an object icon
    void DropObject();

    //INGREDIENTS AND DISHES
    //place object on table
    void PlaceOnTable();



}
