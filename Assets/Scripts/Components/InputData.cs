using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct InputData : IComponentData {
    public KeyCode backKey;
}