using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MoveComponentData : IComponentData {
    public float move_speed;
    public float rotationSpeed;
    public float3 move_dir;
    public quaternion rotation;
    public int destroyed_obj_count;
}