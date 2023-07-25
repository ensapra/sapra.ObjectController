using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sapra.ObjectController;

public class MinimalController : ObjectController
{
    public MinimalModule minimalModule = new MinimalModule();
    protected override void addModules()
    {
        AddModule(minimalModule);
    }
}
