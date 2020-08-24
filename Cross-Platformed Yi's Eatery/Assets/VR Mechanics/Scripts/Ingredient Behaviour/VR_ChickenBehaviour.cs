using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ChickenBehaviour : MonoBehaviour
{
    [HideInInspector] public VR_IngredientProperties Properties { get { return properties; } }
    [SerializeField] private VR_IngredientProperties properties;

}
