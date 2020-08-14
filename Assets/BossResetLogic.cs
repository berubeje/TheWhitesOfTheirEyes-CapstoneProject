using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossResetLogic : IObstacle
{
    private BossController _bossController;

    // Start is called before the first frame update
    private void Awake()
    {
        _bossController = GetComponent<BossController>();
    }

    public override void ResetObstacle()
    {
        _bossController.currentBossHealth = _bossController.maxHealth;
    }

    public override void UnresetObstacle()
    {
        //Nothing to do here
    }
}
