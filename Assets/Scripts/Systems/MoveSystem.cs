using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class MoveSystem : SystemBase {
    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach( ( ref Translation translation, ref Rotation rot, in MoveComponentData moveData ) => {
            translation.Value += moveData.move_dir * moveData.move_speed * deltaTime;
            if ( !moveData.move_dir.Equals( float3.zero ) ) {
                quaternion target = quaternion.LookRotation( moveData.move_dir, new float3( 0, 1, 0 ) );
                rot.Value = math.slerp( rot.Value, target, deltaTime * moveData.rotationSpeed );
            }
        } ).Run();
    }
}
