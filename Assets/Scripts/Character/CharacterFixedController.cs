using UnityEngine;
using System.Collections;

public class CharacterFixedController : SuperCharacterController {

    void Update()
    {
        //Disable SuperCharacterController Update
    }

    public void DoUpdate(float delta)
    {
        base.deltaTime = delta;
        base.SingleUpdate();
    }

}
