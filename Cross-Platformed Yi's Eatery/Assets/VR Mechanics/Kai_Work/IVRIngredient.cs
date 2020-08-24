using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVRIngredient
{
    string LicensePlate { get; }
    double Speed { get; }
    int Wheels { get; }
    void Honk();
}
