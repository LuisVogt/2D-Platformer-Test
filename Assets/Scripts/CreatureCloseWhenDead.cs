using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCloseWhenDead : Creature
{
    public override void Die()
    {
        Application.Quit();
    }
}
