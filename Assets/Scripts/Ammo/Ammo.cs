using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField]
    private AmmoType type;

    public AmmoType Type { get => type; private set => type = value; }
}
