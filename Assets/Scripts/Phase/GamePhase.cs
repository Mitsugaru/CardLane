using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePhase {

    public bool Setup { get; set; } = false;

    /// <summary>
    /// Execute the current Game Phase
    /// </summary>
    public abstract void execute();

    public abstract bool hasCompleted();

    public abstract GamePhase getNextPhase();
}
